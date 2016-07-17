using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ACool_Libary.SocketConn.ACoolSocket
{
    public class ACoolServerInfo
    {
        private int _ServerPort;
        public int ServerPort { get { return _ServerPort; } }

        private string _IpAddress;
        public string IpAddress { get { return _IpAddress; } }
        public ACoolServerInfo(int ServerPort)
        {
            this._ServerPort = ServerPort;
        }

        public ACoolServerInfo(string IpAddress,int ServerPort)
        {
            this._IpAddress = IpAddress;

            this._ServerPort = ServerPort;
        }
    }
}
