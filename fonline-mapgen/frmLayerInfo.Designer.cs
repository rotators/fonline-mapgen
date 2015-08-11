namespace fonline_mapgen
{
    partial class frmLayerInfo
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
            this.lstBuffer = new System.Windows.Forms.ListView();
            this.lstMainBuffer = new System.Windows.Forms.ListView();
            this.lblMainBuffer = new System.Windows.Forms.Label();
            this.lblMapBuffer = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // lstBuffer
            // 
            this.lstBuffer.Activation = System.Windows.Forms.ItemActivation.OneClick;
            this.lstBuffer.AllowDrop = true;
            this.lstBuffer.Location = new System.Drawing.Point(205, 41);
            this.lstBuffer.Name = "lstBuffer";
            this.lstBuffer.Size = new System.Drawing.Size(185, 154);
            this.lstBuffer.TabIndex = 0;
            this.lstBuffer.UseCompatibleStateImageBehavior = false;
            this.lstBuffer.View = System.Windows.Forms.View.List;
            this.lstBuffer.ItemDrag += new System.Windows.Forms.ItemDragEventHandler(this.lstBuffer_ItemDrag);
            this.lstBuffer.DragDrop += new System.Windows.Forms.DragEventHandler(this.lstBuffer_DragDrop);
            this.lstBuffer.DragEnter += new System.Windows.Forms.DragEventHandler(this.lstBuffer_DragEnter);
            this.lstBuffer.KeyUp += new System.Windows.Forms.KeyEventHandler(this.lstBuffer_KeyUp);
            // 
            // lstMainBuffer
            // 
            this.lstMainBuffer.Activation = System.Windows.Forms.ItemActivation.OneClick;
            this.lstMainBuffer.AllowDrop = true;
            this.lstMainBuffer.Location = new System.Drawing.Point(12, 41);
            this.lstMainBuffer.Name = "lstMainBuffer";
            this.lstMainBuffer.Size = new System.Drawing.Size(185, 154);
            this.lstMainBuffer.TabIndex = 1;
            this.lstMainBuffer.UseCompatibleStateImageBehavior = false;
            this.lstMainBuffer.View = System.Windows.Forms.View.List;
            this.lstMainBuffer.ItemDrag += new System.Windows.Forms.ItemDragEventHandler(this.lstMainBuffer_ItemDrag);
            this.lstMainBuffer.DragDrop += new System.Windows.Forms.DragEventHandler(this.lstMainBuffer_DragDrop);
            this.lstMainBuffer.DragEnter += new System.Windows.Forms.DragEventHandler(this.lstMainBuffer_DragEnter);
            this.lstMainBuffer.KeyUp += new System.Windows.Forms.KeyEventHandler(this.lstMainBuffer_KeyUp);
            // 
            // lblMainBuffer
            // 
            this.lblMainBuffer.AutoSize = true;
            this.lblMainBuffer.Location = new System.Drawing.Point(13, 22);
            this.lblMainBuffer.Name = "lblMainBuffer";
            this.lblMainBuffer.Size = new System.Drawing.Size(76, 13);
            this.lblMainBuffer.TabIndex = 2;
            this.lblMainBuffer.Text = "Program buffer";
            // 
            // lblMapBuffer
            // 
            this.lblMapBuffer.AutoSize = true;
            this.lblMapBuffer.Location = new System.Drawing.Point(202, 22);
            this.lblMapBuffer.Name = "lblMapBuffer";
            this.lblMapBuffer.Size = new System.Drawing.Size(58, 13);
            this.lblMapBuffer.TabIndex = 3;
            this.lblMapBuffer.Text = "Map buffer";
            // 
            // frmLayerInfo
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(402, 300);
            this.Controls.Add(this.lblMapBuffer);
            this.Controls.Add(this.lblMainBuffer);
            this.Controls.Add(this.lstMainBuffer);
            this.Controls.Add(this.lstBuffer);
            this.Name = "frmLayerInfo";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Selection buffer";
            this.Load += new System.EventHandler(this.frmDebugInfo_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        public System.Windows.Forms.ListView lstBuffer;
        public System.Windows.Forms.ListView lstMainBuffer;
        private System.Windows.Forms.Label lblMainBuffer;
        private System.Windows.Forms.Label lblMapBuffer;





    }
}