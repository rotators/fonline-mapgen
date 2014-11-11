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
    public partial class frmDebugInfo : Form
    {
        public frmDebugInfo()
        {
            InitializeComponent();
        }

        public void setText(string text)
        {
            textBox1.Text = text;
        }


        private void frmDebugInfo_Load(object sender, EventArgs e)
        {
            this.TopMost = true;
        }
    }
}
