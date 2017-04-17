using GameServer.Protocol;
using SuperSocket.SocketBase;
using SuperSocket.SocketBase.Protocol;

namespace GameServer
{
    class GameServer : AppServer<GameSession, PacketRequestInfo>
    {
        public GameServer()
        : base(new DefaultReceiveFilterFactory<PacketReceiveFilter, PacketRequestInfo>())
        {

        }

        protected override void ExecuteCommand(GameSession session, PacketRequestInfo requestInfo)
        {
            session.OnReceiveMessage(requestInfo.Type, requestInfo.Body);
        }
    }
}
