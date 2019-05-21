using PHI_Control_Center.Classes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PHI_Control_Center
{
    public partial class Configure : Form
    {
        public Configure()
        {
            InitializeComponent();
            TaskbarIcon.Visible = true;
        }

        bool exitclose = false;

        private void EXITToolStripMenuItem_Click(object sender, EventArgs e)
        {
            exitclose = true;
            Application.Exit();
        }

        private void configureToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Show();
        }

        private void Configure_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!exitclose)
            {
                Hide();
                e.Cancel = true;
            }
        }

        private void Configure_Load(object sender, EventArgs e)
        {
            DevicesLB.DataSource = Program.Devices;
        }

        private void DevicesLB_SelectedIndexChanged(object sender, EventArgs e)
        {
            Device d = (Device)DevicesLB.SelectedItem;
            if (d.IsAddon == false)
            {
                pb.Image = new Bitmap(@"Devices/" + d.DeviceId + ".png");
            }
            else
            {
                pb.Image = new Bitmap(@"Devices/A" + (d.DeviceId+d.AddonAddress) + ".png");
            }
        }

        private void pb_MouseClick(object sender, MouseEventArgs e)
        {
            foreach (Bank b in ((Device)DevicesLB.SelectedItem).Banks)
            {
                foreach (Classes.Control c in b.Controls)
                {
                    if (e.X >= c.X && e.X <= c.X + c.W && e.Y >= c.Y && e.Y <= c.Y + c.H)
                    {
                        MessageBox.Show(c.Type.ToString() + " "+c.Id);
                    }
                }
            }
        }
    }
}
