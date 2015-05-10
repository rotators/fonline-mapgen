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
using System.Drawing.Imaging;

namespace fonline_mapgen
{
    public partial class frmMain : Form
    {
        public EditorData EditorData = new EditorData(ErrorMsgBox);

        public List<String> GraphicsPaths = new List<string>();
        public Dictionary<String, FalloutFRM> Frms = new Dictionary<string, FalloutFRM>();

        List<ItemProto> items = new List<ItemProto>();

        Color transparency = Color.FromArgb(11, 0, 11);

        CritterData critterData = new CritterData();

        float scaleFactor = 1.0f;

        // Selection
        MouseSelection mouseSelection = new MouseSelection(new Pen(Brushes.Red, 3.0f));
        
        PointF viewPortSize = new PointF();

        MapperSettings mapperSettings = new MapperSettings();
        ItemProtoParser protoParser = new ItemProtoParser();

        frmPaths frmPaths;
        frmPerformance frmPerformance;
        frmMapTree frmMapTree;
        frmDebugInfo frmDebugInfo;
        frmLoadMap frmLoadMap;
        frmLoading frmLoading;

        string title = "Mapper [ALPHA] - ";

        public static void ErrorMsgBox(string txt)
        {
            MessageBox.Show(txt);
        }

        public frmMain()
        {
            InitializeComponent();
            LoadResources();

            toolStripStatus.Text =
            toolStripStatusHex.Text =
            toolStripStatusProto.Text = "";
            
        }

        private void Exit()
        {
            mapperSettings.View.Tiles    = menuViewTiles.Checked;
            mapperSettings.View.Roofs    =  menuViewRoofs.Checked;
            mapperSettings.View.Critters = menuViewCritters.Checked;
            mapperSettings.View.Items    = menuViewItems.Checked;
            mapperSettings.View.Scenery  = menuViewScenery.Checked;
            mapperSettings.View.Walls    = menuViewSceneryWalls.Checked;

            SettingsManager.SaveSettings(mapperSettings);
            Environment.Exit(0);
        }

        private void LoadMap( string fileName )
        {
            MapperMap map = MapperMap.Load( fileName);
            if( map != null )
            {
                EditorData.AddMap( map );
                EditorData.CurrentMap = map;

                this.Text = title + fileName;

                headerToolStripMenuItem.Enabled =
                menuFileExport.Enabled =
                viewMapTreeToolStripMenuItem.Enabled = true;

                viewPortSize.X = ((map.GetEdgeCoords(FOHexMap.Direction.Right).X) - (map.GetEdgeCoords(FOHexMap.Direction.Left).X)) + 100.0f;
                viewPortSize.Y = ((map.GetEdgeCoords(FOHexMap.Direction.Down).Y)  - (map.GetEdgeCoords(FOHexMap.Direction.Up).Y)) + 100.0f;                 

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
            }

            this.BeginInvoke((MethodInvoker)delegate { 
                frmLoading.SetNextFile(Path.GetFileName(DatPath));
                frmLoading.SetResourceNum(files.Count);
            });

                foreach( DATFile file in files )
                {
                    this.BeginInvoke((MethodInvoker)delegate { frmLoading.SetNextResource(file.FileName); });

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

            var filesToLoad = new List<ZipStorer.ZipFileEntry>();

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

                    filesToLoad.Add(entry);
                }
            }

            this.BeginInvoke((MethodInvoker)delegate
            {
                frmLoading.SetNextFile(Path.GetFileName(ZipPath));
                frmLoading.SetResourceNum(filesToLoad.Count);
            });

            foreach(var entry in filesToLoad)
            {
                    string filename = entry.FilenameInZip.ToLower().Replace('/', '\\');
                    string ext = Path.GetExtension(filename).ToLower();

                    this.BeginInvoke((MethodInvoker)delegate
                    {
                        frmLoading.SetNextResource(filename);
                    });

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

            var screenArea = new RectangleF(pnlViewPort.HorizontalScroll.Value, pnlViewPort.VerticalScroll.Value, pnlViewPort.Width, pnlViewPort.Height);
            //MessageBox.Show(screenArea.ToString());

            DrawMap.OnGraphics(g, map, map.HexMap, EditorData.itemsPid, critterData, Frms, EditorData.drawFlags,
                EditorData.selectFlags, new SizeF(scaleFactor, scaleFactor), screenArea, mouseSelection.selectionArea, mouseSelection.clicked);

            if (mouseSelection.isDown)
                g.DrawRectangle(mouseSelection.rectPen, mouseSelection.GetRect());
        }

        private void RefreshViewport()
        {
            if (Frms.Count == 0)
                return;

            MapperMap map = EditorData.CurrentMap;

            if (map == null)
                return;

            DrawMap.InvalidateCache();
            pnlRenderBitmap.Refresh();

            if (frmDebugInfo != null && !frmDebugInfo.IsDisposed)
            {
                frmDebugInfo.setText("Objects rendered: " + DrawMap.GetNumCachedObjects());
                frmDebugInfo.setText("Objects selected: " + (DrawMap.GetSelectedObjects().Count + DrawMap.GetSelectedTiles().Count));
            }
        }

        private void panel1_Paint( object sender, PaintEventArgs e )
        {
            MapperMap map = EditorData.CurrentMap;

            if (map == null)
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
            pnlRenderBitmap.Width = (int)(viewPortSize.X * scaleFactor);
            pnlRenderBitmap.Height = (int)(viewPortSize.Y * scaleFactor);
        }

        private void panel1_MouseMove( object sender, MouseEventArgs e )
        {
            MapperMap map = EditorData.CurrentMap;

            if( map == null )
                return;

            if (mouseSelection.isDown)
            {
                mouseSelection.mouseRectPos.X = (int)((float)e.X / scaleFactor);
                mouseSelection.mouseRectPos.Y = (int)((float)e.Y / scaleFactor);

                mouseSelection.CalculateSelectionArea();

                DrawMap.InvalidateCache();
                int padding = 70;

                mouseSelection.UpdateMaxRect();

                int x1 = mouseSelection.clickedPos.X - padding;
                int y1 = mouseSelection.clickedPos.Y - padding;
                int x2 = mouseSelection.maxMouseRectPos.X + padding;
                int y2 = mouseSelection.maxMouseRectPos.Y + padding;

                pnlRenderBitmap.Invalidate(new Rectangle(x1, y1, x2 - x1, y2 - y1));
            }

            var hex = map.HexMap.GetHex(new PointF(e.X / scaleFactor, e.Y / scaleFactor + 6.0f));
            toolStripStatusHex.Text = string.Format( "Mouse Coords: {0},{1} - Hex: {2},{3}", e.X, e.Y, hex.X, hex.Y );

            if( map.Objects.Count( x => x.MapX == hex.X && x.MapY == hex.Y ) == 0 )
                toolStripStatusProto.Text = "";

            foreach( var obj in map.Objects.FindAll( x => x.MapX == hex.X && x.MapY == hex.Y ) )
            {
                if ((obj.MapObjType == FOCommon.Maps.MapObjectType.Item ||
                      obj.MapObjType == FOCommon.Maps.MapObjectType.Scenery))
                {
                    ItemProto prot;
                    if (!EditorData.itemsPid.TryGetValue(obj.ProtoId, out prot))
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

        private void headerToolStripMenuItem_Click( object sender, EventArgs e )
        {
            MapperMap map = EditorData.CurrentMap;

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
            SettingsManager.Init();
            mapperSettings = SettingsManager.LoadSettings();

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

            resourceLoader.RunWorkerAsync();
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
                Bitmap bmp = new Bitmap( pnlRenderBitmap.ClientRectangle.Width, pnlRenderBitmap.ClientRectangle.Height );
                pnlRenderBitmap.DrawToBitmap( bmp, pnlRenderBitmap.ClientRectangle );
                bmp.Save( save.FileName );
            }
        }

        private void UpdateSelectFlags(object sender, DrawMap.Flags flag)
        {
            EditorData.UpdateSelectFlags(((ToolStripMenuItem)sender).Checked, flag);
        }

        private void UpdateDrawFlags( object sender, DrawMap.Flags flag )
        {
            EditorData.UpdateDrawFlags(((ToolStripMenuItem)sender).Checked, flag);
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
            
            frmPerformance.ShowDialog();

            pnlRenderBitmap.Visible = true;
            pnlViewPort.Visible = true;
        }

        private void viewMapTreeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (frmMapTree == null || frmMapTree.IsDisposed) frmMapTree = new frmMapTree(EditorData.CurrentMap);
            frmMapTree.Show();
            frmMapTree.TopMost = true;
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
            //RefreshViewport();
        }

        private void findMapsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmFindMaps frmFindMaps = new frmFindMaps(EditorData.mapsFiles, Frms.Keys.ToList<string>());
            frmFindMaps.Show();
        }

        private void panel1_MouseDown(object sender, MouseEventArgs e)
        {
            if (mouseSelection.isDown)
                return;

            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                mouseSelection.clicked = false;
                mouseSelection.clickedPos.X = (int)((float)e.X / scaleFactor);
                mouseSelection.clickedPos.Y = (int)((float)e.Y / scaleFactor);
                mouseSelection.isDown = true;

                mouseSelection.maxMouseRectPos.X = 0;
                mouseSelection.maxMouseRectPos.Y = 0;

                mouseSelection.selectionArea.Width = 1;
                mouseSelection.selectionArea.Height = 1;

                RefreshViewport();
            }
        }

        private void panel1_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                mouseSelection.isDown = false;
                mouseSelection.clicked  = false;

                mouseSelection.CalculateSelectionArea();

                mouseSelection.mouseRectPos.X = 0;
                mouseSelection.mouseRectPos.Y = 0;

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
                    EditorData.CurrentMap.Objects.Remove(obj);
                foreach (var tile in DrawMap.GetSelectedTiles())
                    EditorData.CurrentMap.Tiles.Remove(tile);

                RefreshViewport();
            }
        }

        private void frmMain_KeyDown(object sender, KeyEventArgs e)
        {
            
        }

        private void frmMain_Load(object sender, EventArgs e)
        {
            
        }

        private void resourceLoader_DoWork(object sender, DoWorkEventArgs e)
        {
            GraphicsPaths.Add("art\\misc");
            GraphicsPaths.Add("art\\items");
            GraphicsPaths.Add("art\\tiles");
            GraphicsPaths.Add("art\\misc");
            GraphicsPaths.Add("art\\walls");
            GraphicsPaths.Add("art\\door");
            GraphicsPaths.Add("art\\scenery");

            Stream stream;
            BinaryFormatter formatter = new BinaryFormatter();

            stream = null;

            this.BeginInvoke((MethodInvoker)delegate
            {
                frmLoading = new frmLoading();
                frmLoading.ShowDialog();

                int count = mapperSettings.Paths.DataFiles.Count;

                if (!File.Exists("./critters.dat")) count++;
                if (!File.Exists("./items.dat")) count++;

                frmLoading.SetFilesNum(count);
            });


            if (File.Exists("./critters.dat"))
            {
                stream = File.OpenRead("./critters.dat");
                critterData = (CritterData)formatter.Deserialize(stream);
                stream.Close();
            }
            else
            {
                List<string> crFiles = new List<string>(Directory.GetFiles(mapperSettings.Paths.CritterProtos));

                this.BeginInvoke((MethodInvoker)delegate
                {
                    frmLoading.SetNextFile(mapperSettings.Paths.CritterProtos);
                    frmLoading.SetResourceNum(crFiles.Count);
                });

                foreach (string file in crFiles)
                {
                    this.BeginInvoke((MethodInvoker)delegate
                    {
                        frmLoading.SetNextResource(file);
                    });

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
                    List<string> protFiles = new List<string>(File.ReadAllLines(itemslst));

                    this.BeginInvoke((MethodInvoker)delegate
                    {
                        frmLoading.SetNextFile(itemslst);
                        frmLoading.SetResourceNum(protFiles.Count);
                    });

                    foreach (string file in protFiles)
                    {
                        this.BeginInvoke((MethodInvoker)delegate { frmLoading.SetNextResource(file); });
                        protoParser.LoadProtosFromFile(mapperSettings.Paths.ItemProtos + file, "1.0", FOObj, items, null);
                    }
                }
                if (mapperSettings.Performance.CacheResources)
                {
                    stream = File.Create("./items.dat");
                    formatter.Serialize(stream, items);
                }
            }

            foreach (var item in items)
                EditorData.itemsPid[item.ProtoId] = item;

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

                    if (mapperSettings.Performance.CacheResources)
                    {
                        stream = File.Create("./graphics.dat");
                        formatter.Serialize(stream, Frms);
                    }
                }
            }

            if (mapperSettings.Paths.MapsDir != null)
            {
                while (!Directory.Exists(mapperSettings.Paths.MapsDir))
                {
                    MessageBox.Show("Maps path: " + mapperSettings.Paths.MapsDir + " not found. ", "Maps path not found.", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    if (frmPaths.ShowDialog() == System.Windows.Forms.DialogResult.Cancel)
                        break;
                }
                EditorData.mapsFiles = Directory.GetFiles(mapperSettings.Paths.MapsDir, "*.fomap").ToList<string>();
                this.BeginInvoke((MethodInvoker)delegate
                {
                    frmLoadMap = new frmLoadMap(EditorData.mapsFiles, LoadMap);
                    frmLoadMap.Show();
                    frmLoading.Hide();
                });
            }

            if (stream != null) stream.Close();
        }
    }

    public class MouseSelection
    {
        public MouseSelection(Pen RectanglePen)
        {
            this.rectPen = RectanglePen;
        }

        public void CalculateSelectionArea()
        {
            this.selectionArea = new RectangleF(clickedPos.X, clickedPos.Y, mouseRectPos.X - clickedPos.X, mouseRectPos.Y - clickedPos.Y);
            if (this.selectionArea.Width <= 0)
            {
                this.selectionArea.Width = 1;
                this.selectionArea.Height = 1;
                this.clicked = true;
            }
        }

        public Rectangle GetRect()
        {
            return new Rectangle(clickedPos.X, clickedPos.Y, mouseRectPos.X - clickedPos.X, mouseRectPos.Y - clickedPos.Y);
        }

        public void UpdateMaxRect()
        {
            if (mouseRectPos.X > maxMouseRectPos.X) maxMouseRectPos.X = mouseRectPos.X;
            if (mouseRectPos.Y > maxMouseRectPos.Y) maxMouseRectPos.Y = mouseRectPos.Y;
        }

        public RectangleF selectionArea;
        public Point clickedPos;
        public Point maxMouseRectPos;
        public Point mouseRectPos;
        public Pen rectPen;
        public bool isDown;
        public bool clicked;
    }
}
