using System.Collections.Generic;

namespace CommonProtocol
{
    public class ResSearchRoom : CBaseProtocol
    {
        public ResponseType ResponseType;

        public List<ResRoomInfo> Rooms = new List<ResRoomInfo>();
    }
}
