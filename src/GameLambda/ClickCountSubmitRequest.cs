namespace GameLambda;

public class ClickCountSubmitRequest
{
    public string userId { get; set; }
    
    public string sessionId { get; set; }
    
    public int count { get; set; }
}