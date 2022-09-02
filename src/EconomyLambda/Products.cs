using Amazon.DynamoDBv2.DataModel;

namespace EconomyLambda;

[DynamoDBTable("product-table")]
public class Products
{
    [DynamoDBHashKey, DynamoDBProperty("sku")]

    public string sku { get; set; }
    
    [DynamoDBProperty("user-id")] 
    public string userId { get; set; }

    [DynamoDBProperty("price")] 
    public decimal price { get; set; }
}