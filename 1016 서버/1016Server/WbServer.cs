using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace _1016Server
{
    enum LogType { RUN, STOP, CONNECT, DISCONNECT, ERROR }

    class WbServer
    {
        #region 맴버 필드 및 프로퍼티
        private Socket server;
        private Form1 form;
        private List<Socket> slist = new List<Socket>();

        public string ServerIp { get; private set; }
        public int ServerPort  { get; private set;  }

        #endregion

        #region 기능(외부 접근) Form1에서 접근

        public void ParentInfo(Form1 f)
        {
            form = f;
        }

        public bool ServerRun(int port)
        {
            try
            {
                server = new Socket(AddressFamily.InterNetwork,
                                           SocketType.Stream, ProtocolType.Tcp);
                IPEndPoint ipep = new IPEndPoint(IPAddress.Any, port);
                server.Bind(ipep);
                server.Listen(20);

                IPEndPoint ip = (IPEndPoint)server.LocalEndPoint;
                ServerIp = ip.Address.ToString();
                ServerPort = port;

                Thread th = new Thread(new ParameterizedThreadStart(ServerThread));
                th.Start(this);
                th.IsBackground = true;

                return true;
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
                return false;
            }
        }

        public void ServerStop()
        {
            try
            {
                server.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public void Send(Socket sock, string msg)
        {
            try
            {
                if (sock.Connected)
                {
                    byte[] data = Encoding.Default.GetBytes(msg);
                    this.SendData(sock, data);
                }
                else
                {
                    LogMessage(LogType.ERROR, sock, "클라이언트미연결");
                 }
            }
            catch (Exception ex)
            {
                LogMessage(LogType.ERROR, sock, ex.Message); 
            }
        }

        #endregion

        #region 기능(내부 사용) WbServer -> Form1으로 전달하는 함수
        private void LogMessage(LogType type, Socket sock, string str)
        {
            IPEndPoint ip = (IPEndPoint)sock.RemoteEndPoint;

            String temp = String.Empty;

            if (type == LogType.CONNECT)
            {
                temp = String.Format("[클라이언트접속]{0}:{1} 성공",
                      ip.Address, ip.Port);
            }
            else if(type == LogType.DISCONNECT)
            {
                temp = String.Format("[클라이언트접속해제]{0}:{1} 성공",
                      ip.Address, ip.Port);
            }
            else if(type == LogType.ERROR)
            {
                temp = String.Format("[에러]{0}:{1} {2}",
                      ip.Address, ip.Port, str);
            }

            form.LogMessage(temp);    
        }
        
        private void ShortMessage(byte[] recvdata)
        {
            form.ShortMessage(Encoding.Default.GetString(recvdata));
        }
        
        #endregion

        #region(내부 사용) 자체 호출 기능

        public void SendAllData(byte[] msg)
        {
            foreach (Socket s in slist)
            {
                SendData(s, msg);
            }
        }
           public void SendAll(string msg)
        {
            byte[] data = Encoding.Default.GetBytes(msg);
            SendAllData(data);
        }

        private void SendData( Socket sock,byte[] data)
        {
            try
            {
                int total = 0;
                int size = data.Length;
                int left_data = size;
                int send_data = 0;

                // 전송할 데이터의 크기 전달
                byte[] data_size = new byte[4];
                data_size = BitConverter.GetBytes(size);
                send_data = sock.Send(data_size);

                // 실제 데이터 전송
                while (total < size)
                {
                    send_data = sock.Send(data, total, left_data, SocketFlags.None);
                    total += send_data;
                    left_data -= send_data;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private void ReceiveData(Socket sock, ref byte[] data)
        {
            try
            {
                int total = 0;
                int size = 0;
                int left_data = 0;
                int recv_data = 0;

                // 수신할 데이터 크기 알아내기 
                byte[] data_size = new byte[4];
                recv_data = sock.Receive(data_size, 0, 4, SocketFlags.None);
                size = BitConverter.ToInt32(data_size, 0);
                left_data = size;

                data = new byte[size];

                // 실제 데이터 수신
                while (total < size)
                {
                    recv_data = sock.Receive(data, total, left_data, 0);
                    if (recv_data == 0) break;
                    total += recv_data;
                    left_data -= recv_data;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw ex;
            }
        }
       
        #endregion

        #region thread

        private void ServerThread(object data)
        {
            while (true)
            {
                Socket client = server.Accept();

                slist.Add(client);      //소켓 저장

                LogMessage(LogType.CONNECT, client,"");   //로그메시지 처리  

                //스레드 생성(소켓 전달)
                Thread tr = new Thread(new ParameterizedThreadStart(WorkThread));
                tr.Start(client);
                tr.IsBackground = true;
            }
        }

        private void WorkThread(object data)
        {
            Socket sock = (Socket)data; 

            byte[] msg = null;
            try
            {
                while (true)
                {
                    //수신
                    ReceiveData(sock, ref msg);   // 수신한 문자열이 있으면 화면에 출력

                    //분석요청
                    form.PaserByteData(sock, msg);

                    ShortMessage(msg);

                    //송신
                    //SendData(sock, msg);
                    //SendAllData(sock, msg);   //SendData(sock, msg)  

                    msg = null;
                    
                }
            }
            catch(Exception ex)
            {
                LogMessage(LogType.ERROR, sock, ex.Message);
                LogMessage(LogType.DISCONNECT, sock,"");
               

                slist.Remove(sock); //소켓 제거

                sock.Close();  
            }                   
        }
        #endregion         
    }
}
