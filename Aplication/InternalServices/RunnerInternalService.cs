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
    public class RunnerInternalService : IRunnerInternalService
    {
        private readonly IStarterAnimation _starterAnimation;
        private readonly Animation.Configuration.IConfigurationAnimation _configuration;
        private readonly IIntroducaoAnimation _introducao;
        private readonly ICalculationInternalService _calculation;
        private readonly IDecisionInternalService _decision;
        private readonly IOperationResultInternalService _operationResultService;
        private readonly ITelegramMessageExternalService _telegramMessage;
        private readonly IMonitoringAnimation _monitoring;
        private readonly IVerifyConditionsToRunInternalService _verifyConditionsToRun;

        private BinanceInfoExternalService _binanceInfo;
        private BinanceOperationExternalService _binanceOperation;

        public RunnerInternalService(IStarterAnimation starterAnimation,
            Animation.Configuration.IConfigurationAnimation configuration,
            IIntroducaoAnimation introducao,
            ICalculationInternalService calculation,
            IDecisionInternalService decision,
            IOperationResultInternalService operationResultService,
            ITelegramMessageExternalService telegramMessage,
            IMonitoringAnimation monitoring,
            IVerifyConditionsToRunInternalService verifyConditionsToRun)
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
            _binanceInfo = new BinanceInfoExternalService(config.BinanceKey, config.BinanceSecret);
            _binanceOperation = new BinanceOperationExternalService(config.BinanceKey, config.BinanceSecret);

            _operationResultService.CreateFile();
            _monitoring.Initialize();

            while (true)
            {
                var isAbleToStart = await _verifyConditionsToRun.StartVerification(config);

                if (!_monitoring.IsPaused && isAbleToStart.isAbleToStart)
                {
                    var operationResult = await ExecuteTradingCycle(config);
                    _monitoring.UpdateData(operationResult);
                }

                await Task.Delay(TimeSpan.FromSeconds(GetSecondsFromRunInterval(config.RunInterval)));
            }
        }

        private double GetSecondsFromRunInterval(int runInterval)
        {
            return runInterval switch
            {
                1 => 1,       // 1 segundo
                2 => 5,       // 5 segundos
                3 => 10,       // 10 segundos
                4 => 30,      // 30 segundos
                5 => 60,       // 1 minuto (60 segundos)
                6 => 600,      // 10 minutos (600 segundos)
                7 => 3600,    // 1 hora (3600 segundos)
                _ => 1        // padrão: 1 segundo se a opção for inválida
            };
        }

        private async Task<OperationResultDomain> ExecuteTradingCycle(ConfigurationResultRecord configuration)
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
                return new OperationResultDomain
                {
                    OperationDate = DateTime.Now
                };
            }
        }

        private KlineIntervalEnum GetKlineInterval(ConfigurationResultRecord config)
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

        private OperationResultDomain GetOperationResult(decimal btcPrice, ConfigurationResultRecord configuration, bool executed, decimal rsi, DecisionEnum decision, decimal usdtBalance) 
            => new OperationResultDomain()
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

        private async Task SaveData(bool executed, OperationResultDomain operationResult, ConfigurationResultRecord configuration, DecisionEnum decision, decimal usdtBalance)
        {
            if (executed)
            {
                await _operationResultService.Save(operationResult);
                await Notify(configuration, decision, usdtBalance);
            }
        }

        private async Task Notify(ConfigurationResultRecord configuration, DecisionEnum decision, decimal usdtBalance)
        {
            await _telegramMessage.SendMessage(
                    configuration.TelegramKey,
                    configuration.TelegramChatId,
                    @$"Operação realizada. Tipo: {decision.ToString()}, Saldo Usdt: {usdtBalance} 🤑");
        }
    }
}