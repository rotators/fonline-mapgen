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
using System.Text.RegularExpressions;

namespace fonline_mapgen
{
    public class DrawCall
    {
        public Bitmap Bitmap;
        public string Path;
        public float X;
        public float Y;

        public DrawCall(Bitmap Bitmap, string Path, float X, float Y)
        {
            this.Bitmap = Bitmap;
            this.Path = Path;
            this.X = X;
            this.Y = Y;
        }
    }

    public class OverlayCall
    {
        public string Text;
        public float X;
        public float Y;
        
        public OverlayCall(string Text, float X, float Y)
        {
            this.Text = Text;
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
        public List<DrawCall> CacheList;
    }

    public static class DrawMap
    {
        [DllImport("User32.dll")]
        public static extern IntPtr GetDC(IntPtr wnd);

        [DllImport("User32.dll")]
        public static extern void ReleaseDC(IntPtr dc);

        public static bool cachedCalls;

        public static object ClickedObject;

        public static List<string> GetErrors() { return Errors; }

        private static List<string> Errors = new List<string>();

        //private static Dictionary<string, Bitmap> CachedBitmap = new Dictionary<string, Bitmap>();
        private static Dictionary<int, Bitmap> CachedOpaque = new Dictionary<int, Bitmap>();

        private static List<DrawCall> CachedSceneryDraws = new List<DrawCall>();
        private static List<DrawCall> CachedTileDraws = new List<DrawCall>();
        private static List<DrawCall> CachedRoofTileDraws = new List<DrawCall>();
        private static List<OverlayCall> CachedOverlayDraws = new List<OverlayCall>();
        private static List<List<DrawCall>> CachedDrawsLists = new List<List<DrawCall>>();

        private static Dictionary<int, ItemProto> itemsPids = new Dictionary<int, ItemProto>();

        private static List<MapObject> SelectedObjects = new List<MapObject>();
        private static List<Tile> SelectedTiles = new List<Tile>();

        private static CurrentClick currentClick = null;
        private static Font overlayFont = new Font(FontFamily.GenericSansSerif, 13.0f, FontStyle.Bold);

        public enum Flags
        {
            Tiles = 0x01,
            Roofs = 0x02,
            Critters = 0x04,
            Items = 0x08,
            Scenery = 0x10,
            SceneryWalls = 0x20
        };
        
        private static void Log(string txt)
        {
            System.IO.File.AppendAllText("./gl_log.txt", txt);
        }

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

        public static List<Tile> GetSelectedTiles()
        {
            return SelectedTiles;
        }

        private static bool AddToCache(Bitmap drawBitmap, string path, List<DrawCall> cacheList, 
            PointF coords, bool selectable, MouseSelection mouseSelection, RectangleF screenArea)
        {
            if (mouseSelection.isDown && !screenArea.Contains(coords))
                return false;

            var normalDraw = new DrawCall(drawBitmap, path, coords.X, coords.Y);
            if (selectable && InsideSelection(mouseSelection.selectionArea, new RectangleF(coords.X, coords.Y, drawBitmap.Width, drawBitmap.Height)))
            {
                var opaque = MakeOpaque(drawBitmap, path, 0.5f);
                var opDraw = new DrawCall(opaque, path, coords.X, coords.Y);
                if (mouseSelection.clicked)
                {
                    if (currentClick != null) ClickToCache(currentClick, true);
                    currentClick = new CurrentClick();
                    currentClick.index = cacheList.Count;
                    currentClick.CacheList = cacheList;
                    currentClick.NormalCall = normalDraw;
                    currentClick.OpaqueCall = opDraw;
                }
                else
                {
                    cacheList.Add(opDraw);
                    return true;
                }
            }
            else
                cacheList.Add(normalDraw);
            return false;
        }

        private static void ClickToCache(CurrentClick previous, bool normal)
        {
            if(normal)
                previous.CacheList.Insert(previous.index, previous.NormalCall);
            else
                previous.CacheList.Insert(previous.index, previous.OpaqueCall);
        }

        private static string PreprocessOverlay(string text, MapObject obj, Func<string, string> dataLookup)
        {
            text = text.Replace("%PID%", obj.ProtoId.ToString());

            Regex r = new Regex("%P_(.+?)%", RegexOptions.Multiline);
            var matches = r.Matches(text);
            foreach (Match match in matches)
            {
                string mStr = match.Groups[0].Value;
                mStr = mStr.Replace("%P_", "");
                mStr = mStr.Replace("%", "");
                string data = "ERROR";
                data = dataLookup(mStr);
                text = text.Replace(match.Value, data);
            }
            return text;
        }

        public static void OnGraphics( Graphics gdi, FOMap map, FOHexMap hexMap, Dictionary<int, ItemProto> itemsPid, CritterData critterData,
            Dictionary<string, FalloutFRM> frms, EditorData editorData, SizeF scale, RectangleF screenArea, MouseSelection mouseSelection)
        {

            Flags flags = editorData.drawFlags;
            Flags selectFlags = editorData.selectFlags;
            Flags overlayFlags = editorData.overlayFlags;

            if (!cachedCalls)
            {
                CachedSceneryDraws  = new List<DrawCall>();
                CachedTileDraws     = new List<DrawCall>();
                CachedRoofTileDraws = new List<DrawCall>();
                CachedDrawsLists    = new List<List<DrawCall>>();
                CachedOverlayDraws  = new List<OverlayCall>();
                CachedDrawsLists.Add(CachedTileDraws);
                CachedDrawsLists.Add(CachedSceneryDraws);
                CachedDrawsLists.Add(CachedRoofTileDraws);

                currentClick = null;
                SelectedObjects = new List<MapObject>();
                SelectedTiles = new List<Tile>();
                Errors = new List<string>();

                if (DrawFlag(flags, Flags.Tiles) || DrawFlag(flags, Flags.Roofs))
                {
                    foreach (var tile in map.Tiles)
                    {
                        bool selectable = false;

                        if (!tile.Roof && !DrawFlag(flags, Flags.Tiles))
                            continue;
                        if (!tile.Roof && DrawFlag(selectFlags, Flags.Tiles))
                            selectable = true;
                        if (tile.Roof && !DrawFlag(flags, Flags.Roofs))
                            continue;
                        if(tile.Roof && DrawFlag(selectFlags, Flags.Roofs))
                            selectable = true;


                        List<DrawCall> list = null;
                        if (tile.Roof) list = CachedRoofTileDraws;
                        else list = CachedTileDraws;

                        if (!frms.ContainsKey(tile.Path))
                        {
                            Errors.Add("Tile graphics " + tile.Path + " not loaded.");
                            return;
                        }
                        var tileCoords = hexMap.GetTileCoords(new Point(tile.X, tile.Y), tile.Roof);
                        Bitmap drawBitmap = frms[tile.Path].Frames[0];
                        if (AddToCache(drawBitmap, tile.Path, list, tileCoords, selectable, mouseSelection, screenArea))
                            SelectedTiles.Add(tile);
                    }
                }

                foreach (var obj in map.Objects.OrderBy(x => x.MapX + x.MapY * 2))
                {
                    bool selectable = false;
                    // skip specific object types
                    if (obj.MapObjType == MapObjectType.Critter && !DrawFlag(flags, Flags.Critters))
                        continue;
                    else if (obj.MapObjType == MapObjectType.Item && !DrawFlag(flags, Flags.Items))
                        continue;
                    else if (obj.MapObjType == MapObjectType.Scenery && !DrawFlag(flags, Flags.Scenery))
                        continue;

                    if (obj.MapObjType == MapObjectType.Critter && DrawFlag(selectFlags, Flags.Critters))
                        selectable = true;
                    else if (obj.MapObjType == MapObjectType.Item && DrawFlag(selectFlags, Flags.Items))
                        selectable = true;
                    else if (obj.MapObjType == MapObjectType.Scenery && DrawFlag(selectFlags, Flags.Scenery))
                        selectable = true;

                    Bitmap drawBitmap;
                    PointF coords;
                    string path;

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
                        path = cr;
                        if (!frms.TryGetValue(cr, out frm))
                        {
                            Errors.Add("Critter graphics " + cr + " not loaded.");
                            continue;
                        }
                        if (frm == null)
                            continue;

                        coords = hexMap.GetObjectCoords(new Point(obj.MapX, obj.MapY), frm.Frames[0].Size, new Point(frm.PixelShift.X, frm.PixelShift.Y), new Point(0, 0));
                        drawBitmap = frm.GetAnimFrameByDir(dir, 1);

                        if (DrawFlag(overlayFlags, Flags.Critters))
                        {
                            string text = editorData.overlayCritterFormat;
                            string preprocessed = PreprocessOverlay(text, obj, new Func<string, string>((string mStr) => {
                                string data = "";
                                if (obj.Properties.ContainsKey(mStr)) data = obj.Properties[mStr];
                                if (obj.CritterParams.ContainsKey(mStr)) data = obj.CritterParams[mStr].ToString();
                                return data;
                            }));
                            int lines = text.Count(f => f == '\n');
                            CachedOverlayDraws.Add(new OverlayCall(preprocessed, coords.X, coords.Y - (40 + (15 * (lines - 1)))));
                        }
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
                        path = prot.PicMap;
                        coords = hexMap.GetObjectCoords(new Point(obj.MapX, obj.MapY), frm.Frames[0].Size, 
                            new Point(frm.PixelShift.X, frm.PixelShift.Y), new Point(prot.OffsetX, prot.OffsetY));
                        drawBitmap = frm.Frames[0];
                    }
                    if (AddToCache(drawBitmap, path, CachedSceneryDraws, coords, selectable, mouseSelection, screenArea))
                        SelectedObjects.Add(obj);
                }
            }

            // Rendering
            if (currentClick != null)
                ClickToCache(currentClick, false);

            if (scale.Width != 1.0f && gdi != null)
                gdi.ScaleTransform(scale.Width, scale.Height);

            cachedCalls = true;

            if (gdi != null)
            {
                foreach (var call in CachedTileDraws)
                    gdi.DrawImage(call.Bitmap, call.X, call.Y);
                foreach (var call in CachedSceneryDraws)
                    gdi.DrawImage(call.Bitmap, call.X, call.Y);
                foreach (var call in CachedRoofTileDraws)
                    gdi.DrawImage(call.Bitmap, call.X, call.Y);

                foreach (var call in CachedOverlayDraws)
                {
                    DrawOutlinedText(gdi, call.Text, overlayFont, Brushes.GreenYellow, Brushes.Black, new PointF(call.X, call.Y));
                }
            }
        }

        private static Bitmap MakeOpaque(Bitmap original, string path, double opacity)
        {

            if (CachedOpaque.ContainsKey(original.GetHashCode())) return CachedOpaque[original.GetHashCode()];

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

            // Cache
            CachedOpaque[original.GetHashCode()] = bmp;
            return bmp;
        }

        private static void DrawOutlinedText(Graphics g, string Text, Font tfont, Brush Fill, Brush Outline, PointF p)
        {

            g.DrawString(Text, tfont, Outline, p.X - 1, p.Y - 1);
            g.DrawString(Text, tfont, Outline, p.X - 1, p.Y + 1);
            g.DrawString(Text, tfont, Outline, p.X + 1, p.Y - 1);
            g.DrawString(Text, tfont, Outline, p.X + 1, p.Y + 1);
            g.DrawString(Text, tfont, Fill, p.X, p.Y);
        }

        private static bool InsideSelection(RectangleF selectArea, RectangleF rect)
        {
            return selectArea.IntersectsWith(rect);
        }

        private static bool DrawFlag( Flags flags, Flags flag )
        {
            return ((flags & flag) != 0);
        }
    }
}
