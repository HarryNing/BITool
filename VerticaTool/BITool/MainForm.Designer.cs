namespace BITool
{
    partial class MainForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.cBOriginDC = new System.Windows.Forms.ComboBox();
            this.btnNewSQLDS = new System.Windows.Forms.Button();
            this.btnDelSQLDS = new System.Windows.Forms.Button();
            this.cBOriginDB = new System.Windows.Forms.ComboBox();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.cBTargetDC = new System.Windows.Forms.ComboBox();
            this.btnNewVTDS = new System.Windows.Forms.Button();
            this.btnDelVTDS = new System.Windows.Forms.Button();
            this.cBTargetDB = new System.Windows.Forms.ComboBox();
            this.btnSaveTask = new System.Windows.Forms.Button();
            this.btnBatchSetting = new System.Windows.Forms.Button();
            this.lblMessage = new System.Windows.Forms.Label();
            this.IsDelta = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.BatchName = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.SourceTable = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.SourceCount = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.TargetTable = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.TargetCount = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.SuspendLayout();
            // 
            // cBOriginDC
            // 
            this.cBOriginDC.FormattingEnabled = true;
            this.cBOriginDC.Location = new System.Drawing.Point(16, 18);
            this.cBOriginDC.Name = "cBOriginDC";
            this.cBOriginDC.Size = new System.Drawing.Size(121, 21);
            this.cBOriginDC.TabIndex = 0;
            this.cBOriginDC.SelectedIndexChanged += new System.EventHandler(this.cBOriginDC_SelectedIndexChanged);
            // 
            // btnNewSQLDS
            // 
            this.btnNewSQLDS.Location = new System.Drawing.Point(154, 18);
            this.btnNewSQLDS.Name = "btnNewSQLDS";
            this.btnNewSQLDS.Size = new System.Drawing.Size(75, 23);
            this.btnNewSQLDS.TabIndex = 1;
            this.btnNewSQLDS.Text = "New";
            this.btnNewSQLDS.UseVisualStyleBackColor = true;
            this.btnNewSQLDS.Click += new System.EventHandler(this.btnNewSQLDS_Click);
            // 
            // btnDelSQLDS
            // 
            this.btnDelSQLDS.Location = new System.Drawing.Point(235, 18);
            this.btnDelSQLDS.Name = "btnDelSQLDS";
            this.btnDelSQLDS.Size = new System.Drawing.Size(75, 23);
            this.btnDelSQLDS.TabIndex = 2;
            this.btnDelSQLDS.Text = "Delete";
            this.btnDelSQLDS.UseVisualStyleBackColor = true;
            this.btnDelSQLDS.Click += new System.EventHandler(this.btnDelSQLDS_Click);
            // 
            // cBOriginDB
            // 
            this.cBOriginDB.FormattingEnabled = true;
            this.cBOriginDB.Location = new System.Drawing.Point(16, 55);
            this.cBOriginDB.Name = "cBOriginDB";
            this.cBOriginDB.Size = new System.Drawing.Size(121, 21);
            this.cBOriginDB.TabIndex = 3;
            this.cBOriginDB.SelectedIndexChanged += new System.EventHandler(this.cBOriginDB_SelectedIndexChanged);
            // 
            // dataGridView1
            // 
            this.dataGridView1.AllowUserToAddRows = false;
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.IsDelta,
            this.BatchName,
            this.SourceTable,
            this.SourceCount,
            this.TargetTable,
            this.TargetCount});
            this.dataGridView1.EditMode = System.Windows.Forms.DataGridViewEditMode.EditOnEnter;
            this.dataGridView1.Location = new System.Drawing.Point(16, 99);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.Size = new System.Drawing.Size(913, 508);
            this.dataGridView1.TabIndex = 4;
            this.dataGridView1.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView1_CellClick);
            this.dataGridView1.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView1_CellContentClick);
            this.dataGridView1.CellPainting += new System.Windows.Forms.DataGridViewCellPaintingEventHandler(this.dataGridView1_CellPainting);
            // 
            // cBTargetDC
            // 
            this.cBTargetDC.FormattingEnabled = true;
            this.cBTargetDC.Location = new System.Drawing.Point(599, 18);
            this.cBTargetDC.Name = "cBTargetDC";
            this.cBTargetDC.Size = new System.Drawing.Size(121, 21);
            this.cBTargetDC.TabIndex = 0;
            this.cBTargetDC.SelectedIndexChanged += new System.EventHandler(this.cBTargetDC_SelectedIndexChanged);
            // 
            // btnNewVTDS
            // 
            this.btnNewVTDS.Location = new System.Drawing.Point(737, 18);
            this.btnNewVTDS.Name = "btnNewVTDS";
            this.btnNewVTDS.Size = new System.Drawing.Size(75, 23);
            this.btnNewVTDS.TabIndex = 1;
            this.btnNewVTDS.Text = "New";
            this.btnNewVTDS.UseVisualStyleBackColor = true;
            this.btnNewVTDS.Click += new System.EventHandler(this.btnNewVTDS_Click);
            // 
            // btnDelVTDS
            // 
            this.btnDelVTDS.Location = new System.Drawing.Point(818, 18);
            this.btnDelVTDS.Name = "btnDelVTDS";
            this.btnDelVTDS.Size = new System.Drawing.Size(75, 23);
            this.btnDelVTDS.TabIndex = 2;
            this.btnDelVTDS.Text = "Delete";
            this.btnDelVTDS.UseVisualStyleBackColor = true;
            this.btnDelVTDS.Click += new System.EventHandler(this.btnDelVTDS_Click);
            // 
            // cBTargetDB
            // 
            this.cBTargetDB.FormattingEnabled = true;
            this.cBTargetDB.Location = new System.Drawing.Point(599, 55);
            this.cBTargetDB.Name = "cBTargetDB";
            this.cBTargetDB.Size = new System.Drawing.Size(121, 21);
            this.cBTargetDB.TabIndex = 3;
            this.cBTargetDB.SelectedIndexChanged += new System.EventHandler(this.cBTargetDB_SelectedIndexChanged);
            // 
            // btnSaveTask
            // 
            this.btnSaveTask.Location = new System.Drawing.Point(408, 55);
            this.btnSaveTask.Name = "btnSaveTask";
            this.btnSaveTask.Size = new System.Drawing.Size(99, 29);
            this.btnSaveTask.TabIndex = 5;
            this.btnSaveTask.Text = "Save Batch";
            this.btnSaveTask.UseVisualStyleBackColor = true;
            this.btnSaveTask.Click += new System.EventHandler(this.btnSaveTask_Click);
            // 
            // btnBatchSetting
            // 
            this.btnBatchSetting.Location = new System.Drawing.Point(408, 18);
            this.btnBatchSetting.Name = "btnBatchSetting";
            this.btnBatchSetting.Size = new System.Drawing.Size(99, 29);
            this.btnBatchSetting.TabIndex = 6;
            this.btnBatchSetting.Text = "Batch Setting";
            this.btnBatchSetting.UseVisualStyleBackColor = true;
            this.btnBatchSetting.Click += new System.EventHandler(this.btnBatchSetting_Click);
            // 
            // lblMessage
            // 
            this.lblMessage.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lblMessage.ForeColor = System.Drawing.Color.Red;
            this.lblMessage.Location = new System.Drawing.Point(734, 58);
            this.lblMessage.Name = "lblMessage";
            this.lblMessage.Size = new System.Drawing.Size(190, 28);
            this.lblMessage.TabIndex = 8;
            this.lblMessage.Text = "Message Box";
            // 
            // IsDelta
            // 
            this.IsDelta.DataPropertyName = "IsDelta";
            this.IsDelta.FalseValue = "false";
            this.IsDelta.FillWeight = 50F;
            this.IsDelta.Frozen = true;
            this.IsDelta.HeaderText = "Is Delta";
            this.IsDelta.Name = "IsDelta";
            this.IsDelta.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.IsDelta.TrueValue = "true";
            this.IsDelta.Width = 50;
            // 
            // BatchName
            // 
            this.BatchName.DataPropertyName = "BatchName";
            this.BatchName.Frozen = true;
            this.BatchName.HeaderText = "Batch";
            this.BatchName.Name = "BatchName";
            this.BatchName.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.BatchName.Width = 105;
            // 
            // SourceTable
            // 
            this.SourceTable.DataPropertyName = "SourceTable";
            this.SourceTable.FillWeight = 110F;
            this.SourceTable.Frozen = true;
            this.SourceTable.HeaderText = "Source Table";
            this.SourceTable.Name = "SourceTable";
            this.SourceTable.ReadOnly = true;
            this.SourceTable.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.SourceTable.Width = 120;
            // 
            // SourceCount
            // 
            this.SourceCount.DataPropertyName = "SourceTableCount";
            this.SourceCount.FillWeight = 80F;
            this.SourceCount.Frozen = true;
            this.SourceCount.HeaderText = "Count";
            this.SourceCount.Name = "SourceCount";
            this.SourceCount.ReadOnly = true;
            this.SourceCount.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.SourceCount.Width = 80;
            // 
            // TargetTable
            // 
            this.TargetTable.DataPropertyName = "TargetTable";
            this.TargetTable.FillWeight = 110F;
            this.TargetTable.Frozen = true;
            this.TargetTable.HeaderText = "Target Table";
            this.TargetTable.Name = "TargetTable";
            this.TargetTable.ReadOnly = true;
            this.TargetTable.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.TargetTable.Width = 117;
            // 
            // TargetCount
            // 
            this.TargetCount.DataPropertyName = "TargetTableCount";
            this.TargetCount.FillWeight = 80F;
            this.TargetCount.HeaderText = "Count";
            this.TargetCount.Name = "TargetCount";
            this.TargetCount.ReadOnly = true;
            this.TargetCount.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.TargetCount.Width = 80;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(948, 619);
            this.Controls.Add(this.lblMessage);
            this.Controls.Add(this.btnBatchSetting);
            this.Controls.Add(this.btnSaveTask);
            this.Controls.Add(this.dataGridView1);
            this.Controls.Add(this.cBTargetDB);
            this.Controls.Add(this.cBOriginDB);
            this.Controls.Add(this.btnDelVTDS);
            this.Controls.Add(this.btnNewVTDS);
            this.Controls.Add(this.btnDelSQLDS);
            this.Controls.Add(this.cBTargetDC);
            this.Controls.Add(this.btnNewSQLDS);
            this.Controls.Add(this.cBOriginDC);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "BITool";
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ComboBox cBOriginDC;
        private System.Windows.Forms.Button btnNewSQLDS;
        private System.Windows.Forms.Button btnDelSQLDS;
        private System.Windows.Forms.ComboBox cBOriginDB;
        private System.Windows.Forms.ComboBox cBTargetDC;
        private System.Windows.Forms.Button btnNewVTDS;
        private System.Windows.Forms.Button btnDelVTDS;
        private System.Windows.Forms.ComboBox cBTargetDB;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.Button btnSaveTask;
        private System.Windows.Forms.Button btnBatchSetting;
        private System.Windows.Forms.Label lblMessage;
        private System.Windows.Forms.DataGridViewCheckBoxColumn IsDelta;
        private System.Windows.Forms.DataGridViewComboBoxColumn BatchName;
        private System.Windows.Forms.DataGridViewTextBoxColumn SourceTable;
        private System.Windows.Forms.DataGridViewTextBoxColumn SourceCount;
        private System.Windows.Forms.DataGridViewTextBoxColumn TargetTable;
        private System.Windows.Forms.DataGridViewTextBoxColumn TargetCount;
    }
}