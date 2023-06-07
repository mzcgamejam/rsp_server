using System;
using System.Collections.Generic;
using System.Text;

namespace CommonProtocol
{
    public enum MessageType
    {
        // WebServer
        AccountJoin = 0,
        Login,
        CreateGame,
        CreatePlayer,
        SearchGame,
        EnterRoom,
        TryMatching,
        TryMatchingCancel,
        GetMatchingStatus
    }
}
