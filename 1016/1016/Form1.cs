using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace _1016
{
    public partial class Form1 : Form
    {
        WbClient wb = new WbClient();
        Packet pk = Packet.Instance;
        Sign s = new Sign();

        public bool Islogin { get; private set; }

        public string Name1 { get; private set; }
        public string Id1 { get; private set; }
        public Form1()
        {
            InitializeComponent();
            wb.ParentInfo(this);
            CheckForIllegalCrossThreadCalls = false;
            Islogin = false;
        }


        #region 서버 연결 / 해제
        private void 서버연결ToolStripMenuItem_Click(object sender, EventArgs e)
        {

            ServerInfo dlg = new ServerInfo();
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    if (wb.Connect(dlg.Ip, dlg.Port) == true)
                    {

                        Ui.Fiilpanal(panel1, Color.Blue);
                        Ui.LabelState(label1, true);

                        String temp = String.Format("[연결] {0} : {1} 성공",
                           wb.ServerIp, wb.ServerPort);
                        Ui.LogPrint(listBox1, temp);

                    }
                    else
                    {
                        String temp = String.Format("[연결실패] {0} : {1} 실패",
                            wb.ServerIp, wb.ServerPort);
                        Ui.LogPrint(listBox1, temp);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }


        }

        private void 서버연결해제ToolStripMenuItem_Click(object sender, EventArgs e)
        {

            wb.DisConnect();
            Ui.Fiilpanal(panel1, Color.Red);
            Ui.LabelState(label1, false);

        }
        #endregion

        #region 메뉴 바
        private void 회원가입ToolStripMenuItem_Click(object sender, EventArgs e)
        {

            try
            {
                if (s.ShowDialog() == DialogResult.OK)
                {
                    string msg = null;
                    msg = pk.GetNewMember(s.Id, s.Pw, s.Name, s.Age);
                    wb.Send(msg);

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }
        private void 프로그램종료ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        #endregion

        #region 인코딩,출력
        public void recv(string msg)
        {
            ShortMessage(msg);

        }
        public void ShortMessage(string str)
        {

            Ui.LogPrint(listBox2, str);
        }
        #endregion


        #region 버튼 기능
        private void Button1_Click(object sender, EventArgs e)
        {
            string msg = null;

            Name1 =  textBox1.Text.Trim();
            msg = textBox2.Text;


            msg = pk.GetMessage(Name1, msg);


            wb.Send(msg);

        }



        private void Button2_Click(object sender, EventArgs e)
        {
            string msg = null;
            s.Id = textBox3.Text.Trim();
            s.Pw = textBox4.Text.Trim();

            msg = pk.GetLogin(s.Id, s.Pw);
            textBox2.Text = Name1;

            wb.Send(msg);

        }

        private void Button3_Click(object sender, EventArgs e)
        {
            string msg = null;
            s.Id = textBox3.Text.Trim();

            msg = pk.GetLogOut(s.Id);
            wb.Send(msg);
        }

        private void Button4_Click(object sender, EventArgs e)
        {
            string msg = null;
            s.Id = textBox3.Text.Trim();
            msg = pk.GetDelMember(s.Id);

            wb.Send(msg);
        }
        #endregion

        #region 수신 패킷 분석 및 처리

        public void PaserByteData(byte[] data)
        {
            string msg = Encoding.Default.GetString(data);
            string[] token = msg.Split('@');
            switch (token[0].Trim())
            {
                case "NEWMEMBERACK": NewMemberack(token[1]); break;

                case "DELMEMBERACK": DelMemberack(token[1]); break;
                case "LOGINACK": Loginack(token[1]); break;
                case "LOGOUTACK": Logoutack(token[1]); break;
                case "MESSAGEACK":
                    Messageack(token[1]); break;



            }
        }

        void NewMemberack(string msg)
        {
            string[] sp = msg.Split('#');
            string str = string.Format("{0} 님이 회원 가입", sp[2]);
            if (sp[0].Equals("True"))
            {
                Ui.LogPrint(listBox2, str);
                this.Text = string.Format("{0} 님 {1} 입장하셨습니다.", sp[2], sp[1]);

            }
            else
                MessageBox.Show("회원가입 실패");


        }
        void DelMemberack(string msg)
        {
            string[] sp = msg.Split('#');
            string str = string.Format("{0} 님이 탈퇴", sp[1]);

            if (sp[0].Equals("True"))
            {
                Ui.LogPrint(listBox2, str);
                MessageBox.Show("삭제 성공");

            }
            else
                MessageBox.Show("삭제 실패");

            
        }

        void Loginack(string msg)
        {
            string[] sp = msg.Split('#');
            

            if (sp[0].Equals("True"))
            {
                if (textBox3.Text.Equals(sp[1]) && Islogin == false)
                {
                    string str = string.Format("{0} 님이 입장", sp[1]);
                    this.Text = string.Format("{0} 님이 로그인 하셨습니다.", sp[2]);
                
                    Ui.LogPrint(listBox2, str);
                    Islogin = true;
                    textBox1.Text = sp[2];
               
                }
                else if (Islogin == true)
                {
                    string str = string.Format("{0} 님이 입장", sp[1]);
                    recv(str);
                }

            }
            else
            {
                MessageBox.Show("로그인 실패");
            }
            
        }
        void Logoutack(string msg)
        {

            string[] sp = msg.Split('#');
            string str = string.Format("{0} 나감", sp[1]);
            if (sp[0].Equals("True"))
            {
                this.Text = string.Format("{0} 님이 로그아웃 하셨습니다", sp[1]);
             
                Islogin = false;
                recv(str);
            }
            else
            {
                MessageBox.Show("로그아웃");
            }

         
        }
        void Messageack(string msg)
        {
            string[] sp = msg.Split('#');
           
            if (sp[0].Equals("True")&& Islogin == true)
            {
                string str = string.Format("[{0}] :{1} ", sp[1], sp[2]);
              
                recv(str);    
            }
           

        }
        #endregion
    }
}
