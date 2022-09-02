using System.Collections.Generic;

namespace VerificationLambda;

public class Response
{
    public string userId { get; set; }
    public List<string> wallets { get; set; }
}