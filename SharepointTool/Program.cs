using SharepointTool.E1ReportService;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Services.Protocols;

namespace SharepointTool
{
    class Program
    {
        public class DSSetting
        {
            public string ReportPath { get; set; }

            public string ReportName { get; set; }

            public string DataSourceName { get; set; }

            public string DataSourcePath { get; set; }

        }
        static void Main(string[] args)
        {
            //HDR=YES, First Row as Column Name;IMEX=1, Avoid crash
            string connectionString = string.Format("Provider=Microsoft.ACE.OLEDB.12.0;Data Source={0};Extended Properties=\"Excel 12.0 Xml;HDR=YES;IMEX=1\";",
             Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "datasource.xlsx"));
            List<DSSetting> dsList = new List<DSSetting>();
            using (OleDbConnection conn = new OleDbConnection(connectionString))
            {
                conn.Open();
                OleDbDataAdapter da = new OleDbDataAdapter("SELECT * FROM [Sheet1$]", conn);
                DataSet ds = new DataSet();
                da.Fill(ds);
                DataTable dt = ds.Tables[0];
                foreach (DataRow row in dt.Rows)
                {
                    DSSetting setting = new DSSetting();
                    setting.ReportPath = row["ReportPath"].ToString();
                    setting.ReportName = row["ReportName"].ToString();
                    setting.DataSourceName = row["DataSourceName"].ToString();
                    setting.DataSourcePath = row["DataSourcePath"].ToString();
                    dsList.Add(setting);
                }
            }

            var rs = new ReportingService2010();
            rs.Credentials = System.Net.CredentialCache.DefaultCredentials;
            StringBuilder exceptionSB = new StringBuilder();
            foreach (DSSetting setting in dsList)
            {
                DataSourceReference reference = new DataSourceReference() { Reference = setting.DataSourcePath };
                DataSource dataSource = new DataSource() 
                { 
                    Item = (DataSourceDefinitionOrReference)reference,
                    Name = setting.DataSourceName
                };
                DataSource[] dataSources = new DataSource[] { dataSource };

                try
                {
                    rs.SetItemDataSources(setting.ReportPath, dataSources);
                    Console.WriteLine(setting.ReportPath);
                }
                catch (SoapException ex)
                {
                    Console.WriteLine(ex.Detail.InnerXml.ToString());
                    exceptionSB.AppendLine(string.Format("Set data source for {0} failed", setting.ReportPath));
                }
            }
            Console.WriteLine(exceptionSB.ToString());
            Console.Read();
        }
    }
}
