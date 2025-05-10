using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Record
{
    public record struct ConfigurationResult(
        string BinanceKey,
        string BinanceSecret,
        string TelegramKey,
        string TelegramChatId,
        int klineInterval,
        int BuyRsi,
        int SellRsi,
        int StopLoss,
        int TakeProfit
    );
}
