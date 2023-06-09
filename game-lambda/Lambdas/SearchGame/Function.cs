using Amazon.Lambda.Core;
using CommonProtocol;
using GameLiftWrapper;
using System;
using System.Threading.Tasks;

[assembly: LambdaSerializer(typeof(CustomSerializer.LambdaSerializer))]

namespace SearchGame
{
    public class Function
    {
        public async Task<ResSearchRoom> FunctionHandler(ReqUserId input, ILambdaContext context)
        {
            var res = new ResSearchRoom
            {
                ResponseType = ResponseType.Fail
            };

            var client = new GameLiftBackend();

            var glGameSessionRes = await client.SearchGameSessionsAsync();
            foreach (var gameSession in glGameSessionRes)
            {
                res.Rooms.Add(new ResRoomInfo
                {
                    GameSessionId = gameSession.GameSessionId,
                    Title = gameSession.GameProperties[0].Value,
                    Ip = gameSession.IpAddress,
                    Port = gameSession.Port
                });
            }

            return res;
        }
    }
}
