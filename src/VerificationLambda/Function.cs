using System;
using System.Threading.Tasks;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using Newtonsoft.Json;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace VerificationLambda
{
    public class Function
    {
        public async Task<APIGatewayProxyResponse> FunctionHandler(APIGatewayProxyRequest input, ILambdaContext context)
        {
            Console.WriteLine(JsonConvert.SerializeObject(input));
            var emailFunctions = new VerificationFunctions(new DynamoDBContext(new AmazonDynamoDBClient()));

            Response response = new Response();

            var walletAddress = input.QueryStringParameters["walletAddress"];
            var resp = await emailFunctions.VerifyWalletAddressAsync(walletAddress);

            return new APIGatewayProxyResponse
            {
                StatusCode = 200,
                Body = JsonConvert.SerializeObject(resp)
            };
        }
    }
}