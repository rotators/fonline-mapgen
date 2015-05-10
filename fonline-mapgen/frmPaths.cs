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

        //bool addedData = false;

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
                lstDataDirs.Items.Add(dir);
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnSetItemProtos_Click(object sender, EventArgs e)
        {
            SetFolder(txtItemProtos);
        }

        private bool IsEmpty(string field)
        {
            return MessageBox.Show(field + " has no path set, are you sure you want to continue?", 
                "No path set for " + field, MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == System.Windows.Forms.DialogResult.Yes;
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            settings.Paths.BaseDir = txtBasePath.Text;
            settings.Paths.CritterProtos = txtCritterProtos.Text;
            settings.Paths.ItemProtos = txtItemProtos.Text;
            settings.Paths.FOOBJ = txtFOOBJ.Text;
            settings.Paths.CritterTypes = txtCritterTypes.Text;
            settings.Paths.MapsDir = txtMapsDir.Text;

            if (settings.Paths.BaseDir == "")       if (!IsEmpty(lblBasePath.Text))      return;
            if (settings.Paths.CritterProtos == "") if (!IsEmpty(lblCritterProtos.Text)) return;
            if (settings.Paths.ItemProtos == "")    if (!IsEmpty(lblItemProtos.Text))    return;
            if (settings.Paths.FOOBJ == "")         if (!IsEmpty(lblFOOBJ.Text))         return;
            if (settings.Paths.CritterTypes == "")  if (!IsEmpty(lblCritterTypes.Text))  return;
            if (settings.Paths.MapsDir == "")       if (!IsEmpty(lblMapsDir.Text))       return;

            if(lstDataDirs.Items.Count+lstDataFiles.Items.Count == 0 )
                if(MessageBox.Show("No data files or directories added, no graphics will be able to render without the graphics files loaded. Are you sure you want to continue?", 
                    "No resource paths added", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == System.Windows.Forms.DialogResult.No) return;

            settings.Paths.DataFiles.Clear();
            settings.Paths.DataDirs.Clear();
            foreach (string entry in lstDataFiles.Items)
                settings.Paths.DataFiles.Add(entry);
            foreach (string entry in lstDataDirs.Items)
                settings.Paths.DataDirs.Add(entry);

            SettingsManager.SaveSettings(settings);
            this.Close();
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            openFileDialog1.Multiselect = true;
            openFileDialog1.Filter = "FOnline DAT|*.dat|FOnline ZIP|*.zip";
            openFileDialog1.ShowDialog();
            foreach (var file in openFileDialog1.FileNames)
                if (!lstDataFiles.Items.Contains(file))
                {
                    lstDataFiles.Items.Add(file);
                    //this.addedData = true;
                }
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

        private void btnSetBasePath_Click(object sender, EventArgs e)
        {
            if (folderBrowserDialog1.ShowDialog() != System.Windows.Forms.DialogResult.OK)
                return;
            txtBasePath.Text = folderBrowserDialog1.SelectedPath;
            SetDefault(txtCritterProtos, txtBasePath.Text, @"\proto\critters\");
            SetDefault(txtItemProtos, txtBasePath.Text, @"\proto\items\");
            SetDefault(txtMapsDir, txtBasePath.Text, @"\maps\");
            SetDefault(txtCritterTypes, txtBasePath.Text, @"\data\CritterTypes.cfg");
            SetDefault(txtFOOBJ, txtBasePath.Text, @"\text\engl\FOOBJ.MSG");
        }

        private void btnSetCritterProtos_Click(object sender, EventArgs e)
        {
            SetFolder(txtCritterProtos);
        }

        private void btnSetMapsDir_Click(object sender, EventArgs e)
        {
            SetFolder(txtMapsDir);
        }

        private void btnSetSetFOOBJ_Click(object sender, EventArgs e)
        {
            SetFile(txtFOOBJ);
        }

        private void btnSetCritterTypes_Click(object sender, EventArgs e)
        {
            SetFile(txtCritterTypes);
        }

        private void btnRemove_Click(object sender, EventArgs e)
        {
            foreach (string s in lstDataFiles.SelectedItems.OfType<string>().ToList())
                lstDataFiles.Items.Remove(s);
        }

        private void btnRemoveDir_Click(object sender, EventArgs e)
        {
            foreach (string s in lstDataDirs.SelectedItems.OfType<string>().ToList())
                lstDataDirs.Items.Remove(s);
        }

        private void btnAddDir_Click(object sender, EventArgs e)
        {
            if (folderBrowserDialog1.ShowDialog() != System.Windows.Forms.DialogResult.OK)
                return;
            if(lstDataDirs.Items.Contains(folderBrowserDialog1.SelectedPath))
                return;
            lstDataDirs.Items.Add(folderBrowserDialog1.SelectedPath);
           // this.addedData = true;
        }

        private void btnUpFile_Click(object sender, EventArgs e)
        {
            lstDataFiles.MoveUp();
        }

        private void btnDownFile_Click(object sender, EventArgs e)
        {
            lstDataFiles.MoveDown();
        }

        private void btnUpDir_Click(object sender, EventArgs e)
        {
            lstDataDirs.MoveUp();
        }

        private void btnDownDir_Click(object sender, EventArgs e)
        {
            lstDataDirs.MoveDown();
        }
    }
}
