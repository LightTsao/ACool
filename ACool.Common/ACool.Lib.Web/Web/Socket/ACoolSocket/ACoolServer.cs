using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ACool_Libary.SocketConn.ACoolSocket
{
    public class ACoolServer
    {
        ACoolServerInfo Info = null;

        private ACoolMsgBox MsgBox = null;

        private ACoolClientBox ClientBox = null;

        
        public ACoolServer(int port)
        {
            Info = new ACoolServerInfo(port);

            MsgBox = new ACoolMsgBox();

            ClientBox = new ACoolClientBox(Info, MsgBox);
        }

        public void OpenServer()
        {
            if (Run == null)
            {
                ServerRun();
            }
        }



        private Task Run;

        private string ServerCmd = null;

        private void ServerRun()
        {
            if (Run == null)
            {
                MsgBox.WriteMsg("Open Server.");

                ClientBox.AllowClientLink();

                Run = Task.Factory.StartNew(() =>
                {
                    while (true)
                    {
                        if (ServerCmd != null)
                        {
                            if (ServerCmd == "Close")
                            {
                                ServerCmd = null;
                                break;
                            }
                            else if (ServerCmd == "OpenClient")
                            {
                                ClientBox.AllowClientLink();
                                ServerCmd = null;
                            }
                            else if (ServerCmd == "CloseClient")
                            {
                                ClientBox.CloseClientLink();
                                ServerCmd = null;
                            }
                            else if (ServerCmd.StartsWith("T"))
                            {
                                string IP = ServerCmd.TrimStart('T');

                                ClientBox.T(IP);

                                ServerCmd = null;
                            }
                            else
                            {
                                MsgBox.WriteMsg("無效命令" + ServerCmd);
                                ServerCmd = null;
                            }
                        }

                        //Thread.Sleep(1000 * 1);

                        //MsgBox.WriteMsg(DateTime.Now.ToString("yyyy/MM/dd hh:mm:ss"));
                    }

                    Run = null;

                    MsgBox.WriteMsg("Server Close.");
                });
            }
        }
        public void Cmd(string cmd)
        {
            if (Run != null)
            {
                ServerCmd = cmd;
            }
        }

        public void WaitMsg()
        {
            MsgBox.WaitNewMsg();
        }

        public string GetMsg()
        {
            return MsgBox.GetMsg();
        }

        public bool isConnect()
        {
            return Run != null;
        }


        //private TcpListener _Listener;

        //private int ServerPort;

        //private Task MainWork;

        //private Task ListenClientWork;

        //Dictionary<TcpClient, Task> CurrentClients = new Dictionary<TcpClient, Task>();



        //public void WaitClientMsg(TcpClient _TcpClient)
        //{
        //    Task ClientMsg = null;

        //    CurrentClients.Add(_TcpClient, ClientMsg);

        //    ClientMsg = Task.Factory.StartNew(() =>
        //   {
        //       try
        //       {
        //           while (_TcpClient.Connected)
        //           {
        //               //取得網路串流物件，取得來自 socket client 的訊息
        //               NetworkStream netStream = _TcpClient.GetStream();
        //               byte[] readBuffer = new byte[1024];
        //               int count = 0;
        //               if ((count = netStream.Read(readBuffer, 0, readBuffer.Length)) != 0)
        //               {
        //                   string clientRequest = Encoding.UTF8.GetString(readBuffer, 0, count);
        //                   WriteMsg(" >> " + "From client(" + _TcpClient.Client.ToString() + ") =>" + clientRequest);

        //                   //正確取得 client requst，再回傳給 client
        //                   string serverResponse = "Server to clinet(" + _TcpClient.Client.ToString() + ") => message: " + clientRequest;
        //                   byte[] sendBytes = Encoding.UTF8.GetBytes(serverResponse);
        //                   netStream.Write(sendBytes, 0, sendBytes.Length);
        //                   netStream.Flush();
        //                   WriteMsg(" >> " + serverResponse);
        //               }
        //           }
        //       }
        //       catch (Exception ex)
        //       {
        //           WriteMsg(ex.Message);
        //       }
        //   });
        //}

        //private string CurrentCmd = null;

        //public void SystemCmd(string cmd)
        //{
        //    CurrentCmd = cmd;
        //}

        //public void ListenSystemCmd()
        //{
        //    while (CurrentCmd == null)
        //    {
        //        if (CurrentCmd != null)
        //        {

        //        }

        //        Thread.Sleep(1000 * 1);
        //    }
        //}

        //public void ListenClient()
        //{
        //    if (ListenClientWork == null || (ListenClientWork != null && !ListenClientWork.IsCompleted))
        //    {
        //        ListenClientWork = Task.Factory.StartNew(() =>
        //        {
        //            try
        //            {
        //                WriteMsg("Client Listening.");

        //                //========== 持續接受監聽 socket client 的連線 ========== (start)
        //                while (true)
        //                {
        //                    //監聽到來自 socket client 的連線要求
        //                    TcpClient socket4Client = _Listener.AcceptTcpClient();

        //                    //累加 socket client 識別編號                            
        //                    WriteMsg(" >> " + "Client Request No:" + socket4Client.Client.ToString() + " started!");

        //                    WaitClientMsg(socket4Client);
        //                }
        //                //========== 持續接受監聽 socket client 的連線 ========== (end)

        //            }
        //            catch (Exception ex)
        //            {
        //                WriteMsg(ex.Message);
        //            }
        //        });
        //    }
        //}
        //public void OpenServer()
        //{
        //    if (MainWork == null || (MainWork != null && !MainWork.IsCompleted))
        //    {
        //        MainWork = Task.Factory.StartNew(() =>
        //        {
        //            try
        //            {
        //                if (_Listener != null)
        //                {
        //                    _Listener.Stop();
        //                }

        //                _Listener = new TcpListener(IPAddress.Any, ServerPort);

        //                _Listener.Start();

        //                WriteMsg("Server Open.");

        //                //========== 持續接受監聽 socket client 的連線 ========== (start)
        //                while (true)
        //                {
        //                    ListenClient();

        //                    ListenSystemCmd();
        //                }
        //                //========== 持續接受監聽 socket client 的連線 ========== (end)

        //                WriteMsg("Server Close.");

        //            }
        //            catch (Exception ex)
        //            {
        //                WriteMsg(ex.Message);
        //            }
        //        });
        //    }
        //}


    }
}
