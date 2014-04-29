using System.Drawing;
using System.Windows.Forms;

namespace fonline_mapgen
{
    internal static class ExtensionMethods
    {
        // http://stackoverflow.com/a/15449969
        internal static Point? GetRowColumnIndex( this TableLayoutPanel tlp, Point point )
        {
            if( point.X > tlp.Width || point.Y > tlp.Height )
                return null;

            int w = tlp.Width;
            int h = tlp.Height;
            int[] widths = tlp.GetColumnWidths();

            int i;
            for( i = widths.Length - 1; i >= 0 && point.X < w; i-- )
                w -= widths[i];
            int col = i + 1;

            int[] heights = tlp.GetRowHeights();
            for( i = heights.Length - 1; i >= 0 && point.Y < h; i-- )
                h -= heights[i];

            int row = i + 1;

            return new Point( col, row );
        }
    }
}
