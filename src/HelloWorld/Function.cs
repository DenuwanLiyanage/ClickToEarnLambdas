using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Http;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.Lambda.Core;
using Amazon.Lambda.APIGatewayEvents;
using Newtonsoft.Json;
using JsonSerializer = System.Text.Json.JsonSerializer;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace HelloWorld
{

    public class Function
    {

       

        public async Task<APIGatewayProxyResponse> FunctionHandler(APIGatewayProxyRequest input, ILambdaContext context)
        {
            var dynamo = new AmazonDynamoDBClient();
            var wavesGateway = new WavesGateway();
            var economyFunctions = new EconomyFunctions(wavesGateway,new DynamoDBContext(dynamo));

            var response = new Response();
            
            var jsonInput = JsonConvert.DeserializeObject<Request>(input.Body);
            
            switch (jsonInput?.opCodes)
            {
                case OpCodesForLambda.GetWalletWavesBalance:
                {
                    var request = JsonConvert.DeserializeObject<GetWalletWavesBalanceRequest>(jsonInput.values);

                    var walletBalance = economyFunctions.GetWavesBalance(request?.walletAddress);
                    var balanceResponse = new BalanceResponse { walletAddress = request?.walletAddress, balance = walletBalance };
                    response.response = JsonConvert.SerializeObject(balanceResponse);
                    break;
                }
                case OpCodesForLambda.GetWalletVirtualBalance:
                {
                    var request = JsonConvert.DeserializeObject<GetWalletVirtualBalanceRequest>(jsonInput.values);

                    var walletBalance = await economyFunctions.GetVirtualBalance(request?.userId);
                    var balanceResponse = new VirtualBalanceResponse{ userId = walletBalance.userId, balance = walletBalance.balance };
                    response.response = JsonConvert.SerializeObject(balanceResponse);
                    break;
                }
                case OpCodesForLambda.PurchaseItem:
                {
                    var request = JsonConvert.DeserializeObject<PurchaseItemRequest>(jsonInput.values);

                    var resp =await economyFunctions.PurchaseItemUsingVirtualToken(request);
                    response.response = JsonConvert.SerializeObject(resp);
                    break;
                }
                case OpCodesForLambda.GetProductInfoBySkuAndUserId:
                {
                    var request = JsonConvert.DeserializeObject<GetProductInfoRequest>(jsonInput.values);
                    var resp = await economyFunctions.GetPricingInfoBySku(request);
                    response.response = JsonConvert.SerializeObject(resp);
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
