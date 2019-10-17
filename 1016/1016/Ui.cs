using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace _1016
{
    class Ui
    {

        public static void Fiilpanal(Control c, Color color)
        {
            c.BackColor = color;
        }

        public static void LabelState(Control c, bool b)
        {
            if (b)
            {
                c.Text = "서버 접속중..";
                c.BackColor = Color.Blue;

            }
            else
            {
                c.Text = "서버 정지";
                c.BackColor = Color.Red;

            }
        }

        public static void LogPrint(ListBox c, string msg)
        {
            c.SelectedIndex = c.Items.Count - 1;
            c.SelectedIndex = -1;
            c.Items.Add(msg);
        }

    }
}
