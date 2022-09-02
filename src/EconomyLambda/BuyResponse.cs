namespace EconomyLambda;

public class BuyResponse
{
    public string userId { get; set; }
    public BuyStatus buyStatus{ get; set; }
    public decimal balance { get; set; }
}