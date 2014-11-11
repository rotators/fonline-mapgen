using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

using DATLib;
using FOCommon.Maps;
using FOCommon.Graphic;
using FOCommon.Parsers;
using FOCommon.Items;

namespace fonline_mapgen
{
    public partial class frmMain : Form
    {
        public List<String> GraphicsPaths = new List<string>();
        //public Dictionary<String, Bitmap> Bitmaps = new Dictionary<string, Bitmap>();

        public Dictionary<String, FalloutFRM> Frms = new Dictionary<string, FalloutFRM>();
        List<ItemProto> items = new List<ItemProto>();

        Color transparency = Color.FromArgb(11, 0, 11);

        CritterData critterData = new CritterData();

        Dictionary<int, ItemProto> itemsPid = new Dictionary<int, ItemProto>();

        float scaleFactor = 1.0f;
        Point clickedPos = new Point(0,0);
        int framesPerSecond = 0; // only updated when repainting.

        PointF viewPortSize = new PointF();

        MapperSettings mapperSettings = new MapperSettings();

        //FOCommon.Parsers.FOMapParser parser;

        ItemProtoParser protoParser = new ItemProtoParser();

        frmPaths frmPaths;
        frmPerformance frmPerformance;
        frmMapTree frmMapTree;
        frmDebugInfo frmDebugInfo;

        string title = "Mapper experiment [ALPHA]";


        public MapperMap CurrentMap
        {
            get
            {
                if( this.CurrentMapIdx < 0 || this.CurrentMapIdx >= this.Maps.Count )
                    return (null);

                return (this.Maps[this.CurrentMapIdx]);
            }
            private set
            {
                int idx = this.Maps.IndexOf( value );
                if( idx >= 0 )
                    this.CurrentMapIdx = idx;
                else
                    MessageBox.Show( "Can't set CurrentMapIndex" );
            }
        }
        private int CurrentMapIdx = -1;
        private List<MapperMap> Maps = new List<MapperMap>();
        TabPage TabTemplate;

        /*
        bool drawTiles = true;
        bool drawRoofs = true;
        bool drawCritters = true;
        bool drawItems = true;
        bool drawScenery = true;
        bool drawSceneryWalls = true;
        */
        DrawMap.Flags drawFlags;

        public frmMain()
        {
            InitializeComponent();
            LoadResources();
        }

        private void Form1_Load( object sender, EventArgs e )
        {
            
        }

        private void Exit()
        {
            mapperSettings.View.Tiles = menuViewTiles.Checked;
            mapperSettings.View.Roofs = menuViewRoofs.Checked;
            mapperSettings.View.Critters = menuViewCritters.Checked;
            mapperSettings.View.Items = menuViewItems.Checked;
            mapperSettings.View.Scenery = menuViewScenery.Checked;
            mapperSettings.View.Walls = menuViewSceneryWalls.Checked;

            SettingsManager.SaveSettings(mapperSettings);
            Environment.Exit(0);
        }

        private void LoadMap( string fileName )
        {
            MapperMap map = MapperMap.Load( fileName );
            if( map != null )
            {
                this.Maps.Add( map );
                this.CurrentMap = map;

                this.Text = title + fileName;

                headerToolStripMenuItem.Enabled =
                menuFileExport.Enabled =
                viewMapTreeToolStripMenuItem.Enabled = true;

                DrawMap.InvalidateCache();

                viewPortSize.X = ((map.GetEdgeCoords(FOHexMap.Direction.Right).X) - (map.GetEdgeCoords(FOHexMap.Direction.Left).X)) + 100.0f;
                viewPortSize.Y = ((map.GetEdgeCoords(FOHexMap.Direction.Down).Y)  - (map.GetEdgeCoords(FOHexMap.Direction.Up).Y)) + 100.0f;

                resizeViewport();
                centerViewport();

                panel1.Refresh();
            }
            else
                MessageBox.Show( "Error loading map " + fileName );
        }

        public void WriteLog( string str )
        {
            File.AppendAllText( "./debug.log", str + Environment.NewLine );
        }

        public bool LoadDat( string DatPath, Color Transparency )
        {
            DatReaderError status;
            DAT loadedDat = DATReader.ReadDat( DatPath, out status );
            if( status.Error != DatError.Success )
            {
                MessageBox.Show( "Error loading " + DatPath + ": " + Environment.NewLine + status.Message, "", MessageBoxButtons.OK, MessageBoxIcon.Error );
                return false;
            }

            List<DATFile> files = new List<DATFile>();
            foreach( string path in this.GraphicsPaths )
            {
                files.AddRange( loadedDat.GetFilesByPattern( path ) );
            
                // Critters to load
                foreach (var crType in critterData.crTypeGraphic.Values)
                {
                    var file = loadedDat.GetFileByName("art\\critters\\" + crType.ToUpper() + "AA.FRM"); // Idle anim
                    if (file == null) file = loadedDat.GetFileByName("art\\critters\\" + crType.ToLower() + "aa.frm");
                    if (file == null) continue;

                    files.Add(file);
                }

                foreach( DATFile file in files )
                {
                    string ext = Path.GetExtension( file.FileName ).ToLower();
                    if( !(ext == ".frm" || ext == ".png") )
                        continue;

                    byte[] data = file.GetData();
                    if (data == null)
                    {
                        WriteLog("Erroring opening " + file.FileName + ": " + file.ErrorMsg);
                        continue;
                    }

                    if( ext == ".frm" )
                    {
                        var frm = FalloutFRMLoader.LoadFRM(data, Transparency );
                        frm.FileName = file.Path.ToLower();
                        Frms[frm.FileName] = frm;
                    }
                    else
                    {
                        System.ComponentModel.TypeConverter tc = System.ComponentModel.TypeDescriptor.GetConverter( typeof( Bitmap ) );
                        Bitmap bitmap = (Bitmap)tc.ConvertFrom( data );
                    }
                }

                files.Clear();
            }

            loadedDat.Close();

            return true;
        }

        // TODO: LoadZip
        public bool LoadZip( string ZipPath, Color Transparency )
        {
            return (false);
        }

        private void panel1_Paint( object sender, PaintEventArgs e )
        {
            if( Frms.Count == 0 )
                return;

            MapperMap map = this.CurrentMap;

            if( map == null )
                return;

            var g = e.Graphics;

            if (mapperSettings.Performance.FastRendering)
            {
                g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.None;
                g.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.None;
                g.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighSpeed;
            }
            else
            {
                g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.High;
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                g.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;
                g.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
            }

            DrawMap.OnGraphics(g, map, map.HexMap, itemsPid, critterData, Frms, this.drawFlags, new SizeF(scaleFactor, scaleFactor), clickedPos);
            // new Rectangle(pnlViewPort.HorizontalScroll.Value, pnlViewPort.VerticalScroll.Value, pnlViewPort.Width, pnlViewPort.Height)

            Font font = new System.Drawing.Font(FontFamily.GenericSansSerif, 17.0f, FontStyle.Bold);
            g.DrawString("Selected", font, Brushes.OrangeRed, new PointF(clickedPos.X - 30.0f, clickedPos.Y - 40.0f));
           // 
            if (frmDebugInfo != null && !frmDebugInfo.IsDisposed)
            {
                frmDebugInfo.setText("Objects rendered: " + DrawMap.GetNumCachedObjects());
            }
        }

        private void centerViewport()
        {
            pnlViewPort.VerticalScroll.Value = pnlViewPort.VerticalScroll.Maximum / 3;
            pnlViewPort.HorizontalScroll.Value = pnlViewPort.HorizontalScroll.Maximum / 3;
            // Due to a bug in .NET, appearance of where the control is isn't updated unless do this twice after
            // changing the position over the vertical scrollbar.
            pnlViewPort.HorizontalScroll.Value = pnlViewPort.HorizontalScroll.Maximum / 3;    
            pnlViewPort.Refresh();
        }

        private void resizeViewport()
        {
            panel1.Width = (int)(viewPortSize.X * scaleFactor);
            panel1.Height = (int)(viewPortSize.Y * scaleFactor);
        }

        private void panel1_MouseWheel(object sender, MouseEventArgs e)
        {
            //if (e.Delta != 0) scaleFactor += ((float)e.Delta / 10000.0f);
            if (e.Delta == 0) return;

            if (e.Delta > 0) scaleFactor += 0.03f;
            else scaleFactor -= 0.10f;

            scaleFactor = Math.Max(scaleFactor, 0.03f);

            resizeViewport();
            panel1.Refresh();
        }

        private void panel1_MouseMove( object sender, MouseEventArgs e )
        {
            MapperMap map = this.CurrentMap;

            if( map == null )
                return;
            var hex = map.HexMap.GetHex(new PointF(e.X / scaleFactor, e.Y / scaleFactor + 6.0f));
            toolStripStatusHex.Text = string.Format( "Mouse Coords: {0},{1} - Hex: {2},{3}", e.X, e.Y, hex.X, hex.Y );
            //

            if( map.Objects.Count( x => x.MapX == hex.X && x.MapY == hex.Y ) == 0 )
                toolStripStatusProto.Text = "";

            foreach( var obj in map.Objects.FindAll( x => x.MapX == hex.X && x.MapY == hex.Y ) )
            {
                if ((obj.MapObjType == FOCommon.Maps.MapObjectType.Item ||
                      obj.MapObjType == FOCommon.Maps.MapObjectType.Scenery))
                {
                    ItemProto prot;
                    if (!itemsPid.TryGetValue(obj.ProtoId, out prot))
                        continue;

                    toolStripStatusProto.Text = "Proto: " + (obj.ProtoId);
                    toolStripStatusProto.Text += string.Format(" ({0} - {1})", prot.Name, prot.PicMap);
                }

                if (obj.MapObjType == FOCommon.Maps.MapObjectType.Critter)
                {
                    toolStripStatusProto.Text = "Critter: " + obj.ProtoId;
                }
            }
        }

        private void btnLoadMap_Click( object sender, EventArgs e )
        {
            LoadMap( (string)cmbMaps.SelectedItem );
        }

        private void headerToolStripMenuItem_Click( object sender, EventArgs e )
        {
            MapperMap map = this.CurrentMap;

            if( map == null )
            {
                MessageBox.Show( "Map not loaded!" );
                return;
            }

            frmHeaderEditor formHeaderEditor = new frmHeaderEditor( map.Header );
            if( formHeaderEditor.ShowDialog() == DialogResult.OK )
            {
                // TODO: update map header
            }
        }

        private void LoadResources()
        {
            GraphicsPaths.Add("art\\tiles");
            GraphicsPaths.Add("art\\misc");
            GraphicsPaths.Add("art\\walls");
            GraphicsPaths.Add("art\\door");
            GraphicsPaths.Add("art\\scenery");

            Stream stream;
            BinaryFormatter formatter = new BinaryFormatter();

            SettingsManager.Init();
            mapperSettings = SettingsManager.LoadSettings();
            if (mapperSettings == null)
            {
                mapperSettings = new MapperSettings();
                frmPaths = new frmPaths(mapperSettings);
                frmPaths.ShowDialog();

                mapperSettings.View.Tiles =
                mapperSettings.View.Roofs =
                mapperSettings.View.Critters =
                mapperSettings.View.Items =
                mapperSettings.View.Scenery =
                mapperSettings.View.Walls = true;

                mapperSettings.Performance.CacheResources = true;
                mapperSettings.Performance.FastRendering = true;
            }

            menuViewTiles.Checked = mapperSettings.View.Tiles;
            menuViewRoofs.Checked = mapperSettings.View.Roofs;
            menuViewCritters.Checked = mapperSettings.View.Critters;
            menuViewItems.Checked = mapperSettings.View.Items;
            menuViewScenery.Checked = mapperSettings.View.Scenery;
            menuViewSceneryWalls.Checked = mapperSettings.View.Walls;

            stream = null;

            if (File.Exists("./critters.dat"))
            {
                stream = File.OpenRead("./critters.dat");
                critterData = (CritterData)formatter.Deserialize(stream);
                stream.Close();
            }
            else
            {
                foreach (string file in Directory.GetFiles(mapperSettings.Paths.CritterProtos))
                {
                    int pid = 0;
                    int crType = 0;
                    bool parse = false;
                    foreach (var line in File.ReadAllLines(file))
                    {
                        if (line == "[Critter proto]")
                            if (parse)
                                critterData.crProtos[pid] = crType;
                        parse = true;
                        if (!parse) continue;
                        if (line.StartsWith("Pid="))
                            pid = int.Parse(line.Split('=')[1]);
                        if (line.StartsWith("ST_BASE_CRTYPE="))
                            crType = int.Parse(line.Split('=')[1]);
                    }
                }

                char[] delim = { '\t', ' ' };
                foreach (var line in File.ReadAllLines(mapperSettings.Paths.CritterTypes))
                {
                    if (!line.StartsWith("@")) continue;
                    var toks = line.Split(delim, StringSplitOptions.RemoveEmptyEntries);
                    critterData.crTypeGraphic[int.Parse(toks[1])] = toks[2];
                }
                if (mapperSettings.Performance.CacheResources)
                {
                    stream = File.Create("./critters.dat");
                    formatter.Serialize(stream, critterData);
                }
            }

            if (File.Exists("./items.dat"))
            {
                stream = File.OpenRead("./items.dat");
                items = (List<ItemProto>)formatter.Deserialize(stream);
                stream.Close();
            }
            else
            {
                MSGParser FOObj = new MSGParser(mapperSettings.Paths.FOOBJ);
                FOObj.Parse();

                string itemslst = mapperSettings.Paths.ItemProtos + "\\items.lst";

                if (!File.Exists(itemslst))
                {
                    MessageBox.Show("No " + itemslst + " , unable to load item protos.");
                }
                else
                {
                    foreach (string file in File.ReadAllLines(itemslst))
                    {
                        protoParser.LoadProtosFromFile(mapperSettings.Paths.ItemProtos + file, "1.0", FOObj, items, null);
                    }
                }
                if (mapperSettings.Performance.CacheResources)
                {
                    stream = File.Create("./items.dat");
                    formatter.Serialize(stream, items);
                }
            }

            if (File.Exists("./graphics.dat"))
            {
                stream = File.OpenRead("./graphics.dat");
                Frms = (Dictionary<String, FalloutFRM>)formatter.Deserialize(stream);
                stream.Close();
            }
            else
            {
                if (mapperSettings.Paths.DataFiles == null || mapperSettings.Paths.DataFiles.Count == 0)
                {
                    MessageBox.Show("No datafiles specified, unable to load graphics!");
                    frmPaths = new frmPaths(mapperSettings);
                    frmPaths.ShowDialog();
                }

                if (mapperSettings.Paths.DataFiles != null || mapperSettings.Paths.DataFiles.Count != 0)
                {
                    foreach (string dataFile in mapperSettings.Paths.DataFiles)
                    {
                        string ext = Path.GetExtension(dataFile).ToLower();
                        if (ext != ".dat" && ext != ".zip")
                        {
                            MessageBox.Show("Unknown datafile extension : " + dataFile);
                            continue;
                        }
                        if (ext == ".dat")
                            LoadDat(dataFile, transparency);
                        else
                            LoadZip(dataFile, transparency);
                    }

                    if (mapperSettings.Performance.CacheResources)
                    {
                        stream = File.Create("./graphics.dat");
                        formatter.Serialize(stream, Frms);
                    }
                }
            }

            foreach (var item in items)
                itemsPid[item.ProtoId] = item;

            if (mapperSettings.Paths.MapsDir != null)
                cmbMaps.Items.AddRange(Directory.GetFiles(mapperSettings.Paths.MapsDir, "*.fomap"));

            this.MouseWheel += new System.Windows.Forms.MouseEventHandler(panel1_MouseWheel);

            if(stream != null) stream.Close();
        }

        private void frmMain_Paint( object sender, PaintEventArgs e )
        {
            if( Frms.Count != 0 )
                return;
        }

        #region Menu functions

        private void menuFileOpen_Click( object sender, EventArgs e )
        {
            if( openMapDialog.ShowDialog( this ) == DialogResult.OK && File.Exists( openMapDialog.FileName ) )
            {
                menuFileExport.Enabled = true;
                LoadMap( openMapDialog.FileName );
            }
        }

        private void menuFileExportImage_Click( object sender, EventArgs e )
        {
            SaveFileDialog save = new SaveFileDialog();
            if( save.ShowDialog( this ) == DialogResult.OK )
            {
                Bitmap bmp = new Bitmap( panel1.ClientRectangle.Width, panel1.ClientRectangle.Height );
                panel1.DrawToBitmap( bmp, panel1.ClientRectangle );
                bmp.Save( save.FileName );
            }
        }

        private void UpdateDrawFlags( object sender, DrawMap.Flags flag )
        {
            if( ((ToolStripMenuItem)sender).Checked )
                this.drawFlags = this.drawFlags | flag;
            else
                this.drawFlags = this.drawFlags & ~flag;

            DrawMap.InvalidateCache(); 
            panel1.Refresh();
        }

        private void menuViewTiles_CheckedChanged( object sender, EventArgs e )
        {
            this.UpdateDrawFlags( sender, DrawMap.Flags.Tiles );
        }

        private void menuViewRoofs_CheckedChanged( object sender, EventArgs e )
        {
            this.UpdateDrawFlags( sender, DrawMap.Flags.Roofs );
        }

        private void menuViewCritters_CheckedChanged( object sender, EventArgs e )
        {
            this.UpdateDrawFlags( sender, DrawMap.Flags.Critters );
        }

        private void menuViewItems_CheckedChanged( object sender, EventArgs e )
        {
            this.UpdateDrawFlags( sender, DrawMap.Flags.Items );
        }

        private void menuViewScenery_CheckedChanged( object sender, EventArgs e )
        {
            this.UpdateDrawFlags( sender, DrawMap.Flags.Scenery );
        }

        private void menuViewSceneryWalls_CheckedChanged( object sender, EventArgs e )
        {
            this.UpdateDrawFlags( sender, DrawMap.Flags.SceneryWalls );
        }

        #endregion // Menu functions

        private void frmMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            Exit();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Exit();
        }

        private void pathsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (frmPaths == null || frmPaths.IsDisposed) frmPaths = new frmPaths(mapperSettings);
            frmPaths.Show();
        }

        private void performanceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (frmPerformance == null || frmPerformance.IsDisposed) frmPerformance = new frmPerformance(mapperSettings);
            frmPerformance.Show();
        }

        private void viewMapTreeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (frmMapTree == null || frmMapTree.IsDisposed) frmMapTree = new frmMapTree(CurrentMap);
            frmMapTree.Show();
            frmMapTree.TopMost = true;
        }

        private void panel1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                clickedPos.X = (int)((float)e.X / scaleFactor);
                clickedPos.Y = (int)((float)e.Y / scaleFactor);
                DrawMap.InvalidateCache();
                panel1.Refresh();
            }
        }

        private void debugToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (debugToolStripMenuItem.Checked)
            {
                if(frmDebugInfo == null || frmDebugInfo.IsDisposed)
                    frmDebugInfo = new frmDebugInfo();
                frmDebugInfo.Show();
                return;
            }

            if (frmDebugInfo == null || frmDebugInfo.IsDisposed) return;

            frmDebugInfo.Hide();
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            framesPerSecond = 0;
        }

        private void pnlViewPort_Scroll(object sender, ScrollEventArgs e)
        {
            //panel1.Refresh();
        }
    }
}
