using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace _1016
{
    class WbClient
    {
        private Socket server;
        private Form1 form;
        public string ServerIp { get; private set; }
        public string ServerPort { get; private set; }

        public void ParentInfo(Form1 f)
        {
            form = f;
        }

        #region 서버 연결/해제
        public bool Connect(string ip, string port)
        {
            try
            {
                IPEndPoint ipep = new IPEndPoint(IPAddress.Parse(ip), int.Parse(port));

                server = new Socket(AddressFamily.InterNetwork,
                                          SocketType.Stream, ProtocolType.Tcp);

                server.Connect(ipep);
                MessageBox.Show("서버 접속...");
                IPEndPoint addr = (IPEndPoint)server.LocalEndPoint;
                ServerIp = addr.Address.ToString();
                ServerPort = port;

                Thread th = new Thread(new ParameterizedThreadStart(RecvThread));
                th.Start();
                th.IsBackground = true;
                return true;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
                return false;
            }
        }

        public void DisConnect()
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

        #endregion

        #region 메세지 전달
        //public string MsgSend(string msg)
        //{
        //    try
        //    {
        //        byte[] data = new byte[1024];

        //        server.Send(Encoding.Default.GetBytes(msg));



        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show(ex.Message);
        //    }
        //    return msg;
        //}

        public void Send(string msg)
        {
            try
            {
                if (server.Connected)
                {
                    byte[] data = Encoding.Default.GetBytes(msg);
                    this.MsgSend(data);

                }
                else
                {

                    MessageBox.Show("서버 미연결");
                }
            }
            catch (Exception ex)
            {

                MessageBox.Show("클라이언트 미연결");
            }
        }
        public void MsgSend(byte[] data)
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

        #region 메세지 송신

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
                MessageBox.Show(ex.Message);

            }
        }


        #endregion

        public void RecvThread(object data)
        {

            byte[] msg = null;

            while (true)
            {

                //수신
                ReceiveData(server, ref msg);


                //분석 요청
                form.PaserByteData(msg);        //서버인듯
            }

        }
    }

}


