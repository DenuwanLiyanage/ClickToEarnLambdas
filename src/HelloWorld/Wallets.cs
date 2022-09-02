using System.Collections.Generic;
using Amazon.DynamoDBv2.DataModel;

namespace HelloWorld;

[DynamoDBTable("wallets-table")]
public class Wallets
{
    [DynamoDBHashKey, DynamoDBProperty("user-id")]

    public string userId { get; set; }

    [DynamoDBProperty("wallets")] public List<string> wallets { get; set; }
}