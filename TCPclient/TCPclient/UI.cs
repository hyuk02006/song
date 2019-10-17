using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TCPclient
{
    class UI
    {
        public static void FillDrawing(Control c, Color color)
        {
            c.BackColor = color;
        }

        public static void LabelState(Control c, bool b)
        {
            if(b)
            {
                c.Text = "서버접속중...";
                c.BackColor = Color.Blue;
            }
            else
            {
                c.Text = "서버정지";
                c.BackColor = Color.Red;
            }
        }
        
        public static void LogPrint(ListBox c, string msg)
        {
            msg += "(" + DateTime.Now.ToString() + ")";
            c.Items.Add(msg);
            c.SelectedIndex = c.Items.Count - 1;
            c.SelectedIndex = -1;
        }
    }
}
