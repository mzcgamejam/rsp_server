using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Amazon.Lambda.Core;
using CommonProtocol;
using GameDB;

[assembly: LambdaSerializer(typeof(CustomSerializer.LambdaSerializer))]
namespace RankList
{



    public class Function
    {
        public async Task<ResRankList> FunctionHandler(ReqRankList req, ILambdaContext context)
        {
            DBEnv.SetUp();
            var res = new ResRankList
            {
                ResponseType = ResponseType.Success
            };

            using (var db = new DBConnector())
            {
                var query = new StringBuilder();
                query.Append("SELECT * FROM users ORDER BY point DESC ;");

                using (var cursor = await db.ExecuteReaderAsync(query.ToString()))
                {
                    while (cursor.Read())
                    {
                        res.lstRankInfo.Add(new RankInfo {
                            nickname = (string)cursor["nickname"],
                            win = (int)cursor["win"],
                            lose = (int)cursor["loss"],
                            draw = (int)cursor["draw"],
                            point = (int)cursor["point"]
                        });
                    }
                    return res;

                }
            }
            res.ResponseType = ResponseType.Fail;
            return res;
        }
    }
}
