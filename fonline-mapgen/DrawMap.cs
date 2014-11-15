//
//That probably could be a part of FOCommon... "soon"
//

using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;

using FOCommon.Graphic;
using FOCommon.Items;
using FOCommon.Maps;
using FOCommon.Parsers;
using System.Runtime.InteropServices;
using System;
using System.Drawing.Imaging;



namespace fonline_mapgen
{
    public class DrawCall
    {
        public Bitmap Bitmap;
        public float X;
        public float Y;

        public DrawCall(Bitmap Bitmap, float X, float Y)
        {
            this.Bitmap = Bitmap;
            this.X = X;
            this.Y = Y;
        }
    }

    public static class DrawMap
    {
        [DllImport("Gdi32.dll")]
        public static extern int GetPixel(
        System.IntPtr hdc,    // handle to DC
        int nXPos,  // x-coordinate of pixel
        int nYPos   // y-coordinate of pixel
        );

        [DllImport("User32.dll")]
        public static extern IntPtr GetDC(IntPtr wnd);

        [DllImport("User32.dll")]
        public static extern void ReleaseDC(IntPtr dc);

        public static bool cachedCalls;

        public static object ClickedObject;

        public static List<string> GetErrors() { return Errors; }

        private static List<string> MissingNotified = new List<string>();
        private static List<string> Errors = new List<string>();

        private static List<List<DrawCall>> DeferedClickCalls = new List<List<DrawCall>>();
        private static List<DrawCall> CachedSceneryDraws = new List<DrawCall>();
        private static List<DrawCall> CachedTileDraws = new List<DrawCall>();
        private static List<DrawCall> CachedRoofTileDraws = new List<DrawCall>();
        private static Dictionary<int, ItemProto> itemsPids = new Dictionary<int, ItemProto>();

        private static bool clickFound;

        public enum Flags
        {
            Tiles = 0x01,
            Roofs = 0x02,
            Critters = 0x04,
            Items = 0x08,
            Scenery = 0x10,
            SceneryWalls = 0x20
        };

        public static void InvalidateCache()
        {
            cachedCalls = false;
        }

        public static int GetNumCachedObjects()
        {
            return CachedSceneryDraws.Count + CachedTileDraws.Count + CachedRoofTileDraws.Count;
        }

        public static void OnGraphics( Graphics g, FOMap map, FOHexMap hexMap, Dictionary<int, ItemProto> itemsPid, CritterData critterData,
            Dictionary<string, FalloutFRM> frms, Flags flags, SizeF scale, Point clickPos)
        {
            if (scale.Width != 1.0f)
                g.ScaleTransform(scale.Width, scale.Height);

            if (!cachedCalls)
            {
                CachedSceneryDraws = new List<DrawCall>();
                CachedTileDraws = new List<DrawCall>();
                CachedRoofTileDraws = new List<DrawCall>();
                Errors = new List<string>();
            }

            clickFound = false;

            // Draw normal tiles.
            if (DrawFlag(flags, Flags.Tiles) && !cachedCalls)
            {
                foreach (var tile in map.Tiles.Where(x => !x.Roof))
                {
                    DrawTile(g, hexMap, frms, tile.Path, tile.X, tile.Y, false);
                }
            }

            if (!cachedCalls)
            {
                foreach (var obj in map.Objects.OrderBy(x => x.MapX + x.MapY * 2))
                {
                    // skip specific object types
                    if (obj.MapObjType == MapObjectType.Critter && !DrawFlag(flags, Flags.Critters))
                        continue;
                    else if (obj.MapObjType == MapObjectType.Item && !DrawFlag(flags, Flags.Items))
                        continue;
                    else if (obj.MapObjType == MapObjectType.Scenery && !DrawFlag(flags, Flags.Scenery))
                        continue;

                    if (obj.MapObjType == MapObjectType.Critter)
                    {

                        int crType = 0;
                        critterData.crProtos.TryGetValue(obj.ProtoId, out crType);

                        string dirS;
                        int dir = 0;

                        obj.Properties.TryGetValue("Dir", out dirS);

                        int.TryParse(dirS, out dir);

                        string crTypeS = "";
                        critterData.crTypeGraphic.TryGetValue(crType, out crTypeS);

                        if (DrawCritter(g, hexMap, frms, crTypeS, obj.MapX, obj.MapY, dir, clickPos))
                            ClickedObject = obj;
                    }
                    else
                    {
                        ItemProto prot;
                        if (!itemsPid.TryGetValue(obj.ProtoId, out prot))
                            continue;

                        if (prot.Type == (int)ItemTypes.ITEM_WALL && !DrawFlag(flags, Flags.SceneryWalls))
                            continue;
                        if (DrawScenery(g, hexMap, frms, prot.PicMap, obj.MapX, obj.MapY, prot.OffsetX, prot.OffsetY, clickPos))
                            ClickedObject = obj;
                    }
                }
            }

            // Draw roof tiles
            if( DrawFlag( flags, Flags.Roofs ) && !cachedCalls)
            {
                foreach (var tile in map.Tiles.Where(x => x.Roof))
                {
                    DrawTile(g, hexMap, frms, tile.Path, tile.X, tile.Y, true);
                }
            }

            cachedCalls = true;
            foreach (var call in CachedTileDraws)
                g.DrawImage(call.Bitmap, call.X, call.Y);

            foreach (var call in CachedSceneryDraws)
                g.DrawImage(call.Bitmap, call.X, call.Y);

            foreach (var call in CachedRoofTileDraws)
                g.DrawImage(call.Bitmap, call.X, call.Y);
        }

        private static Bitmap MakeOpaque(Bitmap original, double opacity)
        {
            Bitmap bmp = (Bitmap)original.Clone();

            // Specify a pixel format.
            PixelFormat pxf = PixelFormat.Format32bppArgb;

            // Lock the bitmap's bits.
            Rectangle rect = new Rectangle(0, 0, bmp.Width, bmp.Height);
            BitmapData bmpData = bmp.LockBits(rect, ImageLockMode.ReadWrite, pxf);

            // Get the address of the first line.
            IntPtr ptr = bmpData.Scan0;

            // Declare an array to hold the bytes of the bitmap.
            // This code is specific to a bitmap with 32 bits per pixels 
            // (32 bits = 4 bytes, 3 for RGB and 1 byte for alpha).
            int numBytes = bmp.Width * bmp.Height * 4;
            byte[] argbValues = new byte[numBytes];

            // Copy the ARGB values into the array.
            System.Runtime.InteropServices.Marshal.Copy(ptr, argbValues, 0, numBytes);

            // Manipulate the bitmap, such as changing the
            // RGB values for all pixels in the the bitmap.
            for (int counter = 0; counter < argbValues.Length; counter += 4)
            {
                // argbValues is in format BGRA (Blue, Green, Red, Alpha)

                // If 100% transparent, skip pixel
                if (argbValues[counter + 4 - 1] == 0)
                    continue;

                int pos = 0;
                pos++; // B value
                pos++; // G value
                pos++; // R value

                argbValues[counter + pos] = (byte)(argbValues[counter + pos] * opacity);
            }

            // Copy the ARGB values back to the bitmap
            System.Runtime.InteropServices.Marshal.Copy(argbValues, 0, ptr, numBytes);

            // Unlock the bits.
            bmp.UnlockBits(bmpData);
            return bmp;
        }

        private static bool ClickedInside(Point clickPos, RectangleF rect)
        {
            return ((clickPos.X > rect.X && clickPos.X < rect.X + rect.Width) &&
                (clickPos.Y > rect.Y && clickPos.Y < rect.Y + rect.Height));
        }

        // returns true if clicked.
        private static bool DrawCritter( Graphics g, FOHexMap hexMap, Dictionary<string, FalloutFRM> frms, string critter, int x, int y, int dir, Point clickPos)
        {
            FalloutFRM frm;
            string cr = "art\\critters\\" + critter + "aa.frm";
            if (!frms.TryGetValue(cr, out frm))
            {
                Errors.Add("Critter graphics " + cr + " not loaded.");
                return false;
            }

            if (frm == null)
            {
                return false;
            }

            var coords = hexMap.GetObjectCoords(new Point(x, y), frm.Frames[0].Size, new Point(frm.PixelShift.X, frm.PixelShift.Y), new Point(0, 0));
            var idleFrame = frm.GetAnimFrameByDir(dir, 1);


            if (ClickedInside(clickPos, new RectangleF(coords.X, coords.Y, idleFrame.Width, idleFrame.Height)) && !clickFound)
            {
                clickFound = true;
                var opaque = MakeOpaque(idleFrame, 0.5f);


                return true;
            }
            else
            {
                CachedSceneryDraws.Add(new DrawCall(idleFrame, coords.X, coords.Y));
            }
            return false;
        }

        private static bool DrawScenery( Graphics g, FOHexMap hexMap, Dictionary<string, FalloutFRM> frms, string scenery, int x, int y, int offx2, int offy2, Point clickPos )
        {
            if( !frms.ContainsKey( scenery ) )
            {
                Errors.Add("Scenery graphics " + scenery + " not loaded.");
                return false;
            }

            var frm = frms[scenery];
            var coords = hexMap.GetObjectCoords( new Point( x, y ), frm.Frames[0].Size, new Point( frm.PixelShift.X, frm.PixelShift.Y ), new Point( offx2, offy2 ) );

            if (ClickedInside(clickPos, new RectangleF(coords.X, coords.Y, frm.Frames[0].Width, frm.Frames[0].Height)) && !clickFound)
            {
                clickFound = true;
                var opaque = MakeOpaque(frm.Frames[0], 0.5f);
                CachedSceneryDraws.Add(new DrawCall(opaque, coords.X, coords.Y));
                return true;
            }

            CachedSceneryDraws.Add(new DrawCall(frm.Frames[0], coords.X, coords.Y));
            return false;
        }

        private static void DrawTile( Graphics g, FOHexMap hexMap, Dictionary<string, FalloutFRM> frms, string tile, int x, int y, bool isRoof )
        {
            if( !frms.ContainsKey( tile ) )
            {
                if (!MissingNotified.Contains(tile))
                {
                    Errors.Add("Tile graphics " + tile + " not loaded.");
                }
                return;
            }

            var tileCoords = hexMap.GetTileCoords( new Point( x, y ), isRoof );

            if (isRoof) CachedRoofTileDraws.Add(new DrawCall(frms[tile].Frames[0], tileCoords.X, tileCoords.Y));
            else CachedTileDraws.Add(new DrawCall(frms[tile].Frames[0], tileCoords.X, tileCoords.Y));
        }

        private static bool DrawFlag( Flags flags, Flags flag )
        {
            return ((flags & flag) != 0);
        }
    }
}
