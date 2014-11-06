using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using log4net;

namespace BIToolUtility.Log
{
    public class EtlLog:ILogEx
    {
        private static ILogEx logger;
        private static object locker = new object();

        protected string _logFile;
        private string _seperate = "        ||";
        private ILog _logger;

        public static ILogEx NewLogger(string name)
        {
            var logger = new EtlLog(name);

            return logger;
        }

        public static ILogEx GetLogger()
        {
            if (logger == null)
            {
                lock (locker)
                {
                    if (logger == null)
                        logger = new EtlLog();
                }
            }

            return logger;
        }

        EtlLog()
        {
            this._logger = LogManager.GetLogger("Bitool");
        }

        EtlLog(string name)
        {
            this._logger = LogManager.GetLogger(name);
        }

        public void LogRejected(IList<long> rejectedRows, string fileName, string logFileName)
        {
            StreamReader inputFile = new StreamReader(fileName);
            StreamWriter outputFile = new StreamWriter(logFileName, true);
            string line = string.Empty;
            long lineNumber = 1;
            while ((line = inputFile.ReadLine()) != null)
            {
                if (!rejectedRows.IndexOf(lineNumber).Equals(-1))
                {
                    outputFile.WriteLine(line);
                }
                lineNumber++;
            }
            inputFile.Close();
            inputFile.Dispose();
            outputFile.Flush();
            outputFile.Close();
            outputFile.Dispose();
        }

        protected void InitializeLogFile()
        {
            if (!File.Exists(this._logFile))
            {
                var stream = File.Create(this._logFile);
                StreamWriter sw = new StreamWriter(stream);
                sw.WriteLine(string.Format("Date{0}Time{0}Type{0}Message", this._seperate));

                sw.Close();
                stream.Close();
            }
        }

        public void Log(string message, LogType type)
        {
            if (type == LogType.Message)
                this._logger.Info(message);
            else if (type == LogType.Error)
                this._logger.Error(message);
            else if (type == LogType.Debug)
                this._logger.Debug(message);
        }
    }
}
