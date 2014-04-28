using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DATLib;
using System.IO;

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

        FOCommon.Maps.FOHexMap hexmap;
        FOCommon.Parsers.FOMapParser parser;

        ItemProtoParser protoParser = new ItemProtoParser();

        public FOCommon.Maps.FOMap map;

        public frmMain()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            GraphicsPaths.Add("art\\tiles");
            GraphicsPaths.Add("art\\misc");
            GraphicsPaths.Add("art\\walls");
            GraphicsPaths.Add("art\\door");
            GraphicsPaths.Add("art\\scenery");

            foreach( string dataFile in UGLY.DataFiles )
            {
                LoadDat( dataFile, Color.FromArgb( 11, 0, 11 ) );
            }
            //Bitmaps = Bitmaps.OrderBy(x => x.Key).ToDictionary<String, Bitmap>(;

            cmbMaps.Items.AddRange(Directory.GetFiles(UGLY.ServerDir+@"maps\", "*.fomap"));

            MSGParser FOObj = new MSGParser(UGLY.ServerDir+@"text\engl\FOOBJ.MSG");
            FOObj.Parse();

            protoParser.LoadProtosFromFile( UGLY.ServerDir + @"proto\items\door.fopro", "1.0", FOObj, items, null );
            protoParser.LoadProtosFromFile( UGLY.ServerDir + @"proto\items\misc.fopro", "1.0", FOObj, items, null );
            protoParser.LoadProtosFromFile( UGLY.ServerDir + @"proto\items\generic.fopro", "1.0", FOObj, items, null );
            protoParser.LoadProtosFromFile( UGLY.ServerDir + @"proto\items\wall.fopro", "1.0", FOObj, items, null );

            //string fileName = UGLY.ServerDir+@"maps\hq_camp.fomap";
            LoadMap(UGLY.ServerDir+@"maps\den.fomap");
        }

        private void LoadMap(string fileName)
        {
            parser = new FOCommon.Parsers.FOMapParser(fileName);
            parser.Parse();
            map = parser.Map;
            hexmap = new FOHexMap(new Size(map.Header.MaxHexX, map.Header.MaxHexY));
            this.Text = "Mapper Experiment - " + fileName;
        }

        public void WriteLog(string str)
        {
            File.AppendAllText("./debug.log", str + Environment.NewLine);
        }

        public bool LoadDat(string DatPath, Color Transparency)
        {
            DatReaderError status;
            DAT loadedDat = DATReader.ReadDat(DatPath, out status);
            if (status.Error != DatError.Success)
            {
                MessageBox.Show("Error loading " + DatPath + ": " + Environment.NewLine + status.Message, "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            List<DATFile> files = new List<DATFile>();
            foreach (string path in this.GraphicsPaths)
            {
                files.AddRange(loadedDat.GetFilesByPattern(path));
            }

            foreach (DATFile file in files)
            {
                string ext = Path.GetExtension(file.FileName).ToLower();
                if (!(ext == ".frm" || ext == ".png"))
                    continue;

                if (ext == ".frm")
                {
                    var frm = FalloutFRMLoader.LoadFRM(file.GetData(), Transparency);
                    frm.FileName = file.Path.ToLower();
                    Frms[frm.FileName] = frm; 
                }
                else
                {
                    System.ComponentModel.TypeConverter tc = System.ComponentModel.TypeDescriptor.GetConverter(typeof(Bitmap));
                    Bitmap bitmap = (Bitmap)tc.ConvertFrom(file.GetData());
                }
            }

            loadedDat.Close();

            return true;
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {
            if (Frms.Count == 0) return;
            var g = e.Graphics;
            //g.CompositingMode = System.Drawing.Drawing2D.CompositingMode.SourceCopy;
            g.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighSpeed;
            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;

            // Draw normal tiles.
            foreach (var tile in map.Tiles.Where(x => !x.Roof))
            {
                DrawTile(g, tile.Path, tile.X, tile.Y, false);
            }

            foreach (var obj in map.Objects.OrderBy(x => x.MapX + x.MapY*2))
            {
                // TODO: Draw critters.
                if (!(obj.MapObjType == FOCommon.Maps.MapObjectType.Item ||
                      obj.MapObjType == FOCommon.Maps.MapObjectType.Scenery)) continue;
               
                ItemProto prot = items.Where(x => x.ProtoId == obj.ProtoId).FirstOrDefault();
                if (prot == null)
                    continue;

               // WriteLog("Drawing " + prot.PicMap);

                DrawScenery(g, prot.PicMap, obj.MapX, obj.MapY, prot.OffsetX, prot.OffsetY);
            }

            foreach (var tile in map.Tiles.Where(x => x.Roof))
            {
                DrawTile(g, tile.Path, tile.X, tile.Y, true);
            }
            //g.CompositingMode = System.Drawing.Drawing2D.CompositingMode.SourceOver;
            //MessageBox.Show("paint_event");
        }

        private void DrawScenery(Graphics g, string scenery, int x, int y, int offx2, int offy2)
        {
            if (!Frms.ContainsKey(scenery))
            {
                //MessageBox.Show(scenery + " not found");
                return;
            }

            Font font = new Font(Font.FontFamily, 10.0f, FontStyle.Bold);

            var frm = Frms[scenery];

            //g.DrawString("" + BitHeight[scenery], font, Brushes.Red, offset_x - (x % 2 == 0 ? 20 : 0) + xcoord, offset_y + ycoord - 10);

            var coords = hexmap.GetObjectCoords(new Point(x, y), frm.Frames[0].Size, new Point(frm.PixelShift.X, frm.PixelShift.Y), new Point(offx2, offy2));

            g.DrawImage(frm.Frames[0], coords.X, coords.Y);
        }

        private void DrawTile(Graphics g, string tile, int x, int y, bool isRoof)
        {
            if (!Frms.ContainsKey(tile))
            {
                //MessageBox.Show(tile + " not found");
                return;
            }

            if (tile.Contains("misc"))
                MessageBox.Show(tile);

            var tileCoords = hexmap.GetTileCoords(new Point(x,y), isRoof);

            g.DrawImage(Frms[tile].Frames[0], tileCoords.X, tileCoords.Y);
            
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            panel1.Refresh();
            panel1.Invalidate();
        }

        private void panel1_MouseMove(object sender, MouseEventArgs e)
        {
            var hex = hexmap.GetHex(new PointF(e.X,e.Y + 6.0f));
            lblMouseCoords.Text = string.Format("Mouse Coords: {0},{1} - Hex: {2},{3}", e.X, e.Y, hex.X, hex.Y);

            //

            if (map.Objects.Count(x => x.MapX == hex.X && x.MapY == hex.Y) == 0)
                lblProtos.Text = "Proto: ";
            foreach (var obj in map.Objects.FindAll(x => x.MapX == hex.X && x.MapY == hex.Y))
            {
                if (!(obj.MapObjType == FOCommon.Maps.MapObjectType.Item ||
                      obj.MapObjType == FOCommon.Maps.MapObjectType.Scenery)) continue;
                //MessageBox.Show(""+obj.ProtoId);
                //if()

                ItemProto prot = items.Where(x => x.ProtoId == obj.ProtoId).FirstOrDefault();
                if (prot == null)
                    continue;

                lblProtos.Text = "Proto: " + (obj.ProtoId);
                lblProtos.Text += string.Format(" ({0} - {1})", prot.Name, prot.PicMap);
            }
        }

        private void panel1_MouseClick(object sender, MouseEventArgs e)
        {
            var hex = hexmap.GetHex(new PointF(e.X - 12.0f, e.Y - 12.0f));

            /**/

            //MessageBox.Show("HexX = " + .X);
        }

        private void btnLoadMap_Click(object sender, EventArgs e)
        {
            LoadMap((string)cmbMaps.SelectedItem);
            pnlViewPort.Invalidate();
            pnlViewPort.Refresh();
        }

        private void headerToolStripMenuItem_Click( object sender, EventArgs e )
        {
            if( this.map == null )
            {
                MessageBox.Show("Map not loaded!");
                return;
            }

            frmHeaderEditor formHeaderEditor = new frmHeaderEditor( this.map.Header );
            formHeaderEditor.ShowDialog();
        }
    }
}
