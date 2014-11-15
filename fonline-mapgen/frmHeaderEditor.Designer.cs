namespace fonline_mapgen
{
    partial class frmHeaderEditor
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose( bool disposing )
        {
            if( disposing && (components != null) )
            {
                components.Dispose();
            }
            base.Dispose( disposing );
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.txtModule = new System.Windows.Forms.TextBox();
            this.labelAt = new System.Windows.Forms.Label();
            this.txtFunction = new System.Windows.Forms.TextBox();
            this.labelModule = new System.Windows.Forms.Label();
            this.labelFunction = new System.Windows.Forms.Label();
            this.checkNoLogout = new System.Windows.Forms.CheckBox();
            this.colorDialog = new System.Windows.Forms.ColorDialog();
            this.checkScripted = new System.Windows.Forms.CheckBox();
            this.labelVersion = new System.Windows.Forms.Label();
            this.txtVersion = new System.Windows.Forms.TextBox();
            this.labelSize = new System.Windows.Forms.Label();
            this.txtSizeW = new System.Windows.Forms.TextBox();
            this.txtSizeH = new System.Windows.Forms.TextBox();
            this.labelSizeX = new System.Windows.Forms.Label();
            this.groupMeta = new System.Windows.Forms.GroupBox();
            this.groupUser = new System.Windows.Forms.GroupBox();
            this.labelDayColor = new System.Windows.Forms.Label();
            this.dayColors = new System.Windows.Forms.TableLayoutPanel();
            this.btnSave = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.groupMeta.SuspendLayout();
            this.groupUser.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // txtModule
            // 
            this.txtModule.Location = new System.Drawing.Point(9, 35);
            this.txtModule.Name = "txtModule";
            this.txtModule.Size = new System.Drawing.Size(100, 20);
            this.txtModule.TabIndex = 0;
            // 
            // labelAt
            // 
            this.labelAt.Location = new System.Drawing.Point(110, 38);
            this.labelAt.Name = "labelAt";
            this.labelAt.Size = new System.Drawing.Size(40, 13);
            this.labelAt.TabIndex = 1;
            this.labelAt.Text = "@";
            this.labelAt.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // txtFunction
            // 
            this.txtFunction.Location = new System.Drawing.Point(151, 35);
            this.txtFunction.Name = "txtFunction";
            this.txtFunction.Size = new System.Drawing.Size(100, 20);
            this.txtFunction.TabIndex = 2;
            // 
            // labelModule
            // 
            this.labelModule.AutoSize = true;
            this.labelModule.Location = new System.Drawing.Point(10, 16);
            this.labelModule.Name = "labelModule";
            this.labelModule.Size = new System.Drawing.Size(42, 13);
            this.labelModule.TabIndex = 3;
            this.labelModule.Text = "Module";
            // 
            // labelFunction
            // 
            this.labelFunction.AutoSize = true;
            this.labelFunction.Location = new System.Drawing.Point(148, 16);
            this.labelFunction.Name = "labelFunction";
            this.labelFunction.Size = new System.Drawing.Size(48, 13);
            this.labelFunction.TabIndex = 4;
            this.labelFunction.Text = "Function";
            // 
            // checkNoLogout
            // 
            this.checkNoLogout.AutoSize = true;
            this.checkNoLogout.Location = new System.Drawing.Point(9, 84);
            this.checkNoLogout.Name = "checkNoLogout";
            this.checkNoLogout.Size = new System.Drawing.Size(72, 17);
            this.checkNoLogout.TabIndex = 5;
            this.checkNoLogout.Text = "No logout";
            this.checkNoLogout.UseVisualStyleBackColor = true;
            // 
            // colorDialog
            // 
            this.colorDialog.Color = System.Drawing.Color.Pink;
            this.colorDialog.FullOpen = true;
            this.colorDialog.ShowHelp = true;
            this.colorDialog.SolidColorOnly = true;
            // 
            // checkScripted
            // 
            this.checkScripted.AutoSize = true;
            this.checkScripted.Location = new System.Drawing.Point(9, 61);
            this.checkScripted.Name = "checkScripted";
            this.checkScripted.Size = new System.Drawing.Size(65, 17);
            this.checkScripted.TabIndex = 6;
            this.checkScripted.Text = "Scripted";
            this.checkScripted.UseVisualStyleBackColor = true;
            this.checkScripted.CheckedChanged += new System.EventHandler(this.checkScripted_CheckedChanged);
            // 
            // labelVersion
            // 
            this.labelVersion.AutoSize = true;
            this.labelVersion.Location = new System.Drawing.Point(6, 16);
            this.labelVersion.Name = "labelVersion";
            this.labelVersion.Size = new System.Drawing.Size(42, 13);
            this.labelVersion.TabIndex = 10;
            this.labelVersion.Text = "Version";
            // 
            // txtVersion
            // 
            this.txtVersion.Location = new System.Drawing.Point(151, 13);
            this.txtVersion.Name = "txtVersion";
            this.txtVersion.ReadOnly = true;
            this.txtVersion.Size = new System.Drawing.Size(100, 20);
            this.txtVersion.TabIndex = 11;
            this.txtVersion.Text = "0";
            this.txtVersion.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // labelSize
            // 
            this.labelSize.AutoSize = true;
            this.labelSize.Location = new System.Drawing.Point(6, 43);
            this.labelSize.Name = "labelSize";
            this.labelSize.Size = new System.Drawing.Size(27, 13);
            this.labelSize.TabIndex = 12;
            this.labelSize.Text = "Size";
            // 
            // txtSizeW
            // 
            this.txtSizeW.Location = new System.Drawing.Point(151, 40);
            this.txtSizeW.Name = "txtSizeW";
            this.txtSizeW.ReadOnly = true;
            this.txtSizeW.Size = new System.Drawing.Size(40, 20);
            this.txtSizeW.TabIndex = 13;
            this.txtSizeW.Text = "0";
            this.txtSizeW.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // txtSizeH
            // 
            this.txtSizeH.Location = new System.Drawing.Point(211, 40);
            this.txtSizeH.Name = "txtSizeH";
            this.txtSizeH.ReadOnly = true;
            this.txtSizeH.Size = new System.Drawing.Size(40, 20);
            this.txtSizeH.TabIndex = 14;
            this.txtSizeH.Text = "0";
            this.txtSizeH.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // labelSizeX
            // 
            this.labelSizeX.Location = new System.Drawing.Point(191, 40);
            this.labelSizeX.Name = "labelSizeX";
            this.labelSizeX.Size = new System.Drawing.Size(20, 20);
            this.labelSizeX.TabIndex = 15;
            this.labelSizeX.Text = "x";
            this.labelSizeX.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // groupMeta
            // 
            this.groupMeta.AutoSize = true;
            this.groupMeta.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.groupMeta.Controls.Add(this.labelVersion);
            this.groupMeta.Controls.Add(this.txtSizeW);
            this.groupMeta.Controls.Add(this.txtVersion);
            this.groupMeta.Controls.Add(this.txtSizeH);
            this.groupMeta.Controls.Add(this.labelSize);
            this.groupMeta.Controls.Add(this.labelSizeX);
            this.groupMeta.Dock = System.Windows.Forms.DockStyle.Top;
            this.groupMeta.Location = new System.Drawing.Point(3, 3);
            this.groupMeta.Name = "groupMeta";
            this.groupMeta.Size = new System.Drawing.Size(263, 79);
            this.groupMeta.TabIndex = 16;
            this.groupMeta.TabStop = false;
            // 
            // groupUser
            // 
            this.groupUser.Controls.Add(this.labelDayColor);
            this.groupUser.Controls.Add(this.dayColors);
            this.groupUser.Controls.Add(this.labelModule);
            this.groupUser.Controls.Add(this.txtModule);
            this.groupUser.Controls.Add(this.labelAt);
            this.groupUser.Controls.Add(this.txtFunction);
            this.groupUser.Controls.Add(this.labelFunction);
            this.groupUser.Controls.Add(this.checkScripted);
            this.groupUser.Controls.Add(this.checkNoLogout);
            this.groupUser.Dock = System.Windows.Forms.DockStyle.Top;
            this.groupUser.Location = new System.Drawing.Point(3, 82);
            this.groupUser.Name = "groupUser";
            this.groupUser.Size = new System.Drawing.Size(263, 145);
            this.groupUser.TabIndex = 17;
            this.groupUser.TabStop = false;
            // 
            // labelDayColor
            // 
            this.labelDayColor.AutoSize = true;
            this.labelDayColor.Location = new System.Drawing.Point(6, 107);
            this.labelDayColor.Name = "labelDayColor";
            this.labelDayColor.Size = new System.Drawing.Size(52, 13);
            this.labelDayColor.TabIndex = 9;
            this.labelDayColor.Text = "Day color";
            // 
            // dayColors
            // 
            this.dayColors.ColumnCount = 7;
            this.dayColors.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 22F));
            this.dayColors.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 4F));
            this.dayColors.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 22F));
            this.dayColors.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 4F));
            this.dayColors.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 22F));
            this.dayColors.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 4F));
            this.dayColors.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 22F));
            this.dayColors.Location = new System.Drawing.Point(151, 99);
            this.dayColors.Name = "dayColors";
            this.dayColors.RowCount = 1;
            this.dayColors.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.dayColors.Size = new System.Drawing.Size(100, 21);
            this.dayColors.TabIndex = 8;
            this.dayColors.CellPaint += new System.Windows.Forms.TableLayoutCellPaintEventHandler(this.dayColors_CellPaint);
            this.dayColors.Click += new System.EventHandler(this.dayColors_Click);
            // 
            // btnSave
            // 
            this.btnSave.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.btnSave.Location = new System.Drawing.Point(5, 5);
            this.btnSave.Margin = new System.Windows.Forms.Padding(5);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(75, 25);
            this.btnSave.TabIndex = 18;
            this.btnSave.Text = "Save";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.btnCancel.Location = new System.Drawing.Point(183, 5);
            this.btnCancel.Margin = new System.Windows.Forms.Padding(5);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 25);
            this.btnCancel.TabIndex = 19;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Controls.Add(this.btnSave, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.btnCancel, 1, 0);
            this.tableLayoutPanel1.Location = new System.Drawing.Point(3, 227);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 1;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(263, 35);
            this.tableLayoutPanel1.TabIndex = 20;
            // 
            // frmHeaderEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.ClientSize = new System.Drawing.Size(269, 262);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Controls.Add(this.groupUser);
            this.Controls.Add(this.groupMeta);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "frmHeaderEditor";
            this.Padding = new System.Windows.Forms.Padding(3);
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "frmHeaderEditor";
            this.groupMeta.ResumeLayout(false);
            this.groupMeta.PerformLayout();
            this.groupUser.ResumeLayout(false);
            this.groupUser.PerformLayout();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtModule;
        private System.Windows.Forms.Label labelAt;
        private System.Windows.Forms.TextBox txtFunction;
        private System.Windows.Forms.Label labelModule;
        private System.Windows.Forms.Label labelFunction;
        private System.Windows.Forms.CheckBox checkNoLogout;
        private System.Windows.Forms.ColorDialog colorDialog;
        private System.Windows.Forms.CheckBox checkScripted;
        private System.Windows.Forms.Label labelVersion;
        private System.Windows.Forms.TextBox txtVersion;
        private System.Windows.Forms.Label labelSize;
        private System.Windows.Forms.TextBox txtSizeW;
        private System.Windows.Forms.TextBox txtSizeH;
        private System.Windows.Forms.Label labelSizeX;
        private System.Windows.Forms.GroupBox groupMeta;
        private System.Windows.Forms.GroupBox groupUser;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.TableLayoutPanel dayColors;
        private System.Windows.Forms.Label labelDayColor;
    }
}