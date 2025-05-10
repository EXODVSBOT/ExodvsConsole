using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aplication.ExternalServices
{
    public interface ITelegramMessage
    {
        Task SendMessage(string token, string chatId, string message);
    }
}
