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
    public class ACoolClientBox
    {
        private ACoolServerInfo ServerInfo;

        private ACoolMsgBox MsgBox;

        private TcpListener _Listener;
        public ACoolClientBox(ACoolServerInfo Info, ACoolMsgBox MsgBox)
        {
            this.ServerInfo = Info;

            this.MsgBox = MsgBox;
        }
        public void AllowClientLink()
        {
            if (_Listener != null)
            {
                _Listener.Stop();
            }

            _Listener = new TcpListener(IPAddress.Any, this.ServerInfo.ServerPort);

            _Listener.Start();

            if (Run == null)
            {
                ServerRun();
            }
        }

        Task Run = null;

        bool bAllowClientLink = false;

        public List<ACoolClientInfo> Clients = new List<ACoolClientInfo>();
        private void ServerRun()
        {
            if (Run == null)
            {
                Run = Task.Factory.StartNew(() =>
                {
                    MsgBox.WriteMsg("Allow Client Link.");

                    while (true)
                    {
                        TcpClient _TcpClient = _Listener.AcceptTcpClient();

                        if (bAllowClientLink)
                        {
                            bAllowClientLink = false;
                            break;
                        }

                        ACoolClientInfo client = new ACoolClientInfo(_TcpClient, MsgBox);

                        MsgBox.WriteMsg(client.IP + " Conntect.");

                        Clients.Add(client);

                        client.Commondicate();
                    }
                });
            }
        }

        public void T(string IP)
        {
            ACoolClientInfo client = Clients.FirstOrDefault(x => x.IP == IP);

            if (client != null)
            {
                client.Break();

                Clients.Remove(client);
            }
            else
            {
                MsgBox.WriteMsg($"找不到{IP}的連線");
            }
        }
        public void CloseClientLink()
        {
            if (Run != null)
            {
                bAllowClientLink = true;

                MsgBox.WriteMsg("Close Client Link.");

                Run = null;
            }
        }
    }
}
