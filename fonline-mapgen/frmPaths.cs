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
    public partial class frmPaths : Form
    {
        MapperSettings settings;

        const string dirSuffix = "[Directory]";

        public frmPaths(MapperSettings settings)
        {
            InitializeComponent();
            this.settings = settings;
            txtBasePath.Text = settings.Paths.BaseDir;
            txtCritterProtos.Text = settings.Paths.CritterProtos;
            txtItemProtos.Text = settings.Paths.ItemProtos;
            txtFOOBJ.Text = settings.Paths.FOOBJ;
            txtCritterTypes.Text = settings.Paths.CritterTypes;
            txtMapsDir.Text = settings.Paths.MapsDir;
            if (settings.Paths.DataFiles == null) settings.Paths.DataFiles = new List<string>();
            foreach (var file in settings.Paths.DataFiles)
                lstDataFiles.Items.Add(file);
            foreach (var dir in settings.Paths.DataDirs)
                lstDataFiles.Items.Add(dir + " " + dirSuffix);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            SetFolder(txtItemProtos);
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            settings.Paths.BaseDir = txtBasePath.Text;
            settings.Paths.CritterProtos = txtCritterProtos.Text;
            settings.Paths.ItemProtos = txtItemProtos.Text;
            settings.Paths.FOOBJ = txtFOOBJ.Text;
            settings.Paths.CritterTypes = txtCritterTypes.Text;
            settings.Paths.MapsDir = txtMapsDir.Text;

            settings.Paths.DataFiles.Clear();
            foreach (string entry in lstDataFiles.Items)
            {
                if (entry.Contains(dirSuffix))
                {
                    string trimmed = entry.Remove(entry.IndexOf(dirSuffix[0]) - 1, dirSuffix.Length + 1);
                    settings.Paths.DataDirs.Add((string)trimmed);
                }
                else
                    settings.Paths.DataFiles.Add((string)entry);
            }

            SettingsManager.SaveSettings(settings);
            this.Close();
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            openFileDialog1.Multiselect = true;
            openFileDialog1.Filter = "FOnline DAT|*.dat|FOnline ZIP|*.zip";
            openFileDialog1.ShowDialog();
            foreach (var file in openFileDialog1.FileNames)
                lstDataFiles.Items.Add(file);
        }

        private void SetFolder(TextBox txt)
        {
            if (folderBrowserDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                txt.Text = folderBrowserDialog1.SelectedPath;
        }

        private void SetFile(TextBox txt)
        {
            if (openFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                txt.Text = openFileDialog1.FileName;
        }

        private void SetDefault(TextBox txt, string path, string def)
        {
            if (txt.Text == "") txt.Text = path + def;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (folderBrowserDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                txtBasePath.Text = folderBrowserDialog1.SelectedPath;
            SetDefault(txtCritterProtos, txtBasePath.Text, @"\proto\critters\");
            SetDefault(txtItemProtos, txtBasePath.Text, @"\proto\items\");
            SetDefault(txtMapsDir, txtBasePath.Text, @"\maps\");
            SetDefault(txtCritterTypes, txtBasePath.Text, @"\data\CritterTypes.cfg");
            SetDefault(txtFOOBJ, txtBasePath.Text, @"\text\engl\FOOBJ.MSG");
        }

        private void button5_Click(object sender, EventArgs e)
        {
            SetFolder(txtCritterProtos);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            SetFolder(txtMapsDir);
        }

        private void button6_Click(object sender, EventArgs e)
        {
            SetFile(txtFOOBJ);
        }

        private void button7_Click(object sender, EventArgs e)
        {
            SetFile(txtCritterTypes);
        }

        private void btnRemove_Click(object sender, EventArgs e)
        {
            lstDataFiles.Items.Remove(lstDataFiles.SelectedItem);
        }

        private void btnAddDir_Click(object sender, EventArgs e)
        {
            if (folderBrowserDialog1.ShowDialog() != System.Windows.Forms.DialogResult.OK)
                return;

            lstDataFiles.Items.Add(folderBrowserDialog1.SelectedPath + " " + dirSuffix);
        }
    }
}
