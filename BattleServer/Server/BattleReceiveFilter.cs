using SuperSocket.Common;
using SuperSocket.Facility.Protocol;
using SuperSocket.SocketBase.Protocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BattleServer.Server
{
    class BattleReceiveFilter : FixedHeaderReceiveFilter<BinaryRequestInfo>
    {
        public BattleReceiveFilter() : base(1)
        {

        }
         
        protected override int GetBodyLengthFromHeader(byte[] header, int offset, int length)
        {
            return (int)header[offset + 4] * 256 + (int)header[offset + 5];
        }

        protected override BinaryRequestInfo ResolveRequestInfo(ArraySegment<byte> header, byte[] bodyBuffer, int offset, int length)
        {
            return new BinaryRequestInfo(Encoding.UTF8.GetString(header.Array, header.Offset, 4), bodyBuffer.CloneRange(offset, length));
        }

        public override BinaryRequestInfo Filter(byte[] readBuffer, int offset, int length, bool toBeCopied, out int rest)
        {
            rest = 0;

            var requestInfo = new BinaryRequestInfo(string.Empty, readBuffer.CloneRange(offset, length));

            return requestInfo;
        }
    }
}
