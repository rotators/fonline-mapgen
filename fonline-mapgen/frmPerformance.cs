using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace fonline_mapgen
{
    public partial class frmPerformance : Form
    {
        MapperSettings settings;

        public frmPerformance(MapperSettings settings)
        {
            InitializeComponent();
            this.settings = settings;
            chkCache.Checked = settings.Performance.CacheResources;
            cmbRendering.SelectedIndex = (settings.Performance.FastRendering ? 0 : 1);
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnClearCache_Click(object sender, EventArgs e)
        {
            File.Delete(".//graphics.dat");
            File.Delete(".//critters.dat");
            File.Delete(".//items.dat");
            MessageBox.Show("Cache deleted, restart to reload resources.");
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            this.settings.Performance.FastRendering = (cmbRendering.SelectedIndex == 0);
            this.settings.Performance.CacheResources = chkCache.Checked;
            this.Close();
        }
    }
}
