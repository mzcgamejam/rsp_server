using Aws.GameLift.Server;
using BattleServer.Server;
using CommonType;
using System;
using System.Collections.Concurrent;
using System.IO;

namespace BattleServer.Game
{
    public class RoomManager
    {
        private static ConcurrentDictionary<string, Room> _rooms = new ConcurrentDictionary<string, Room>();

        public static void BattleEnter(BattleSession session)
        {
            if (true == _rooms.TryGetValue(session.GameSessionId, out var room))
            {
                room.EnterRoom(session);
            }
            else
            {
                CreateRoom(session);
            }
        }

        private static bool CreateRoom(BattleSession session)
        {
            var room = new Room();
            room.AddPlayer(session);
            GameLiftServerAPI.AcceptPlayerSession(session.PlayerSessionId);
            return _rooms.TryAdd(session.GameSessionId, room);
        }

        public static void BattleAttack(string gameSessionId, PlayerType playerType, AttackType attackType)
        {
            if (true == _rooms.TryGetValue(gameSessionId, out var room))
            {
                room.BattleAttack(playerType, attackType);
            }
        }

        public static void BattleProvoke(string gameSessionId, PlayerType playerType, int provokeType)
        {
            if (true == _rooms.TryGetValue(gameSessionId, out var room))
            {
                room.BattleProvoke(playerType, provokeType);
            }
        }

        public static bool RemoveRoom(BattleSession session)
        {
            if (true == _rooms.TryGetValue(session.GameSessionId, out var room))
            {
                GameLiftServerAPI.RemovePlayerSession(session.PlayerSessionId);
                room.RemovePlayer(session.PlayerType);
                if (room.IsEmptyPlayers())
                {
                    room.Dispose();
                    Console.WriteLine("RemoveRoom");
                    _rooms.TryRemove(session.GameSessionId, out var room2);
                    session.Close();
                    return true;
                }
            }
            return false;
        }
    }
}
