namespace fonline_mapgen
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Windows.Forms;

    public class DoubleBufferPanel : Panel
    {
        public DoubleBufferPanel()
        {
            // Set the value of the double-buffering style bits to true.
            this.SetStyle(ControlStyles.DoubleBuffer |
            ControlStyles.UserPaint |
            ControlStyles.AllPaintingInWmPaint | 
            ControlStyles.OptimizedDoubleBuffer,
            true);

            this.UpdateStyles();
        }
    }
}
