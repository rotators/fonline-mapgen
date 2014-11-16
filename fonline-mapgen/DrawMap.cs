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

    // Selection logic
    public class CurrentClick
    {
        public DrawCall NormalCall;
        public DrawCall OpaqueCall;
        public int index;
        public DrawMap.Flags Type;
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

        private static List<string> Errors = new List<string>();

        private static List<DrawCall> CachedSceneryDraws = new List<DrawCall>();
        private static List<DrawCall> CachedTileDraws = new List<DrawCall>();
        private static List<DrawCall> CachedRoofTileDraws = new List<DrawCall>();
        private static Dictionary<int, ItemProto> itemsPids = new Dictionary<int, ItemProto>();

        private static List<MapObject> SelectedObjects = new List<MapObject>();
        private static List<Tile> SelectedTiles = new List<Tile>();

        private static CurrentClick currentClick = null;

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


        public static List<MapObject> GetSelectedObjects()
        {
            return SelectedObjects;
        }

        private static void AddToCache(CurrentClick previous, bool normal)
        {
            List<DrawCall> list = null;
            if(previous.Type == Flags.Tiles)
                list = CachedTileDraws;
            if(previous.Type == Flags.Scenery) // Can be critter also, it's the same cache.
                list = CachedSceneryDraws;
            if(previous.Type == Flags.Roofs)
                list = CachedRoofTileDraws;

            if(normal)
                list.Insert(previous.index, previous.NormalCall);
            else
                list.Insert(previous.index, previous.OpaqueCall);
        }

        public static void OnGraphics( Graphics g, FOMap map, FOHexMap hexMap, Dictionary<int, ItemProto> itemsPid, CritterData critterData,
            Dictionary<string, FalloutFRM> frms, Flags flags, SizeF scale, RectangleF selectionArea, bool clicked)
        {
            if (scale.Width != 1.0f)
                g.ScaleTransform(scale.Width, scale.Height);

            if (!cachedCalls)
            {
                CachedSceneryDraws = new List<DrawCall>();
                CachedTileDraws = new List<DrawCall>();
                CachedRoofTileDraws = new List<DrawCall>();
                currentClick = null;
                Errors = new List<string>();
            }

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

                    Bitmap drawBitmap;
                    PointF coords;

                    if (obj.MapObjType == MapObjectType.Critter)
                    {
                        string dirS;
                        int dir = 0;
                        obj.Properties.TryGetValue("Dir", out dirS);
                        int.TryParse(dirS, out dir);

                        string crType = "";
                        critterData.GetCritterType(obj.ProtoId, out crType);

                        FalloutFRM frm;
                        string cr = "art\\critters\\" + crType + "aa.frm";
                        if (!frms.TryGetValue(cr, out frm))
                        {
                            Errors.Add("Critter graphics " + cr + " not loaded.");
                            continue;
                        }
                        if (frm == null)
                            continue;

                        coords = hexMap.GetObjectCoords(new Point(obj.MapX, obj.MapY), frm.Frames[0].Size, new Point(frm.PixelShift.X, frm.PixelShift.Y), new Point(0, 0));
                        drawBitmap = frm.GetAnimFrameByDir(dir, 1);
                    }
                    // Scenery or Item
                    else
                    {
                        ItemProto prot;
                        if (!itemsPid.TryGetValue(obj.ProtoId, out prot))
                            continue;
                        if (prot.Type == (int)ItemTypes.ITEM_WALL && !DrawFlag(flags, Flags.SceneryWalls))
                            continue;
                        if (!frms.ContainsKey(prot.PicMap))
                        {
                            Errors.Add("Scenery graphics " + prot.PicMap + " not loaded.");
                            continue;
                        }
                        var frm = frms[prot.PicMap];
                        coords = hexMap.GetObjectCoords(new Point(obj.MapX, obj.MapY), frm.Frames[0].Size, 
                            new Point(frm.PixelShift.X, frm.PixelShift.Y), new Point(prot.OffsetX, prot.OffsetY));
                        drawBitmap = frm.Frames[0];
                    }

                    var normalDraw = new DrawCall(drawBitmap, coords.X, coords.Y);
                    if (InsideSelection(selectionArea, new RectangleF(coords.X, coords.Y, drawBitmap.Width, drawBitmap.Height)))
                    {
                        var opaque = MakeOpaque(drawBitmap, 0.5f);
                        var opDraw = new DrawCall(opaque, coords.X, coords.Y);

                        if (clicked)
                        {
                            if (currentClick != null) AddToCache(currentClick, true);
                            currentClick = new CurrentClick();
                            currentClick.index = CachedSceneryDraws.Count;
                            currentClick.Type = Flags.Scenery;
                            currentClick.NormalCall = normalDraw;
                            currentClick.OpaqueCall = opDraw;
                        }
                        else
                        {
                            CachedSceneryDraws.Add(opDraw);
                            SelectedObjects.Add(obj);
                        }
                    }
                    else
                        CachedSceneryDraws.Add(normalDraw);
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

            if (currentClick != null)
                AddToCache(currentClick, false);

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

        private static bool InsideSelection(RectangleF selectArea, RectangleF rect)
        {
            return selectArea.IntersectsWith(rect);
        }

        private static void DrawTile( Graphics g, FOHexMap hexMap, Dictionary<string, FalloutFRM> frms, string tile, int x, int y, bool isRoof )
        {
            if (!frms.ContainsKey(tile))
            {
                Errors.Add("Tile graphics " + tile + " not loaded.");
                return;
            }

            var tileCoords = hexMap.GetTileCoords(new Point(x, y), isRoof);

            if (isRoof) CachedRoofTileDraws.Add(new DrawCall(frms[tile].Frames[0], tileCoords.X, tileCoords.Y));
            else CachedTileDraws.Add(new DrawCall(frms[tile].Frames[0], tileCoords.X, tileCoords.Y));
        }

        private static bool DrawFlag( Flags flags, Flags flag )
        {
            return ((flags & flag) != 0);
        }
    }
}
