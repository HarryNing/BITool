using BIToolUtility.Configuration;
using BIToolUtility.Delta;
using BIToolUtility.Helper;
using BIToolUtility.Log;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Vertica.Data.VerticaClient;

namespace BIToolUtility
{
    enum ProcessStatus
    {
        Unprocessed,
        Processing,
        Processed
    }
    public class BIToolClient
    {
        private string _delimiter = "|";
        private string _pipeAsciiCode = "&#124;";
        private string _slashAsciiCode = "&#92";
        private Transform _config;
        private string workFolder;
        private string logFolder;

        ILogEx _log;

        private bool _isDeltaLoad;

        private int totalCount;
        private int batchCount;

        public EventHandler<ProgressReportArgs> ProgressReportChanged;

        public BIToolClient(Transform config, ILogEx log)
        {
            this._config = config;
            this._isDeltaLoad = this._config.IsDeltaLoad;
            this._log = log;
            string workRoot = System.Configuration.ConfigurationManager.AppSettings["WorkFolder"];
            workFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, 
                string.Format("{0}\\{1}\\", workRoot, this._config.TargetTableName));
            logFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "log\\");
            if (!Directory.Exists(this.logFolder))
            {
                Directory.CreateDirectory(this.logFolder);
            }
            if (!Directory.Exists(this.workFolder))
            {
                Directory.CreateDirectory(this.workFolder);
            }
        }

        private bool IsFirstStepDone;
        private int ThreadCount;
        private Dictionary<string, ProcessStatus> FileNameDic = new Dictionary<string, ProcessStatus>();
        private bool IsFirstStepError;
        public void ExecuteTransformAsync()
        {
            IsFirstStepDone = false;
            IsFirstStepError = false;
            ThreadCount = BIToolConfigHelper.Config.ThreadCount;
            batchCount = this._config.BatchCount;
            Task firstTask = Task.Run(() => this.CopySQLStreamToDisk());
            Task secondTask = Task.Run(() => this.CopyDiskToVertica());
            try
            {
                firstTask.Wait();
            }
            catch (Exception)
            {
                IsFirstStepError = true;
            }
        }
        public void ExecuteTransform()
        {
            IsFirstStepDone = false;
            ThreadCount = 1;
            batchCount = this._config.BatchCount;
            this.CopySQLStreamToDisk();
            this.CopyDiskToVertica();
        }

        private void CopySQLStreamToDisk()
        {
            this.LogMessage(LogType.Message, string.Format("Begin to Transform {0}...", this._config.TargetTableName));
            try
            {
                int recordBatchCounter = 0;
                int fileCounter = 0;
                int readCount = 0;
                bool streamWriterClosed = false;
                string maxUpdateDate = string.Empty;
                if (this._config.IsDeltaLoad)
                {
                    VerticaDeltaLoader loader = new VerticaDeltaLoader(this._config, _log);
                    loader.LoadDelta(out maxUpdateDate);
                }
                totalCount = GetSourceTotalCount(this._config.IsDeltaLoad, maxUpdateDate);
                string fileName = GenFileName(fileCounter);
                StreamWriter streamWriter = new StreamWriter(fileName);
                using (SqlConnection connection = new SqlConnection(this._config.SqlConnection))
                {
                    connection.Open();
                    string commandText = this._config.IsDeltaLoad ?
                        string.Format("{0} where UpdateDate>='{1}'", this._config.SelectText, maxUpdateDate) : this._config.SelectText;
                    this.LogMessage(LogType.Message, "CommandText:" + commandText);
                    using (SqlCommand command = new SqlCommand(commandText, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.SequentialAccess))
                        {
                            if (reader.Read())
                            {
                                if (!(reader.IsDBNull(0)))
                                {
                                    do
                                    {
                                        if (recordBatchCounter < batchCount - 1)
                                        {
                                            if (recordBatchCounter.Equals(0) && (fileCounter > 0))
                                            {
                                                fileName = GenFileName(fileCounter);
                                                streamWriter = new StreamWriter(fileName);
                                                streamWriterClosed = false;
                                            }
                                            this.WriteLine(reader, streamWriter);
                                            readCount++;
                                            recordBatchCounter = recordBatchCounter + 1;
                                        }
                                        else if (recordBatchCounter == batchCount - 1)
                                        {
                                            this.WriteLine(reader, streamWriter);
                                            readCount++;
                                            streamWriter.Flush();
                                            streamWriter.Close();
                                            streamWriter.Dispose();
                                            recordBatchCounter = 0;
                                            streamWriterClosed = true;
                                            lock (FileNameDic)
                                            {
                                                FileNameDic.Add(fileName, ProcessStatus.Unprocessed);
                                            }
                                            fileCounter = fileCounter + 1;
                                            if (ThreadCount > 1)
                                            {
                                                this.LogMessage(LogType.Message, "Start a new thread to copy file to Vertica...");
                                                ThreadCount = ThreadCount - 1;
                                                Task.Run(() => this.CopyDiskToVertica());
                                            }
                                            ProgressReport(readCount * 100 / totalCount, GetTargetTotalCount());
                                        }
                                    }
                                    while (reader.Read());
                                    if (!streamWriterClosed)
                                    {
                                        streamWriter.Flush();
                                        streamWriter.Close();
                                        streamWriter.Dispose();
                                        lock (FileNameDic)
                                        {
                                            FileNameDic.Add(fileName, ProcessStatus.Unprocessed);
                                        }
                                    }
                                    ProgressReport(readCount * 100 / totalCount, GetTargetTotalCount());
                                    IsFirstStepDone = true;
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                this.LogMessage(LogType.Error, ex.Message);
                throw ex;
            }
        }

        private int GetSourceTotalCount(bool isDelta, string maxUpdateDate)
        {
            int totalCount = 0;
            try
            {
                using (SqlConnection connection = new SqlConnection(_config.SqlConnection))
                {
                    connection.Open();
                    string commandText = string.Format("select count(1) as totalCount from {0} ", _config.SourceTableName);
                    if (isDelta)
                    {
                        commandText = commandText + string.Format("where UpdateDate>='{0}' ;", maxUpdateDate);
                    }
                    SqlCommand command = new SqlCommand(commandText, connection);
                    SqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        totalCount = Convert.ToInt32(reader["totalCount"].ToString());
                    }
                    connection.Close();
                }

            }
            catch (Exception ex)
            {
                LogMessage(LogType.Error, ex.ToString());
                throw ex;
            }
            return totalCount;
        }

        private int GetTargetTotalCount()
        {
            int totalCount = 0;
            try
            {
                using (VerticaConnection connection = new VerticaConnection(this._config.VerticaConnection))
                {
                    connection.Open();
                    VerticaCommand command = new VerticaCommand(string.Format("select count(1) as totalCount from {0} ;", this._config.TargetTableName), connection);
                    VerticaDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        totalCount = Convert.ToInt32(reader["totalCount"]);
                    }
                    connection.Close();
                }

            }
            catch (Exception ex)
            {
                LogMessage(LogType.Error, ex.ToString());
                throw ex;
            }
            return totalCount;
        }

        private void CopyDiskToVertica()
        {
            while (!IsFirstStepDone || !AllFileProcessed())
            {
                if (IsFirstStepError)
                {
                    break;
                }
                CopyFileListToVertica();
            }
            if (!IsFirstStepError)
            {
                ProgressReport(100, GetTargetTotalCount());
            }
        }

        private bool AllFileProcessed()
        {
            lock (FileNameDic)
            {
                if (!FileNameDic.ContainsValue(ProcessStatus.Processing)
                    && !FileNameDic.ContainsValue(ProcessStatus.Unprocessed))
                {
                    return true;
                }
            }
            return false;
        }

        private void CopyFileListToVertica()
        {
            List<string> fileNameList = null;
            lock (FileNameDic)
            {
                fileNameList = FileNameDic.Where(f => f.Value == ProcessStatus.Unprocessed).Select(f => f.Key).ToList();
            }
            foreach (string fileName in fileNameList)
            {
                lock (FileNameDic)
                {
                    if (FileNameDic[fileName] == ProcessStatus.Processing)
                    {
                        continue;
                    }
                    FileNameDic[fileName] = ProcessStatus.Processing;
                }
                CopyFileToVertica(fileName);
            }
        }

        private void CopyFileToVertica(string fileName)
        {
            VerticaConnection verticaConnection = new VerticaConnection(this._config.VerticaConnection);
            VerticaLogProperties.SetLogPath(this.logFolder, false);
            VerticaLogLevel level = VerticaLogLevel.Trace;
            VerticaLogProperties.SetLogLevel(level, false);
            string lognamespace = "Vertica.Data.VerticaClient";
            VerticaLogProperties.SetLogNamespace(lognamespace, false);

            try
            {
                verticaConnection.Open();
                using (verticaConnection)
                {
                    verticaConnection.InfoMessage += new VerticaInfoMessageEventHandler(connection_InfoMessage);

                    if (!this._isDeltaLoad)
                    {
                        this.LogMessage(LogType.Message, string.Format("Truncate table {0} in vertica!", this._config.TargetTableName));
                        VerticaCommand command = new VerticaCommand("Truncate table " + this._config.TargetTableName + ";", verticaConnection);
                        command.ExecuteNonQuery();
                        this._isDeltaLoad = true;
                    }

                    VerticaTransaction txn = verticaConnection.BeginTransaction();
                    FileStream inputfile = File.OpenRead(fileName);
                    string copy = "copy " + this._config.TargetTableName + " from stdin delimiter '" + this._delimiter + "' no commit";
                    VerticaCopyStream vcs = new VerticaCopyStream(verticaConnection, copy);
                    vcs.Start();
                    vcs.AddStream(inputfile, false);
                    vcs.Execute();
                    long rowsInserted = vcs.Finish();
                    IList<long> rowsRejected = vcs.Rejects; // does not work when rejected or exceptions defined
                    inputfile.Close();
                    if (rowsRejected.Count > 0)
                    {
                        string logFileName = GetRejectedLogFileName(fileName);
                        this._log.LogRejected(rowsRejected, fileName, logFileName);
                    }
                    txn.Commit();
                    inputfile.Dispose();
                    File.Delete(fileName);
                }
                verticaConnection.Close();
            }
            catch (Exception ex)
            {
                this.LogMessage(LogType.Error, ex.ToString());
                throw ex;
            }
            finally
            {
                lock (FileNameDic)
                {
                    FileNameDic[fileName] = ProcessStatus.Processed;
                }
                verticaConnection.Close();
            }
        }

        private string GetRejectedLogFileName(string fileNameWithPath)
        {
            string rejectedFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "log\\rejected\\");
            if (!Directory.Exists(rejectedFolder))
            {
                Directory.CreateDirectory(rejectedFolder);
            }
            string fileName = Path.GetFileNameWithoutExtension(fileNameWithPath);
            string logFileName = @"%01Rejected_%02.log".Replace("%01", rejectedFolder).Replace("%02", fileName);
            return logFileName;
        }

        private string GenFileName(int fileCounter)
        {
            string fileName = @"%05%00_%01_%02.txt"
                .Replace("%00", _config.TargetTableName)
                .Replace("%01", DateTime.Now.ToString("MMddyyyy_hh_mm_ss_fff_tt"))
                .Replace("%05", workFolder)
                .Replace("%02", fileCounter.ToString());
            return fileName;
        }

        private void WriteLine(SqlDataReader reader, StreamWriter streamWriter)
        {
            string value = string.Empty;
            Dictionary<string, string> keyValue = new Dictionary<string, string>();

            for (int i = 0; i < reader.FieldCount; i++)
            {
                string originStr = reader[i].ToString();
                string str = originStr.Replace(this._delimiter, this._pipeAsciiCode)
                    .Replace("\\", this._slashAsciiCode)
                    .Replace("\n", string.Empty)
                    .Replace("\r", string.Empty)
                    .Replace("False", "0")
                    .Replace("True", "1");

                value = value + str + this._delimiter;
            }
            streamWriter.Write(value);
            streamWriter.WriteLine();
        }

        void connection_InfoMessage(object sender, VerticaInfoMessageEventArgs e)
        {
            this.LogMessage(LogType.Message, e.SqlState + ": " + e.Message);
        }

        void LogMessage(LogType type, string message)
        {
            this._log.Log(message, type);
        }

        void ProgressReport(int prValue, int totalCount)
        {
            if (ProgressReportChanged != null)
            {
                this.ProgressReportChanged(this, new ProgressReportArgs()
                {
                    PRValue = prValue,
                    TotalCount = totalCount
                });
            }
        }
    }
}
