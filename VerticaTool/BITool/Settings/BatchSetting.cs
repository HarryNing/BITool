using BIToolUtility.Configuration;
using BIToolUtility.Helper;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BITool
{
    public partial class BatchSetting : Form
    {
        public BatchSetting()
        {
            InitializeComponent();
            foreach (string batch in BIToolConfigHelper.Config.Batches)
            {
                this.listBatch.Items.Add(batch);
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtBatchName.Text))
            {
                return;
            }
            if (BatchExist(txtBatchName.Text.Trim()))
            {
                txtBatchName.Text = string.Empty;
                return;
            }
            listBatch.Items.Add(txtBatchName.Text.Trim());
            txtBatchName.Text = string.Empty;
        }

        private bool BatchExist(string batchName)
        {
            bool isExist = false;
            foreach (var item in listBatch.Items)
            {
                if (batchName == item.ToString())
                {
                    isExist = true;
                    break;
                }
            }
            return isExist;
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (listBatch.SelectedItem == null)
            {
                return;
            }
            listBatch.Items.Remove(listBatch.SelectedItem);
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            List<string> batches = new List<string>();
            foreach (var batch in listBatch.Items)
            {
                batches.Add(batch.ToString());
            }
            BIToolConfigHelper.Config.Batches = batches;
            BIToolConfigHelper.SaveConfig();
            BIToolConfigHelper.BatchConfigs = BIToolConfigHelper.BatchConfigs.Where(c => batches.Contains(c.BatchName)).ToList();
            MainForm mainform = (MainForm)this.Owner;
            mainform.BindBatch();
            this.Close();
        }

        private void txtBatchName_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                if (string.IsNullOrWhiteSpace(txtBatchName.Text))
                {
                    return;
                }
                if (BatchExist(txtBatchName.Text.Trim()))
                {
                    txtBatchName.Text = string.Empty;
                    return;
                }
                listBatch.Items.Add(txtBatchName.Text.Trim());
                txtBatchName.Text = string.Empty;
            }
        }
    }
}
