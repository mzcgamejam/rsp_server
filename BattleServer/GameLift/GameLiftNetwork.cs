using Aws.GameLift.Server;
using Aws.GameLift.Server.Model;
using System;
using System.Collections.Generic;
using static BattleTimer.BattleTimer;

namespace BattleServer.GameLift
{
    public class GameLiftNetwork
    {
        //private Timer _timer = new Timer(1, false, WaitProcessEnding);
        public void Init(int port)
        {
            var processParams = new ProcessParameters(OnGameSession,
                OnGameSessionUpdate,
                OnProcessTerminate,
                OnHealthCheck,
                port,
                new LogParameters(new List<string>()
                {
                    "C:\\Game\\game_server_log.txt"
                }));

            if (GameLiftServerAPI.InitSDK().Success)
            {
                if (GameLiftServerAPI.ProcessReady(processParams).Success)
                {
                    Console.WriteLine("ProcessReady Success");
                }
            }
        }

        public void Ending()
        {
            var processEndingOutcome = GameLiftServerAPI.ProcessEnding();

            if (processEndingOutcome.Success)
                Console.WriteLine("ProcessEnding success.");
            else
                Console.WriteLine("ProcessEnding failure : " + processEndingOutcome.Error.ToString());

            //_timer.Start();
            Server.BattleServer.Instance.Dispose();
            Environment.Exit(0);
        }

        public void OnGameSession(GameSession gameSession)
        {
            // game-specific tasks when starting a new game session, such as loading map
            // When ready to receive players
            var activateGameSessionOutcome = GameLiftServerAPI.ActivateGameSession();
            if (activateGameSessionOutcome.Success)
            {
                
            }
        }
        public void OnProcessTerminate()
        {
            // game-specific tasks required to gracefully shut down a game session,
            // such as notifying players, preserving game state data, and other cleanup
            Console.WriteLine("OnProcessTerminated");

            Ending();
        }

        public bool OnHealthCheck()
        {
            bool isHealthy = true;
            // complete health evaluation within 60 seconds and set health
            return isHealthy;
        }

        //https://docs.aws.amazon.com/ko_kr/gamelift/latest/apireference/API_UpdateGameSession.html
        public void OnGameSessionUpdate(UpdateGameSession updateGameSession)
        {
        }
    }
}
