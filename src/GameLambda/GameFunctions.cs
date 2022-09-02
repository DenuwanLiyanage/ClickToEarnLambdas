using System.Threading.Tasks;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;

namespace GameLambda;

public class GameFunctions
{
    private readonly DynamoDBContext _contextDb;

    public GameFunctions(DynamoDBContext contextDb)
    {
        _contextDb = contextDb;
    }

    public async Task<ClickCountSubmitResponse> SubmitClickCount(ClickCountSubmitRequest clickCountSubmitRequest)
    {
        var playersForSession = await GetNoOfPlayersForSession(clickCountSubmitRequest.sessionId);

        var request = new ClickCount
        {
            userId = clickCountSubmitRequest.userId,
            sessionId = clickCountSubmitRequest.sessionId,
            count = clickCountSubmitRequest.count
        };
        var submitResponse = new ClickCountSubmitResponse
        {
            sessionId = clickCountSubmitRequest.sessionId
        };
        switch (playersForSession)
        {
            case 0:
                //todo create a session
                await _contextDb.SaveAsync(request);
                submitResponse.status = "Session Create Successfully";
                break;
            case 2:
                //todo match exist response
                submitResponse.status = "Two players existing for this session";
                break;
            case 1:
                await _contextDb.SaveAsync(request);
                submitResponse.status = "Click count submitted";
                break;
        }

        return submitResponse;
    }

    private async Task<int> GetNoOfPlayersForSession(string sessionId)
    {
        var productsTask = await _contextDb.QueryAsync<ClickCount>(sessionId, new DynamoDBOperationConfig
        {
            ConditionalOperator = ConditionalOperatorValues.And
        }).GetRemainingAsync();

        return productsTask.Count;
    }

    public async Task<GetMatchWinnerResponse> GetWinner(string sessionId)
    {
        var productsTask = await _contextDb.QueryAsync<ClickCount>(sessionId, new DynamoDBOperationConfig
        {
            ConditionalOperator = ConditionalOperatorValues.And
        }).GetRemainingAsync();

        var winner = "";
        var count = 0;
        if (productsTask.Count != 2)
        {
            winner = "Match not ended yet";
            return new GetMatchWinnerResponse
            {
                userId = winner,
                count = count
            };
        }

        var player1Count = productsTask[0].count;
        var player2Count = productsTask[1].count;

        if (player1Count == player2Count)
        {
            winner = "Both";
            count = player1Count;
            return new GetMatchWinnerResponse
            {
                userId = winner,
                count = count
            };
        }

        winner = player1Count > player2Count ? productsTask[0].userId : productsTask[1].userId;
        count = player1Count > player2Count ? player1Count : player2Count;

        var winnerResponse = new GetMatchWinnerResponse
        {
            userId = winner,
            count = count
        };
        return winnerResponse;
    }

    public async Task<VirtualBalance> AddRewards(string userId, decimal rewards)
    {
        var virtualBalance = await _contextDb.LoadAsync<VirtualBalance>(userId);
        var balance = virtualBalance.balance;
        balance += rewards;
        virtualBalance.balance = balance;

        await _contextDb.SaveAsync(virtualBalance);
        return virtualBalance;
    }
}