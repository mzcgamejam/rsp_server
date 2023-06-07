using Amazon.GameLift;
using Amazon.GameLift.Model;
using CommonConfig;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GameLiftWrapper
{
    public class GameLiftBackend
    {
        private AmazonGameLiftClient _client = null;
        private string _fleetId;
        private string _serviceURL;

        public GameLiftBackend()
        {
            _fleetId = EnvironmentReader.Instance.infos.GameLift.FleetId;
            _serviceURL = EnvironmentReader.Instance.infos.GameLift.ServiceURL;
        }

        private AmazonGameLiftClient GetGameLiftClient()
        {
            if (_client != null)
                return _client;

            _client = new AmazonGameLiftClient();

            //if (string.IsNullOrEmpty(_serviceURL))
            //{
            //    _client = new AmazonGameLiftClient();
            //}
            //else
            //{
            //    _client = new AmazonGameLiftClient(new AmazonGameLiftConfig
            //    {
            //        ServiceURL = _serviceURL
            //    });
            //}

            return _client;
        }

        public async Task<CreateGameSessionResponse> CreateGameSessionAsync(string gameName)
        {
            var client = GetGameLiftClient();
            Console.WriteLine(client);
            Console.WriteLine(_fleetId);
            try
            {
                return await client.CreateGameSessionAsync(new CreateGameSessionRequest
                {
                    FleetId = _fleetId,
                    MaximumPlayerSessionCount = 2,
                    GameProperties = new List<GameProperty>
                    {
                        new GameProperty { Key = "Title", Value = gameName.ToString()
                        }
                    }
                });
            }
            catch(Amazon.GameLift.AmazonGameLiftException e)
            {
                Console.WriteLine(e.ToString());
                throw e;
            }
        }

        public async Task<CreatePlayerSessionResponse> CreatePlayerSessionAsync(string gameSessionId, int userId)
        {
            var client = GetGameLiftClient();
            return await client.CreatePlayerSessionAsync(new CreatePlayerSessionRequest
            {
                GameSessionId = gameSessionId,
                PlayerId = userId.ToString(),
                PlayerData = "{}"
            });
        }

        public async Task<IEnumerable<GameSession>> SearchGameSessionsAsync()
        {
            var client = GetGameLiftClient();
            Console.WriteLine(_fleetId);
            var gameSessions = await client.SearchGameSessionsAsync(new SearchGameSessionsRequest
            {
                FleetId = _fleetId,
                FilterExpression = "playerSessionCount = 1"

            });
            return gameSessions.GameSessions;
        }

        public async Task<GetGameSessionLogUrlResponse> GetGameSessionLogUrl(string gameSessionId)
        {
            var client = GetGameLiftClient();
            return await client.GetGameSessionLogUrlAsync(new GetGameSessionLogUrlRequest
            {
                GameSessionId = gameSessionId
            });
        }
    }
}
