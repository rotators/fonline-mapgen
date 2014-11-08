using System;
using System.Collections.Generic;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;

using FOCommon.Parsers;
using FOCommon.Maps;
using FOCommon.Items;
using FOCommon.Graphic;

namespace fonline_mapgen
{
    public partial class MapperMap : FOMap
    {
        public string Name;
        public FOHexMap HexMap { get; private set; }

        internal MapperMap( FOMap map )
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

        private void InitHexMap()
        {
            this.HexMap = new FOHexMap(new PointF(this.Header.MaxHexX * 16, 100.0f), new Size( this.Header.MaxHexX, this.Header.MaxHexY ) );
        }

        public void Draw(Graphics g, Dictionary<int, ItemProto> itemsPid, Dictionary<string, FalloutFRM> frms, DrawMap.Flags flags)
        {
            DrawMap.OnGraphics( g, this, this.HexMap, itemsPid, frms, flags, new SizeF(1.0f, 1.0f));
        }

        public static MapperMap Load( string fileName )
        {
            FOMapParser parser = new FOMapParser( fileName );
            if( parser.Parse() )
                return (new MapperMap( parser.Map ));

            return (null);
        }
    }
}
