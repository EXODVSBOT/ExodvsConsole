using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Binance.Net.Enums;

namespace Aplication.ExternalServices
{
    public interface IBinanceInfoExternalService
    {
        Task<List<decimal>> GetHistoricalPrices(string symbol, KlineInterval interval, int limit);
        Task<decimal> GetBalance();
        Task<decimal> GetBTCPrice();
        Task<bool> VerifyBinanceKeys();
    }
}
