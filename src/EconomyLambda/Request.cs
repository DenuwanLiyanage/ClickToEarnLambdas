namespace EconomyLambda;

public class Request
{
    public OpCodesForLambda opCodes { get; set; }
    public string values { get; set; }

    public override string ToString()
    {
        return "Request( \n" +
               $"opCode = {opCodes} ) \n" +
               $"values = {values} )";
    }
}