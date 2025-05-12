using System;
using Animation.Configuration;
using Animation.Introducao;
using Animation.Start;
using Aplication.ExternalServices;
using Domain.Enum;
using Domain.Class;
using Animation.Monitoring;
using Domain.Record;
using Microsoft.Extensions.Configuration;
using System.Security.Cryptography;

namespace Aplication.InternalServices
{
    public class Runner : IRunner
    {
        private readonly IStarterAnimation _starterAnimation;
        private readonly Animation.Configuration.IConfiguration _configuration;
        private readonly IIntroducao _introducao;
        private readonly ICalculation _calculation;
        private readonly IDecision _decision;
        private readonly IOperationResultService _operationResultService;
        private readonly ITelegramMessage _telegramMessage;
        private readonly IMonitoring _monitoring;
        private readonly IVerifyConditionsToRun _verifyConditionsToRun;

        private BinanceInfo _binanceInfo;
        private BinanceOperation _binanceOperation;

        public Runner(IStarterAnimation starterAnimation,
            Animation.Configuration.IConfiguration configuration,
            IIntroducao introducao,
            ICalculation calculation,
            IDecision decision,
            IOperationResultService operationResultService,
            ITelegramMessage telegramMessage,
            IMonitoring monitoring,
            IVerifyConditionsToRun verifyConditionsToRun)
        {
            _starterAnimation = starterAnimation;
            _configuration = configuration;
            _introducao = introducao;
            _calculation = calculation;
            _decision = decision;
            _operationResultService = operationResultService;
            _telegramMessage = telegramMessage;
            _monitoring = monitoring;
            _verifyConditionsToRun = verifyConditionsToRun;
        }

        public async Task Run()
        {
            _starterAnimation.PlayAnimation();
            _introducao.ExibirTutorial();

            var config = _configuration.GetConfiguration();
            _binanceInfo = new BinanceInfo(config.BinanceKey, config.BinanceSecret);
            _binanceOperation = new BinanceOperation(config.BinanceKey, config.BinanceSecret);

            _operationResultService.CreateFile();
            _monitoring.Initialize();
            var isAbleToStart = await _verifyConditionsToRun.StartVerification(config);

            while (true && isAbleToStart.isAbleToStart)
            {
                if (!_monitoring.IsPaused)
                {
                    var operationResult = await ExecuteTradingCycle(config);
                    _monitoring.UpdateData(operationResult);
                }

                await Task.Delay(TimeSpan.FromSeconds(10));
            }
        }

        private async Task<OperationResult> ExecuteTradingCycle(ConfigurationResult configuration)
        {
            try
            {
                var btcPrice = await _binanceInfo.GetBTCPrice();
                var klineInterval = GetKlineInterval(configuration);
                var candles = await _calculation.DefinirQuantidadeDeCandles(klineInterval, 14);
                var prices = await _binanceInfo.GetHistoricalPrices("BTCUSDT", (Binance.Net.Enums.KlineInterval)klineInterval, 50);
                var rsi = _calculation.GetRSI(prices, 14);
                var decision = await _decision.AnalyzeMarket(rsi, configuration,btcPrice);
                var executed = await _binanceOperation.Operate(decision, await _binanceInfo.GetBalance());
                var usdtBalance = await _binanceInfo.GetBalance();
                var operationResult = GetOperationResult(btcPrice, configuration, executed, rsi, decision,usdtBalance);
                await SaveData(executed, operationResult, configuration, decision,  usdtBalance);

                return operationResult;
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Erro no ciclo: {ex.Message}");
                Console.ResetColor();

                // Retorna um resultado vazio em caso de erro
                return new OperationResult
                {
                    OperationDate = DateTime.Now
                };
            }
        }

        private KlineIntervalEnum GetKlineInterval(ConfigurationResult config)
            => config.klineInterval switch
            {
                1 => KlineIntervalEnum.OneSecond,
                2 => KlineIntervalEnum.OneMinute,
                3 => KlineIntervalEnum.ThreeMinutes,
                4 => KlineIntervalEnum.FiveMinutes,
                5 => KlineIntervalEnum.FifteenMinutes,
                6 => KlineIntervalEnum.ThirtyMinutes,
                7 => KlineIntervalEnum.OneHour,
                8 => KlineIntervalEnum.TwoHour,
                9 => KlineIntervalEnum.FourHour,
                10 => KlineIntervalEnum.SixHour,
                11 => KlineIntervalEnum.EightHour,
                12 => KlineIntervalEnum.TwelveHour,
                13 => KlineIntervalEnum.OneDay,
                14 => KlineIntervalEnum.ThreeDay,
                15 => KlineIntervalEnum.OneWeek,
                16 => KlineIntervalEnum.OneMonth,
                _ => KlineIntervalEnum.FourHour 
            };

        private OperationResult GetOperationResult(decimal btcPrice, ConfigurationResult configuration, bool executed, decimal rsi, DecisionEnum decision, decimal usdtBalance) 
            => new OperationResult()
            {
                BitcoinPrice = btcPrice,
                BuyRsi = configuration.BuyRsi,
                SellRsi = configuration.SellRsi,
                Executed = executed,
                MarketRsi = rsi,
                Decision = (int)decision,
                StopLoss = configuration.StopLoss,
                TakeProfit = configuration.TakeProfit,
                OperationDate = DateTime.Now,
                UsdtBalance = usdtBalance,
                KlineInterval = configuration.klineInterval
            };

        private async Task SaveData(bool executed, OperationResult operationResult, ConfigurationResult configuration, DecisionEnum decision, decimal usdtBalance)
        {
            if (executed)
            {
                await _operationResultService.Save(operationResult);
                await Notify(configuration, decision, usdtBalance);
            }
        }

        private async Task Notify(ConfigurationResult configuration, DecisionEnum decision, decimal usdtBalance)
        {
            await _telegramMessage.SendMessage(
                    configuration.TelegramKey,
                    configuration.TelegramChatId,
                    @$"Operação realizada. Tipo: {decision.ToString()}, Saldo Usdt: {usdtBalance} 🤑");
        }
    }
}