namespace AccountLambda;

public class VerifyWalletRequest
{
    public string walletAddress { get; set; }
    public string signature { get; set; }
}