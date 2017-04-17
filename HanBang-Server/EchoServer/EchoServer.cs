using EchoServer.Protocol;
using SuperSocket.SocketBase;
using SuperSocket.SocketBase.Protocol;

namespace EchoServer
{
    class EchoServer : AppServer<EchoSession, MyRequestInfo>
    {
        public EchoServer()
        : base(new DefaultReceiveFilterFactory<MyReceiveFilter, MyRequestInfo>())
        {
            
        }

        protected override void ExecuteCommand(EchoSession session, MyRequestInfo requestInfo)
        {
            session.OnReceiveMessage(requestInfo.Type, requestInfo.Body);
        }
    }
}
