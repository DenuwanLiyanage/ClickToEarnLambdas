using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Http;
using System.Text.Json;
using Amazon;
using Amazon.Lambda.Core;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.SimpleEmail;
using Newtonsoft.Json;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace HelloWorld
{

    public class Function
    {
        public async Task<APIGatewayProxyResponse> FunctionHandler(APIGatewayProxyRequest input, ILambdaContext context)
        {
            var emailFunctions = new EmailFunctions(new AmazonSimpleEmailServiceClient(RegionEndpoint.USWest2));

            Response response = new Response();

            var resp = await emailFunctions.SendAnEmailAsync("denuwan.sds@gmail.com", "Verify", "ver");
            
            return new APIGatewayProxyResponse
            {
                StatusCode = 200,
                Body = JsonConvert.SerializeObject(response)
            };
        }
    }
}
