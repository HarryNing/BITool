using BITool.Extension;
using BIToolUtility;
using BIToolUtility.Common;
using BIToolUtility.Configuration;
using BIToolUtility.Helper;
using BIToolUtility.Log;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Vertica.Data.VerticaClient;

namespace BITool
{
    public partial class MainForm : Form
    {
        private Connection currentOriginConnection
        {
            get
            {
                return (Connection)cBOriginDC.SelectedItem;
            }
        }
        private Connection currentTargetConnection
        {
            get
            {
                return (Connection)cBTargetDC.SelectedItem;
            }
        }
        public MainForm()
        {
            InitializeComponent();
            lblMessage.Text = string.Empty;
            dataGridView1.AutoGenerateColumns = false;
            DataGridViewButtonColumn btnCreateTable = new DataGridViewButtonColumn();
            btnCreateTable.Text = "Create Table";
            btnCreateTable.HeaderText = "Create Table";
            btnCreateTable.Name = "btnCreateTable";
            btnCreateTable.UseColumnTextForButtonValue = true;
            btnCreateTable.Width = 100;
            btnCreateTable.Resizable = DataGridViewTriState.False;
            dataGridView1.Columns.Add(btnCreateTable);

            DataGridViewButtonColumn btnSync = new DataGridViewButtonColumn();
            btnSync.Text = "Sync Data";
            btnSync.HeaderText = "Sync Data";
            btnSync.Name = "btnSync";
            btnSync.UseColumnTextForButtonValue = true;
            btnSync.Width = 100;
            btnSync.Resizable = DataGridViewTriState.False;
            dataGridView1.Columns.Add(btnSync);

            DataGridViewProgressColumn prBar = new DataGridViewProgressColumn();
            prBar.DataPropertyName = "PRValue";
            prBar.HeaderText = "Sync Progress";
            prBar.Resizable = DataGridViewTriState.False;
            dataGridView1.Columns.Add(prBar);

            BindBatch();

            this.cBOriginDC.DataSource = BIToolConfigHelper.Config.SourceConnections;
            this.cBOriginDC.DisplayMember = "Name";

            this.cBTargetDC.DataSource = BIToolConfigHelper.Config.TargetConnections;
            this.cBTargetDC.DisplayMember = "Name";
        }

        private void BindDCDataSource(Connection oSelectedItem, Connection tSelectedItem)
        {
            this.cBOriginDC.DataSource = null;
            this.cBOriginDC.DataSource = BIToolConfigHelper.Config.SourceConnections;
            this.cBOriginDC.SelectedItem = oSelectedItem;
            this.cBOriginDC.DisplayMember = "Name";

            this.cBTargetDC.DataSource = null;
            this.cBTargetDC.DataSource = BIToolConfigHelper.Config.TargetConnections;
            this.cBTargetDC.SelectedItem = tSelectedItem;
            this.cBTargetDC.DisplayMember = "Name";
        }

        private void btnNewSQLDS_Click(object sender, EventArgs e)
        {
            DbConnectionSetting dbconnection = new DbConnectionSetting();
            dbconnection.Connection = new Connection();
            dbconnection.Connection.ConnectionType = ConnectionType.MSSQLSERVER;

            if (dbconnection.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                BIToolConfigHelper.AddSourceConnection(dbconnection.Connection);
                BindDCDataSource(dbconnection.Connection, currentTargetConnection);
            }
        }

        private void btnNewVTDS_Click(object sender, EventArgs e)
        {
            DbConnectionSetting dbconnection = new DbConnectionSetting();
            dbconnection.Connection = new Connection();
            dbconnection.Connection.ConnectionType = ConnectionType.Vertica;

            if (dbconnection.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                BIToolConfigHelper.AddTargetConnection(dbconnection.Connection);
                BindDCDataSource(currentOriginConnection, dbconnection.Connection);
            }
        }

        private void RefreshOriginDatabase()
        {
            if (this.cBOriginDC.SelectedItem == null)
            {
                this.cBOriginDB.Items.Clear();
                this.cBOriginDB.Text = string.Empty;
                return;
            }
            try
            {
                if (this.cBOriginDC.SelectedItem != null)
                {
                    this.cBOriginDB.Items.Clear();
                    this.cBOriginDB.Text = string.Empty;
                    using (SqlConnection connection = new SqlConnection(currentOriginConnection.ConnectionString))
                    {
                        connection.Open();
                        SqlCommand command = new SqlCommand("select * from sys.databases where database_id>4;", connection);
                        SqlDataReader reader = command.ExecuteReader();
                        while (reader.Read())
                        {
                            this.cBOriginDB.Items.Add(reader["name"]);
                        }
                        connection.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }


        private void RefreshTargetDatabase()
        {
            if (this.cBTargetDC.SelectedItem == null)
            {
                this.cBTargetDB.Items.Clear();
                this.cBTargetDB.Text = string.Empty;
                return;
            }
            try
            {
                if (this.cBTargetDC.SelectedItem != null)
                {
                    this.cBTargetDB.Items.Clear();
                    this.cBTargetDB.Text = string.Empty;
                    using (VerticaConnection connection = new VerticaConnection(currentTargetConnection.ConnectionString))
                    {
                        connection.Open();
                        VerticaCommand command = new VerticaCommand("select * from schemata where is_system_schema='f';", connection);
                        VerticaDataReader reader = command.ExecuteReader();
                        while (reader.Read())
                        {
                            this.cBTargetDB.Items.Add(reader["schema_name"]);
                        }
                        connection.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnDelSQLDS_Click(object sender, EventArgs e)
        {
            if (BIToolConfigHelper.Config.SourceConnections.Count == 1)
            {
                MessageBox.Show("One target connection at least");
                return;
            }
            if (DialogResult.OK == MessageBox.Show("Delete this connection?", "Confirm Message",
                MessageBoxButtons.OKCancel, MessageBoxIcon.Question))
            {
                BIToolConfigHelper.Config.SourceConnections.Remove(currentOriginConnection);
                BIToolConfigHelper.SaveConfig();
                BindDCDataSource(BIToolConfigHelper.Config.SourceConnections.First(), currentTargetConnection);
            }
        }

        private void btnDelVTDS_Click(object sender, EventArgs e)
        {
            if (BIToolConfigHelper.Config.TargetConnections.Count == 1)
            {
                MessageBox.Show("One origin connection at least");
                return;
            }
            if (DialogResult.OK == MessageBox.Show("Delete this connection?", "Confirm Message",
                MessageBoxButtons.OKCancel, MessageBoxIcon.Question))
            {

                BIToolConfigHelper.Config.TargetConnections.Remove(currentTargetConnection);
                BIToolConfigHelper.SaveConfig();
                BindDCDataSource(currentOriginConnection, BIToolConfigHelper.Config.TargetConnections.First());
            }
        }

        private void dataGridView1_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            //首先判断当前cell是不是delete button
            if ((e.ColumnIndex == dataGridView1.Columns["btnCreateTable"].Index)
                && e.RowIndex >= 0
                && e.RowIndex < dataGridView1.Rows.Count)
            {
                TableData data = TableDataHelper.SourceTableList[e.RowIndex];
                //如果CanDel = 0，重绘按钮为disable状态
                if (e.ColumnIndex == dataGridView1.Columns["btnCreateTable"].Index && !data.NotExist)
                {
                    Rectangle newRect = new Rectangle(e.CellBounds.X + 1,
                    e.CellBounds.Y + 1, e.CellBounds.Width - 4,
                    e.CellBounds.Height - 4);
                    using (Brush gridBrush = new SolidBrush(this.dataGridView1.GridColor),
                            backColorBrush = new SolidBrush(e.CellStyle.BackColor))
                    {
                        //先抹去原来的cell背景
                        e.Graphics.FillRectangle(backColorBrush, e.CellBounds);
                        using (Pen gridLinePen = new Pen(gridBrush))
                        {
                            // 画出上下两条边线，左右边线无需画
                            e.Graphics.DrawLine(gridLinePen, e.CellBounds.Left,
                                e.CellBounds.Bottom - 1, e.CellBounds.Right - 1,
                                e.CellBounds.Bottom - 1);
                            e.Graphics.DrawLine(gridLinePen, e.CellBounds.Right - 1,
                                e.CellBounds.Top, e.CellBounds.Right - 1,
                                e.CellBounds.Bottom);
                            // 计算button的大小
                            Rectangle buttonArea = e.CellBounds;
                            buttonArea.X += 1;
                            buttonArea.Y += 1;
                            buttonArea.Height -= 2;
                            buttonArea.Width -= 2;

                            // 画按钮
                            ButtonRenderer.DrawButton(e.Graphics, buttonArea,
                                System.Windows.Forms.VisualStyles.PushButtonState.Disabled);

                            // 画文字，用灰色表示disable状态
                            if (e.ColumnIndex == dataGridView1.Columns["btnCreateTable"].Index)
                            {
                                TextRenderer.DrawText(e.Graphics, "Create Table", dataGridView1.Font, buttonArea, SystemColors.GrayText);
                            }
                        }
                    }
                    e.Handled = true;
                }
            }
        }

        private void cBOriginDB_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.cBOriginDB.SelectedItem == null)
            {
                return;
            }
            try
            {
                if (!this.cBOriginDB.SelectedItem.Equals(string.Empty))
                {
                    currentOriginConnection.Database = this.cBOriginDB.SelectedItem.ToString();
                    using (SqlConnection connection = new SqlConnection(currentOriginConnection.ConnectionString))
                    {
                        connection.Open();
                        SqlCommand command = new SqlCommand(QueryHelper.GetSourceTableQuery, connection);
                        SqlDataReader reader = command.ExecuteReader();
                        List<TableData> sourceTableList = new List<TableData>();
                        while (reader.Read())
                        {
                            sourceTableList.Add(new TableData()
                            {
                                SourceTable = reader["TableName"].ToString(),
                                SourceTableCount = Convert.ToInt32(reader["RowsCount"]),
                                CreateTableText = reader["CreateTableText"].ToString(),
                                TargetTable = string.Empty,
                                TargetTableCount = 0,
                                PRValue = 0
                            });
                        }
                        TableDataHelper.SourceTableList = sourceTableList;
                        TableDataHelper.RefreshData();
                        dataGridView1.DataSource = new BindingCollection<TableData>(TableDataHelper.SourceTableList);
                        connection.Close();
                    }
                }
                else
                {
                    TableDataHelper.SourceTableList = new List<TableData>();
                    TableDataHelper.RefreshData();
                    dataGridView1.DataSource = new BindingCollection<TableData>(TableDataHelper.SourceTableList);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void cBTargetDB_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.cBTargetDB.SelectedItem == null)
            {
                return;
            }
            try
            {
                if (!this.cBTargetDB.SelectedItem.Equals(string.Empty))
                {
                    using (VerticaConnection connection = new VerticaConnection(currentTargetConnection.ConnectionString))
                    {
                        connection.Open();
                        VerticaCommand command = new VerticaCommand(QueryHelper.GetTargetTableQuery
                            .Replace("@@", cBTargetDB.SelectedItem.ToString()), connection);
                        VerticaDataReader reader = command.ExecuteReader();
                        List<TableData> targetTableList = new List<TableData>();
                        while (reader.Read())
                        {
                            targetTableList.Add(new TableData()
                            {
                                TargetTable = reader["table_name"].ToString(),
                                //TargetTableCount = Convert.ToInt32(reader["row_count"]),//Not accurate,just projection storage
                                TargetColumns = reader["column_names"].ToString()
                            });
                        }
                        TableDataHelper.TargetTableList = targetTableList;
                        TableDataHelper.RefreshData();
                        dataGridView1.Invalidate();
                        connection.Close();
                    }
                }
                else
                {
                    TableDataHelper.TargetTableList = new List<TableData>();
                    TableDataHelper.RefreshData();
                    dataGridView1.Invalidate();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private async void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || cBTargetDB.SelectedItem == null)
            {
                return;
            }
            TableData data = TableDataHelper.SourceTableList[e.RowIndex];
            if (e.ColumnIndex == dataGridView1.Columns["btnCreateTable"].Index)
            {
                if (!data.NotExist)
                {
                    return;
                }
                string createCommandText = data.CreateTableText.Replace("@@", cBTargetDB.SelectedItem.ToString());
                string getColumnsText = string.Format(QueryHelper.GetTargetColumnsQuery, cBTargetDB.SelectedItem.ToString(), data.SourceTable);
                try
                {
                    using (VerticaConnection connection = new VerticaConnection(currentTargetConnection.ConnectionString))
                    {
                        connection.Open();
                        VerticaCommand createCommand = new VerticaCommand(createCommandText, connection);
                        createCommand.ExecuteNonQuery();
                        VerticaDataReader reader = new VerticaCommand(getColumnsText, connection).ExecuteReader();
                        while (reader.Read())
                        {
                            data.TargetColumns = reader["column_names"].ToString();
                        }

                        connection.Close();
                        data.TargetTable = data.SourceTable;
                        data.TargetTableCount = 0;
                        TableDataHelper.TargetTableList.Add(new TableData() { TargetTable = data.SourceTable, TargetTableCount = 0 });
                        dataGridView1.InvalidateRow(e.RowIndex);
                        lblMessage.Text = "Table created in Vertica";
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
            else if (e.ColumnIndex == dataGridView1.Columns["btnSync"].Index)
            {
                if (string.IsNullOrEmpty(data.TargetTable))
                {
                    MessageBox.Show("Target table not exist, create it first");
                    return;
                }
                lblMessage.Text = "";
                data.PRValue = 0;
                ILogEx log = EtlLog.NewLogger(data.SourceTable);
                Transform config = new Transform()
                {
                    SqlConnection = currentOriginConnection.ConnectionString,
                    VerticaConnection = currentTargetConnection.ConnectionString,
                    SelectText = string.Format("select {0} from {1}", data.TargetColumns, data.SourceTable),
                    IsDeltaLoad = data.IsDelta,
                    TargetTableName = cBTargetDB.SelectedItem.ToString() + "." + data.TargetTable,
                    SourceTableName = data.SourceTable,
                    BatchCount = GetBatchCount(data)
                };
                BIToolClient client = new BIToolClient(config, log);
                client.ProgressReportChanged += new EventHandler<ProgressReportArgs>((obj, args) =>
                {
                    data.TargetTableCount = args.TotalCount;
                    data.PRValue = args.PRValue;
                    dataGridView1.InvalidateRow(e.RowIndex);
                });
                await Task.Run(() => { client.ExecuteTransformAsync(); });
            }
        }

        private static int GetBatchCount(TableData data)
        {
            if (data.SourceTableCount < 10 * 10000)
            {
                return 10000;
            }
            else if (data.SourceTableCount < 100 * 10000)
            {
                return 5 * 10000;
            }
            else if (data.SourceTableCount < 500 * 10000)
            {
                return 10 * 10000;
            }
            else if (data.SourceTableCount < 1000 * 10000)
            {
                return 25 * 10000;
            }
            else
            {
                return 50 * 10000;
            }
        }

        private void btnSaveTask_Click(object sender, EventArgs e)
        {
            if (TableDataHelper.SourceTableList == null)
            {
                MessageBox.Show("Source DB not selected");
                return;
            }
            if (TableDataHelper.TargetTableList == null)
            {
                MessageBox.Show("Target DB not selected");
                return;
            }
            List<TableData> batchedList = TableDataHelper.SourceTableList
                .Where(t => !string.IsNullOrEmpty(t.BatchName)
                         && !string.IsNullOrEmpty(t.TargetTable)
                         ).ToList();
            if (batchedList.Count == 0)
            {
                MessageBox.Show("Select at least one batch");
                return;
            }
            List<BatchConfig> batchConfigs = new List<BatchConfig>();
            foreach (var tableGroup in batchedList.GroupBy(p => p.BatchName))
            {
                List<Transform> transforms = new List<Transform>();
                foreach (var table in tableGroup)
                {
                    transforms.Add(GetTransform(table));
                }
                BatchConfig config = new BatchConfig()
                {
                    BatchName = tableGroup.Key,
                    Transforms = transforms
                };
                batchConfigs.Add(config);
            }
            BIToolConfigHelper.SaveBatchConfig(batchConfigs);
            lblMessage.Text = "Batch config save successfully";
        }

        private Transform GetTransform(TableData table)
        {
            Transform transform = new Transform()
            {
                SqlConnection = currentOriginConnection.ConnectionString,
                VerticaConnection = currentTargetConnection.ConnectionString,
                SelectText = string.Format("select {0} from {1}", table.TargetColumns, table.SourceTable),
                IsDeltaLoad = table.IsDelta,
                TargetTableName = cBTargetDB.SelectedItem.ToString() + "." + table.TargetTable,
                SourceTableName = table.SourceTable,
                BatchCount = GetBatchCount(table)
            };
            return transform;
        }

        private void btnBatchSetting_Click(object sender, EventArgs e)
        {
            BatchSetting batchSetting = new BatchSetting();
            batchSetting.ShowDialog(this);
        }

        private void cBOriginDC_SelectedIndexChanged(object sender, EventArgs e)
        {
            RefreshOriginDatabase();
        }

        private void cBTargetDC_SelectedIndexChanged(object sender, EventArgs e)
        {
            RefreshTargetDatabase();
        }

        internal void BindBatch()
        {
            if (dataGridView1.IsCurrentCellDirty)
            {
                dataGridView1.CommitEdit(DataGridViewDataErrorContexts.Commit);
            }
            TableDataHelper.RefreshData();
            DataGridViewComboBoxColumn cbBatch = (DataGridViewComboBoxColumn)dataGridView1.Columns[1];
            List<string> batches = new List<string>();
            batches.Add(string.Empty);
            foreach (var item in BIToolConfigHelper.Config.Batches)
            {
                batches.Add(item);
            }
            cbBatch.DataSource = batches;
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || cBTargetDB.SelectedItem == null)
            {
                return;
            }
            TableData data = TableDataHelper.SourceTableList[e.RowIndex];
            if (e.ColumnIndex == dataGridView1.Columns["TargetCount"].Index)
            {
                if (data.TargetTable != string.Empty)
                {
                    data.TargetTableCount = GetTargetTableCount(data.TargetTable);
                    dataGridView1.InvalidateCell(e.ColumnIndex, e.RowIndex);
                }
            }
        }

        private int GetTargetTableCount(string targetTable)
        {
            int targetTableCount = 0;
            try
            {
                if (!this.cBTargetDB.SelectedItem.Equals(string.Empty))
                {
                    using (VerticaConnection connection = new VerticaConnection(currentTargetConnection.ConnectionString))
                    {
                        connection.Open();
                        string commandText = string.Format("select count(1) as table_count from {0}.{1};", cBTargetDB.SelectedItem.ToString(), targetTable);
                        VerticaCommand command = new VerticaCommand(commandText, connection);
                        VerticaDataReader reader = command.ExecuteReader();
                        while (reader.Read())
                        {
                            targetTableCount = Convert.ToInt32(reader["table_count"]);
                        }
                        connection.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            return targetTableCount;
        }
    }
}
