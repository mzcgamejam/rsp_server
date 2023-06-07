using Amazon.Lambda.Core;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO;

namespace CustomSerializer
{
    public class LambdaSerializer : ILambdaSerializer
    {
        public T Deserialize<T>(Stream requestStream)
        {
            using (var streamReader = new StreamReader(requestStream))
            {
                var read = streamReader.ReadToEnd();
                var jObj = JObject.Parse(read);
                return JsonConvert.DeserializeObject<T>(jObj.ToString());
            }
        }

        public void Serialize<T>(T response, Stream responseStream)
        {
            var writer = new StreamWriter(responseStream);
            writer.Write(JsonConvert.SerializeObject(response));
            writer.Flush();
        }
    }
}
