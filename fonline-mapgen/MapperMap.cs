using System;
using System.Collections.Generic;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;
using System.Linq;

using FOCommon.Parsers;
using FOCommon.Maps;
using FOCommon.Items;
using FOCommon.Graphic;
using System.ComponentModel;

namespace fonline_mapgen
{
    public partial class MapperMap : FOMap
    {
        public string Name;
        public FOHexMap HexMap { get; private set; }

        /// <summary>
        /// Map parser to load and save map
        /// </summary>
        public static FOMapParser parser;

        /// <summary>
        /// Instance of this class
        /// </summary>
        public static MapperMap instance;

        /// <summary>
        /// Selection buffer
        /// </summary>
        public List<Selection> Selection = new List<Selection>();

        /// <summary>
        /// Current map selection
        /// </summary>
        public Selection MapSelection = new Selection();

        internal MapperMap(FOMap map)
        {
            this.Header = map.Header;
            this.Tiles = map.Tiles;
            this.Objects = map.Objects;

            this.InitHexMap();
        }

        internal MapperMap()
            : base()
        {
            this.Header.MaxHexX = this.Header.MaxHexY = 50;
            this.Header.WorkHexX = this.Header.WorkHexY = (ushort)(this.Header.WorkHexY / 2);

            this.InitHexMap();
        }

        public void MapSelect(List<Tile> tiles, List<MapObject> objects)
        {
            MapSelection.Objects.Clear();
            MapSelection.Tiles.Clear();
            MapSelection.Objects.AddRange(objects);
            MapSelection.Tiles.AddRange(tiles);
        }

        /// <summary>
        /// Remove selection from map
        /// </summary>
        /// <param name="selection"></param>
        public void RemoveSelection(Selection selection)
        {
            foreach (var obj in selection.Objects)
	        {
                this.Objects.Remove(obj);
	        }
            foreach (var obj in selection.Tiles)
            {
                this.Tiles.Remove(obj);
            }

            Selection.Remove(selection);
        }

        public PointF GetEdgeCoords(FOHexMap.Direction dir)
        {
            List<Point> hexes = new List<Point>();
            foreach (var tile in this.Tiles)
            {
                hexes.Add(new Point(tile.X, tile.Y));
            }
            return this.HexMap.GetEdgeCoords(hexes, dir);
        }

        private void InitHexMap()
        {
            this.HexMap = new FOHexMap(new Size(this.Header.MaxHexX, this.Header.MaxHexY));

            PointF baseOffset = new PointF(0, 0);
            Tile rightTile = this.Tiles[0];

            List<Point> hexes = new List<Point>();
            foreach (var tile in this.Tiles)
            {
                hexes.Add(new Point(tile.X, tile.Y));
            }

            PointF edgeLeft = this.HexMap.GetEdgeCoords(hexes, FOHexMap.Direction.Left);
            PointF edgeUp = this.HexMap.GetEdgeCoords(hexes, FOHexMap.Direction.Up);

            baseOffset = new PointF(-edgeLeft.X + 100.0f, edgeUp.Y);
            this.HexMap = new FOHexMap(baseOffset, new Size(this.Header.MaxHexX, this.Header.MaxHexY));
        }

        public static MapperMap Load( string fileName)
        {
            parser = new FOMapParser( fileName );
            if (parser.Parse())
            {
                instance = new MapperMap(parser.Map);
                return instance;
            }

            return (null);
        }

        /// <summary>
        /// Save map
        /// </summary>
        public void Save()
        {
            parser.Save();
        }
    }
}
