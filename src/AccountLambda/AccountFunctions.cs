using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Amazon.DynamoDBv2.DataModel;
using Amazon.SimpleEmail;
using Amazon.SimpleEmail.Model;

namespace AccountLambda;

public interface ICountFunctions
{
}

public class AccountFunctions : ICountFunctions
{
    private readonly DynamoDBContext _contextDb;
    private readonly AmazonSimpleEmailServiceClient _emailClient;


    public AccountFunctions(AmazonSimpleEmailServiceClient emailClient, DynamoDBContext contextDb)
    {
        _contextDb = contextDb;
        _emailClient = emailClient;
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

    // public async Task<WalletsVerificationData> VerifyWalletAddressAsync(string walletAddress)
    // {
    //     var wallets = await _contextDb.QueryAsync<WalletsVerificationData>(walletAddress, new DynamoDBOperationConfig()
    //     {
    //         IndexName = "wallet-address-index",
    //         ConsistentRead = false,
    //     }).GetRemainingAsync();
    //
    //     //todo implement with signature
    //     var wallet = wallets.FirstOrDefault();
    //     if (wallet != null) wallet.isVerified = true;
    //
    //     await _contextDb.SaveAsync(wallet);
    //     return wallet;
    // }

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

    public async Task<string> SendAnEmailAsync(string body)
    {
        var sendRequest = new SendEmailRequest
        {
            Source = "denuwan.metaroon@gmail.com",
            Destination = new Destination
            {
                ToAddresses =
                    new List<string> { "denuwan.sds@gmail.com" }
            },
            Message = new Message
            {
                Subject = new Content("Verify wallet address"),
                Body = new Body
                {
                    Text = new Content
                    {
                        Charset = "UTF-8",
                        Data = body
                    }
                }
            },
        };
        try
        {
            Console.WriteLine("Sending email using Amazon SES...");
            var response = await _emailClient.SendEmailAsync(sendRequest);

            Console.WriteLine("The email was sent successfully." + response.HttpStatusCode);
            return "Success";
        }
        catch (Exception ex)
        {
            Console.WriteLine("The email was not sent.");
            Console.WriteLine("Error message: " + ex.Message);
            return "Failed";
        }
    }
}