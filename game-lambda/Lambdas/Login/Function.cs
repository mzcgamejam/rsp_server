using Amazon.Lambda.Core;
using CommonProtocol;
using GameDB;
using System.Text;
using System.Threading.Tasks;

[assembly: LambdaSerializer(typeof(CustomSerializer.LambdaSerializer))]

namespace Login
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
                        res.UserId = (int)cursor["userId"];
                        return res;
                    }

                }
            }
            res.ResponseType = ResponseType.Fail;
            return res;
        }
    }
}
