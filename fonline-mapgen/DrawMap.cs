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
using SharpGL;

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

    // Selection logic
    public class CurrentClick
    {
        public DrawCall NormalCall;
        public DrawCall OpaqueCall;
        public int index;
        public List<DrawCall> CacheList;
    }

    public class GLBinding
    {
        public uint[] frames;
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
        private static List<List<DrawCall>> CachedDrawsLists = new List<List<DrawCall>>();

        private static Dictionary<int, ItemProto> itemsPids = new Dictionary<int, ItemProto>();

        private static List<MapObject> SelectedObjects = new List<MapObject>();
        private static List<Tile> SelectedTiles = new List<Tile>();

        private static Dictionary<string, int> glIds = new Dictionary<string, int>();

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

        private static bool AddToCache(Bitmap drawBitmap, string path, List<DrawCall> cacheList, PointF coords, RectangleF selectionArea, bool selectable, bool clicked)
        {
            var normalDraw = new DrawCall(drawBitmap, path, coords.X, coords.Y);
            if (selectable && InsideSelection(selectionArea, new RectangleF(coords.X, coords.Y, drawBitmap.Width, drawBitmap.Height)))
            {
                var opaque = MakeOpaque(drawBitmap, 0.5f);
                var opDraw = new DrawCall(opaque, path, coords.X, coords.Y);
                if (clicked)
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

        private static uint BindGlTexture(OpenGL gl, string path, Bitmap bitmap)
        {
            uint glDrawId = 0;
            if (!glIds.Keys.Contains(path))
            {
                var bmp = bitmap;
                var rect = new Rectangle(0, 0, bmp.Width, bmp.Height);

                uint[] u = new uint[1];
                gl.GenTextures(1, u);
                gl.BindTexture(OpenGL.GL_TEXTURE_2D, u[0]);

                glDrawId = u[0];
                glIds[path] = (int)u[0];

                //  Tell OpenGL where the texture data is.
                BitmapData bmpData = bmp.LockBits(rect, ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);

                gl.TexEnv(OpenGL.GL_TEXTURE_ENV, OpenGL.GL_TEXTURE_ENV_MODE, OpenGL.GL_REPLACE);

                gl.TexImage2D(OpenGL.GL_TEXTURE_2D, 0, OpenGL.GL_RGBA, bmp.Width, bmp.Height, 0, OpenGL.GL_BGRA, OpenGL.GL_UNSIGNED_BYTE,
                    bmpData.Scan0);
                //  Specify linear filtering.
                gl.TexParameter(OpenGL.GL_TEXTURE_2D, OpenGL.GL_TEXTURE_MIN_FILTER, OpenGL.GL_NEAREST);
                gl.TexParameter(OpenGL.GL_TEXTURE_2D, OpenGL.GL_TEXTURE_MAG_FILTER, OpenGL.GL_NEAREST);

                bmp.UnlockBits(bmpData);
            }
            else
            {
                glDrawId = (uint)glIds[path];
            }
            return glDrawId;
        }

        public static void OnGraphics( Graphics gdi, SharpGL.OpenGL gl, FOMap map, FOHexMap hexMap, Dictionary<int, ItemProto> itemsPid, CritterData critterData,
            Dictionary<string, FalloutFRM> frms, Flags flags, Flags selectFlags, SizeF scale, RectangleF selectionArea, bool clicked)
        {
            if (!cachedCalls)
            {
                CachedSceneryDraws = new List<DrawCall>();
                CachedTileDraws = new List<DrawCall>();
                CachedRoofTileDraws = new List<DrawCall>();
                CachedDrawsLists = new List<List<DrawCall>>();
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
                        if (AddToCache(drawBitmap, tile.Path, list, tileCoords, selectionArea, selectable, clicked))
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

                    if (AddToCache(drawBitmap, path, CachedSceneryDraws, coords, selectionArea, selectable, clicked))
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
            }
            else
            {
                //uint[] ids = new uint[CachedTileDraws.Count];

                float MinX = CachedTileDraws.Min(x => x.X);
                float MinY = CachedTileDraws.Min(x => x.Y);
                float MaxX = CachedTileDraws.Max(x => x.X);
                float MaxY = CachedTileDraws.Max(x => x.Y);

                float deltX = MaxX - MinX;
                float deltY = MaxY - MinY;

                /*foreach (var list in CachedDrawsLists)
                {
                    foreach (var call in list)
                    {
                        float scaleX = Math.Abs((call.X - MinX) / (MaxX - MinX));
                        float scaleY = Math.Abs((call.Y - MinY) / (MaxY - MinY));

                        uint glDrawId = BindGlTexture(gl, call.Path, call.Bitmap);
                        DrawGlTexture(gl, glDrawId, scaleX, scaleY);
                    }
                }*/

                //float normX = CachedTileDraws[0].X;
                //float normY = CachedTileDraws[0].Y;

                float factorX = Math.Abs((1 - MinX) / (MaxX - MinX));
                float factorY = Math.Abs((1 - MinY) / (MaxY - MinY));

                foreach (var call in CachedTileDraws)
                {
                    float scaleX = Math.Abs((call.X - MinX) / (MaxX - MinX));
                    float scaleY = Math.Abs((call.Y - MinY) / (MaxY - MinY));


                    uint glDrawId = BindGlTexture(gl, call.Path, call.Bitmap);
                    DrawGlTexture(gl, glDrawId, scaleX, scaleY);
                }

                foreach (var call in CachedSceneryDraws)
                {
                    float scaleX = Math.Abs((call.X - MinX) / (MaxX - MinX));
                    float scaleY = Math.Abs((call.Y - MinY) / (MaxY - MinY));

                    float factor = 70.0f;
                    //float posX = Math.Abs((call.X - MinX) / (MaxX - MinX));
                    //float posY = Math.Abs((call.Y - MinY) / (MaxY - MinY));
                    gl.Enable(OpenGL.GL_BLEND);
                    gl.BlendFunc(OpenGL.GL_SRC_ALPHA, OpenGL.GL_ONE_MINUS_SRC_ALPHA);
                    uint glDrawId = BindGlTexture(gl, call.Path, call.Bitmap);
                    gl.BindTexture(OpenGL.GL_TEXTURE_2D, glDrawId);
                    gl.PushMatrix();
                    gl.Translate((scaleX * factor), (scaleY * factor), 0.0f);
                    //gl.Scale(1.0f / call.Bitmap.Width, 1.0f / call.Bitmap.Height, 0.0f);
                    //gl.Rotate(0.45f, 0.0f, 0.0f);
                    //float b = 1.0f / 32;

                    float width = ((1.0f / 64) * call.Bitmap.Width);
                    float height = ((1.0f / 32) * call.Bitmap.Height);

                    //float width = Math.Abs((call.Bitmap.Width - MinX) / (MaxX - MinX));
                    //float height = Math.Abs((call.Bitmap.Height - MinY) / (MaxY - MinY));


                    gl.Begin(OpenGL.GL_QUADS);
                    gl.TexCoord(0.0f, 0.0f);
                    gl.Vertex(0.0, height, 0.0f);
                    gl.TexCoord(0.0f, 1.0f);
                    gl.Vertex(0.0, 0.0, 0.0f);
                    gl.TexCoord(1.0f, 1.0f);
                    gl.Vertex(width, 0.0, 0.0f);
                    gl.TexCoord(1.0f, 0.0f);
                    gl.Vertex(width, height, 0.0f);

                    gl.End();
                    gl.PopMatrix();
                }

                foreach (var call in CachedRoofTileDraws)
                {
                    float scaleX = Math.Abs((call.X - MinX) / (MaxX - MinX));
                    float scaleY = Math.Abs((call.Y - MinY) / (MaxY - MinY));

                    uint glDrawId = BindGlTexture(gl, call.Path, call.Bitmap);
                    DrawGlTexture(gl, glDrawId, scaleX, scaleY);
                }
            }
        }

        private static void DrawGlTexture(OpenGL gl, uint glDrawId, float X, float Y)
        {
            float factor = 70.0f;
            gl.Enable(OpenGL.GL_BLEND);
            gl.BlendFunc(OpenGL.GL_SRC_ALPHA, OpenGL.GL_ONE_MINUS_SRC_ALPHA);
            gl.BindTexture(OpenGL.GL_TEXTURE_2D, glDrawId);
            gl.PushMatrix();

            gl.Translate((X * factor), (Y * factor), 0.0f);
            gl.Begin(OpenGL.GL_QUADS);
            gl.TexCoord(0.0f, 0.0f);
            gl.Vertex(0.0, 1.0f, 0.0f);
            gl.TexCoord(0.0f, 1.0f);
            gl.Vertex(0.0, 0.0, 0.0f);
            gl.TexCoord(1.0f, 1.0f);
            gl.Vertex(1.0f, 0.0, 0.0f);
            gl.TexCoord(1.0f, 0.0f);
            gl.Vertex(1.0f, 1.0f, 0.0f);
            gl.End();
            gl.PopMatrix();
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

        private static bool DrawFlag( Flags flags, Flags flag )
        {
            return ((flags & flag) != 0);
        }
    }
}
