using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using PluginBase;

namespace MagicPlugin
{
    public partial class dialogForm : Form,IDialogForm
    {
        public dialogForm()
        {
            InitializeComponent();
        }

        public List<object> go(List<object> start)
        {
            if (start == null)
            {
                start = new List<object>();
                start.Add("");
            }
            textBox1.Text = Convert.ToString(start[0]);
            ShowDialog();
            start[0] = textBox1.Text;
            return start;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
