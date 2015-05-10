namespace fonline_mapgen
{
    partial class frmLoading
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
            this.progressResource = new System.Windows.Forms.ProgressBar();
            this.progressFiles = new System.Windows.Forms.ProgressBar();
            this.lblResource = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // progressResource
            // 
            this.progressResource.Location = new System.Drawing.Point(12, 64);
            this.progressResource.Name = "progressResource";
            this.progressResource.Size = new System.Drawing.Size(526, 23);
            this.progressResource.TabIndex = 0;
            // 
            // progressFiles
            // 
            this.progressFiles.Location = new System.Drawing.Point(12, 35);
            this.progressFiles.Name = "progressFiles";
            this.progressFiles.Size = new System.Drawing.Size(526, 23);
            this.progressFiles.TabIndex = 2;
            // 
            // lblResource
            // 
            this.lblResource.AutoSize = true;
            this.lblResource.Location = new System.Drawing.Point(12, 98);
            this.lblResource.Name = "lblResource";
            this.lblResource.Size = new System.Drawing.Size(53, 13);
            this.lblResource.TabIndex = 3;
            this.lblResource.Text = "Resource";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(9, 9);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(152, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "Loading data, please stand by.";
            // 
            // frmLoading
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(550, 117);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.lblResource);
            this.Controls.Add(this.progressFiles);
            this.Controls.Add(this.progressResource);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmLoading";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Loading...";
            this.TopMost = true;
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ProgressBar progressResource;
        private System.Windows.Forms.ProgressBar progressFiles;
        private System.Windows.Forms.Label lblResource;
        private System.Windows.Forms.Label label2;
    }
}