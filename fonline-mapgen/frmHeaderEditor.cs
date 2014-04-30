using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Text.RegularExpressions;

using FOCommon;
using FOCommon.Maps;

namespace fonline_mapgen
{
    /// <summary>
    /// Edits map header.
    /// After user closes dialog, Header property can be used to update map
    /// </summary>
    public partial class frmHeaderEditor : Form
    {
        public MapHeader Header;

        private Color[] dayColor = new Color[4];
        private SolidBrush[] dayColorBrush = new SolidBrush[4];

        public frmHeaderEditor( MapHeader header )
        {
            if( header == null )
                throw new ArgumentNullException( "header" );

            InitializeComponent();

            this.txtVersion.Text = header.Version + "";
            this.txtSizeW.Text = header.MaxHexX + "";
            this.txtSizeH.Text = header.MaxHexY + "";

            if( header.ScriptModule != "-" && header.ScriptFunc != "-" )
            {
                this.txtModule.Text = header.ScriptModule;
                this.txtFunction.Text = header.ScriptFunc;
                this.checkScripted.Checked = true;
            }

            if( header.NoLogOut )
                this.checkNoLogout.Checked = true;

            this.checkScripted_CheckedChanged( null, null );

            string[] _DayColor = { header.DayColor0, header.DayColor1, header.DayColor2, header.DayColor3 };

            int idx = 0;
            foreach( string color in _DayColor )
            {
                string _color = color;
                while( _color.Contains( "  " ) )
                {
                    _color = _color.Replace( "  ", " " );
                }
                this.dayColor[idx++] = Utils.GetColor( _color.Trim(), ' ' );
            }
            this.UpdateDayColorCells();

            this.Header = header;
        }

        private void Form_Load( object sender, EventArgs e )
        {
            this.tableLayoutPanel1.CellPaint += new TableLayoutCellPaintEventHandler( dayColors_CellPaint );
        }

        private void dayColors_CellPaint( object sender, TableLayoutCellPaintEventArgs e )
        {
            int idx = e.Column;
            if( idx % 2 == 0 )
                idx /= 2;
            else
                return;

            e.Graphics.FillRectangle( this.dayColorBrush[idx], e.CellBounds );
        }

        private void dayColors_Click( object sender, EventArgs e )
        {
            Point? p = this.dayColors.GetRowColumnIndex( dayColors.PointToClient( Cursor.Position ) );
            if( p == null || p.Value.X % 2 != 0 )
                return;

            int idx = p.Value.X / 2;
            this.colorDialog.Color = this.dayColor[idx];

            if( this.colorDialog.ShowDialog( this ) == DialogResult.OK )
            {
                this.dayColor[idx] = this.colorDialog.Color;
                UpdateDayColorCells();
                this.dayColors.Refresh();
            }
        }

        private void UpdateDayColorCells()
        {
            for( int c = 0; c < this.dayColor.Length; c++ )
            {
                this.dayColorBrush[c] = new SolidBrush( this.dayColor[c] );
            }
        }

        private void checkScripted_CheckedChanged( object sender, EventArgs e )
        {
            this.txtModule.Enabled = this.txtFunction.Enabled = this.checkScripted.Checked;
        }

        private void btnSave_Click( object sender, EventArgs e )
        {
            if( this.checkScripted.Checked )
            {
                this.Header.ScriptModule = this.txtModule.Text;
                this.Header.ScriptFunc = this.txtFunction.Text;
            }
            else
                this.Header.ScriptModule = this.Header.ScriptFunc = "-";

            // no fancy formatting like in SDK mapper 
            this.Header.DayColor0 = this.dayColor[0].R + " " + this.dayColor[0].G + " " + this.dayColor[0].B;
            this.Header.DayColor1 = this.dayColor[1].R + " " + this.dayColor[1].G + " " + this.dayColor[1].B;
            this.Header.DayColor2 = this.dayColor[2].R + " " + this.dayColor[2].G + " " + this.dayColor[2].B;
            this.Header.DayColor3 = this.dayColor[3].R + " " + this.dayColor[3].G + " " + this.dayColor[3].B;

            this.Header.NoLogOut = this.checkNoLogout.Checked;

            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void btnCancel_Click( object sender, EventArgs e )
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}
