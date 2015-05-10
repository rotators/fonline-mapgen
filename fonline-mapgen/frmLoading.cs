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
    public partial class frmLoading : Form
    {
        private int FilesDone = 0;
        private int ResourcesDone = 0;
        private string DataFile;
        private string Resource;

        //private int NumDataFiles;
        //private int NumResources;

        public frmLoading()
        {
            InitializeComponent();
        }

        public void SetFilesNum(int DataFiles)
        {
            progressFiles.Maximum = DataFiles;
        }

        public void SetResourceNum(int Resources)
        {
            progressResource.Maximum = Resources;
        }

        public void SetNextFile(string DataFile)
        {
            this.FilesDone++;
            this.ResourcesDone = 0;
            this.DataFile = DataFile;
            UpdateProgress();
        }

        public void SetNextResource(string Resource)
        {
            this.ResourcesDone++;
            this.Resource = Resource;
            UpdateProgress();
        }

        public void UpdateProgress()
        {
            progressFiles.Value = (FilesDone == -1  ? 0 : FilesDone);
            progressResource.Value = ResourcesDone;
            lblResource.Text = "Loading " + Resource + " from " + DataFile;
        }
    }
}
