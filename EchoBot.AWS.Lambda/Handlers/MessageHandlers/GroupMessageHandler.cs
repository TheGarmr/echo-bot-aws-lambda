using Telegram.Bot;
using Telegram.Bot.Types;

namespace EchoBot.AWS.Lambda.Handlers.MessageHandlers;

public static class GroupMessageHandler
{
    public static async Task HandleAsync(ITelegramBotClient botClient, Message message, CancellationToken cancellationToken)
    {
        var response = $"Echo from group: {message.Text}";
        await botClient.SendTextMessageAsync(message.Chat.Id, response, cancellationToken: cancellationToken);
    }
}