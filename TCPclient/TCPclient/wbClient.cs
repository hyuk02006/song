using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Windows.Forms;
using System.Threading;

namespace TCPclient
{
    class wbClient
    {
        enum LogType { RUN, STOP, CONNECT, DISCONNECT, ERROR }

        public Socket server;
        public string serverIP { get; private set; }
        public int serverPort { get; private set; }
        private Form1 form;
        Thread thread;

        public void ParentInfo(Form1 f)
        {
            form = f;
        }

        #region 서버 연결
        public bool Connect(string ip, int port)
        {
            serverIP = ip;
            serverPort = port;
            try
            {
                IPEndPoint ipep = new IPEndPoint(IPAddress.Parse(ip), port);

                server = new Socket(AddressFamily.InterNetwork,
                                         SocketType.Stream, ProtocolType.Tcp);

                server.Connect(ipep);
                thread = new Thread(new ThreadStart(ClientThread));//new () <= 델리게이션 매개변수 o ThreadStart , x ParameterizedThreadStart

                thread.Start();
                thread.IsBackground = true; //백그라운드에서 돌다 프로그램 종료시 알아서 죽겠다.

                MessageBox.Show("서버에 접속...");
                
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        #endregion

        #region 연결 해제
        public void Disconnect()
        {
            try
            {
                thread.Abort();
                server.Close();
            }
            catch (Exception ex)
            {
               MessageBox.Show(ex.Message);
            }
        }
        #endregion

        #region 에러 주석
        //private void LogMessage(LogType type, string str)
        //{
        //    IPEndPoint ip = (IPEndPoint)server.RemoteEndPoint;
        //
        //    String temp = String.Empty;
        //
        //    if (type == LogType.CONNECT)
        //    {
        //        temp = String.Format("[클라이언트접속]{0}:{1} 성공",
        //              ip.Address, ip.Port);
        //    }
        //    else if (type == LogType.DISCONNECT)
        //    {
        //        temp = String.Format("[클라이언트접속해제]{0}:{1} 성공",
        //              ip.Address, ip.Port);
        //    }
        //    else if (type == LogType.ERROR)
        //    {
        //        temp = String.Format("[에러]{0}:{1} {2}",
        //              ip.Address, ip.Port, str);
        //    }
        //
        //    form.LogMessage(temp);
        //}
        #endregion

        #region 스레드, 서버에서 데이터 받기
        private void ClientThread() // ParameterizedThreadStart 일때 (object data)
        {
            byte[] data = null;
            while (true)
            {
                //수신
                ReceiveData(ref data);
                //분석요청
                form.PaserByteData(data);
                //form.Recive(data); //화면 출력
            }
        }
        #endregion

        #region 데이터 보내기
        public void SendData(byte[] data)//string msg)
        {
            try
            {
                int total = 0;
                int size = data.Length;
                int left_data = size;
                int send_data = 0;

                // 전송할 데이터의 크기 전달 4바이트 를 보낸다. 4바이트안에는 데이터 크기를 보낸다.
                byte[] data_size = new byte[4];
                data_size = BitConverter.GetBytes(size);
                send_data = server.Send(data_size);

                // 실제 데이터 전송
                while (total < size)
                {
                    send_data = server.Send(data, total, left_data, SocketFlags.None);
                    total += send_data;
                    left_data -= send_data;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        #endregion

        public void Send(string msg)
        {
            try
            {
                if (server.Connected)
                {
                    byte[] data = Encoding.Default.GetBytes(msg);
                    this.SendData(data);
                }
                else
                {
                    MessageBox.Show("서버미연결");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("클라이언트미연결");
            }
        }

        #region 테스트 리시브
        private void ReceiveData(ref byte[] data)
        {
            try
            {
                int total = 0;
                int size = 0;
                int left_data = 0;
                int recv_data = 0;

                // 수신할 데이터 크기 알아내기 
                byte[] data_size = new byte[4];
                recv_data = server.Receive(data_size, 0, 4, SocketFlags.None);
                size = BitConverter.ToInt32(data_size, 0);
                left_data = size;

                data = new byte[size];

                // 실제 데이터 수신
                while (total < size)
                {
                    recv_data = server.Receive(data, total, left_data, 0);
                    if (recv_data == 0) break;
                    total += recv_data;
                    left_data -= recv_data;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        #endregion
    }
}

