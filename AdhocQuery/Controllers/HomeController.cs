using AdhocQuery.Models;
using AdhocQuery.Services;
using ICSharpCode.SharpZipLib.Core;
using ICSharpCode.SharpZipLib.Zip;
using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Text;
using System.Web.Mvc;

namespace AdhocQuery.Controllers
{
    public class HomeController : Controller
    {
        const string preRunSQLTemplate = "SET FMTONLY ON; @SQL";

        public ActionResult Index()
        {
            ViewBag.Message = "Adhoc Query Tool";
            return View();
        }

        [HttpPost]
        public JsonResult Query(string sqlScript, string jobId)
        {
            AdhocQueryModel model = new AdhocQueryModel();
            try
            {
                using (SqlConnection conn = new SqlConnection(GetConnectionString()))
                {
                    conn.Open();
                    string preRunSQL = preRunSQLTemplate.Replace("@SQL", sqlScript);
                    SqlCommand cmd = new SqlCommand(preRunSQL, conn);
                    cmd.CommandTimeout = 60;
                    try
                    {
                        cmd.ExecuteReader();
                    }
                    catch (Exception ex)
                    {
                        model.Message = "Error message:@Error".Replace("@Error", ex.Message);
                        model.IsFailed = true;
                        return Json(model);
                    }
                }
                JobManager.Instance.DoJobAsync(job =>
                {
                    try
                    {
                        string filePath = AppDomain.CurrentDomain.BaseDirectory + "CSV\\@FileName.csv".Replace("@FileName", job.Id);
                        var queryJob = (QueryJob)job;
                        GenerateZipfile(sqlScript, filePath, queryJob);
                    }
                    catch (Exception ex)
                    {
                        job.IsExceptioned = true;
                        job.ExceptionString = ex.ToString();
                    }
                }, new QueryJob(jobId));
                return Json(model);
            }
            catch (Exception e)
            {
                model.Message = e.Message;
                model.IsFailed = true;
                return Json(model);
            }
        }

        private static string GetConnectionString()
        {
            return ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
        }

        protected void GenerateZipfile(string sqlScript, string filePath, QueryJob job)
        {
            using (SqlConnection conn = new SqlConnection(GetConnectionString()))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand(sqlScript, conn))
                {
                    cmd.CommandTimeout = 60 * 30;
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            if (!(reader.IsDBNull(0)))
                            {
                                StreamWriter sWriter = new StreamWriter(filePath, false, Encoding.UTF8);
                                StringBuilder sbHeader = new StringBuilder();
                                for (int i = 0; i < reader.FieldCount; i++)
                                {
                                    sbHeader.Append(reader.GetName(i)).Append(',');
                                }
                                sbHeader.AppendLine();
                                sWriter.Write(sbHeader.ToString());
                                int readCount = 0;
                                string htmlString = BuildTableTH(reader);
                                bool sendHtml = false;
                                do
                                {
                                    if (job != null && job.CancellationToken.IsCancellationRequested)
                                    {
                                        sWriter.Flush();
                                        sWriter.Close();
                                        sWriter.Dispose();
                                        job.TempFilePath = filePath;
                                        return;
                                    }
                                    StringBuilder sbRow = new StringBuilder();
                                    for (int i = 0; i < reader.FieldCount; i++)
                                    {
                                        sbRow.Append(reader[i].ToString()
                                            .Replace(",", ";")
                                            .Replace("\n", string.Empty)
                                            .Replace("\r", string.Empty)
                                            ).Append(',');
                                    }
                                    sbRow.AppendLine();
                                    sWriter.Write(sbRow.ToString());
                                    readCount++;
                                    if (readCount <= 5)
                                    {
                                        htmlString += BuildTableTR(reader);

                                    }
                                    if (readCount == 5)
                                    {
                                        sendHtml = true;
                                        job.SendTop5(htmlString);
                                    }
                                    if (readCount % 1000 == 0)
                                    {
                                        job.ReportProgress(readCount);
                                        sWriter.Flush();
                                    }
                                }
                                while (reader.Read());
                                sWriter.Flush();
                                sWriter.Close();
                                sWriter.Dispose();
                                if (sendHtml == false)
                                {
                                    job.SendTop5(htmlString);
                                }
                                job.ReportProgress(readCount);
                                job.ReportQueryComplete();

                                string zipfilePath = filePath.Substring(0, filePath.LastIndexOf(".")) + ".zip";
                                using (ZipOutputStream zoStream = new ZipOutputStream(System.IO.File.Create(zipfilePath)))
                                {
                                    zoStream.SetLevel(9); // 0 - store only to 9 - means best compression
                                    string fileName = filePath.Substring(filePath.LastIndexOf("\\") + 1);
                                    ZipEntry entry = new ZipEntry(fileName);
                                    zoStream.PutNextEntry(entry);

                                    using (FileStream fs = System.IO.File.OpenRead(filePath))
                                    {
                                        byte[] buffer = new byte[4096];
                                        StreamUtils.Copy(fs, zoStream, buffer);
                                        fs.Close();
                                    }
                                    zoStream.Close();
                                }
                                job.TempFilePath = filePath;
                            }
                        }
                    }
                }
            }
        }

        private string BuildTableTR(SqlDataReader reader)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("<tr>");
            for (int i = 0; i < reader.FieldCount; i++)
            {
                sb.Append("<td>").Append(reader[i].ToString()).Append("</td>");
            }
            sb.Append("</tr>");
            return sb.ToString();
        }

        private string BuildTableTH(SqlDataReader reader)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("<tr>");
            for (int i = 0; i < reader.FieldCount; i++)
            {
                sb.Append("<th>").Append(reader.GetName(i)).Append("</th>");
            }
            sb.Append("</tr>");
            return sb.ToString();
        }
    }
}
