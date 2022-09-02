using System;
using System.Threading.Tasks;
using Amazon;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using Amazon.Lambda.Serialization.SystemTextJson;
using Amazon.SimpleEmail;
using Newtonsoft.Json;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(DefaultLambdaJsonSerializer))]

namespace AccountLambda
{
    public class Function
    {
        public async Task<APIGatewayProxyResponse> FunctionHandler(APIGatewayProxyRequest input, ILambdaContext context)
        {
            var dynamo = new AmazonDynamoDBClient();
            var accountFunctions = new AccountFunctions(new AmazonSimpleEmailServiceClient(RegionEndpoint.USWest2),
                new DynamoDBContext(dynamo));
            Response response = new Response();

            var jsonInput = JsonConvert.DeserializeObject<Request>(input.Body);

            switch (jsonInput?.opCodes)
            {
                case OpCodesForLambda.GetWalletsByUserId:
                {
                    var request = JsonConvert.DeserializeObject<GetWalletsRequest>(jsonInput.values);

                    var accounts = await accountFunctions.GetWalletsByUserIdAsync(request?.userId);
                    response = new Response { userId = accounts.userId, wallets = accounts.wallets };
                    break;
                }
                case OpCodesForLambda.AddWalletToUser:
                {
                    var request = JsonConvert.DeserializeObject<AddWalletRequest>(jsonInput.values);
                    await accountFunctions.AddNewWalletToAccountAsync(request?.userId, request?.walletAddress);
                    var accounts = await accountFunctions.GetWalletsByUserIdAsync(request?.userId);
                    var body =
                        $"https://hjjujamzwa.execute-api.us-west-2.amazonaws.com/test-stage/verify?userId={request?.userId}&walletAddress={request?.walletAddress}";
                    var emailResponse = await accountFunctions.SendAnEmailAsync(body);
                    Console.WriteLine(emailResponse);
                    response = new Response { userId = accounts.userId, wallets = accounts.wallets };
                    break;
                }
                case OpCodesForLambda.GetUnverifiedWallets:
                {
                    var request = JsonConvert.DeserializeObject<GetWalletsRequest>(jsonInput.values);
                    var unVerifiedWallets = await accountFunctions.GetUnVerifiedWalletsOfUser(request?.userId);
                    response = new Response { userId = unVerifiedWallets.userId, wallets = unVerifiedWallets.wallets };
                    break;
                }

                default:
                    throw new ArgumentOutOfRangeException();
            }


            LambdaLogger.Log("output: " + JsonConvert.SerializeObject(response));
            return new APIGatewayProxyResponse
            {
                StatusCode = 200,
                Body = JsonConvert.SerializeObject(response)
            };
        }
    }
}