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
using FOCommon.Graphic;
using FOCommon.Parsers;
using FOCommon.Items;

namespace fonline_mapgen
{
    public partial class Form1 : Form
    {
        public List<String> GraphicsPaths = new List<string>();
        public Dictionary<String, Bitmap> Bitmaps = new Dictionary<string, Bitmap>();
        public Dictionary<String, int> BitHeight = new Dictionary<string,int>();
        public Dictionary<String, int> BitWidth = new Dictionary<string, int>();

        public Dictionary<String, int> BitShiftX = new Dictionary<string, int>();
        public Dictionary<String, int> BitShiftY = new Dictionary<string, int>();

        List<ItemProto> items = new List<ItemProto>();

        float offset_x = 3000;
        float offset_y = 100;

        float offset_tile_x = -64;
        float offset_tile_y = -16;


        ItemProtoParser protoParser = new ItemProtoParser();

        public FOCommon.Maps.FOMap map;

        public Form1()
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
           // LoadDat(@"H:\FOnline\FOnlineDev\FONLINE.DAT", Color.FromArgb(11, 0, 11));
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

                //if (!file.Path.ToLower().Contains(pattern))
                //   continue;

                if (ext == ".frm")
                {
                    Bitmap bmapRaw = FalloutFRM.Load(file.GetData())[0];

                    List<Bitmap> bmaps = FalloutFRM.Load(file.GetData(), Transparency);
                    //List<Bitmap> bmaps2 = FalloutFRM.Load(file.GetData(), Color.FromArgb(255,255,255));

                    Bitmaps[file.Path.ToLower()] = bmaps[0];
                    BitShiftX[file.Path.ToLower()] = FalloutFRM.GetPixelShiftX(file.GetData());
                    BitShiftY[file.Path.ToLower()] = FalloutFRM.GetPixelShiftY(file.GetData());

                    //WriteLog(string.Format("shift x {0} - file {1}", FalloutFRM.GetPixelShiftX(file.GetData()), file.Path.ToLower()));
                    //WriteLog(string.Format("shift y {0} - file {1}", FalloutFRM.GetPixelShiftY(file.GetData()), file.Path.ToLower()));
                    //Bitmaps[file.Path.ToLower()] = bmaps[0];
                    
                    /*int firsth = -1, lasth = -1;
                    int firstw = -1, lastw = -1;

                    int maxh = bmapRaw.Height;
                    int maxw = bmapRaw.Width;

                    for (int h = 0; h < maxh; h++)
                    {
                        for (int w = 0; w < maxw; w++)
                        {
                            var c = bmapRaw.GetPixel(w, h);
                            if (c == null)
                                break;
                            if (c == Transparency)
                            {
                                
                              //  WriteLog(string.Format("Transparent - h: {0} - w: {1}", h, maxw/2));
                                continue;
                            }
                            if (firsth == -1)
                            {
                                firsth = h;
                            }
                            else
                            {
                                lasth = h;
                            }
                                
                            //else
                            //    lasth = h;
                            //if (firstw == -1)
                            //    firstw = w;
                            //else
                            //    lastw = w;
                        //}
                        //if(firsth != -1)
                        //    break;
                       }

                          //  if (firstw == -1) firstw = w;
                           // else lastw = w;
                            //MessageBox.Show("w="+w);
                        //}
                    }
                    BitHeight[file.Path.ToLower()] = (lasth - firsth);
                    //WriteLog(string.Format("bmpRaw: h:{0}, w:{1} - crop: firstw:{2}, firsth:{3}, h:{4}, w:{5} - image {6}", bmapRaw.Height, bmapRaw.Width, firstw, firsth, (lasth - firsth) - 1, (firstw - lastw) - 1, file.Path.ToLower()));

                    //WriteLog*/

                    /*Bitmap bmpCrop = null;
                    try
                    {
                        bmpCrop = bmaps[0].Clone(new Rectangle(0, firsth, bmaps[0].Width, (lasth - firsth)), bmaps[0].PixelFormat);
                        
                    }
                    catch (Exception e)
                    {
                        //WriteLog(string.Format("{0} fail: {1}", file.Path.ToLower(), e.Message));
                    }*/
                   
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
            //Image.FromHbitmap

            // Draw normal tiles.
            foreach (var tile in map.Tiles.Where(x => !x.Roof))
            {
                DrawTile(g, tile.Path, tile.X, tile.Y);
            }

            /*for (int i = 0; i < 32; i++)
            {
                for(int y = 0; y < 32; y++)
                    DrawScenery(g, "art\\misc\\grid_scr.frm", i, y, 0, 0);
            }*/

            foreach (var obj in map.Objects.OrderBy(x => x.MapX + x.MapY))
            {
                if (!(obj.MapObjType == 1 || obj.MapObjType == 2))
                    continue;
                ItemProto prot = items.Where(x => x.ProtoId == obj.ProtoId).FirstOrDefault();
                if (prot == null)
                    continue;

               // WriteLog("Drawing " + prot.PicMap);

                DrawScenery(g, prot.PicMap, obj.MapX, obj.MapY, prot.OffsetX, prot.OffsetY);
            }

            /*foreach (var tile in map.Tiles.Where(x => x.Roof))
            {
                DrawTile(g, tile.Path, tile.X, tile.Y);
            }*/

            //DrawFiltered(g, "misc");
            //DrawFiltered(g, "walls");


            //g.DrawImage(Bitmaps["art\\misc\\grid_scr.frm"], offset_x + 150, offset_y + 100);
            //g.DrawImage(Bitmaps["art\\misc\\grid_scr.frm"], offset_x + (float)numericUpDown8.Value, offset_y + (float)numericUpDown9.Value);
            //DrawScenery(g, "art\\misc\\grid_scr.frm", obj.MapX, obj.MapY, prot.OffsetX, prot.OffsetY);
        }



        private void DrawScenery(Graphics g, string scenery, int x, int y, int offx2, int offy2)
        {
            if (!Bitmaps.ContainsKey(scenery))
            {
                //MessageBox.Show(scenery + " not found");
                return;
            }

            float xcoord=0.0f;
            float ycoord=0.0f;

            float ox = x * (float)32;
            float oy = y * (float)16;

            float cx = x;
            float cy = y;

            xcoord = (oy - ox) - offx2;
            ycoord = (Math.Abs((cy + (cx / 2))) * 12) - offy2;

            if (!(y == 0 && x == 0))
            {
                if (x % 2 == 1)
                {
                    ycoord -= 6;
                }
                if (x > 1)
                {
                    xcoord += (x/2)*16;
                }
            }
            /*if (scenery.Contains("walls"))
            {
                xcoord -= (Bitmaps[scenery].Width);
                ycoord -= Bitmaps[scenery].Height;//Math(Bitmaps[scenery].Height, (float)numericUpDown10.Value);
            }
            else
            {*/
            xcoord -= (Bitmaps[scenery].Width/2);
            xcoord += BitShiftX[scenery];
            ycoord -= (Bitmaps[scenery].Height);
            ycoord += BitShiftY[scenery];
            //ycoord -= BitHeight[scenery];
            // }
            /**/

            /*if (x % 2 == 1)
            {
               // ycoord = ycoord = (x-1 + y) * 12;
                xcoord -= 16;
                ycoord -= 16;
            }*/

            if (x % 2 == 0)
            {
                //xcoord += (float)numericUpDown10.Value;
                //ycoord += (float)13;
            }
            /*if(y % 2 == 1)
            {
                ycoord -= (float)24;
            }*/
            Font font = new Font(Font.FontFamily, 10.0f, FontStyle.Bold);

            //if (scenery.Contains("jas1000.frm"))
             //   ycoord += 13;

            //g.DrawString("" + BitHeight[scenery], font, Brushes.Red, offset_x - (x % 2 == 0 ? 20 : 0) + xcoord, offset_y + ycoord - 10);

            //xcoord = ((-x + y)*size + Bitmaps[scenery].Width) - ((float)offsetx * x) + offx2;
            //ycoord = (x / y)*size + ((float)offsety * y) + offy2;

            g.DrawImage(Bitmaps[scenery], offset_x + xcoord, offset_y + ycoord);
            
            //g.DrawString(x + "," + y /*+ "(" + Math.Abs(y+(x/2)) + ")"*/, SystemFonts.DefaultFont, Brushes.Green, offset_x + xcoord, offset_y + ycoord - 30);
            //g.DrawLine(Pens.Pink, offset_x + xcoord, offset_y + ycoord, offset_x + xcoord, offset_y + ycoord);
        }

        private void DrawTile(Graphics g, string tile, int x, int y)
        {
            float xcoord=0;
            float ycoord = 0;

            /*if (!(x == 0 && y == 0))
            {
                xcoord = ((-x + y) * size) - ((float)12.0f * x);
                ycoord = ((x + y) * size / 2) + ((float)3.0f * y);
            }*/


            float ox = x * (float)32;
            float oy = y * (float)16;

            float cx = x;
            float cy = y;

            xcoord = (oy - ox);
            ycoord = (Math.Abs((cy + (cx / 2))) * 12);

            if (!(y == 0 && x == 0))
            {
                if (x % 2 == 1)
                {
                    ycoord -= 6;
                }
                if (x > 1)
                {
                    xcoord += (x / 2) * 16;
                }
            }


           /*if (x % 2 == 0)
            {
                xcoord =x*(int)numericUpDown1.Value;
                ycoord =y* (int)numericUpDown2.Value;
            }
            else
            {
                xcoord =x*(int)numericUpDown3.Value;
                ycoord=y*(int)numericUpDown4.Value;
            }*/

            //else x= 12;

            //int xcoord = ((-x * size) + (y * (int)(size/2)));


            //xcoord = -x * ((Bitmaps[tile].Width) + y * (Bitmaps[tile].Width));
            //ycoord = x * (Bitmaps[tile].Height) + y * (Bitmaps[tile].Height);

            /*if (x % 2 == 0)
                xcoord = ((-x + y) * size);
            else
                xcoord = ((-x + y) * size)*/

            if (!Bitmaps.ContainsKey(tile))
            {
                //MessageBox.Show(tile + " not found");
                return;
            }

            if (tile.Contains("misc"))
                MessageBox.Show(tile);

            g.DrawImage(Bitmaps[tile], offset_x + offset_tile_x + xcoord, offset_y + offset_tile_y +  ycoord);
            
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            panel1.Refresh();
            panel1.Invalidate();
        }
    }
}
