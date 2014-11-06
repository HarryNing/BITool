using BIToolUtility;
using BIToolUtility.Configuration;
using BIToolUtility.Helper;
using BIToolUtility.Log;
using BIToolUtility.Mail;
using System;
using System.IO;

namespace Batch
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                return;
            }
            string batchName = args[0];
            ILogEx logger = EtlLog.GetLogger();
            string batchConfigPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Batch\\");

            BatchConfig config = null;
            try
            {
                config = SerializeHelper.LoadFromXml<BatchConfig>(batchConfigPath + batchName);
                foreach (var trans in config.Transforms)
                {
                    try
                    {
                        var newLogger = EtlLog.NewLogger(trans.SourceTableName);
                        BIToolClient client = new BIToolClient(trans, newLogger);
                        client.ExecuteTransform();
                    }
                    catch (Exception e)
                    {
                        logger.Log(e.ToString(), LogType.Error);
                        var smtp = new SmtpClientAdaptor(GetSMTPServer());
                        string error = string.Empty;
                        smtp.ExceptionNotify(string.Format("Execute transformation {0} failed", trans.SourceTableName),
                            e.Message + "\n" + e.StackTrace, out error);
                        if (error != string.Empty)
                        {
                            logger.Log(string.Format("Send notification email failed {0}", error), LogType.Error);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Log(ex.ToString(), LogType.Error);
                return;
            }
        }

        private static SmtpServer GetSMTPServer()
        {
            var appSetting = System.Configuration.ConfigurationManager.AppSettings;
            SmtpServer server = new SmtpServer();
            try
            {
                server.Server = appSetting["Server"];
                server.PickupDirectoryLocation = appSetting["PickupDirectoryLocation"];
                server.User = appSetting["User"];
                server.Password = appSetting["Password"];
                server.ExceptionFrom = appSetting["ExceptionFrom"];
                server.ExceptionTo = appSetting["ExceptionTo"];
                server.UsePickupDirectoryLocation = Convert.ToBoolean(appSetting["UsePickupDirectoryLocation"]);
            }
            catch (Exception)
            { }
            return server;
        }
    }
}