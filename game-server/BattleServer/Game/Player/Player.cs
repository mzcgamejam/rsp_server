using BattleProtocol;
using BattleProtocol.Entities;
using BattleServer.Server;
using CommonType;
using MessagePack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BattleServer.Game
{
    public class Player
    {
        private int _score = 0;
        public BattleSession Session;

        private AttackType _attackType;
        public AttackType AttackType { get { return _attackType; } set { _attackType = value; } } 


        public Player(BattleSession session, PlayerType playerType)
        {
            Session = session;
            session.PlayerType = playerType;
        }

        public void Send(MessageType messageType, byte[] protocol)
        {
            Session.Send(messageType, protocol);
        }

        public PlayerType GetWinner(Player enemy)
        {
            if ((_attackType != AttackType.None && enemy.AttackType == AttackType.None) ||
                (_attackType == AttackType.Scissors && enemy.AttackType == AttackType.Paper) ||
                (_attackType == AttackType.Paper && enemy.AttackType == AttackType.Rock) ||
                (_attackType == AttackType.Rock && enemy.AttackType == AttackType.Scissors))
            {
                return Session.PlayerType;
            }
            else if(_attackType == enemy.AttackType)
            {
                return PlayerType.None;
            }
            else
            {
                return enemy.Session.PlayerType;
            }
        }

        public PlayerType GetWinnerByPoint(Player enemy)
        {
            if (_score > enemy.GetScore())
                return PlayerType.Player1;
            else if (_score < enemy.GetScore())
                return PlayerType.Player2;
            else
                return PlayerType.None;
        }

        public void AddScore()
        {
            _score++;
        }

        public bool IsWin()
        {
            return _score == 3;
        }

        public int GetScore()
        {
            return _score;
        }
            
    }
}
