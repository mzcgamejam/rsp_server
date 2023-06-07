using BattleProtocol;
using BattleServer.Game;
using CommonType;
using SuperSocket.SocketBase;
using SuperSocket.SocketBase.Protocol;
using System;
using System.IO;

namespace BattleServer.Server
{
    public class BattleSession : AppSession<BattleSession, BinaryRequestInfo>
    {
        public int UserId;
        public string GameSessionId;
        public string PlayerSessionId;
        public PlayerType PlayerType;
        
        public void Send(MessageType messageType, byte[] data)
        {
            var header = BitConverter.GetBytes((int)messageType);
            StreamSend(header, data);
        }

        private void StreamSend(byte[] header, byte[] body)
        {
            using (var stream = new MemoryStream(header.Length + sizeof(int) + body.Length))
            {
                stream.Write(header, 0, header.Length);
                stream.Write(BitConverter.GetBytes(body.Length), 0, sizeof(int));
                stream.Write(body, 0, body.Length);
                stream.Seek(0, SeekOrigin.Begin);
                Send(stream.GetBuffer(), 0, (int)stream.Length);
            }
        }
    }
}
