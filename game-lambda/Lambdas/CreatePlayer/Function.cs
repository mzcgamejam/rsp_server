using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amazon.GameLift.Model;
using Amazon.Lambda.Core;
using CommonProtocol;
using GameLiftWrapper;

[assembly: LambdaSerializer(typeof(CustomSerializer.LambdaSerializer))]

namespace CreatePlayer
{
    public class Function
    {
        public async Task<ResCreatePlayer> FunctionHandler(ReqCreatePlayer req, ILambdaContext context)
        {
            var res = new ResCreatePlayer();
            var client = new GameLiftBackend();

            var isGetSessionSuccess = true;
            CreatePlayerSessionResponse glPlayerSessionRes = null;
            for (int i = 0; i < 50; i++)
            {
                try
                {
                    glPlayerSessionRes = await client.CreatePlayerSessionAsync(req.GameSessionId, req.UserId);
                    res.MessageType = MessageType.TryMatching;
                    res.IpAddress = glPlayerSessionRes.PlayerSession.IpAddress;
                    res.PlayerSessionId = glPlayerSessionRes.PlayerSession.PlayerSessionId;
                    res.Port = glPlayerSessionRes.PlayerSession.Port;

                    isGetSessionSuccess = true;

                }
                catch (InvalidGameSessionStatusException e)
                {
                    isGetSessionSuccess = false;
                    continue;
                }
                catch (GameSessionFullException e)
                {
                    isGetSessionSuccess = false;
                    continue;
                }
                catch (Exception e)
                {
                    isGetSessionSuccess = false;
                    continue;
                }

                if (isGetSessionSuccess)
                    break;
            }

            if (glPlayerSessionRes != null)
                res.ResponseType = ResponseType.Success;

            return res;
        }
    }
}
