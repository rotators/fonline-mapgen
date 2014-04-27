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
        public Dictionary<String, Bitmap> Bitmaps = new Dictionary<string, Bitmap>();
        public Dictionary<String, int> BitHeight = new Dictionary<string,int>();
        public Dictionary<String, int> BitWidth = new Dictionary<string, int>();

        public Dictionary<String, int> BitShiftX = new Dictionary<string, int>();
        public Dictionary<String, int> BitShiftY = new Dictionary<string, int>();

        List<ItemProto> items = new List<ItemProto>();

        FOCommon.Maps.FOHexMap hexmap = new FOCommon.Maps.FOHexMap();

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

            LoadDat(@"H:\FOnline\FOnlineStable\MASTER.DAT", Color.FromArgb(11, 0, 11));
            LoadDat(@"H:\FOnline\FOnlineDev\FONLINE.DAT", Color.FromArgb(11, 0, 11));
            //Bitmaps = Bitmaps.OrderBy(x => x.Key).ToDictionary<String, Bitmap>(;

            //string fileName = @"H:\FOnline\Factions\trunk\maps\hq_camp.fomap";
            string fileName = @"H:\FOnline\Factions\trunk\maps\newr1.fomap";
            FOCommon.Parsers.FOMapParser parser = new FOCommon.Parsers.FOMapParser(fileName);

            MSGParser FOObj = new MSGParser(@"H:\FOnline\Factions\trunk\text\engl\FOOBJ.MSG");
            FOObj.Parse();

            protoParser.LoadProtosFromFile(@"H:\FOnline\Factions\trunk\proto\items\door.fopro", "1.0", FOObj, items, null);
            protoParser.LoadProtosFromFile(@"H:\FOnline\Factions\trunk\proto\items\misc.fopro", "1.0", FOObj, items, null);
            protoParser.LoadProtosFromFile(@"H:\FOnline\Factions\trunk\proto\items\generic.fopro", "1.0", FOObj, items, null);
            protoParser.LoadProtosFromFile(@"H:\FOnline\Factions\trunk\proto\items\wall.fopro", "1.0", FOObj, items, null);

            this.Text = "Mapper experiment - " + fileName;

            parser.Parse();
            map = parser.Map;
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
                    Bitmap bmapRaw = FalloutFRM.Load(file.GetData())[0];

                    List<Bitmap> bmaps = FalloutFRM.Load(file.GetData(), Transparency);

                    Bitmaps[file.Path.ToLower()] = bmaps[0];
                    BitShiftX[file.Path.ToLower()] = FalloutFRM.GetPixelShiftX(file.GetData());
                    BitShiftY[file.Path.ToLower()] = FalloutFRM.GetPixelShiftY(file.GetData());
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
            if (Bitmaps.Count == 0) return;
            var g = e.Graphics;

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

        }

        private void DrawScenery(Graphics g, string scenery, int x, int y, int offx2, int offy2)
        {
            if (!Bitmaps.ContainsKey(scenery))
            {
                //MessageBox.Show(scenery + " not found");
                return;
            }

            Font font = new Font(Font.FontFamily, 10.0f, FontStyle.Bold);

            //g.DrawString("" + BitHeight[scenery], font, Brushes.Red, offset_x - (x % 2 == 0 ? 20 : 0) + xcoord, offset_y + ycoord - 10);

            var coords = hexmap.GetObjectCoords(new Point(x, y), Bitmaps[scenery].Size, new Point(BitShiftX[scenery], BitShiftY[scenery]), new Point(offx2, offy2));

            g.DrawImage(Bitmaps[scenery], coords.X, coords.Y);
            
            //g.DrawString(x + "," + y /*+ "(" + Math.Abs(y+(x/2)) + ")"*/, SystemFonts.DefaultFont, Brushes.Green, offset_x + xcoord, offset_y + ycoord - 30);
            //g.DrawLine(Pens.Pink, offset_x + xcoord, offset_y + ycoord, offset_x + xcoord, offset_y + ycoord);
        }

        private void DrawTile(Graphics g, string tile, int x, int y, bool isRoof)
        {
            if (!Bitmaps.ContainsKey(tile))
            {
                //MessageBox.Show(tile + " not found");
                return;
            }

            if (tile.Contains("misc"))
                MessageBox.Show(tile);

            var tileCoords = hexmap.GetTileCoords(new Point(x,y), isRoof);

            g.DrawImage(Bitmaps[tile], tileCoords.X, tileCoords.Y);
            
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            panel1.Refresh();
            panel1.Invalidate();
        }
    }
}
