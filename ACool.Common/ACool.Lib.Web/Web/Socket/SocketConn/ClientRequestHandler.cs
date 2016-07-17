using System;
using System.Collections.Generic;
using System.Text;

using System.Net.Sockets;
using System.ComponentModel;

namespace SocketConn
{
    public class ClientRequestHandler
    {
        #region private property

        /// <summary>
        /// socket client 識別號碼
        /// </summary>
        private int _ClientNo;


        /// <summary>
        /// socket client reuqest
        /// </summary>
        private TcpClient _TcpClient;

        #endregion


        #region constructor

        /// 
        /// constructor
        /// 
        /// socket client 識別號碼
        /// socket client reuqest
        ///
        public ClientRequestHandler(int inClientNo, TcpClient inTcpClient)
        {
            this._ClientNo = inClientNo;
            this._TcpClient = inTcpClient;
        }

        #endregion


        #region public method

        /// 
        /// server & client 間相互進行通訊
        /// 
        public void DoCommunicate()
        {
            //產生 BackgroundWorker 負責處理每一個 socket client 的 reuqest
            BackgroundWorker bgwSocket = new BackgroundWorker();
            bgwSocket.DoWork += new DoWorkEventHandler(bgwSocket_DoWork);
            bgwSocket.RunWorkerAsync();

        }

        #endregion


        #region BackgroundWorker

        /// 
        /// 處理 socket client request
        /// 
        private void bgwSocket_DoWork(object sender, DoWorkEventArgs e)
        {
            //server & client 已經連線完成
            while (_TcpClient.Connected)
            {
                //取得網路串流物件，取得來自 socket client 的訊息
                NetworkStream netStream = _TcpClient.GetStream();
                byte[] readBuffer = new byte[1024];
                int count = 0;
                if ((count = netStream.Read(readBuffer, 0, readBuffer.Length)) != 0)
                {
                    string clientRequest = Encoding.UTF8.GetString(readBuffer, 0, count);
                    Write(" >> " + "From client(" + _ClientNo + ") =>" + clientRequest);

                    //正確取得 client requst，再回傳給 client
                    string serverResponse = "Server to clinet(" + _ClientNo + ") => message: " + clientRequest;
                    byte[] sendBytes = Encoding.UTF8.GetBytes(serverResponse);
                    netStream.Write(sendBytes, 0, sendBytes.Length);
                    netStream.Flush();
                    //Write(" >> " + serverResponse);
                }
            }
        }

        public Action ToSay = null;

        public void Write(string Msg)
        {
            if (ToSay != null)
            {
                ToSay();
            }

            SocketServer.sMessages.Add(Msg);
        }

        #endregion
    }
}