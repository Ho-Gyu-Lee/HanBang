using GameServer.Protocol;
using SuperSocket.SocketBase;
using SuperSocket.SocketBase.Protocol;

namespace GameServer
{
    class GameServer : AppServer<GameUserSession, PacketRequestInfo>
    {
        public GameServer()
        : base(new DefaultReceiveFilterFactory<PacketReceiveFilter, PacketRequestInfo>())
        {

        }

        protected override void ExecuteCommand(GameUserSession session, PacketRequestInfo requestInfo)
        {
            session.OnReceiveMessage(requestInfo.Type, requestInfo.Body);
        }
    }
}
