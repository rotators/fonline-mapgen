namespace fonline_mapgen
{
    partial class frmPaths
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
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.btnSetFOOBJDir = new System.Windows.Forms.Button();
            this.lblFOOBJ = new System.Windows.Forms.Label();
            this.txtFOOBJ = new System.Windows.Forms.TextBox();
            this.btnSetMapsDir = new System.Windows.Forms.Button();
            this.lblMapsDir = new System.Windows.Forms.Label();
            this.txtMapsDir = new System.Windows.Forms.TextBox();
            this.btnSetCritterTypes = new System.Windows.Forms.Button();
            this.lblCritterTypes = new System.Windows.Forms.Label();
            this.txtCritterTypes = new System.Windows.Forms.TextBox();
            this.btnSetItemProtosDir = new System.Windows.Forms.Button();
            this.lblItemProtos = new System.Windows.Forms.Label();
            this.txtItemProtos = new System.Windows.Forms.TextBox();
            this.btnSetCritterProtosDir = new System.Windows.Forms.Button();
            this.lblCritterProtos = new System.Windows.Forms.Label();
            this.txtCritterProtos = new System.Windows.Forms.TextBox();
            this.lblBasePath = new System.Windows.Forms.Label();
            this.btnSetBasePaths = new System.Windows.Forms.Button();
            this.txtBasePath = new System.Windows.Forms.TextBox();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.btnDownDir = new System.Windows.Forms.Button();
            this.btnUpDir = new System.Windows.Forms.Button();
            this.lstDataDirs = new System.Windows.Forms.ListBox();
            this.btnRemoveDir = new System.Windows.Forms.Button();
            this.btnAddDir = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.btnDownFile = new System.Windows.Forms.Button();
            this.btnUpFile = new System.Windows.Forms.Button();
            this.lstDataFiles = new System.Windows.Forms.ListBox();
            this.btnRemove = new System.Windows.Forms.Button();
            this.btnAdd = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnOk = new System.Windows.Forms.Button();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Location = new System.Drawing.Point(12, 12);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(538, 396);
            this.tabControl1.TabIndex = 22;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.btnSetFOOBJDir);
            this.tabPage1.Controls.Add(this.lblFOOBJ);
            this.tabPage1.Controls.Add(this.txtFOOBJ);
            this.tabPage1.Controls.Add(this.btnSetMapsDir);
            this.tabPage1.Controls.Add(this.lblMapsDir);
            this.tabPage1.Controls.Add(this.txtMapsDir);
            this.tabPage1.Controls.Add(this.btnSetCritterTypes);
            this.tabPage1.Controls.Add(this.lblCritterTypes);
            this.tabPage1.Controls.Add(this.txtCritterTypes);
            this.tabPage1.Controls.Add(this.btnSetItemProtosDir);
            this.tabPage1.Controls.Add(this.lblItemProtos);
            this.tabPage1.Controls.Add(this.txtItemProtos);
            this.tabPage1.Controls.Add(this.btnSetCritterProtosDir);
            this.tabPage1.Controls.Add(this.lblCritterProtos);
            this.tabPage1.Controls.Add(this.txtCritterProtos);
            this.tabPage1.Controls.Add(this.lblBasePath);
            this.tabPage1.Controls.Add(this.btnSetBasePaths);
            this.tabPage1.Controls.Add(this.txtBasePath);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(530, 370);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Base Paths";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // btnSetFOOBJDir
            // 
            this.btnSetFOOBJDir.Location = new System.Drawing.Point(477, 91);
            this.btnSetFOOBJDir.Name = "btnSetFOOBJDir";
            this.btnSetFOOBJDir.Size = new System.Drawing.Size(34, 20);
            this.btnSetFOOBJDir.TabIndex = 50;
            this.btnSetFOOBJDir.Text = "...";
            this.btnSetFOOBJDir.UseVisualStyleBackColor = true;
            this.btnSetFOOBJDir.Click += new System.EventHandler(this.btnSetSetFOOBJ_Click);
            // 
            // lblFOOBJ
            // 
            this.lblFOOBJ.AutoSize = true;
            this.lblFOOBJ.Location = new System.Drawing.Point(6, 95);
            this.lblFOOBJ.Name = "lblFOOBJ";
            this.lblFOOBJ.Size = new System.Drawing.Size(68, 13);
            this.lblFOOBJ.TabIndex = 49;
            this.lblFOOBJ.Text = "FOOBJ.MSG";
            // 
            // txtFOOBJ
            // 
            this.txtFOOBJ.Location = new System.Drawing.Point(129, 92);
            this.txtFOOBJ.Name = "txtFOOBJ";
            this.txtFOOBJ.Size = new System.Drawing.Size(342, 20);
            this.txtFOOBJ.TabIndex = 48;
            // 
            // btnSetMapsDir
            // 
            this.btnSetMapsDir.Location = new System.Drawing.Point(477, 118);
            this.btnSetMapsDir.Name = "btnSetMapsDir";
            this.btnSetMapsDir.Size = new System.Drawing.Size(34, 20);
            this.btnSetMapsDir.TabIndex = 47;
            this.btnSetMapsDir.Text = "...";
            this.btnSetMapsDir.UseVisualStyleBackColor = true;
            this.btnSetMapsDir.Click += new System.EventHandler(this.btnSetMapsDir_Click);
            // 
            // lblMapsDir
            // 
            this.lblMapsDir.AutoSize = true;
            this.lblMapsDir.Location = new System.Drawing.Point(6, 122);
            this.lblMapsDir.Name = "lblMapsDir";
            this.lblMapsDir.Size = new System.Drawing.Size(78, 13);
            this.lblMapsDir.TabIndex = 46;
            this.lblMapsDir.Text = "Maps Directory";
            // 
            // txtMapsDir
            // 
            this.txtMapsDir.Location = new System.Drawing.Point(129, 118);
            this.txtMapsDir.Name = "txtMapsDir";
            this.txtMapsDir.Size = new System.Drawing.Size(342, 20);
            this.txtMapsDir.TabIndex = 45;
            // 
            // btnSetCritterTypes
            // 
            this.btnSetCritterTypes.Location = new System.Drawing.Point(477, 36);
            this.btnSetCritterTypes.Name = "btnSetCritterTypes";
            this.btnSetCritterTypes.Size = new System.Drawing.Size(34, 20);
            this.btnSetCritterTypes.TabIndex = 44;
            this.btnSetCritterTypes.Text = "...";
            this.btnSetCritterTypes.UseVisualStyleBackColor = true;
            this.btnSetCritterTypes.Click += new System.EventHandler(this.btnSetCritterTypes_Click);
            // 
            // lblCritterTypes
            // 
            this.lblCritterTypes.AutoSize = true;
            this.lblCritterTypes.Location = new System.Drawing.Point(6, 39);
            this.lblCritterTypes.Name = "lblCritterTypes";
            this.lblCritterTypes.Size = new System.Drawing.Size(81, 13);
            this.lblCritterTypes.TabIndex = 43;
            this.lblCritterTypes.Text = "CritterTypes.cfg";
            // 
            // txtCritterTypes
            // 
            this.txtCritterTypes.Location = new System.Drawing.Point(129, 36);
            this.txtCritterTypes.Name = "txtCritterTypes";
            this.txtCritterTypes.Size = new System.Drawing.Size(342, 20);
            this.txtCritterTypes.TabIndex = 42;
            // 
            // btnSetItemProtosDir
            // 
            this.btnSetItemProtosDir.Location = new System.Drawing.Point(477, 145);
            this.btnSetItemProtosDir.Name = "btnSetItemProtosDir";
            this.btnSetItemProtosDir.Size = new System.Drawing.Size(34, 20);
            this.btnSetItemProtosDir.TabIndex = 41;
            this.btnSetItemProtosDir.Text = "...";
            this.btnSetItemProtosDir.UseVisualStyleBackColor = true;
            this.btnSetItemProtosDir.Click += new System.EventHandler(this.btnSetItemProtos_Click);
            // 
            // lblItemProtos
            // 
            this.lblItemProtos.AutoSize = true;
            this.lblItemProtos.Location = new System.Drawing.Point(6, 148);
            this.lblItemProtos.Name = "lblItemProtos";
            this.lblItemProtos.Size = new System.Drawing.Size(105, 13);
            this.lblItemProtos.TabIndex = 40;
            this.lblItemProtos.Text = "Item Protos Directory";
            // 
            // txtItemProtos
            // 
            this.txtItemProtos.Location = new System.Drawing.Point(129, 145);
            this.txtItemProtos.Name = "txtItemProtos";
            this.txtItemProtos.Size = new System.Drawing.Size(342, 20);
            this.txtItemProtos.TabIndex = 39;
            // 
            // btnSetCritterProtosDir
            // 
            this.btnSetCritterProtosDir.Location = new System.Drawing.Point(477, 64);
            this.btnSetCritterProtosDir.Name = "btnSetCritterProtosDir";
            this.btnSetCritterProtosDir.Size = new System.Drawing.Size(34, 20);
            this.btnSetCritterProtosDir.TabIndex = 32;
            this.btnSetCritterProtosDir.Text = "...";
            this.btnSetCritterProtosDir.UseVisualStyleBackColor = true;
            this.btnSetCritterProtosDir.Click += new System.EventHandler(this.btnSetCritterProtos_Click);
            // 
            // lblCritterProtos
            // 
            this.lblCritterProtos.AutoSize = true;
            this.lblCritterProtos.Location = new System.Drawing.Point(6, 67);
            this.lblCritterProtos.Name = "lblCritterProtos";
            this.lblCritterProtos.Size = new System.Drawing.Size(112, 13);
            this.lblCritterProtos.TabIndex = 31;
            this.lblCritterProtos.Text = "Critter Protos Directory";
            // 
            // txtCritterProtos
            // 
            this.txtCritterProtos.Location = new System.Drawing.Point(129, 64);
            this.txtCritterProtos.Name = "txtCritterProtos";
            this.txtCritterProtos.Size = new System.Drawing.Size(342, 20);
            this.txtCritterProtos.TabIndex = 30;
            // 
            // lblBasePath
            // 
            this.lblBasePath.AutoSize = true;
            this.lblBasePath.Location = new System.Drawing.Point(6, 13);
            this.lblBasePath.Name = "lblBasePath";
            this.lblBasePath.Size = new System.Drawing.Size(56, 13);
            this.lblBasePath.TabIndex = 23;
            this.lblBasePath.Text = "Base Path";
            // 
            // btnSetBasePaths
            // 
            this.btnSetBasePaths.Location = new System.Drawing.Point(477, 9);
            this.btnSetBasePaths.Name = "btnSetBasePaths";
            this.btnSetBasePaths.Size = new System.Drawing.Size(34, 20);
            this.btnSetBasePaths.TabIndex = 22;
            this.btnSetBasePaths.Text = "...";
            this.btnSetBasePaths.UseVisualStyleBackColor = true;
            this.btnSetBasePaths.Click += new System.EventHandler(this.btnSetBasePath_Click);
            // 
            // txtBasePath
            // 
            this.txtBasePath.Location = new System.Drawing.Point(129, 10);
            this.txtBasePath.Name = "txtBasePath";
            this.txtBasePath.Size = new System.Drawing.Size(342, 20);
            this.txtBasePath.TabIndex = 21;
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.groupBox2);
            this.tabPage2.Controls.Add(this.groupBox1);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(530, 370);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Resources";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.btnDownDir);
            this.groupBox2.Controls.Add(this.btnUpDir);
            this.groupBox2.Controls.Add(this.lstDataDirs);
            this.groupBox2.Controls.Add(this.btnRemoveDir);
            this.groupBox2.Controls.Add(this.btnAddDir);
            this.groupBox2.Location = new System.Drawing.Point(6, 181);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(518, 183);
            this.groupBox2.TabIndex = 5;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Data Directories";
            // 
            // btnDownDir
            // 
            this.btnDownDir.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnDownDir.Location = new System.Drawing.Point(473, 56);
            this.btnDownDir.Name = "btnDownDir";
            this.btnDownDir.Size = new System.Drawing.Size(38, 32);
            this.btnDownDir.TabIndex = 10;
            this.btnDownDir.Text = "↓";
            this.btnDownDir.UseVisualStyleBackColor = true;
            this.btnDownDir.Click += new System.EventHandler(this.btnDownDir_Click);
            // 
            // btnUpDir
            // 
            this.btnUpDir.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnUpDir.Location = new System.Drawing.Point(473, 21);
            this.btnUpDir.Name = "btnUpDir";
            this.btnUpDir.Size = new System.Drawing.Size(38, 32);
            this.btnUpDir.TabIndex = 9;
            this.btnUpDir.Text = "↑";
            this.btnUpDir.UseVisualStyleBackColor = true;
            this.btnUpDir.Click += new System.EventHandler(this.btnUpDir_Click);
            // 
            // lstDataDirs
            // 
            this.lstDataDirs.FormattingEnabled = true;
            this.lstDataDirs.Location = new System.Drawing.Point(7, 22);
            this.lstDataDirs.Name = "lstDataDirs";
            this.lstDataDirs.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.lstDataDirs.Size = new System.Drawing.Size(460, 108);
            this.lstDataDirs.TabIndex = 8;
            // 
            // btnRemoveDir
            // 
            this.btnRemoveDir.Location = new System.Drawing.Point(96, 138);
            this.btnRemoveDir.Name = "btnRemoveDir";
            this.btnRemoveDir.Size = new System.Drawing.Size(132, 23);
            this.btnRemoveDir.TabIndex = 7;
            this.btnRemoveDir.Text = "Remove Selected";
            this.btnRemoveDir.UseVisualStyleBackColor = true;
            this.btnRemoveDir.Click += new System.EventHandler(this.btnRemoveDir_Click);
            // 
            // btnAddDir
            // 
            this.btnAddDir.Location = new System.Drawing.Point(7, 138);
            this.btnAddDir.Name = "btnAddDir";
            this.btnAddDir.Size = new System.Drawing.Size(83, 23);
            this.btnAddDir.TabIndex = 6;
            this.btnAddDir.Text = "Add";
            this.btnAddDir.UseVisualStyleBackColor = true;
            this.btnAddDir.Click += new System.EventHandler(this.btnAddDir_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.btnDownFile);
            this.groupBox1.Controls.Add(this.btnUpFile);
            this.groupBox1.Controls.Add(this.lstDataFiles);
            this.groupBox1.Controls.Add(this.btnRemove);
            this.groupBox1.Controls.Add(this.btnAdd);
            this.groupBox1.Location = new System.Drawing.Point(6, 9);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(518, 166);
            this.groupBox1.TabIndex = 4;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Data Files (MASTER.DAT ...)";
            // 
            // btnDownFile
            // 
            this.btnDownFile.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnDownFile.Location = new System.Drawing.Point(474, 54);
            this.btnDownFile.Name = "btnDownFile";
            this.btnDownFile.Size = new System.Drawing.Size(38, 32);
            this.btnDownFile.TabIndex = 5;
            this.btnDownFile.Text = "↓";
            this.btnDownFile.UseVisualStyleBackColor = true;
            this.btnDownFile.Click += new System.EventHandler(this.btnDownFile_Click);
            // 
            // btnUpFile
            // 
            this.btnUpFile.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnUpFile.Location = new System.Drawing.Point(474, 19);
            this.btnUpFile.Name = "btnUpFile";
            this.btnUpFile.Size = new System.Drawing.Size(38, 32);
            this.btnUpFile.TabIndex = 4;
            this.btnUpFile.Text = "↑";
            this.btnUpFile.UseVisualStyleBackColor = true;
            this.btnUpFile.Click += new System.EventHandler(this.btnUpFile_Click);
            // 
            // lstDataFiles
            // 
            this.lstDataFiles.FormattingEnabled = true;
            this.lstDataFiles.Location = new System.Drawing.Point(8, 20);
            this.lstDataFiles.Name = "lstDataFiles";
            this.lstDataFiles.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.lstDataFiles.Size = new System.Drawing.Size(460, 108);
            this.lstDataFiles.TabIndex = 3;
            // 
            // btnRemove
            // 
            this.btnRemove.Location = new System.Drawing.Point(97, 136);
            this.btnRemove.Name = "btnRemove";
            this.btnRemove.Size = new System.Drawing.Size(132, 23);
            this.btnRemove.TabIndex = 2;
            this.btnRemove.Text = "Remove Selected";
            this.btnRemove.UseVisualStyleBackColor = true;
            this.btnRemove.Click += new System.EventHandler(this.btnRemove_Click);
            // 
            // btnAdd
            // 
            this.btnAdd.Location = new System.Drawing.Point(8, 136);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(83, 23);
            this.btnAdd.TabIndex = 1;
            this.btnAdd.Text = "Add";
            this.btnAdd.UseVisualStyleBackColor = true;
            this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(103, 414);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(96, 29);
            this.btnCancel.TabIndex = 24;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnOk
            // 
            this.btnOk.Location = new System.Drawing.Point(12, 414);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(85, 29);
            this.btnOk.TabIndex = 23;
            this.btnOk.Text = "OK";
            this.btnOk.UseVisualStyleBackColor = true;
            this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
            // 
            // frmPaths
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(566, 455);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOk);
            this.Controls.Add(this.tabControl1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmPaths";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Paths";
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            this.tabPage2.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog1;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.Button btnSetCritterProtosDir;
        private System.Windows.Forms.Label lblCritterProtos;
        private System.Windows.Forms.TextBox txtCritterProtos;
        private System.Windows.Forms.Label lblBasePath;
        private System.Windows.Forms.Button btnSetBasePaths;
        private System.Windows.Forms.TextBox txtBasePath;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button btnDownDir;
        private System.Windows.Forms.Button btnUpDir;
        private System.Windows.Forms.ListBox lstDataDirs;
        private System.Windows.Forms.Button btnRemoveDir;
        private System.Windows.Forms.Button btnAddDir;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button btnDownFile;
        private System.Windows.Forms.Button btnUpFile;
        private System.Windows.Forms.ListBox lstDataFiles;
        private System.Windows.Forms.Button btnRemove;
        private System.Windows.Forms.Button btnAdd;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnOk;
        private System.Windows.Forms.Button btnSetFOOBJDir;
        private System.Windows.Forms.Label lblFOOBJ;
        private System.Windows.Forms.TextBox txtFOOBJ;
        private System.Windows.Forms.Button btnSetMapsDir;
        private System.Windows.Forms.Label lblMapsDir;
        private System.Windows.Forms.TextBox txtMapsDir;
        private System.Windows.Forms.Button btnSetCritterTypes;
        private System.Windows.Forms.Label lblCritterTypes;
        private System.Windows.Forms.TextBox txtCritterTypes;
        private System.Windows.Forms.Button btnSetItemProtosDir;
        private System.Windows.Forms.Label lblItemProtos;
        private System.Windows.Forms.TextBox txtItemProtos;
    }
}