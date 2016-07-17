using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ACool_Libary.SocketConn
{
    public interface ISocketServer
    {
        bool isOpen();
        void OpenServer();//Client
        void CloseServer();

        //Active Action
        void GetClientList();
        void RemoveClientLink();
        void SendMsgToAllClient(string msg);
        string GetAllMsg();

        //Passive Action
        string WaitCancelLinkFromClient();
        string WaitReceiveMsgFromClient();

    }
}
