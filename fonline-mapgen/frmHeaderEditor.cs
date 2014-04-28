using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using FOCommon.Maps;

namespace fonline_mapgen
{
    public partial class frmHeaderEditor : Form
    {
        public MapHeader Header;

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

            this.Header = header;
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

        private void btnDayColor1_Click( object sender, EventArgs e )
        {
            this.colorDialog1.ShowDialog();
        }

        private void btnCancel_Click( object sender, EventArgs e )
        {
            this.Close();
        }

    }
}
