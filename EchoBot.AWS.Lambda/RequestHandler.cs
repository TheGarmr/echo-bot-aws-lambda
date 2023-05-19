using System.Net;
using System.Text;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using EchoBot.AWS.Lambda.Handlers;
using Newtonsoft.Json;
using Telegram.Bot;
using Telegram.Bot.Types;

[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace EchoBot.AWS.Lambda;

public class RequestHandler
{
    private const string TokenEnvironmentName = "BOT_TOKEN";

    public async Task<APIGatewayProxyResponse> HandleRequest(APIGatewayProxyRequest request, ILambdaContext context)
    {
        Console.OutputEncoding = Encoding.UTF8;
        if (string.IsNullOrWhiteSpace(request.HttpMethod))
        {
            context.Logger.LogError("HTTP Method is null.");
            return BadRequest("HTTP Method is null.");
        }

        try
        {
            switch (request.HttpMethod.ToUpper())
            {
                case "GET":
                    return await HandleGet(request, context);
                case "POST":
                    return await HandlePost(request, context);
            }
        }
        catch (Exception e)
        {
            context.Logger.LogError(e.Message);
            return BadRequest("Unprocessable http method");
        }

        return BadRequest("Unprocessable http method");
    }

    public async Task<APIGatewayProxyResponse> HandleGet(APIGatewayProxyRequest request, ILambdaContext context)
    {
        var botClient = GetClient();
        var me = await botClient.GetMeAsync();

        return Ok(me);
    }

    public async Task<APIGatewayProxyResponse> HandlePost(APIGatewayProxyRequest request, ILambdaContext context)
    {
        if (string.IsNullOrWhiteSpace(request.Body))
        {
            throw new Exception("HTTP body is null.");
        }

        context.Logger.LogInformation($"Received body: {request.Body}");

        var botClient = GetClient();
        var update = JsonConvert.DeserializeObject<Update>(request.Body);
        await UpdateHandler.HandleUpdateAsync(context.Logger, botClient, update, CancellationToken.None);

        return Ok("Ok");
    }

    private static TelegramBotClient GetClient()
    {
        var token = Environment.GetEnvironmentVariable(TokenEnvironmentName);
        return new TelegramBotClient(token);
    }

    private static APIGatewayProxyResponse Ok(object body)
    {
        var serializedBody = JsonConvert.SerializeObject(body);
        return Ok(serializedBody);
    }

    private static APIGatewayProxyResponse Ok(string body)
    {
        return new APIGatewayProxyResponse
        {
            StatusCode = (int)HttpStatusCode.OK,
            Body = body,
            Headers = new Dictionary<string, string> { { "Content-Type", "application/json; charset=UTF-8" } }
        };
    }

    private static APIGatewayProxyResponse BadRequest(string message)
    {
        var response = new APIGatewayProxyResponse
        {
            StatusCode = (int)HttpStatusCode.BadRequest,
            Body = message,
            Headers = new Dictionary<string, string> { { "Content-Type", "application/json; charset=UTF-8" } }
        };
        return response;
    }
}