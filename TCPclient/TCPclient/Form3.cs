using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TCPclient
{
    public partial class Form3 : Form
    {
        public string ID { get; private set; }
        public string PW { get; private set; }
        public string NAME { get; private set; }
        public int AGE { get; private set; }

        public Form3()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            ID = textBox1.Text;
            PW = textBox2.Text;
            NAME = textBox3.Text;
            AGE = int.Parse(textBox4.Text);
            this.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.Close();
        }
    }
}
