using System;
using System.Threading.Tasks;
using Amazon.Lambda.Core;
using CommonProtocol;
using GameLiftWrapper;

[assembly: LambdaSerializer(typeof(CustomSerializer.LambdaSerializer))]

namespace CreateGame
{
    public class Function
    {
        public async Task<ResCreateGame> FunctionHandler(ReqCreateGame req, ILambdaContext context)
        {
            Console.WriteLine("call");
            var res = new ResCreateGame
            {
                ResponseType = ResponseType.Fail
            };
            var client = new GameLiftBackend();
            Console.WriteLine("client", client);
            var glGameSessionRes = await client.CreateGameSessionAsync(req.GameName);
            Console.WriteLine("gamesessionres", glGameSessionRes);
            
            if (glGameSessionRes == null)
            {
                Console.WriteLine("NULL");
                return res;
            }

            Console.WriteLine("Success");


            res.ResponseType = ResponseType.Success;
            res.MessageType = MessageType.TryMatching;
            res.IpAddress = glGameSessionRes.GameSession.IpAddress;
            res.GameSessionId = glGameSessionRes.GameSession.GameSessionId;
            res.Port = glGameSessionRes.GameSession.Port;

            Console.WriteLine("IP:" + glGameSessionRes.GameSession.IpAddress);
            Console.WriteLine("Port:" + glGameSessionRes.GameSession.Port);
            Console.WriteLine("GameSessionId:" + glGameSessionRes.GameSession.GameSessionId);

            return res;
        }
    }
}
