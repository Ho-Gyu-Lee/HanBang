using SuperSocket.SocketBase.Config;
using SuperSocket.SocketBase.Logging;

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
                SendingQueueSize = 100,
            };

            //서버 설정 셋업
            server.Setup(serverConfig, logFactory: new ConsoleLogFactory());

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
