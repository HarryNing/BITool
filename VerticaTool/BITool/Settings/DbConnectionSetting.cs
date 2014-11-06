using BIToolUtility.Configuration;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Vertica.Data.VerticaClient;

namespace BITool
{
    public partial class DbConnectionSetting : Form
    {
        public Connection Connection { get; set; }

        public DbConnectionSetting()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (!this.Validation())
            {
                MessageBox.Show("Some values are missing!");
            }
            else
            {
                this.Connection.Name = this.txtName.Text;
                this.Connection.Host = this.txtServer.Text;
                this.Connection.Username = this.txtUsername.Text;
                this.Connection.Password = this.txtPassword.Text;
                this.Connection.Database = this.Connection.ConnectionType == ConnectionType.MSSQLSERVER ? "master" : this.txtDatabase.Text;
            }

            DialogResult = System.Windows.Forms.DialogResult.OK;

            this.Close();
        }

        private bool Validation()
        {
            if (string.IsNullOrWhiteSpace(this.txtName.Text)
                || string.IsNullOrWhiteSpace(this.txtServer.Text)
                || string.IsNullOrWhiteSpace(this.txtUsername.Text)
                || string.IsNullOrWhiteSpace(this.txtPassword.Text)
                || (this.Connection.ConnectionType == ConnectionType.MSSQLSERVER? false :string.IsNullOrWhiteSpace(this.txtDatabase.Text)))
                return false;
            return true;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.Close();
        }

        private void btnTest_Click(object sender, EventArgs e)
        {
           string error;
           if (this.TestConnection(out error))
           {
               MessageBox.Show("Test connection suceed!");
           }
           else
           {
               MessageBox.Show("Test failed." + error);
           }
        }

        public bool TestConnection(out string Error)
        {
            Error = string.Empty;
            VerticaConnection verticaConnection = null;

            try
            {
                if (Connection.ConnectionType == ConnectionType.MSSQLSERVER)
                {
                    using (SqlConnection connection = new SqlConnection(string.Format("server={0};initial catalog=master;uid={1};password={2};", this.txtServer.Text, this.txtUsername.Text, this.txtPassword.Text)))
                    {
                        connection.Open();
                        connection.Close();
                    }
                }
                else
                {
                    VerticaConnectionStringBuilder builder = new VerticaConnectionStringBuilder();
                    builder.Host = this.txtServer.Text;
                    builder.Database = this.txtDatabase.Text;
                    builder.User = this.txtUsername.Text;
                    builder.Password = this.txtPassword.Text;
                    verticaConnection = new VerticaConnection(builder.ToString());

                    verticaConnection.Open();

                    verticaConnection.Close();
                }

                return true;
            }
            catch (Exception ex)
            {
                Error = ex.Message;

                if (verticaConnection != null) verticaConnection.Close();
                return false;
            }
        }
    }
}
