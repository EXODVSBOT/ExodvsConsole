using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;

namespace Aplication.ExternalServices
{
    public class TelegramMessageExternalService : ITelegramMessageExternalService
    {
        public async Task SendMessage(string token, string chatId, string message)
        {
            if (token == String.Empty || chatId == string.Empty) return;

            var botClient = new TelegramBotClient(token);
            await botClient.SendMessage(
                    chatId: chatId,
                    text: message
                );
        }
    }
}
