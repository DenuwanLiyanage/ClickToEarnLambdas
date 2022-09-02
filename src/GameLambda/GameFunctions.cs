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

    public async Task<string> GetWinner(string sessionId)
    {
        var productsTask = await _contextDb.QueryAsync<ClickCount>(sessionId, new DynamoDBOperationConfig
        {
            ConditionalOperator = ConditionalOperatorValues.And
        }).GetRemainingAsync();

        var winner = "";
        if (productsTask.Count != 2)
        {
            winner = "Match not ended yet";
            return winner;
        }

        var player1Count = productsTask[0].count;
        var player2Count = productsTask[1].count;

        if (player1Count == player2Count)
        {
            winner = "Both";
            return winner;
        }

        winner = player1Count > player2Count ? productsTask[0].userId : productsTask[1].userId;
        return winner;
    }
}