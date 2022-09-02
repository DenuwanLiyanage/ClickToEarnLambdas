using System.Linq;
using System.Threading.Tasks;
using Amazon.DynamoDBv2.DataModel;

namespace VerificationLambda;

public interface IVerificationFunctions
{
}

public class VerificationFunctions : IVerificationFunctions
{
    private readonly DynamoDBContext _contextDb;

    public VerificationFunctions(DynamoDBContext contextDb)
    {
        _contextDb = contextDb;
    }


    public async Task<WalletsVerificationData> VerifyWalletAddressAsync(string walletAddress)
    {
        var wallets = await _contextDb.QueryAsync<WalletsVerificationData>(walletAddress, new DynamoDBOperationConfig()
        {
            IndexName = "wallet-address-index",
            ConsistentRead = false,
        }).GetRemainingAsync();

        //todo implement with signature
        var wallet = wallets.FirstOrDefault();
        if (wallet != null) wallet.isVerified = true;

        await _contextDb.SaveAsync(wallet);
        return wallet;
    }
}