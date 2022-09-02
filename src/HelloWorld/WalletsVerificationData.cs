using Amazon.DynamoDBv2.DataModel;

namespace HelloWorld;

[DynamoDBTable("wallet-verification-info-table")]
public class WalletsVerificationData
{
    [DynamoDBHashKey, DynamoDBProperty("user-id")]

    public string userId { get; set; }

    [DynamoDBGlobalSecondaryIndexHashKey("wallet-address"), DynamoDBProperty("wallet-address")]
    public string wallet { get; set; }

    [DynamoDBProperty("is-verified")] public bool isVerified { get; set; }
}