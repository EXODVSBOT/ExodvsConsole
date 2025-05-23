﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Binance.Net.Clients;
using Binance.Net.Enums;
using CryptoExchange.Net.Authentication;
using Domain.Enum;

namespace Aplication.ExternalServices
{
    public class BinanceOperationExternalService : IBinanceOperationExternalService
    {
        private readonly BinanceRestClient _client;
        private const decimal FeeRate = 0.001m;

        public BinanceOperationExternalService(string apiKey, string apiSecret)
        {
            _client = new BinanceRestClient(options =>
            {
                options.ApiCredentials = new ApiCredentials(apiKey, apiSecret);
            });
        }

        public async Task<bool> Operate(DecisionEnum decision, decimal ballance)
        {
            if(ballance < 0.3m || decision == DecisionEnum.Keep)
            {
                return false;
            }

            if (decision == DecisionEnum.Buy) 
            { 
               return await Buy(ballance);
            }

            if (decision == DecisionEnum.Sell)
            {
               return await Sell();
            }

            return false;
        }

        public async Task<bool> Buy(decimal usdtBalance)
        {
            try
            {
                int usdtBalanceInt = (int)Math.Floor(usdtBalance);

                // Obter o preço atual do BTC em USDT
                var ticker = await _client.SpotApi.ExchangeData.GetPriceAsync("BTCUSDT");
                var marketPrice = ticker.Data.Price;

                // Definir o valor mínimo de quantidade permitido para BTC na Binance
                decimal minQuantity = 0.00001m;

                // Calcular a quantidade de BTC a ser comprada, descontando 0,1% da taxa
                decimal quantityToBuy = usdtBalanceInt / marketPrice * (1 - FeeRate);

                // Arredondar para o múltiplo mais próximo do minQuantity
                quantityToBuy = Math.Floor(quantityToBuy / minQuantity) * minQuantity;

                // Verificar se a quantidade calculada atende o limite mínimo
                if (quantityToBuy < minQuantity)
                {
                    return false;
                }

                // Colocar a ordem de compra
                var orderResult = await _client.SpotApi.Trading.PlaceOrderAsync(
                    symbol: "BTCUSDT",
                    side: OrderSide.Buy,
                    type: SpotOrderType.Market,
                    quantity: quantityToBuy
                );

                // Verificar se a ordem foi executada com sucesso
                if (orderResult.Success)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<bool> Sell()
        {
            try
            {
                // Passo 1: Obter informações da conta
                var accountInfo = await _client.SpotApi.Account.GetAccountInfoAsync();

                if (!accountInfo.Success)
                {
                    return false;
                }

                // Passo 2: Obter saldo disponível de BTC
                var btcBalance = accountInfo.Data.Balances.FirstOrDefault(b => b.Asset == "BTC")?.Available ?? 0;

                if (btcBalance <= 0)
                {
                    return false;
                }

                // Definir o valor mínimo de quantidade permitido para venda de BTC
                decimal minQuantity = 0.00001m;

                // Calcular quantidade a vender com 0,1% de desconto para a taxa
                decimal quantityToSell = btcBalance * (1 - FeeRate);

                // Arredondar para o múltiplo mais próximo de minQuantity
                quantityToSell = Math.Floor(quantityToSell / minQuantity) * minQuantity;

                // Verificar se a quantidade calculada atende ao limite mínimo
                if (quantityToSell < minQuantity)
                {
                    return false;
                }

                // Colocar a ordem de venda
                var orderResult = await _client.SpotApi.Trading.PlaceOrderAsync(
                    symbol: "BTCUSDT",
                    side: OrderSide.Sell,
                    type: SpotOrderType.Market,
                    quantity: quantityToSell
                );

                // Verificar se a ordem foi executada com sucesso
                if (orderResult.Success)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}
