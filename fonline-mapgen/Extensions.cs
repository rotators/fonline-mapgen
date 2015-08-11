using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Linq;
using System;

namespace fonline_mapgen
{
    internal static class ExtensionMethods
    {
        public static IList<T> Clone<T>(this IList<T> listToClone) where T : ICloneable
        {
            return listToClone.Select(item => (T)item.Clone()).ToList();
        }

        public static void MoveUp(this ListBox lst)
        {
            MoveItem(lst, -1);
        }

        public static void MoveDown(this ListBox lst)
        {
            MoveItem(lst, 1);
        }

        public static void MoveItem(this ListBox lst, int direction)
        {
            // Checking selected item
            if (lst.SelectedItem == null || lst.SelectedIndex < 0)
                return; // No selected item - nothing to do

            // Calculate new index using move direction
            int newIndex = lst.SelectedIndex + direction;

            // Checking bounds of the range
            if (newIndex < 0 || newIndex >= lst.Items.Count)
                return; // Index out of range - nothing to do

            object selected = lst.SelectedItem;

            // Removing removable element
            lst.Items.Remove(selected);
            // Insert it in new position
            lst.Items.Insert(newIndex, selected);
            // Restore selection
            lst.SetSelected(newIndex, true);
        }



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
