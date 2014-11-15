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
    public partial class frmErrors : Form
    {
        public frmErrors(string map, string errors)
        {
            InitializeComponent();
            textBox1.Text = errors;
            this.Text = "Errors while opening " + map;
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
