using Amazon.Lambda.Core;
using Telegram.Bot;
using Telegram.Bot.Exceptions;

namespace EchoBot.AWS.Lambda.Handlers;

public static class ErrorHandler
{
    public static Task HandleErrorAsync(ILambdaLogger logger, ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
    {
        var errorMessage = exception switch
        {
            ApiRequestException apiRequestException => $"Telegram API Error code: [{apiRequestException.ErrorCode}]. Message: {apiRequestException.Message}",
            _ => exception.ToString()
        };

        logger.LogError(errorMessage);

        return Task.CompletedTask;
    }
}