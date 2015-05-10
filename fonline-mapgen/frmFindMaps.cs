using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using FOCommon.Parsers;
using System.IO;

namespace fonline_mapgen
{
    public partial class frmFindMaps : Form
    {
        private List<string> maps;
        private List<string> graphics;
        List<string> results = new List<string>();
        string searchGraphic;

        public frmFindMaps(List<string> maps, List<string> graphics)
        {
            InitializeComponent();
            this.maps = maps;
            this.graphics = graphics;
            cmbGraphics.Items.AddRange(graphics.ToArray());
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            lstResults.Items.Clear();
            searchGraphic = cmbGraphics.Text;
            backgroundWorker1.RunWorkerAsync();
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            results.Clear();
            foreach (var map in this.maps)
            {
                backgroundWorker1.ReportProgress(0, map);
                /*FOMapParser parser = new FOMapParser(map);
                if (!parser.Parse())
                    continue;

                foreach (var tile in parser.Map.Tiles)
                    if (tile.Path.ToLower() == searchGraphic)
                        results.Add(map);*/

                // Much faster than using parser
                var sr = File.OpenText(map);
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    if (line.Contains(searchGraphic))
                    {
                        results.Add(map);
                        break;
                    }
                    if (line == "[Objects]")
                        break;
                }
                sr.Close();
                // TODO: Add support for objects.
                //foreach (var obj in mmap.Objects)
            }
        }

        private void backgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            lblSearching.Text = "Searching in " + (string)e.UserState;
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            lstResults.Items.AddRange(results.ToArray());
            lblSearching.Text = "Search completed.";
        }

        private void lstResults_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            // lstResults.SelectedItem
        }
    }
}
