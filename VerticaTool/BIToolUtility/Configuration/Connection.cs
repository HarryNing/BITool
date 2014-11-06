using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vertica.Data.VerticaClient;

namespace BIToolUtility.Configuration
{
    public enum ConnectionType
    {
        MSSQLSERVER,
        Vertica
    }

    public class Connection
    {
        public string Name { get; set; }
        public string Host { get; set; }
        public string Database { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public ConnectionType ConnectionType { get; set; }

        public string ConnectionString
        {
            get
            {
                VerticaConnectionStringBuilder builder = new VerticaConnectionStringBuilder();
                builder.Host = Host;
                builder.Database = Database;
                builder.User = Username;
                builder.Password = Password;
                return ConnectionType == Configuration.ConnectionType.MSSQLSERVER ?
                string.Format("server={0};initial catalog={1};uid={2};password={3};", this.Host, Database, Username, Password) :
                builder.ToString();
            }
        }
    }
}
