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
    public partial class frmHeaderEditor : Form
    {
        public MapHeader Header;

        Color[] dayColor = new Color[4];
        SolidBrush[] dayColorBrush = new SolidBrush[4];

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

            e.Graphics.FillRectangle( dayColorBrush[idx], e.CellBounds );
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

        private void UpdateHeader()
        {
            if( this.checkScripted.Checked )
            {
                Header.ScriptModule = this.txtModule.Text;
                Header.ScriptFunc = this.txtFunction.Text;
            }
            else
                Header.ScriptModule = Header.ScriptFunc = "-";

            Header.NoLogOut = this.checkNoLogout.Checked;
        }

        private void checkScripted_CheckedChanged( object sender, EventArgs e )
        {
            this.txtModule.Enabled = this.txtFunction.Enabled = this.checkScripted.Checked;
        }

        private void btnCancel_Click( object sender, EventArgs e )
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}
