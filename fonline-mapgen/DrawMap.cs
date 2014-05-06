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
    public static class DrawMap
    {
        public enum Flags
        {
            Tiles = 0x01,
            Roofs = 0x02,
            Critters = 0x04,
            Items = 0x08,
            Scenery = 0x10,
            SceneryWalls = 0x20
        };

        public static void OnGraphics( Graphics g, FOMap map, FOHexMap hexMap, List<ItemProto> items, Dictionary<string, FalloutFRM> frms, Flags flags )
        {
            // should it really be here? maybe callee should set it instead?
            g.CompositingQuality = CompositingQuality.HighSpeed;
            g.InterpolationMode = InterpolationMode.NearestNeighbor;

            // Draw normal tiles.
            if( DrawFlag( flags, Flags.Tiles ) )
            {
                foreach( var tile in map.Tiles.Where( x => !x.Roof ) )
                {
                    DrawTile( g, hexMap, frms, tile.Path, tile.X, tile.Y, false );
                }
            }

            foreach( var obj in map.Objects.OrderBy( x => x.MapX + x.MapY * 2 ) )
            {
                // TODO: Draw critters.
                if( !(obj.MapObjType == MapObjectType.Item ||
                      obj.MapObjType == MapObjectType.Scenery) )
                    continue;

                // skip specific object types
                if( obj.MapObjType == MapObjectType.Critter && !DrawFlag( flags, Flags.Critters ) )
                    continue;
                else if( obj.MapObjType == MapObjectType.Item && !DrawFlag( flags, Flags.Items ) )
                    continue;
                else if( obj.MapObjType == MapObjectType.Scenery && !DrawFlag( flags, Flags.Scenery ) )
                    continue;

                ItemProto prot = items.Where( x => x.ProtoId == obj.ProtoId ).FirstOrDefault();
                if( prot == null )
                    continue;

                // WriteLog("Drawing " + prot.PicMap);

                if( prot.Type == (int)ItemTypes.ITEM_WALL && !DrawFlag( flags, Flags.SceneryWalls ) )
                    continue;

                DrawScenery( g, hexMap, frms, prot.PicMap, obj.MapX, obj.MapY, prot.OffsetX, prot.OffsetY );
            }

            // Draw roof tiles
            if( DrawFlag( flags, Flags.Roofs ) )
            {
                foreach( var tile in map.Tiles.Where( x => x.Roof ) )
                {
                    DrawTile( g, hexMap, frms, tile.Path, tile.X, tile.Y, true );
                }
            }
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

            g.DrawImage( frms[tile].Frames[0], tileCoords.X, tileCoords.Y );
        }

        private static bool DrawFlag( Flags flags, Flags flag )
        {
            return ((flags & flag) != 0);
        }
    }
}
