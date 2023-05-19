using Amazon.Lambda.Core;
using EchoBot.AWS.Lambda.Handlers.MessageHandlers;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace EchoBot.AWS.Lambda.Handlers;

public static class UpdateHandler
{
    public static async Task HandleUpdateAsync(ILambdaLogger logger, ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
    {
        var handler = update.Type switch
        {
            UpdateType.Message => OnMessageReceived(botClient, update.Message!, cancellationToken),
            _ => Task.CompletedTask
        };
        try
        {
            await handler;
        }
        catch (Exception e)
        {
            await ErrorHandler.HandleErrorAsync(logger, botClient, e, cancellationToken);
        }
    }

    private static async Task OnMessageReceived(ITelegramBotClient botClient, Message message, CancellationToken cancellationToken)
    {
        var handler = message.Chat.Type switch
        {
            ChatType.Group => GroupMessageHandler.HandleAsync(botClient, message!, cancellationToken),
            ChatType.Supergroup => GroupMessageHandler.HandleAsync(botClient, message!, cancellationToken),
            ChatType.Private => PrivateMessageHandler.HandleAsync(botClient, message!, cancellationToken),
            _ => Task.CompletedTask
        };

        await handler;
    }
}