using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;

namespace EconomyLambda;

public class EconomyFunctions
{

    private readonly WavesGateway _wavesGateway;
    private readonly DynamoDBContext _contextDb;

    public EconomyFunctions(WavesGateway wavesGateway,DynamoDBContext contextDb)
    {
        _wavesGateway = wavesGateway;
        _contextDb = contextDb;
    }

    public  decimal GetWavesBalance(string walletAddress)
    {
        return _wavesGateway.GetAddressWavesBalance(walletAddress);
    }
    
    public async Task<VirtualBalance> GetVirtualBalance(string userId)
    {
        var virtualBalance = await _contextDb.LoadAsync<VirtualBalance>(userId);
        return virtualBalance;
    }

    public async Task<Products> GetPricingInfoBySku(GetProductInfoRequest getProductInfoRequest)
    {
        var queryFilter = new List<ScanCondition>();
        var scanCondition = new ScanCondition("userId", ScanOperator.Equal, getProductInfoRequest.userId);
        queryFilter.Add(scanCondition);
        
        var productsTask = await _contextDb.QueryAsync<Products>(getProductInfoRequest.sku,new DynamoDBOperationConfig
        {
            QueryFilter   = queryFilter,
            ConditionalOperator = ConditionalOperatorValues.And
        }).GetRemainingAsync();
        
        return productsTask.FirstOrDefault();
    }
    public async Task<BuyResponse> PurchaseItemUsingVirtualToken(PurchaseItemRequest purchaseItemRequest)
    {
        var getProductInfoReq = new GetProductInfoRequest
        {
            userId = purchaseItemRequest.userId,
            sku = purchaseItemRequest.sku
        };

        var productInfo = await GetPricingInfoBySku(getProductInfoReq);
        var unitPrice = productInfo?.price;
        var cost = unitPrice * purchaseItemRequest.quantity;

        var userBalance = await GetVirtualBalance(purchaseItemRequest.userId);
        var buyResponse =await BuyItem(userBalance,cost);
        return buyResponse;

    }

    private async Task<BuyResponse> BuyItem(VirtualBalance balance,decimal? cost)
    {
        var buyResponse = new BuyResponse{userId = balance.userId};
        if (balance.balance > cost)
        {
            var newBalance = balance.balance - cost;
            var resp =await UpdateBalance(balance.userId, newBalance);
            buyResponse.balance = newBalance.Value;
            if (resp)
            {
                buyResponse.buyStatus = BuyStatus.Success;
            }
            else
            {
                buyResponse.buyStatus = BuyStatus.Fail;
            }
        }

        return buyResponse;
    }

    private async Task<bool> UpdateBalance(string userId, decimal? newBalance)
    {
        if (newBalance != null)
        {
            var virtualBalance = new VirtualBalance
            {
                userId = userId,
                balance = newBalance.Value
            };

            await _contextDb.SaveAsync(virtualBalance);
            return true;
        }

        return false;
    }
}
