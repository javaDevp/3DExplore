using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PerformanceDemo
{
    public partial class FrmTest : Form
    {
        public FrmTest()
        {
            InitializeComponent();
        }

        private void btnShowForm1_Click(object sender, EventArgs e)
        {
            using (var frm = new Form1())
            {
                frm.ShowDialog();
            }
        }

        private void btnShowForm2_Click(object sender, EventArgs e)
        {
            using (var frm = new Form2())
            {
                frm.ShowDialog();
            }
        }
    }
}
