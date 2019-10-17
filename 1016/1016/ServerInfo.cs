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
    public partial class ServerInfo : Form
    {
        public string Ip { get; private set; }
        public string Port { get; private set; }

      
        public ServerInfo()
        {
            InitializeComponent();
        }

        #region 서버 정보

        private void Button1_Click(object sender, EventArgs e)
        {
            Port = textBox1.Text;
            Ip = textBox2.Text;
            this.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.Close();
        }

        private void Button2_Click(object sender, EventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.Close();

        }

        #endregion
    }
}
