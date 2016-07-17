using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ACool_Libary.SocketConn.ACoolSocket
{
    public class ACoolClient
    {
        private ACoolServerInfo Client;

        private ACoolServerInfo Server;

        private TcpClient _TcpClient;

        private ACoolMsgBox MsgBox;

        public ACoolClient(string inRemoteIpAddr, int inRemotePortNum)
        {
            Client = new ACoolServerInfo(inRemoteIpAddr, inRemotePortNum);

            MsgBox = new ACoolMsgBox();
        }

        public void Connect(string inRemoteIpAddr, int inRemotePortNum)
        {
            Server = new ACoolServerInfo(inRemoteIpAddr, inRemotePortNum);

            //初始化 socket client
            _TcpClient = new TcpClient(Client.IpAddress, Client.ServerPort);

            _TcpClient.Connect(Server.IpAddress, Server.ServerPort);

            MsgBox.WriteMsg("Client Socket Program - Server Connected ...");
        }

        public void BreakConnect()
        {
            if (_TcpClient != null)
            {
                _TcpClient.Close();
            }
        }

        public string Send(string inMessage)
        {
            try
            {
                //取得用來傳送訊息至 socket server 的 stream 物件
                NetworkStream serverStream = _TcpClient.GetStream();

                //將資料轉為 byte[]
                byte[] outStream = System.Text.Encoding.UTF8.GetBytes(inMessage);

                //將資料寫入 stream object (表示傳送資料至 socket server)
                serverStream.Write(outStream, 0, outStream.Length);
                serverStream.Flush();

                //讀取 socket server 回傳值並轉為 string
                byte[] inStream = new byte[10025];
                serverStream.Read(inStream, 0, (int)_TcpClient.ReceiveBufferSize);
                string returndata = System.Text.Encoding.UTF8.GetString(inStream);

                MsgBox.WriteMsg("Data from Server : " + returndata);
                return returndata;
            }
            catch (Exception exp)
            {
                MsgBox.WriteMsg(exp.ToString());
                return exp.ToString();
            }
        }

        public string GetMsg()
        {
            return MsgBox.GetMsg();
        }

    }
}
