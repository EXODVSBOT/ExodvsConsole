using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Binance.Net.Clients;
using Binance.Net.Enums;
using CryptoExchange.Net.Authentication;

namespace Aplication.ExternalServices
{
    public class BinanceInfo : IBinanceInfo
    {
        private readonly string _apiKey;
        private readonly string _apiSecret;
        private readonly BinanceRestClient _client;

        public BinanceInfo(string apiKey, string apiSecret)
        {
            _apiKey = apiKey;
            _apiSecret = apiSecret;
            _client = new BinanceRestClient(options =>
            {
                options.ApiCredentials = new ApiCredentials(apiKey, apiSecret);
            });
        }

        public async Task<bool> VerifyBinanceKeys()
        {
            try
            {
                var result = await _client.SpotApi.Account.GetAccountInfoAsync();

                return result.Success;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<List<decimal>> GetHistoricalPrices(string symbol, KlineInterval interval, int limit)
        {
            try
            {
                var result = await _client.SpotApi.ExchangeData.GetKlinesAsync(symbol, interval, limit: limit);

                if (result.Success)
                {
                    return result.Data.Select(k => k.ClosePrice).ToList();
                }
                else
                {
                    Console.WriteLine($"Erro ao buscar preço do Bitcoin: {result.Error}");
                    return new List<decimal>();
                }
            }
            catch (Exception e)
            {

                Console.WriteLine($"Exceção em GetHistoricalPrices: {e.Message}");
                return new List<decimal>(); // ou relance, se quiser
            }
            
        }

        public async Task<decimal> GetBTCPrice()
        {
            var result = await _client.SpotApi.ExchangeData.GetTickerAsync("BTCUSDT");

            if (result.Success)
            {
                return result.Data.LastPrice;
            }
            else
            {
                Console.WriteLine($"Erro ao buscar preço do Bitcoin: {result.Error}");
                return 0;
            }
        }
        public async Task<decimal> GetBalance()
        {
            try
            {
                var accountInfo = await _client.SpotApi.Account.GetAccountInfoAsync();

                if (accountInfo.Success)
                {
                    var USDT = accountInfo.Data.Balances.FirstOrDefault(x => x.Asset == "USDT");
                    return USDT.Total;
                }

                Console.WriteLine($"Erro ao obter informações da conta: {accountInfo.Error}");
                return 0.00m;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao buscar saldo: {ex.Message}");
                throw;
            }
        }
    }
}
