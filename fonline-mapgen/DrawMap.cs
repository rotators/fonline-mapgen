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
        public static bool cachedScenery;
        public static bool cachedTiles;
        public static bool cachedRoofTiles;

        private static List<DrawCall> CachedSceneryDraws = new List<DrawCall>();
        private static List<DrawCall> CachedTileDraws = new List<DrawCall>();
        private static List<DrawCall> CachedRoofTileDraws = new List<DrawCall>();
        private static Dictionary<int, ItemProto> itemsPids = new Dictionary<int, ItemProto>();

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
            cachedScenery = false;
            cachedTiles = false;
            cachedRoofTiles = false;
        }

        public static void OnGraphics( Graphics g, FOMap map, FOHexMap hexMap, Dictionary<int, ItemProto> itemsPid, 
            Dictionary<string, FalloutFRM> frms, Flags flags, SizeF scale)
        {
            // should it really be here? maybe callee should set it instead?
            g.CompositingQuality = CompositingQuality.HighSpeed;
            //g.InterpolationMode = InterpolationMode.HighQualityBilinear;

            g.ScaleTransform(scale.Width, scale.Height);

            if (!cachedScenery) CachedSceneryDraws = new List<DrawCall>();
            if (!cachedTiles) CachedTileDraws = new List<DrawCall>();
            if (!cachedRoofTiles) CachedRoofTileDraws = new List<DrawCall>();

            // Draw normal tiles.
            if( DrawFlag( flags, Flags.Tiles ) )
            {
                if (!cachedTiles)
                {
                    foreach (var tile in map.Tiles.Where(x => !x.Roof))
                    {
                        DrawTile(g, hexMap, frms, tile.Path, tile.X, tile.Y, false);
                    }
                }
            }

            cachedTiles = true;
            foreach (var call in CachedTileDraws)
                g.DrawImage(call.Bitmap, call.X, call.Y);

            if (!cachedScenery)
            {
                foreach (var obj in map.Objects.OrderBy(x => x.MapX + x.MapY * 2))
                {
                    // TODO: Draw critters.
                    if (!(obj.MapObjType == MapObjectType.Item ||
                          obj.MapObjType == MapObjectType.Scenery))
                        continue;

                    // skip specific object types
                    if (obj.MapObjType == MapObjectType.Critter && !DrawFlag(flags, Flags.Critters))
                        continue;
                    else if (obj.MapObjType == MapObjectType.Item && !DrawFlag(flags, Flags.Items))
                        continue;
                    else if (obj.MapObjType == MapObjectType.Scenery && !DrawFlag(flags, Flags.Scenery))
                        continue;

                    ItemProto prot;
                    if (!itemsPid.TryGetValue(obj.ProtoId, out prot))
                        continue;

                    // WriteLog("Drawing " + prot.PicMap);

                    if (prot.Type == (int)ItemTypes.ITEM_WALL && !DrawFlag(flags, Flags.SceneryWalls))
                        continue;

                    DrawScenery(g, hexMap, frms, prot.PicMap, obj.MapX, obj.MapY, prot.OffsetX, prot.OffsetY);
                }
            }

            cachedScenery = true;
            foreach(var call in CachedSceneryDraws)
                g.DrawImage(call.Bitmap, call.X, call.Y);

            // Draw roof tiles
            if( DrawFlag( flags, Flags.Roofs ) )
            {
                if (!cachedRoofTiles)
                {
                    foreach (var tile in map.Tiles.Where(x => x.Roof))
                    {
                        DrawTile(g, hexMap, frms, tile.Path, tile.X, tile.Y, true);
                    }
                }
                cachedRoofTiles = true;
            }

            foreach (var call in CachedRoofTileDraws)
                g.DrawImage(call.Bitmap, call.X, call.Y);
            
            //g.CompositingMode = System.Drawing.Drawing2D.CompositingMode.SourceOver;
            //MessageBox.Show("paint_event");
        }

        private static void DrawScenery( Graphics g, FOHexMap hexMap, Dictionary<string, FalloutFRM> frms, string scenery, int x, int y, int offx2, int offy2 )
        {
            if( !frms.ContainsKey( scenery ) )
            {
                //MessageBox.Show(scenery + " not found");
                return;
            }

            //Font font = new Font( Font.FontFamily, 10.0f, FontStyle.Bold );

            var frm = frms[scenery];

            //g.DrawString("" + BitHeight[scenery], font, Brushes.Red, offset_x - (x % 2 == 0 ? 20 : 0) + xcoord, offset_y + ycoord - 10);

            var coords = hexMap.GetObjectCoords( new Point( x, y ), frm.Frames[0].Size, new Point( frm.PixelShift.X, frm.PixelShift.Y ), new Point( offx2, offy2 ) );

            g.DrawImage( frm.Frames[0], coords.X, coords.Y );
            CachedSceneryDraws.Add(new DrawCall(frm.Frames[0], coords.X, coords.Y));
        }

        private static void DrawTile( Graphics g, FOHexMap hexMap, Dictionary<string, FalloutFRM> frms, string tile, int x, int y, bool isRoof )
        {
            if( !frms.ContainsKey( tile ) )
            {
                //MessageBox.Show(tile + " not found");
                return;
            }

            //if( tile.Contains( "misc" ) )
            //    MessageBox.Show( tile );

            var tileCoords = hexMap.GetTileCoords( new Point( x, y ), isRoof );

            if (isRoof) CachedRoofTileDraws.Add(new DrawCall(frms[tile].Frames[0], tileCoords.X, tileCoords.Y));
            else CachedTileDraws.Add(new DrawCall(frms[tile].Frames[0], tileCoords.X, tileCoords.Y));

            g.DrawImage( frms[tile].Frames[0], tileCoords.X, tileCoords.Y );
        }

        private static bool DrawFlag( Flags flags, Flags flag )
        {
            return ((flags & flag) != 0);
        }
    }
}
