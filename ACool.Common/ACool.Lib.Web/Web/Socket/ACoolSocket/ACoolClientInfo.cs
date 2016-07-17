using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ACool_Libary.SocketConn.ACoolSocket
{
    public class ACoolClientInfo
    {
        private TcpClient _TcpClient;

        private ACoolMsgBox MsgBox;

        public string UserName;

        public string IP;
        public ACoolClientInfo(TcpClient client, ACoolMsgBox MsgBox)
        {
            this._TcpClient = client;

            this.MsgBox = MsgBox;

            IP = _TcpClient.Client.RemoteEndPoint.ToString();

            this.UserName = IP;
        }

        
        public void Commondicate()
        {
            Task.Factory.StartNew(() =>
            {
                while (_TcpClient.Connected)
                {
                    //取得網路串流物件，取得來自 socket client 的訊息
                    NetworkStream netStream = _TcpClient.GetStream();
                    byte[] readBuffer = new byte[1024];
                    int count = 0;
                    if ((count = netStream.Read(readBuffer, 0, readBuffer.Length)) != 0)
                    {
                        string clientRequest = Encoding.UTF8.GetString(readBuffer, 0, count);

                        if (clientRequest.StartsWith("[") && clientRequest.EndsWith("]"))
                        {
                            string OldName = UserName;

                            UserName = clientRequest.TrimStart('[').TrimEnd(']');

                            MsgBox.WriteMsg($"{OldName}({IP}) 改名為 {UserName}");
                        }
                        else
                        {
                            MsgBox.WriteMsg($"{UserName}({IP}) : {clientRequest}");
                        }

                        //正確取得 client requst，再回傳給 client
                        string serverResponse = "Server to clinet(" + _TcpClient + ") => message: " + clientRequest;
                        byte[] sendBytes = Encoding.UTF8.GetBytes(serverResponse);
                        netStream.Write(sendBytes, 0, sendBytes.Length);
                        netStream.Flush();
                        //MsgBox.WriteMsg(" >> " + serverResponse);
                    }
                }
            });
        }

        public void Break()
        {
            _TcpClient.Close();

            MsgBox.WriteMsg(IP + " BreakConntect.");
        }
    }
}
