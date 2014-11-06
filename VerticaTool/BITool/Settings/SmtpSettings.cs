using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using BIToolUtility.Configuration;
using BIToolUtility.Mail;

namespace BITool
{
    public partial class SmtpSettings : Form
    {
        public SmtpServer SmtpServer
        {
            get;
            set;
        }

        public SmtpSettings()
        {
            InitializeComponent();
        }

        private void SmtpSettings_Load(object sender, EventArgs e)
        {
            this.txtServer.Text = this.SmtpServer.Server;
            this.txtUser.Text = this.SmtpServer.User;
            this.txtPassword.Text = this.SmtpServer.Password;
            this.txtPickupLoaction.Text = this.SmtpServer.PickupDirectoryLocation;
            this.checkBox1.Checked = this.SmtpServer.UsePickupDirectoryLocation;
            this.txtFrom.Text = this.SmtpServer.ExceptionFrom;
            this.txtTo.Text = this.SmtpServer.ExceptionTo;
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            this.SmtpServer.Server = this.txtServer.Text;
            this.SmtpServer.User = this.txtUser.Text;
            this.SmtpServer.Password = this.txtPassword.Text;
            this.SmtpServer.PickupDirectoryLocation = this.txtPickupLoaction.Text;
            this.SmtpServer.UsePickupDirectoryLocation = this.checkBox1.Checked;
            this.SmtpServer.ExceptionFrom = this.txtFrom.Text;
            this.SmtpServer.ExceptionTo = this.txtTo.Text;

            DialogResult = System.Windows.Forms.DialogResult.OK;

            this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnTestSend_Click(object sender, EventArgs e)
        {
            try
            {
                var smtp = new SmtpClientAdaptor(new SmtpServer());//Not Implemented
                string error;
                if (smtp.ExceptionNotify("Test Mail", "This is a test connection mail!", out error))
                    MessageBox.Show("Send test mail succeed!");
                else
                    MessageBox.Show(error);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "----" + ex.StackTrace);
            }
        }
    }
}
