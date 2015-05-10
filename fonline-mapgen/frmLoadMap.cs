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
    public partial class frmLoadMap : Form
    {
        public delegate void LoadMap(string map);
        private LoadMap CallLoadMap;
        private List<String> Maps;

        private void btnLoadMap_Click(object sender, EventArgs e)
        {
            CallLoadMap((string)lstMaps.SelectedItem);
        }

        public frmLoadMap(List<string> maps, LoadMap callback)
        {
            InitializeComponent();
            this.CallLoadMap = callback;
            lstMaps.Items.AddRange(maps.ToArray());
            this.Maps = maps;
        }

        private void txtFilename_TextChanged(object sender, EventArgs e)
        {
            lstMaps.Items.Clear();

            if(txtFilename.Text == "")
            {
                lstMaps.Items.AddRange(Maps.ToArray());
                return;
            }

            foreach (var map in Maps)
            {
                if (map.Contains(txtFilename.Text))
                    lstMaps.Items.Add(map);
            }
        }

        private void lstMaps_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            CallLoadMap((string)lstMaps.SelectedItem);
        }
    }
}
