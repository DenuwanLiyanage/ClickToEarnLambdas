using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amazon.DynamoDBv2.DataModel;

namespace HelloWorld;

public interface ICountFunctions
{
}

public class AccountFunctions : ICountFunctions
{
    private readonly DynamoDBContext _contextDb;

    public AccountFunctions(DynamoDBContext contextDb)
    {
        _contextDb = contextDb;
    }

    public async Task<Wallets> GetWalletsByUserIdAsync(string id)
    {
        var wallets = await _contextDb.LoadAsync<Wallets>(id);
        return wallets;
    }

    private async Task SaveNewWalletWithVerificationDetailsAsync(WalletsVerificationData wallets)
    {
        await _contextDb.SaveAsync(wallets);
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

    public async Task AddNewWalletToAccountAsync(string id, string walletAddress)
    {
        var walletsFromTable = await _contextDb.LoadAsync<Wallets>(id);
        walletsFromTable.wallets.Add(walletAddress);
        await _contextDb.SaveAsync(walletsFromTable);

        var verificationDetails = new WalletsVerificationData
        {
            userId = id,
            wallet = walletAddress,
            isVerified = false
        };
        await SaveNewWalletWithVerificationDetailsAsync(verificationDetails);
    }

    public async Task<Wallets> GetUnVerifiedWalletsOfUser(string id)
    {
        var wallets = await _contextDb.QueryAsync<WalletsVerificationData>(id).GetRemainingAsync();

        var unverifiedWallets = new List<string>();
        foreach (var wallet in wallets)
        {
            if (!wallet.isVerified)
            {
                unverifiedWallets.Add(wallet.wallet);
            }
        }

        var newWallets = new Wallets
        {
            userId = id,
            wallets = unverifiedWallets
        };

        return newWallets;
    }
}