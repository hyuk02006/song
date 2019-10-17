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
    public partial class Form1 : Form
    {
        wbClient cli = new wbClient();
        Packet packet = Packet.Instance;
        public string ID { get; private set; }
        public string NAME { get; private set; }

        public bool Messinger { get; private set; }

        public Form1()
        {
            Messinger = false;
            InitializeComponent();
            cli.ParentInfo(this);
            CheckForIllegalCrossThreadCalls = false;
            ButtonEnable(false, false, false, false);
            ToolmenueEnable(false, true, false);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            panel1.BackColor = Color.Red;
        }

        #region  소켓에서 보낸 정보
        //public void LogMessage(string str)
        //{
        //    str += "(" + DateTime.Now.ToString() + ")";
        //    UI.LogPrint(listBox1, str);
        //}
        //
        //public void ShortMessage(string str)
        //{
        //    str += "(" + DateTime.Now.ToString() + ")";
        //    UI.LogPrint(listBox2, str);
        //}
        #endregion

        #region 메뉴기능
        private void 서버연결ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form2 fo2 = new Form2();
            if(fo2.ShowDialog() == DialogResult.OK)
            {
                if (cli.Connect(fo2.Ip, fo2.Port) == true)
                {
                    UI.FillDrawing(panel1, Color.Blue);
                    UI.LabelState(label1, true);
                    String temp = String.Format("[연결] {0} : {1} 성공",
                        cli.serverIP, cli.serverPort);
                    UI.LogPrint(listBox1, temp);
                    ButtonEnable(false, true, false, true);
                    ToolmenueEnable(true, false, true);
                }
                else
                {
                    String temp = String.Format("[연결] {0} : {1} 실패",
                        cli.serverIP, cli.serverPort);
                    UI.LogPrint(listBox1, temp);
                }
            }
        }

        private void 서버연결해제ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            cli.Disconnect();
            UI.FillDrawing(panel1, Color.Red);
            UI.LabelState(label1, false);

            String temp = String.Format("[서버종료] {0} : {1} 성공",
                        cli.serverIP, cli.serverPort);
            UI.LogPrint(listBox1, temp);
            ButtonEnable(false, false, false, false);
            ToolmenueEnable(false, true, false);
        }

        public void Recive(byte[] data)
        {
            string msg = Encoding.Default.GetString(data);
                UI.LogPrint(listBox2, msg);
        }

        private void 회원가입ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form3 fo3 = new Form3();
            if (fo3.ShowDialog() == DialogResult.OK)
            {
                string msg = packet.GetNewMember(fo3.ID, fo3.PW, fo3.NAME, fo3.AGE);
                cli.Send(msg);
            }
        }

        #endregion

        #region 기본기능
        private void button1_Click(object sender, EventArgs e) //메세지 전송
        {
            string msg = packet.GetMessage(ID, textBox2.Text);
            cli.Send(msg);
        }

        private void button2_Click(object sender, EventArgs e) //로그인
        {
            string id = textBox3.Text;
            string pw = textBox4.Text;

            string msg = packet.GetLogin(id,pw);
            cli.Send(msg);
        }

        private void button3_Click(object sender, EventArgs e) // 로그아웃
        {
            string id = textBox3.Text;
            string msg = packet.GetLogOut(id);
            cli.Send(msg);
        }

        private void button4_Click(object sender, EventArgs e) // 탈퇴
        {
            string id = textBox3.Text;
            string msg = packet.GetDelMember(id);
            cli.Send(msg);
        }
        #endregion

        #region 수신 패킷 구분, 처리(클라이언트 -> 서버)
        //public void PaserByteData(byte[] data)
        //{
        //    string msg = Encoding.Default.GetString(data);
        //    string[] token = msg.Split('@');
        //    switch (token[0].Trim())
        //    {
        //        case "NEWMEMBER":
        //            NewMember(token[1]);//아이디 중복조회 결과 사용가능
        //            break;
        //    }
        //}
        //
        //void NewMember(string msg)
        //{
        //    string[] sp = msg.Split('#');
        //    Member mem = new Member(sp[0], sp[1], sp[2], int.Parse(sp[3]));
        //
        //    string ackmessage;
        //    if(MemberControl.Instance.Insert(mem))
        //        ackmessage = packet.GetNewMemberAck(mem.Id, mem.Name, true);
        //    else
        //        ackmessage = packet.GetNewMemberAck(mem.Id, mem.Name, false);
        //
        //    cli.Send(ackmessage); // 원래 서버 역할 결과값 보내기
        //}
        #endregion

        #region 수신 패킷 구분, 처리(서버 결과값 -> 클라이언트)
        public void PaserByteData(byte[] data)
        {
            string msg = Encoding.Default.GetString(data);
            string[] token = msg.Split('@');
            switch (token[0].Trim())
            {
                case "NEWMEMBERACK":
                    InsertResult(token[1]);//아이디 중복조회 결과 사용가능
                    break;
                case "DELMEMBERACK":
                    DelResult(token[1]);
                    break;

                case "LOGINACK":
                    LoginResult(token[1]);
                    break;

                case "LOGOUTACK":
                    LogoutResult(token[1]);
                    break;

                case "MESSAGEACK":
                    MsgResult(token[1]);
                    break;

            }
        }

        void InsertResult(string msg)
        {
            string[] sp = msg.Split('#');
            //bool b = bool.Parse(sp[0]);
            //string id = sp[1];
            //string name = sp[2];
            string temp = string.Format("{0}님 회원가입하셨습니다.",sp[1]);

            if (sp[0].Equals("True"))
            {
                UI.LogPrint(listBox2, temp);
                this.Text = string.Format(sp[2] + "님" + sp[1] + "입장하셨습니다.");
            }
            else
                MessageBox.Show("가입실패");
        }

        void DelResult(string msg)
        {
            string[] sp = msg.Split('#');
            string temp = string.Format("{0}님 회원탈퇴하셨습니다.", sp[1]);

            if (sp[0].Equals("True"))
            {
                UI.LogPrint(listBox2, temp);
            }
            else
                MessageBox.Show("탈퇴실패");

        }

        void LoginResult(string msg)
        {
            string[] sp = msg.Split('#');
            string temp = string.Format("{0}님 로그인하셨습니다.", sp[1]);
            
            ID = sp[1];
            NAME = sp[2];
            if (sp[0].Equals("True"))
            {
                UI.LogPrint(listBox2, temp);
                textBox1.Text = sp[2];
                Messinger = true;
                ButtonEnable(true, false, true, false);
                ToolmenueEnable(false, false, true);
            }
            else
                MessageBox.Show("로그인실패");

        }

        void LogoutResult(string msg)
        {
            string[] sp = msg.Split('#');

            if (sp[0].Equals("True"))
            {
                string temp = string.Format("{0}님 로그아웃하셨습니다.", sp[1]);
                UI.LogPrint(listBox2, temp);
                Messinger = false;
                ButtonEnable(false, true, false, true);
                ToolmenueEnable(true, false, true);
            }
            else
                MessageBox.Show("탈퇴실패");

        }

        void MsgResult(string msg)
        {
            string[] sp = msg.Split('#');
            string Print = string.Format(sp[0] + " : " + sp[1]);
            if (Messinger == true)
            {
                UI.LogPrint(listBox2, Print);
            }
        }
        #endregion

        void ButtonEnable(bool a1, bool a2, bool a3, bool a4)
        {
            button1.Enabled = a1; // 전송
            button2.Enabled = a2; // 로그인
            button3.Enabled = a3; // 로그아웃
            button4.Enabled = a4; // 탈퇴
        }

        void ToolmenueEnable(bool a1, bool a2, bool a3)
        {

            회원가입ToolStripMenuItem.Enabled = a1;
            서버연결ToolStripMenuItem.Enabled = a2;
            서버연결해제ToolStripMenuItem.Enabled = a3;

        }

        #region 쓰레기통
        private void 로그인ToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void 로그아웃ToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }
        #endregion
    }
}
