using System;
using Animation.Configuration;
using Animation.Introducao;
using Animation.Start;
using Aplication.ExternalServices;
using Domain.Enum;
using Domain.Class;
using Animation.Monitoring;
using Domain.Record;

namespace Aplication.InternalServices
{
    public class Runner : IRunner
    {
        private readonly IStarterAnimation _starterAnimation;
        private readonly IConfiguration _configuration;
        private readonly IIntroducao _introducao;
        private readonly ICalculation _calculation;
        private readonly IDecision _decision;
        private readonly IOperationResultService _operationResultService;
        private readonly ITelegramMessage _telegramMessage;
        private readonly IMonitoring _monitoring;

        private BinanceInfo _binanceInfo;
        private BinanceOperation _binanceOperation;

        public Runner(IStarterAnimation starterAnimation,
            IConfiguration configuration,
            IIntroducao introducao,
            ICalculation calculation,
            IDecision decision,
            IOperationResultService operationResultService,
            ITelegramMessage telegramMessage,
            IMonitoring monitoring)
        {
            _starterAnimation = starterAnimation;
            _configuration = configuration;
            _introducao = introducao;
            _calculation = calculation;
            _decision = decision;
            _operationResultService = operationResultService;
            _telegramMessage = telegramMessage;
            _monitoring = monitoring;
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

            while (true)
            {
                if (!_monitoring.IsPaused)
                {
                    var operationResult = await ExecuteTradingCycle(config);
                    _monitoring.UpdateData(operationResult);
                }

                await Task.Delay(TimeSpan.FromSeconds(1));
            }
        }

        private async Task<OperationResult> ExecuteTradingCycle(ConfigurationResult configuration)
        {
            try
            {
                var btcPrice = await _binanceInfo.GetBTCPrice();
                var klineInterval = configuration.klineInterval switch
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

                var candles = await _calculation.DefinirQuantidadeDeCandles(klineInterval, 14);
                var prices = await _binanceInfo.GetHistoricalPrices("BTCUSDT", (Binance.Net.Enums.KlineInterval)klineInterval, 50);
                var rsi = _calculation.GetRSI(prices, 14);
                var decision = await _decision.Analise(rsi, configuration);
                var executed = await _binanceOperation.Operate(decision, await _binanceInfo.GetBalance());
                var usdtBalance = await _binanceInfo.GetBalance();

                var operationResult = new OperationResult()
                {
                    BitcoinPrice = btcPrice,
                    BuyRsi = configuration.BuyRsi,
                    SellRsi = configuration.SellRsi,
                    Executed = executed,
                    MarketRsi = rsi,
                    StopLoss = configuration.StopLoss,
                    TakeProfit = configuration.TakeProfit,
                    OperationDate = DateTime.Now,
                    UsdtBalance = usdtBalance,
                    KlineInterval = configuration.klineInterval
                };

                if (executed)
                {
                    _operationResultService.Save(operationResult);
                    await _telegramMessage.SendMessage(
                        configuration.TelegramKey,
                        configuration.TelegramChatId,
                        @$"Operação realizada. Tipo: {decision.ToString()}, Saldo Usdt: {usdtBalance} 🤑");
                }

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
    }
}