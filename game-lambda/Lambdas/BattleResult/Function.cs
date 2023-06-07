using System.Text;
using System.Threading.Tasks;
using Amazon.Lambda.Core;
using Amazon.Lambda.SQSEvents;
using CommonProtocol;
using CommonType;
using GameDB;
using Newtonsoft.Json;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace BattleResult
{
    public class Function
    {
        public async Task FunctionHandler(SQSEvent sqsEvent, ILambdaContext context)
        {
            foreach (var record in sqsEvent.Records)
            {
                var res = JsonConvert.DeserializeObject<SqsBattleResult>(record.Body);

                DBEnv.SetUp();
                using (var db = new DBConnector())
                {
                    var query = new StringBuilder();

                    switch(res.WinType)
                    {
                        case WinType.Win:
                            query.Append("UPDATE users SET win = win + 1, point = point + 2 WHERE userId=");
                            break;
                        case WinType.Loss:
                            query.Append("UPDATE users SET loss = loss + 1, `point` = IF(`point` - 1 < 0, 0, `point` - 1) WHERE userId=");
                            break;
                        default:
                            query.Append("UPDATE users SET draw = draw + 1 WHERE userId=");
                            break;
                    }
                    query.Append(res.UserId).Append(";");

                    LambdaLogger.Log("SQL:" + query.ToString());

                    await db.ExecuteNonQueryAsync(query.ToString());
                }
            }
        }
    }
}
