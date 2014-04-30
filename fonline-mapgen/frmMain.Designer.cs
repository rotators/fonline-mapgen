namespace fonline_mapgen
{
    partial class frmMain
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
            this.btnLoadMap = new System.Windows.Forms.Button();
            this.pnlViewPort = new System.Windows.Forms.Panel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.lblMouseCoords = new System.Windows.Forms.Label();
            this.lblProtos = new System.Windows.Forms.Label();
            this.cmbMaps = new System.Windows.Forms.ComboBox();
            this.menu = new System.Windows.Forms.MenuStrip();
            this.stuffToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.headerToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuFile = new System.Windows.Forms.ToolStripMenuItem();
            this.menuFileOpen = new System.Windows.Forms.ToolStripMenuItem();
            this.menuView = new System.Windows.Forms.ToolStripMenuItem();
            this.menuViewTiles = new System.Windows.Forms.ToolStripMenuItem();
            this.menuViewRoofs = new System.Windows.Forms.ToolStripMenuItem();
            this.menuViewCritters = new System.Windows.Forms.ToolStripMenuItem();
            this.menuViewItems = new System.Windows.Forms.ToolStripMenuItem();
            this.menuViewScenery = new System.Windows.Forms.ToolStripMenuItem();
            this.openMapDialog = new System.Windows.Forms.OpenFileDialog();
            this.menuViewSceneryWalls = new System.Windows.Forms.ToolStripMenuItem();
            this.pnlViewPort.SuspendLayout();
            this.menu.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnLoadMap
            // 
            this.btnLoadMap.Location = new System.Drawing.Point( 547, 16 );
            this.btnLoadMap.Name = "btnLoadMap";
            this.btnLoadMap.Size = new System.Drawing.Size( 131, 25 );
            this.btnLoadMap.TabIndex = 1;
            this.btnLoadMap.Text = "Load Map";
            this.btnLoadMap.UseVisualStyleBackColor = true;
            this.btnLoadMap.Click += new System.EventHandler( this.btnLoadMap_Click );
            // 
            // pnlViewPort
            // 
            this.pnlViewPort.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.pnlViewPort.AutoScroll = true;
            this.pnlViewPort.Controls.Add( this.panel1 );
            this.pnlViewPort.Location = new System.Drawing.Point( 12, 88 );
            this.pnlViewPort.Name = "pnlViewPort";
            this.pnlViewPort.Size = new System.Drawing.Size( 1004, 538 );
            this.pnlViewPort.TabIndex = 7;
            // 
            // panel1
            // 
            this.panel1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.panel1.Location = new System.Drawing.Point( 3, 3 );
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size( 5003, 5350 );
            this.panel1.TabIndex = 0;
            this.panel1.Paint += new System.Windows.Forms.PaintEventHandler( this.panel1_Paint );
            this.panel1.MouseMove += new System.Windows.Forms.MouseEventHandler( this.panel1_MouseMove );
            // 
            // lblMouseCoords
            // 
            this.lblMouseCoords.AutoSize = true;
            this.lblMouseCoords.Font = new System.Drawing.Font( "Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)) );
            this.lblMouseCoords.Location = new System.Drawing.Point( 694, 16 );
            this.lblMouseCoords.Name = "lblMouseCoords";
            this.lblMouseCoords.Size = new System.Drawing.Size( 164, 25 );
            this.lblMouseCoords.TabIndex = 8;
            this.lblMouseCoords.Text = "Mouse Coords: ";
            // 
            // lblProtos
            // 
            this.lblProtos.Font = new System.Drawing.Font( "Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)) );
            this.lblProtos.Location = new System.Drawing.Point( 12, 49 );
            this.lblProtos.Name = "lblProtos";
            this.lblProtos.Size = new System.Drawing.Size( 1279, 25 );
            this.lblProtos.TabIndex = 9;
            this.lblProtos.Text = "Protos:";
            // 
            // cmbMaps
            // 
            this.cmbMaps.DropDownWidth = 600;
            this.cmbMaps.FormattingEnabled = true;
            this.cmbMaps.Location = new System.Drawing.Point( 15, 19 );
            this.cmbMaps.Name = "cmbMaps";
            this.cmbMaps.Size = new System.Drawing.Size( 517, 21 );
            this.cmbMaps.TabIndex = 10;
            // 
            // menu
            // 
            this.menu.BackColor = System.Drawing.SystemColors.Control;
            this.menu.Items.AddRange( new System.Windows.Forms.ToolStripItem[] {
            this.stuffToolStripMenuItem,
            this.menuFile,
            this.menuView} );
            this.menu.Location = new System.Drawing.Point( 0, 0 );
            this.menu.Name = "menu";
            this.menu.Size = new System.Drawing.Size( 1028, 24 );
            this.menu.TabIndex = 11;
            this.menu.Text = "menuStrip1";
            // 
            // stuffToolStripMenuItem
            // 
            this.stuffToolStripMenuItem.DropDownItems.AddRange( new System.Windows.Forms.ToolStripItem[] {
            this.headerToolStripMenuItem} );
            this.stuffToolStripMenuItem.Name = "stuffToolStripMenuItem";
            this.stuffToolStripMenuItem.Size = new System.Drawing.Size( 43, 20 );
            this.stuffToolStripMenuItem.Text = "Stuff";
            // 
            // headerToolStripMenuItem
            // 
            this.headerToolStripMenuItem.Name = "headerToolStripMenuItem";
            this.headerToolStripMenuItem.Size = new System.Drawing.Size( 109, 22 );
            this.headerToolStripMenuItem.Text = "Header";
            this.headerToolStripMenuItem.Click += new System.EventHandler( this.headerToolStripMenuItem_Click );
            // 
            // menuFile
            // 
            this.menuFile.DropDownItems.AddRange( new System.Windows.Forms.ToolStripItem[] {
            this.menuFileOpen} );
            this.menuFile.Name = "menuFile";
            this.menuFile.Size = new System.Drawing.Size( 35, 20 );
            this.menuFile.Text = "&File";
            // 
            // menuFileOpen
            // 
            this.menuFileOpen.Name = "menuFileOpen";
            this.menuFileOpen.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.O)));
            this.menuFileOpen.Size = new System.Drawing.Size( 152, 22 );
            this.menuFileOpen.Text = "&Open";
            this.menuFileOpen.Click += new System.EventHandler( this.menuFileOpen_Click );
            // 
            // menuView
            // 
            this.menuView.DropDownItems.AddRange( new System.Windows.Forms.ToolStripItem[] {
            this.menuViewTiles,
            this.menuViewRoofs,
            this.menuViewCritters,
            this.menuViewItems,
            this.menuViewScenery} );
            this.menuView.Name = "menuView";
            this.menuView.Size = new System.Drawing.Size( 41, 20 );
            this.menuView.Text = "View";
            // 
            // menuViewTiles
            // 
            this.menuViewTiles.CheckOnClick = true;
            this.menuViewTiles.Name = "menuViewTiles";
            this.menuViewTiles.Size = new System.Drawing.Size( 152, 22 );
            this.menuViewTiles.Text = "Tiles";
            this.menuViewTiles.CheckedChanged += new System.EventHandler( this.menuViewTiles_CheckedChanged );
            // 
            // menuViewRoofs
            // 
            this.menuViewRoofs.CheckOnClick = true;
            this.menuViewRoofs.Name = "menuViewRoofs";
            this.menuViewRoofs.Size = new System.Drawing.Size( 152, 22 );
            this.menuViewRoofs.Text = "Roofs";
            this.menuViewRoofs.CheckedChanged += new System.EventHandler( this.menuViewRoofs_CheckedChanged );
            // 
            // menuViewCritters
            // 
            this.menuViewCritters.CheckOnClick = true;
            this.menuViewCritters.Enabled = false;
            this.menuViewCritters.Name = "menuViewCritters";
            this.menuViewCritters.Size = new System.Drawing.Size( 152, 22 );
            this.menuViewCritters.Text = "Critters";
            this.menuViewCritters.CheckedChanged += new System.EventHandler( this.menuViewCritters_CheckedChanged );
            // 
            // menuViewItems
            // 
            this.menuViewItems.CheckOnClick = true;
            this.menuViewItems.Name = "menuViewItems";
            this.menuViewItems.Size = new System.Drawing.Size( 152, 22 );
            this.menuViewItems.Text = "Items";
            this.menuViewItems.CheckedChanged += new System.EventHandler( this.menuViewItems_CheckedChanged );
            // 
            // menuViewScenery
            // 
            this.menuViewScenery.CheckOnClick = true;
            this.menuViewScenery.DropDownItems.AddRange( new System.Windows.Forms.ToolStripItem[] {
            this.menuViewSceneryWalls} );
            this.menuViewScenery.Name = "menuViewScenery";
            this.menuViewScenery.Size = new System.Drawing.Size( 152, 22 );
            this.menuViewScenery.Text = "Scenery";
            this.menuViewScenery.CheckedChanged += new System.EventHandler( this.menuViewScenery_CheckedChanged );
            // 
            // openMapDialog
            // 
            this.openMapDialog.DefaultExt = "fomap";
            this.openMapDialog.Filter = "FOnline map|*.fomap";
            this.openMapDialog.RestoreDirectory = true;
            this.openMapDialog.ShowReadOnly = true;
            this.openMapDialog.SupportMultiDottedExtensions = true;
            // 
            // menuViewSceneryWalls
            // 
            this.menuViewSceneryWalls.CheckOnClick = true;
            this.menuViewSceneryWalls.Name = "menuViewSceneryWalls";
            this.menuViewSceneryWalls.Size = new System.Drawing.Size( 152, 22 );
            this.menuViewSceneryWalls.Text = "Walls";
            this.menuViewSceneryWalls.CheckedChanged += new System.EventHandler( this.menuViewSceneryWalls_CheckedChanged );
            // 
            // frmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF( 6F, 13F );
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.ClientSize = new System.Drawing.Size( 1028, 638 );
            this.Controls.Add( this.cmbMaps );
            this.Controls.Add( this.lblProtos );
            this.Controls.Add( this.lblMouseCoords );
            this.Controls.Add( this.pnlViewPort );
            this.Controls.Add( this.btnLoadMap );
            this.Controls.Add( this.menu );
            this.DoubleBuffered = true;
            this.MainMenuStrip = this.menu;
            this.Name = "frmMain";
            this.Text = "Mapper experiment";
            this.Load += new System.EventHandler( this.Form1_Load );
            this.Paint += new System.Windows.Forms.PaintEventHandler( this.frmMain_Paint );
            this.pnlViewPort.ResumeLayout( false );
            this.menu.ResumeLayout( false );
            this.menu.PerformLayout();
            this.ResumeLayout( false );
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnLoadMap;
        private System.Windows.Forms.Panel pnlViewPort;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label lblMouseCoords;
        private System.Windows.Forms.Label lblProtos;
        private System.Windows.Forms.ComboBox cmbMaps;
        private System.Windows.Forms.MenuStrip menu;
        private System.Windows.Forms.ToolStripMenuItem stuffToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem headerToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem menuFile;
        private System.Windows.Forms.ToolStripMenuItem menuFileOpen;
        private System.Windows.Forms.OpenFileDialog openMapDialog;
        private System.Windows.Forms.ToolStripMenuItem menuView;
        private System.Windows.Forms.ToolStripMenuItem menuViewTiles;
        private System.Windows.Forms.ToolStripMenuItem menuViewRoofs;
        private System.Windows.Forms.ToolStripMenuItem menuViewCritters;
        private System.Windows.Forms.ToolStripMenuItem menuViewItems;
        private System.Windows.Forms.ToolStripMenuItem menuViewScenery;
        private System.Windows.Forms.ToolStripMenuItem menuViewSceneryWalls;
    }
}

