using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace _1016
{
    public partial class Sign : Form
    {
        Packet pk = Packet.Instance;

        public string Id { get;set; }
        public string Pw { get;  set; }
        public string Name { get; private set; }
        public int Age { get; private set; }


        public Sign()
        {
            InitializeComponent();
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            string msg = null;
            Id = textBox1.Text.Trim();
            Pw = textBox2.Text.Trim();
            Name = textBox3.Text.Trim();
            Age = int.Parse(textBox4.Text.Trim());

     
            this.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.Close();
        }
    }
}
