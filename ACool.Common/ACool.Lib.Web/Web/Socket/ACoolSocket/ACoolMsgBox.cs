using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ACool_Libary.SocketConn.ACoolSocket
{
    public class ACoolMsgBox
    {
        List<string> ServerMsgs = new List<string>();
        public void WriteMsg(string msg)
        {
            ServerMsgs.Add(msg);

            if (isWait)
            {
                isWait = false;
            }
        }

        bool isWait = false;

        public void WaitNewMsg()
        {
            isWait = true;

            while (isWait)
            {
                //需要釋放放棄等待的Request
            }
        }

        public string GetMsg()
        {
            return string.Join("\r\n", ServerMsgs.Reverse<string>().Take(10));
        }
    }
}
