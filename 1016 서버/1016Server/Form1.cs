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

namespace _1016Server
{
    public partial class Form1 : Form
    {
        WbServer server = new WbServer();

        public Form1()
        {
            InitializeComponent();
            server.ParentInfo(this);        //<===============
            CheckForIllegalCrossThreadCalls = false;
        }

        #region  소켓에서 보낸 정보
        public void LogMessage(string str)
        {
            str += "(" + DateTime.Now.ToString() + ")";
            Ui.LogPrint(listBox1, str);
        }

        public void ShortMessage(string str)
        {
            str += "(" + DateTime.Now.ToString() + ")";
            Ui.LogPrint(listBox2, str);
        }
        #endregion

        #region 메뉴

        private void 프로그램종료XToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void 서버연결CToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ServerInfo dlg = new ServerInfo();
            if(dlg.ShowDialog()== DialogResult.OK)
            {
                if (server.ServerRun(dlg.Port) == true)
                {
                    Ui.FillDrawing(panel1, Color.Blue);
                    Ui.LabelState(label1, true);

                    String temp = String.Format("[서버실행]{0}:{1} 성공",
                        server.ServerIp, server.ServerPort);
                    Ui.LogPrint(listBox1, temp);
                }
                else
                {
                    String temp = String.Format("[서버실행]{0}:{1} 실패",
                        server.ServerIp, server.ServerPort);
                    Ui.LogPrint(listBox1, temp);
                }
            }            
        }

        private void 서버연결해제DToolStripMenuItem_Click(object sender, EventArgs e)
        {
            server.ServerStop();

            Ui.FillDrawing(panel1, Color.Red);
            Ui.LabelState(label1, false);

            String temp = String.Format("[서버종료]{0}:{1} 성공",
                        server.ServerIp, server.ServerPort);
            Ui.LogPrint(listBox1, temp);
        }
        #endregion
        

        #region 수신패킷 분석 및 처리
        public void PaserByteData(Socket sock, byte[] data)
        {
            string msg = Encoding.Default.GetString(data);
            string[] token = msg.Split('@');
            switch (token[0].Trim())
            {
                case "NEWMEMBER": NewMember(sock, token[1]); break;
                case "DELMEMBER": DelMember(sock, token[1]); break;
                case "LOGIN": Login(sock,token[1]); break;
                case "LOGOUT": Logout(sock,token[1]); break;
                case "MESSAGE":
                    Message(sock, token[1]); break;
            }
        }
        void NewMember(Socket sock, string msg)
        {
            Ui.LogPrint(listBox2, "[수신]" + msg);

            string[] sp = msg.Split('#');

            Member mem = new Member(sp[0], sp[1], sp[2], int.Parse(sp[3]));

            string ackmessage;
            if(MemberControl.Instance.Insert(mem))
               ackmessage =  Packet.Instance.GetNewMemberAck(mem.Id, mem.Name, true);
            else
                ackmessage= Packet.Instance.GetNewMemberAck(mem.Id, mem.Name, false);

            Ui.LogPrint(listBox2, "[송신]" + ackmessage);

            server.Send(sock, ackmessage);
        }
        
        void DelMember(Socket sock, string msg)
        {
            Ui.LogPrint(listBox2, "[수신]" + msg);

            string[] sp = msg.Split('#');

            string id = sp[0];

            string ackmessage;
            if (MemberControl.Instance.Delete(id))
                ackmessage = Packet.Instance.GetDelMemberAck(id, true);
            else
                ackmessage = Packet.Instance.GetDelMemberAck(id, false);

            Ui.LogPrint(listBox2, "[송신]" + ackmessage);

            server.Send(sock, ackmessage);
        }
        void Login(Socket sock, string msg)
        {
            Ui.LogPrint(listBox2, "[수신]" + msg);

            string[] sp = msg.Split('#');

            string id = sp[0];
            string pw = sp[1];


            string ackmessage;
            Member mem = MemberControl.Instance.Login(id,pw);
            if (mem != null)
                ackmessage = Packet.Instance.GetLoginAck(id, pw, true);
        
            else
                ackmessage = Packet.Instance.GetLoginAck(id, pw, false);
            Ui.LogPrint(listBox2, "[송신]" + ackmessage);

            server.SendAll(ackmessage);
        }
        void Logout(Socket sock, string msg)
        {
            Ui.LogPrint(listBox2, "[수신]" + msg);

            string[] sp = msg.Split('#');

            string id = sp[0];

            string ackmessage;
            if (MemberControl.Instance.Logout(id))
                ackmessage = Packet.Instance.GetLogoutAck(id, true);
            else
                ackmessage = Packet.Instance.GetLogoutAck(id, false);

            Ui.LogPrint(listBox2, "[송신]" + ackmessage);

            server.SendAll(ackmessage);
        }
        void Message(Socket sock, string msg)
        {
            Ui.LogPrint(listBox2, "[수신]" + msg);

            string[] sp = msg.Split('#');
            string id = sp[0];
            string Cmsg = sp[1];
            string ackmessage;
            ackmessage = Packet.Instance.GetMessageAck(id, Cmsg,true);
            server.SendAll(ackmessage);
        }
        #endregion
    }
}
