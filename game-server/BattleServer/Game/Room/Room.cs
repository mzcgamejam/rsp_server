using Amazon;
using Amazon.Runtime;
using Amazon.Runtime.CredentialManagement;
using Amazon.SecurityToken;
using Amazon.SecurityToken.Model;
using Amazon.SQS;
using Amazon.SQS.Model;
using Aws.GameLift.Server;
using BattleProtocol;
using BattleProtocol.Entities;
using BattleServer.GameLift;
using BattleServer.Server;
using CommonType;
using MessagePack;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using static BattleTimer.BattleTimer;
using BattleServer.Config;

namespace BattleServer.Game
{
    public class Room : IDisposable
    {
        private int _round = 1;
        private int _roundSecond = 5 * 2;
        private object _lock = new object();
        private BattleProgress _battleProgress = new BattleProgress();
        private Timer _roundTimer;
        private Dictionary<PlayerType, Player> _players = new Dictionary<PlayerType, Player>();

        public Room()
        {
            _roundTimer = new Timer(_roundSecond * 1000, false, EnqueueRoundEnd);
        }

        private void EnqueueEnd(PlayerType winner, bool isRetreat)
        {
            _battleProgress.EnqueueEnd(new BattleActionEnd(BattleEnd, winner, isRetreat));
        }

        private void EnqueueRoundEnd()
        {
            _battleProgress.EnqueueRoundEnd(new BattleActionRoundEnd(RoundEnd, GetWinner(), false));
        }

        private void BattleEnd(PlayerType winPlayer, bool isRetreat)
        {
            BattleResultSendToSQS(winPlayer);
            SendAll(MessageType.BattleResult, MessagePackSerializer.Serialize(new ProtoBattleResult
            {
                Msg = MessageType.BattleResult,
                Winner = winPlayer,
                IsBattleEnd = true
            }));

            foreach (var player in _players)
            {
                GameLiftServerAPI.RemovePlayerSession(player.Value.Session.PlayerSessionId);
            }

            RemoveRooms();
            var gamelift = new GameLiftNetwork();
            gamelift.Ending();
        }

        private void RoundEnd(PlayerType winPlayer, bool isRetreat)
        {
            
            if (winPlayer != PlayerType.None)
            {
                _players[winPlayer].AddScore();
                Console.WriteLine("Round");
                if (_players[winPlayer].IsWin())
                {
                    Console.WriteLine("Round Win");
                    BattleEnd(_players[PlayerType.Player1].GetWinnerByPoint(_players[PlayerType.Player2]), false);
                    //EnqueueEnd(winPlayer, false);
                    return;
                }    
            }

            if (_round == 5)
            {
                BattleEnd(_players[PlayerType.Player1].GetWinnerByPoint(_players[PlayerType.Player2]), false);
                //EnqueueEnd(_players[PlayerType.Player1].GetWinnerByPoint(_players[PlayerType.Player2]), false);
                return;
            }

            _round++;

            _players[PlayerType.Player1].AttackType = AttackType.None;
            _players[PlayerType.Player2].AttackType = AttackType.None;

            var roundStartDateTime = DateTime.UtcNow;
            SendAll(MessageType.BattleResult, MessagePackSerializer.Serialize(new ProtoBattleResult
            {
                Msg = MessageType.BattleResult,
                Winner = winPlayer,
                IsBattleEnd = false,
                RoundStartDateTimeTicks = roundStartDateTime.Ticks,
                RoundSecond = _roundSecond
            }));

            _roundTimer.Start();
        }

        public void EnterRoom(BattleSession session)
        {
            lock (_lock)
            {
                if (IsFullPlayers())
                {
                    session.Send(BattleProtocol.MessageType.BattleEnterResult,
                        MessagePackSerializer.Serialize(new ProtoBattleEnterResult
                        {
                            Msg = BattleProtocol.MessageType.BattleEnterResult,
                            ResultType = ResultType.AlreadyFull
                        }));
                }
                else
                {
                    AddPlayer(session);

                    if (IsFullPlayers())
                    {
                        GameStart();
                    }

                    GameLiftServerAPI.AcceptPlayerSession(session.PlayerSessionId);
                }
            }
        }

        public bool IsFullPlayers()
        {
            return _players.Count == 2;
        }

        public bool IsEmptyPlayers()
        {
            return _players.Count == 0;
        }

        public void GameStart()
        {
            _battleProgress.TimerStart();
            _roundTimer.Start();
            var gameStartDateTime = DateTime.UtcNow;
            Send(_players[PlayerType.Player1], MessageType.BattleEnterResult,
                MessagePackSerializer.Serialize(new ProtoBattleEnterResult
                {
                    Msg = MessageType.BattleEnterResult,
                    ResultType = ResultType.Success,
                    PlayerType = PlayerType.Player1,
                    GameStartDateTimeTicks = gameStartDateTime.Ticks,
                    GameSecond = _roundSecond
                }));

            Send(_players[PlayerType.Player2], MessageType.BattleEnterResult,
                MessagePackSerializer.Serialize(new ProtoBattleEnterResult
                {
                    Msg = MessageType.BattleEnterResult,
                    ResultType = ResultType.Success,
                    PlayerType = PlayerType.Player2,
                    GameStartDateTimeTicks = gameStartDateTime.Ticks,
                    GameSecond = _roundSecond
                }));
        }

        public void AddPlayer(BattleSession session) 
        {
            if (_players.Count == 0)
                _players.Add(PlayerType.Player1, new Player(session, PlayerType.Player1));
            else if (_players.Count == 1 && _players.ContainsKey(PlayerType.Player1))
                _players.Add(PlayerType.Player2, new Player(session, PlayerType.Player2));
            else if (_players.Count == 1 && _players.ContainsKey(PlayerType.Player2))
                _players.Add(PlayerType.Player1, new Player(session, PlayerType.Player1));
            else
                throw new Exception("Check the add player");
        }

        public void RemovePlayer(PlayerType playerType)
        {
            _players.Remove(playerType);
        }

        private void SendAll(MessageType messageType, byte[] protocol)
        {
            foreach (var player in _players)
            {
                player.Value.Send(messageType, protocol);
            }
        }

        private void Send(Player player, MessageType messageType, byte[] protocol)
        {
            player.Send(messageType, protocol);
        }

        private PlayerType GetWinner()
        {
            return _players[PlayerType.Player1].GetWinner(_players[PlayerType.Player2]);
        }

        private void RemoveRooms()
        {
            if (_players.Count == 0)
                return;

            if (_players.ContainsKey(PlayerType.Player1))
            {
                RoomManager.RemoveRoom(_players?[PlayerType.Player1].Session);
            }

            if (_players.ContainsKey(PlayerType.Player2))
            {
                RoomManager.RemoveRoom(_players?[PlayerType.Player2].Session);
            }
        }

        public void BattleAttack(PlayerType playerType, AttackType attackType)
        {
            _players[playerType].AttackType = attackType;
        }

        public void BattleProvoke(PlayerType playerType, int ProvokeType)
        {
            Send(_players[playerType], MessageType.BattleProvoke, MessagePackSerializer.Serialize(new ProtoBattleProvoke
            {
                Msg = MessageType.BattleProvoke,
                PlayerType = playerType,
                ProvokeType = ProvokeType
            }));
        }

        public void BattleResultSendToSQS(PlayerType winPlayer)
        {   
            var sqsConfig = new AmazonSQSConfig();

            sqsConfig.ServiceURL = ConfigManager.sqsUrl;
            sqsConfig.RegionEndpoint = RegionEndpoint.GetBySystemName(ConfigManager.region);

            var credential = new BasicAWSCredentials(ConfigManager.accessKey, ConfigManager.secretKey);
            var sqsClient = new AmazonSQSClient(credential, sqsConfig);

            SQSClientSendMessage(sqsClient, PlayerType.Player1, winPlayer);
            SQSClientSendMessage(sqsClient, PlayerType.Player2, winPlayer);
        }
        
        private void SQSClientSendMessage(AmazonSQSClient sqsClient, PlayerType player, PlayerType winPlayer)
        {
            using (StreamWriter outputFile = new StreamWriter(@"C:\Game\myserver.txt"))
            {
                try
                {
                   sqsClient.SendMessage(new SendMessageRequest(ConfigManager.sqsUrl,
                   JsonConvert.SerializeObject(new CommonProtocol.SqsBattleResult
                   {
                       UserId = _players[player].Session.UserId,
                       WinType = GetWinType(player, winPlayer)
                   })));
                }
                catch(Exception ex)
                {
                    outputFile.WriteLine(ex.Message);
                    outputFile.WriteLine(ex.ToString());
                }
            }

        }

        private WinType GetWinType(PlayerType player, PlayerType winPlayer)
        {
            return winPlayer == PlayerType.None ? WinType.Draw : 
                (player == winPlayer ? WinType.Win : WinType.Loss);
        }

        private Dictionary<string, MessageAttributeValue> GetMessageAttributes(PlayerType player, PlayerType enemyPlayer, PlayerType winPlayer)
        {
            if (false == _players.ContainsKey(player))
                return null;

            return new Dictionary<string, MessageAttributeValue>
            {
                {
                    "userId", new MessageAttributeValue{
                        DataType = "String",
                        StringValue = _players[player].Session.UserId.ToString()}
                },
                {
                    "winType", new MessageAttributeValue{
                        DataType = "String",
                        StringValue =
                        (winPlayer == player ? 1 :
                        (winPlayer == enemyPlayer ? 2 : 3)).ToString()}
                }
            };
        }

        public void Dispose()
        {
            _battleProgress.TimerStop();
            _battleProgress.Dispose();
        }
    }
}
