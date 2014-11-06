using BIToolUtility.Configuration;
using BIToolUtility.Helper;
using BIToolUtility.Log;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vertica.Data.VerticaClient;

namespace BIToolUtility.Delta
{
    public class VerticaDeltaLoader
    {
        private Transform _config;
        private ILogEx _log;
        public VerticaDeltaLoader(Transform config, ILogEx log)
        {
            this._config = config;
            this._log = log;
        }
        public void LoadDelta(out string maxUpdateDate)
        {
            maxUpdateDate = string.Empty;
            string primaryKeys = string.Empty;
            GetPKsAndMaxUpdateDateFromVertica(out maxUpdateDate, out primaryKeys);
            if (maxUpdateDate != string.Empty)
            {
                this._log.Log(string.Format("Max UpdateDate in {0} is {1}", _config.TargetTableName, maxUpdateDate), LogType.Message);
                GetPrimaryKeyValueStringAndDelete(primaryKeys, maxUpdateDate);
            }
        }

        private void GetPrimaryKeyValueStringAndDelete(string primaryKeys, string maxUpdateDate)
        {
            string[] primaryKeysplits = primaryKeys.Split(',');
            List<string> trimedPKs = new List<string>();
            foreach (string pk in primaryKeysplits)
            {
                trimedPKs.Add(pk.Trim());
            }

            StringBuilder sb = new StringBuilder();
            try
            {
                using (SqlConnection connection = new SqlConnection(this._config.SqlConnection))
                {
                    connection.Open();
                    string commandText = string.Format("select {0} from {1} with(nolock) where UpdateDate >= '{2}'",
                        primaryKeys, this._config.SourceTableName, maxUpdateDate);
                    SqlCommand command = new SqlCommand(commandText, connection);
                    SqlDataReader reader = command.ExecuteReader();
                    int readCount = 0;
                    while (reader.Read())
                    {
                        readCount++;
                        foreach (string primaryKey in trimedPKs)
                        {
                            if (primaryKey == trimedPKs.First())
                            {
                                sb.Append("(");
                            }
                            if (primaryKey != trimedPKs.Last())
                            {
                                sb.Append(string.Format("'{0}'", reader[primaryKey])).Append(",");
                            }
                            else
                            {
                                sb.Append(string.Format("'{0}'", reader[primaryKey]));
                            }
                            if (primaryKey == trimedPKs.Last())
                            {
                                sb.Append(")");
                            }
                        }
                        if (readCount == 1000)
                        {
                            DeleteUpdatedDataInVertica(primaryKeys, sb.ToString());
                            sb = new StringBuilder();
                            readCount = 0;
                            continue;
                        }
                        sb.Append(",");
                    }
                    sb.Remove(sb.Length - 1, 1);
                    connection.Close();
                }
            }
            catch (Exception ex)
            {
                this._log.Log(ex.ToString(), LogType.Error);
                throw ex;
            }
            DeleteUpdatedDataInVertica(primaryKeys, sb.ToString());
        }

        private void DeleteUpdatedDataInVertica(string primaryKeys, string primaryKeyValueString)
        {
            try
            {
                using (VerticaConnection connection = new VerticaConnection(this._config.VerticaConnection))
                {
                    connection.Open();
                    string commandText = string.Format("delete from {0} where ({1}) in ({2});",
                        this._config.TargetTableName, primaryKeys, primaryKeyValueString);
                    VerticaCommand command = new VerticaCommand(commandText, connection);
                    command.ExecuteNonQuery();
                    connection.Close();
                }
            }
            catch (Exception ex)
            {
                this._log.Log(ex.ToString(), LogType.Error);
                throw ex;
            }
        }

        private void GetPKsAndMaxUpdateDateFromVertica(out string maxUpdateDate, out string primaryKeys)
        {
            maxUpdateDate = string.Empty;
            primaryKeys = string.Empty;
            string[] targetTableSplits = this._config.TargetTableName.Split('.');
            try
            {
                using (VerticaConnection connection = new VerticaConnection(this._config.VerticaConnection))
                {
                    connection.Open();
                    string commandText = string.Format(QueryHelper.GetPKsMaxUpdateDateQuery, targetTableSplits[0], targetTableSplits[1]);
                    VerticaCommand command = new VerticaCommand(commandText, connection);
                    VerticaDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        primaryKeys = reader[0].ToString();
                        maxUpdateDate = reader[1].ToString();
                        DateTime datetime;
                        if (DateTime.TryParse(maxUpdateDate, out datetime))
                        {
                            maxUpdateDate = datetime.ToString("yyyy-MM-dd HH:mm:ss");
                        }
                        else
                        {
                            maxUpdateDate = "2000-01-01 00:00:00";
                        }
                    }
                    connection.Close();
                }
            }
            catch (Exception ex)
            {
                this._log.Log(ex.ToString(), LogType.Error);
                throw ex;
            }
        }
    }
}
