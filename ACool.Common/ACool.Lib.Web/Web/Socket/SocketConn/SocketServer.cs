using System;
using System.Collections.Generic;
using System.Text;

using System.Net.Sockets;
using System.Net;
using System.ComponentModel;
using System.Threading.Tasks;

namespace SocketConn
{
    public class SocketServer
    {
        #region private property

        /// <summary>
        /// socket server TCP port number
        /// </summary>
        private int _PortNumber;


        /// <summary>
        /// TCP Listener
        /// </summary>
        private TcpListener _Listener;


        /// <summary>
        /// 用來處理 Listener 工作的執行緒
        /// </summary>
        private BackgroundWorker _bgwServer = new BackgroundWorker();


        /// 
        /// socker client 識別編號
        /// 
        private int _ClientNo = 0;

        #endregion


        #region static public property

        /// 
        /// 工作進行中產生之訊息
        /// 
        public static List<string> sMessages = new List<string>();

 


        #endregion


        #region constructor

        /// 
        /// constructor
        /// 
        ///         socket server TCP port number
        public SocketServer(int inPortNumber)
        {
            this._PortNumber = inPortNumber;

            //委派事件
            _bgwServer.DoWork += new DoWorkEventHandler(_bgwServer_DoWork);
        }

        #endregion


        #region public method

        /// 
        /// 開始監聽
        /// 
        public void Start()
        {
            if (!_bgwServer.IsBusy)
            {
                _bgwServer.RunWorkerAsync();
            }
        }

        #endregion


        #region BackgroundWorker

        /// 

        /// 處理 Listener 工作
        /// 
        /// 

        /// 

        private void _bgwServer_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                //若 TCP Listner 正在工作，則停止
                if (_Listener != null)
                    _Listener.Stop();

                //初始化 TCPListener
                _Listener = new TcpListener(IPAddress.Any, _PortNumber);

                //啟動 listener
                _Listener.Start();
                sMessages.Add(" >> " + "Server Started");

                //========== 持續接受監聽 socket client 的連線 ========== (start)
                while (true)
                {
                    //監聽到來自 socket client 的連線要求
                    TcpClient socket4Client = _Listener.AcceptTcpClient();

                    //累加 socket client 識別編號
                    _ClientNo++;
                    sMessages.Add(" >> " + "Client Request No:" + Convert.ToString(_ClientNo) + " started!");

                    //產生 BackgroundWorker 負責處理每一個 Socket Client 的要求
                    ClientRequestHandler handler = new ClientRequestHandler(_ClientNo, socket4Client);
                    handler.DoCommunicate();

                    handler.ToSay = () =>
                    {
                        isWait = false;
                    };
                }
                //========== 持續接受監聽 socket client 的連線 ========== (end)
            }
            catch (Exception exp)
            {
                sMessages.Add(exp.ToString());
            }
        }

        #endregion
        bool isWait = false;
        public void Wait()
        {
            isWait = true;

            while (isWait)
            {

            }
        }
    }
}


