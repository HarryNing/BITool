using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace BIToolUtility.Mail
{
    public class SmtpClientAdaptor
    {
        private SmtpServer _smtpServer;

        public SmtpClientAdaptor(SmtpServer smtpServer)
        {
            this._smtpServer = smtpServer;
        }


        public void SendEmail(MailMessage mMessage, List<string> recipients)
        {
            try
            {
                SmtpClient client = new SmtpClient(this._smtpServer.Server);
                client.UseDefaultCredentials = false;
                client.Credentials = new System.Net.NetworkCredential(this._smtpServer.User, this._smtpServer.Password);

                if (this._smtpServer.UsePickupDirectoryLocation)
                {
                    client.DeliveryMethod = SmtpDeliveryMethod.SpecifiedPickupDirectory;
                    client.PickupDirectoryLocation = this._smtpServer.PickupDirectoryLocation;
                }
                else
                {
                    client.DeliveryMethod = SmtpDeliveryMethod.Network;
                }

                foreach (var item in recipients)
                {
                    mMessage.To.Add(item);
                }

                mMessage.IsBodyHtml = true;

                client.Send(mMessage);

                mMessage.Dispose();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public bool ExceptionNotify(string subject, string errorMessage, out string error)
        {
            error = string.Empty;

            try
            {
                MailMessage message = new MailMessage();
                message.From = new MailAddress(string.IsNullOrEmpty(_smtpServer.ExceptionFrom) ? "BIEtlException@ef.com" : _smtpServer.ExceptionFrom);
                message.Subject = subject;
                message.Body = errorMessage;
                SmtpClientAdaptor smtpClient = new SmtpClientAdaptor(_smtpServer);
                smtpClient.SendEmail(message, _smtpServer.ExceptionTo.Split(';').ToList());

                return true;
            }
            catch (Exception ex)
            {
                error = ex.Message + "------" + ex.StackTrace;

                return false;
            }
        }
    }
}
