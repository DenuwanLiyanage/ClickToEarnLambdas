using Amazon.DynamoDBv2.DataModel;

namespace EconomyLambda;

[DynamoDBTable("virtual-balance-table")]
public class VirtualBalance
{
    [DynamoDBHashKey, DynamoDBProperty("user-id")]

    public string userId { get; set; }

    [DynamoDBProperty("virtual-balance")] 
    public decimal balance { get; set; }
}