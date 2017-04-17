using SuperSocket.SocketBase.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EchoServer
{
    class Program
    {
        static void Main(string[] args)
        {
            EchoServer server = new EchoServer();

            ServerConfig serverConfig = new ServerConfig
            {
                Ip          = "Any",
                Port        = 10001,
                LogFactory  = "ConsoleLogFactory",
                SendingQueueSize = 100,
            };

            //서버 설정 셋업
            server.Setup(serverConfig);

            //서버 시작
            if (false == server.Start())
            {
                return;
            }

            while(true)
            {
                System.Threading.Thread.Sleep(1000);
            }
        }
    }
}
