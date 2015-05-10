using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace fonline_mapgen
{
    public partial class frmOverlay : Form
    {
        public string CritterFormat;
        public string SceneryFormat;

        public frmOverlay(MapperSettings mapperSettings)
        {
            InitializeComponent();
            txtCritter.Text = mapperSettings.UI.Overlay.CritterFormat;
            txtScenery.Text = mapperSettings.UI.Overlay.SceneryFormat;
            
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            CritterFormat = txtCritter.Text;
            SceneryFormat = txtScenery.Text;
            this.Close();
        }
    }
}
