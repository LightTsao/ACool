using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ACool_Libary.SocketConn
{
    public interface ISocketClient
    {
        //Active Action
        void ConnectToServer();
        string CancelLinkFromClient();
        void SendMsgToServer(string msg);
        string GetAllMsg();

        //Passive Action
        string WaitReceiveMsgFromServer();
    }
}
