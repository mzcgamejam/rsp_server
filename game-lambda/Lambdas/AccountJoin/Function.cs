using System;
using System.Text;
using System.Threading.Tasks;
using Amazon.Lambda.Core;
using CommonProtocol;
using GameDB;

[assembly: LambdaSerializer(typeof(CustomSerializer.LambdaSerializer))]

namespace AccountJoin
{
    public class Function
    {
        public async Task<ResAccountJoin> FunctionHandler(ReqAccountJoin req, ILambdaContext context)
        {
            DBEnv.SetUp();
            var res = new ResAccountJoin
            {
                ResponseType = ResponseType.Success
            };

            using (var db = new DBConnector())
            {
                var query = new StringBuilder();
                query.Append("SELECT userId FROM users WHERE nickname ='")
                    .Append(req.Name).Append("';");

                using (var cursor = await db.ExecuteReaderAsync(query.ToString()))
                {
                    if (cursor.Read())
                    {
                        res.ResponseType = ResponseType.DuplicateName;
                        return res;
                    }
                }

                query.Clear();
                query.Append("INSERT INTO users (nickname, win, loss, draw, point) VALUES ('")
                    .Append(req.Name).Append("', 0, 0, 0, 0);");
                await db.ExecuteNonQueryAsync(query.ToString());
                res.UserId = (int)db.LastInsertedId();
            }
            return res;
        }
    }
}
