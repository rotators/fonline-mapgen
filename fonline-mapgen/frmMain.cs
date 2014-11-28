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
using System.IO.Compression;
using SharpGL;
using System.Drawing.Imaging;

namespace fonline_mapgen
{
    public partial class frmMain : Form
    {
        public List<String> GraphicsPaths = new List<string>();
        //public Dictionary<String, Bitmap> Bitmaps = new Dictionary<string, Bitmap>();

        public Dictionary<String, FalloutFRM> Frms = new Dictionary<string, FalloutFRM>();
        uint[] textures = new uint[1];
        bool glLoadedImages = false;
        bool glIsInit = false;

        List<ItemProto> items = new List<ItemProto>();

        Color transparency = Color.FromArgb(11, 0, 11);

        CritterData critterData = new CritterData();

        List<string> maps = new List<string>();
        Dictionary<int, ItemProto> itemsPid = new Dictionary<int, ItemProto>();

        float scaleFactor = 1.0f;

        float glScale = 0.1f;

        bool glMode = true;

        // Selection
        Point clickedPos = new Point(0,0);
        Point mouseRectPos = new Point(0, 0);
        Pen rectPen = new Pen(Brushes.LightGreen, 5.0f);
        bool isMouseDown = false;
        bool selectionClicked = false;

        float rotation;

        float glPosX = 0.0f;
        float glPosY = 0.0f;

        RectangleF selectionArea = new RectangleF();
        PointF viewPortSize = new PointF();

        MapperSettings mapperSettings = new MapperSettings();

        //FOCommon.Parsers.FOMapParser parser;

        ItemProtoParser protoParser = new ItemProtoParser();

        frmPaths frmPaths;
        frmPerformance frmPerformance;
        frmMapTree frmMapTree;
        frmDebugInfo frmDebugInfo;

        string title = "Mapper [ALPHA] - ";


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
        //TabPage TabTemplate;

        DrawMap.Flags drawFlags;
        DrawMap.Flags selectFlags;

        public frmMain()
        {
            InitializeComponent();
            LoadResources();

            toolStripStatus.Text =
            toolStripStatusHex.Text =
            toolStripStatusProto.Text = "";

            openGLControl1.Location = pnlViewPort.Location;
            openGLControl1.Width = pnlViewPort.Width;
            openGLControl1.Height = pnlViewPort.Height;
            openGLControl1.RenderTrigger = RenderTrigger.TimerBased;

            if (glMode)
            {
                panel1.Visible = false;
                pnlViewPort.Visible = false;
                openGLControl1.Enabled = true;
            }
            else
            {
                openGLControl1.Enabled = false;
                openGLControl1.Visible = false;
            }
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
            MapperMap map = MapperMap.Load( fileName, glMode );
            if( map != null )
            {
                this.Maps.Add( map );
                this.CurrentMap = map;

                this.Text = title + fileName;

                headerToolStripMenuItem.Enabled =
                menuFileExport.Enabled =
                viewMapTreeToolStripMenuItem.Enabled = true;

                viewPortSize.X = ((map.GetEdgeCoords(FOHexMap.Direction.Right).X) - (map.GetEdgeCoords(FOHexMap.Direction.Left).X)) + 100.0f;
                viewPortSize.Y = ((map.GetEdgeCoords(FOHexMap.Direction.Down).Y)  - (map.GetEdgeCoords(FOHexMap.Direction.Up).Y)) + 100.0f;

                if (glMode)
                {
                    /*OpenGL gl = openGLControl1.OpenGL;
                    gl.MatrixMode(OpenGL.GL_PROJECTION);
                    gl.LoadIdentity();*/
                    //gl.Viewport(0, 0, (int)viewPortSize.X, (int)viewPortSize.Y);
                }
                 

                resizeViewport();
                centerViewport();
                RefreshViewport();

                // TODO: Check critter/proto names and PID in the map itself instead of just graphics.
                var errors = DrawMap.GetErrors();
                errors.Sort();
                frmErrors frmErrors = new frmErrors(fileName, string.Join(Environment.NewLine, errors.Distinct().ToArray()));
                if(errors.Count != 0)
                    frmErrors.ShowDialog();
            }
            else
                MessageBox.Show( "Error loading map " + fileName );
        }

        public void WriteLog( string str )
        {
            File.AppendAllText( "./debug.log", str + Environment.NewLine );
        }

        public bool LoadDat( string DatPath, List<string> crFiles, Color Transparency )
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

        public bool LoadDir(string DirPath, List<string> crFiles, Color Transparency)
        {
            List<string> filenames = new List<string>();
            foreach (var path in this.GraphicsPaths)
            {
                var fullPath = DirPath + Path.DirectorySeparatorChar + path;
                if(Directory.Exists(fullPath))
                    filenames.AddRange(Directory.GetFiles(fullPath));
            }
            foreach(var crFile in crFiles)


            foreach (var filename in filenames)
            {
                string ext = Path.GetExtension(filename).ToLower();
                if (!(ext == ".frm" || ext == ".png"))
                    continue;
                byte[] bytes = File.ReadAllBytes(filename);
                if (ext == ".frm")
                {
                    var frm = FalloutFRMLoader.LoadFRM(bytes, Transparency);
                    Frms[filename] = frm;
                }
                else
                {
                    System.ComponentModel.TypeConverter tc = System.ComponentModel.TypeDescriptor.GetConverter(typeof(Bitmap));
                    Bitmap bitmap = (Bitmap)tc.ConvertFrom(bytes);
                    Frms[filename] = new FalloutFRM();
                    Frms[filename].Frames = new List<Bitmap>();
                    Frms[filename].Frames.Add(bitmap);
                    Frms[filename].FileName = filename.Remove(filename.IndexOf(DirPath), DirPath.Length);
                }
            }
            return true;
        }

        public bool LoadZip( string ZipPath, List<string> crFiles, Color Transparency )
        {
            if (!File.Exists(ZipPath))
            {
                MessageBox.Show("Unable to load " + ZipPath + ", doesn't exist.", "Mapper", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            var zip = ZipStorer.Open(ZipPath, FileAccess.Read);
            if(zip == null)
            {
                MessageBox.Show("Unable to load " + ZipPath + ".", "Mapper", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            var patternToCheck = new List<string>();
            foreach (var cr in crFiles)
                patternToCheck.Add(cr);
            foreach (var path in this.GraphicsPaths)
                patternToCheck.Add(path);

            var entries = zip.ReadCentralDir();
            foreach (var entry in entries)
            {
                foreach(var path in patternToCheck)
                {
                    if (!entry.FilenameInZip.Replace('/', '\\').Contains(path))
                        continue;
                    if (entry.CompressedSize == 0) continue;

                    string filename = entry.FilenameInZip.ToLower().Replace('/', '\\');
                    string ext = Path.GetExtension( filename ).ToLower();
                    
                    if( !(ext == ".frm" || ext == ".png") )
                        continue;

                    byte[] bytes;

                    using (MemoryStream stream = new MemoryStream())
                    {
                        zip.ExtractFile(entry, stream);
                        bytes = stream.ToArray();
                    }
                    if (ext == ".frm")
                    {
                        var frm = FalloutFRMLoader.LoadFRM(bytes, Transparency);
                        Frms[filename] = frm;
                    }
                    else
                    {
                        System.ComponentModel.TypeConverter tc = System.ComponentModel.TypeDescriptor.GetConverter(typeof(Bitmap));
                        Bitmap bitmap = (Bitmap)tc.ConvertFrom(bytes);
                        Frms[filename] = new FalloutFRM();
                        Frms[filename].Frames = new List<Bitmap>();
                        Frms[filename].Frames.Add(bitmap);
                        Frms[filename].FileName = entry.FilenameInZip;
                    }

                }
            }
            return true;
        }

        private void DrawGDI(Graphics g, MapperMap map)
        {
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


            DrawMap.OnGraphics(g, null, map, map.HexMap, itemsPid, critterData, Frms, this.drawFlags, 
                this.selectFlags, new SizeF(scaleFactor, scaleFactor), selectionArea, selectionClicked, 0.0f, 0.0f);

            if (isMouseDown)
                g.DrawRectangle(rectPen, clickedPos.X, clickedPos.Y, mouseRectPos.X - clickedPos.X, mouseRectPos.Y - clickedPos.Y);
        }

        private void DrawGL(MapperMap map)
        {
            OpenGL gl = openGLControl1.OpenGL;

            //  Load the identity matrix.
            gl.Clear(OpenGL.GL_COLOR_BUFFER_BIT);
            gl.Enable(OpenGL.GL_TEXTURE_2D);
            gl.Enable(OpenGL.GL_MULTISAMPLE);

            // Flip since it's bitmaps
            gl.MatrixMode(OpenGL.GL_TEXTURE);
            gl.LoadIdentity();
            gl.Scale(1.0f, -1.0f, 1.0f);
            gl.MatrixMode(OpenGL.GL_MODELVIEW);
            gl.PushMatrix();

            gl.Translate(glPosX, glPosY, 0.0f);
            gl.Scale(glScale, glScale, 0.0f);

            DrawMap.OnGraphics(null, openGLControl1.OpenGL, map, map.HexMap, itemsPid, critterData, Frms,
                this.drawFlags, this.selectFlags, new SizeF(scaleFactor, scaleFactor), selectionArea, selectionClicked, (float)numericUpDown1.Value, (float)numericUpDown2.Value);


            gl.PopMatrix();
            gl.Flush();
        }

        private void RefreshViewport()
        {
            if (Frms.Count == 0)
                return;

            MapperMap map = this.CurrentMap;

            if (map == null)
                return;

            DrawMap.InvalidateCache();
            if (glMode)
            {
                DrawGL(map);
            }
            else 
            {
                panel1.Refresh();
            }

            if (frmDebugInfo != null && !frmDebugInfo.IsDisposed)
            {
                frmDebugInfo.setText("Objects rendered: " + DrawMap.GetNumCachedObjects());
                frmDebugInfo.setText("Objects selected: " + (DrawMap.GetSelectedObjects().Count + DrawMap.GetSelectedTiles().Count));
            }
        }

        private void panel1_Paint( object sender, PaintEventArgs e )
        {
            MapperMap map = this.CurrentMap;

            if (map == null)
                return;
            if (glMode)
                return;

            DrawGDI(e.Graphics, map);
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
            RefreshViewport();
        }

        private void panel1_MouseMove( object sender, MouseEventArgs e )
        {
            MapperMap map = this.CurrentMap;

            if( map == null )
                return;

            if (isMouseDown)
            {
                mouseRectPos.X = (int)((float)e.X / scaleFactor);
                mouseRectPos.Y = (int)((float)e.Y / scaleFactor);

                selectionArea = new RectangleF(clickedPos.X, clickedPos.Y, mouseRectPos.X - clickedPos.X, mouseRectPos.Y - clickedPos.Y);

                DrawMap.InvalidateCache();
                RefreshViewport();
            }

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
            glMode = mapperSettings.GLMode;

            frmPaths = new frmPaths(mapperSettings);
            if (mapperSettings == null)
            {
                mapperSettings = new MapperSettings();
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
                    MessageBox.Show("No " + itemslst + " , unable to load item protos.", "No items.lst", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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
                    frmPaths.ShowDialog();
                }

                if (mapperSettings.Paths.DataFiles != null || mapperSettings.Paths.DataFiles.Count != 0)
                {

                    List<string> crFiles = new List<string>();
                    foreach (var crType in critterData.crTypeGraphic.Values)
                    {
                        crFiles.Add("art\\critters\\" + crType.ToUpper() + "AA.FRM"); // Idle anim
                        crFiles.Add("art\\critters\\" + crType.ToLower() + "aa.frm");
                    }

                    foreach (string dataFile in mapperSettings.Paths.DataFiles)
                    {
                        string ext = Path.GetExtension(dataFile).ToLower();
                        if (ext != ".dat" && ext != ".zip")
                        {
                            MessageBox.Show("Unknown datafile extension : " + dataFile);
                            continue;
                        }
                        if (ext == ".dat")
                            LoadDat(dataFile, crFiles, transparency);
                        else
                            LoadZip(dataFile, crFiles, transparency);
                    }
                    foreach (string dataDir in mapperSettings.Paths.DataDirs)
                    {
                        LoadDir(dataDir, crFiles, transparency);
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
            {
                while(!Directory.Exists(mapperSettings.Paths.MapsDir))
                {
                    MessageBox.Show("Maps path: " + mapperSettings.Paths.MapsDir + " not found. ", "Maps path not found.", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    if (frmPaths.ShowDialog() == System.Windows.Forms.DialogResult.Cancel)
                        break;
                }
                maps = Directory.GetFiles(mapperSettings.Paths.MapsDir, "*.fomap").ToList<string>();
                cmbMaps.Items.AddRange(maps.ToArray());
            }
                

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

        private void UpdateSelectFlags(object sender, DrawMap.Flags flag)
        {
            if (((ToolStripMenuItem)sender).Checked)
                this.selectFlags = this.selectFlags | flag;
            else
                this.selectFlags = this.selectFlags & ~flag;
        }

        private void UpdateDrawFlags( object sender, DrawMap.Flags flag )
        {
            if( ((ToolStripMenuItem)sender).Checked )
                this.drawFlags = this.drawFlags | flag;
            else
                this.drawFlags = this.drawFlags & ~flag;

            RefreshViewport();
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
            
            //bool oldMode = glMode;
            frmPerformance.ShowDialog();
            glMode = mapperSettings.GLMode;

            panel1.Visible = !glMode;
            pnlViewPort.Visible = !glMode;
            openGLControl1.Enabled = glMode;
            openGLControl1.Visible = glMode;

            /*if (mapperSettings.GLMode && oldMode != glMode)
            {

            }
            if (!mapperSettings.GLMode && oldMode == glMode)
            {

            }*/
        }

        private void viewMapTreeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (frmMapTree == null || frmMapTree.IsDisposed) frmMapTree = new frmMapTree(CurrentMap);
            frmMapTree.Show();
            frmMapTree.TopMost = true;
        }

        private void panel1_MouseDown(object sender, MouseEventArgs e)
        {
            if (isMouseDown)
                return;

            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                selectionClicked = false;
                clickedPos.X = (int)((float)e.X / scaleFactor);
                clickedPos.Y = (int)((float)e.Y / scaleFactor);
                isMouseDown = true;
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

        private void pnlViewPort_Scroll(object sender, ScrollEventArgs e)
        {
            
        }

        private void toolsToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void findMapsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmFindMaps frmFindMaps = new frmFindMaps(maps, Frms.Keys.ToList<string>());
            frmFindMaps.Show();
        }

        private void panel1_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                isMouseDown = false;
                selectionClicked = false;
                selectionArea = new RectangleF(clickedPos.X, clickedPos.Y, mouseRectPos.X - clickedPos.X, mouseRectPos.Y - clickedPos.Y);

                if (selectionArea.Width <= 0)
                {
                    selectionArea.Width = 1;
                    selectionArea.Height = 1;
                    selectionClicked = true;
                }

                mouseRectPos.X = 0;
                mouseRectPos.Y = 0;

                RefreshViewport();
            }
        }

        private void menuSelectionTiles_CheckedChanged(object sender, EventArgs e)
        {
            UpdateSelectFlags(sender, DrawMap.Flags.Tiles);
        }

        private void menuSelectionRoofs_CheckedChanged(object sender, EventArgs e)
        {
            UpdateSelectFlags(sender, DrawMap.Flags.Roofs);
        }

        private void menuSelectionCritters_CheckedChanged(object sender, EventArgs e)
        {
            UpdateSelectFlags(sender, DrawMap.Flags.Critters);
        }

        private void menuSelectionItems_CheckedChanged(object sender, EventArgs e)
        {
            UpdateSelectFlags(sender, DrawMap.Flags.Items);
        }

        private void menuSelectionScenery_CheckedChanged(object sender, EventArgs e)
        {
            UpdateSelectFlags(sender, DrawMap.Flags.Scenery);
        }

        private void menuSelectionSceneryWalls_CheckedChanged(object sender, EventArgs e)
        {
            UpdateSelectFlags(sender, DrawMap.Flags.SceneryWalls);
        }

        private void frmMain_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
            {
                foreach (var obj in DrawMap.GetSelectedObjects())
                    CurrentMap.Objects.Remove(obj);
                foreach (var tile in DrawMap.GetSelectedTiles())
                    CurrentMap.Tiles.Remove(tile);

                RefreshViewport();
            }
        }


        private void openGLControl1_OpenGLInitialized(object sender, EventArgs e)
        {
            OpenGL gl = openGLControl1.OpenGL;
            gl.ClearColor(0, 0, 0, 0);

            gl.Disable(OpenGL.GL_DEPTH_TEST);

            gl.MatrixMode(OpenGL.GL_PROJECTION);
            gl.LoadIdentity();
            gl.Ortho(0.0, 1, 1, 0, -1, 1);
            gl.Viewport(0, 0, openGLControl1.Width, openGLControl1.Height);
            gl.MatrixMode(OpenGL.GL_MODELVIEW);
        }

        private void openGLControl1_Resized(object sender, EventArgs e)
        {
            //  Get the OpenGL object.
            OpenGL gl = openGLControl1.OpenGL;
        }

        private void openGLControl1_OpenGLDraw(object sender, RenderEventArgs args)
        {
            if (CurrentMap == null)
                return;

            DrawGL(CurrentMap);
        }

        private void frmMain_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.W)
                glPosY -= 0.5f;
            if (e.KeyCode == Keys.S)
                glPosY += 0.5f;
            if (e.KeyCode == Keys.A)
                glPosX += 0.5f;
            if (e.KeyCode == Keys.D)
                glPosX -= 0.5f;

            if (e.KeyCode == Keys.PageUp)
                glScale += 0.005f;
            if (e.KeyCode == Keys.PageDown)
                glScale -= 0.005f;
        }
    }
}
