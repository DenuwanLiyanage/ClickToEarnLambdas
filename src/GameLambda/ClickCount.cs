using Amazon.DynamoDBv2.DataModel;

namespace GameLambda;

[DynamoDBTable("user-click-count-table")]
public class ClickCount
{
    [DynamoDBHashKey,DynamoDBProperty("session-id")] 
    public string sessionId { get; set; }
    
    [DynamoDBProperty("user-id")]
    public string userId { get; set; }

    [DynamoDBProperty("count")] 
    public int count { get; set; }
}