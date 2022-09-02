using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using Amazon.Lambda.Serialization.SystemTextJson;
using Newtonsoft.Json;
using JsonSerializer = System.Text.Json.JsonSerializer;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(DefaultLambdaJsonSerializer))]

namespace GameLambda
{

    public class Function
    {
        public async Task<APIGatewayProxyResponse> FunctionHandler(APIGatewayProxyRequest input, ILambdaContext context)
        {
            var dynamo = new AmazonDynamoDBClient();
            var gameFunctions = new GameFunctions(new DynamoDBContext(dynamo));

            var jsonInput = JsonConvert.DeserializeObject<Request>(input.Body);

            var response = new Response();

            
            switch (jsonInput?.opCodes)
            {
                case OpCodesForLambda.SubmitClickCount:
                {
                    var request = JsonConvert.DeserializeObject<ClickCountSubmitRequest>(jsonInput.values);
                    var clickCountSubmitResponse = await gameFunctions.SubmitClickCount(request);
                    response.response = JsonConvert.SerializeObject(clickCountSubmitResponse);
                    break;
                }
                case OpCodesForLambda.FindWinner:
                {
                    var request = JsonConvert.DeserializeObject<GetMatchWinnerRequest>(jsonInput.values);
                    var clickCountSubmitResponse = await gameFunctions.GetWinner(request?.sessionId);
                    response.response = clickCountSubmitResponse;
                    break;
                }
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return new APIGatewayProxyResponse
            {
                Body = JsonSerializer.Serialize(response),
                StatusCode = 200,
                Headers = new Dictionary<string, string> { { "Content-Type", "application/json" } }
            };
        }
    }
}
