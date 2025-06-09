using System;
using System.Linq;
using System.Threading.Tasks;
using Data;
using Domain.Class;
using Domain.Enum;
using Domain.Record;

namespace Aplication.InternalServices
{
    public class DecisionInternalService : IDecisionInternalService
    {
        private readonly IOperationResultRepository _operation;

        public DecisionInternalService(IOperationResultRepository operation)
        {
            _operation = operation ?? throw new ArgumentNullException(nameof(operation));
        }

        public async Task<DecisionEnum> AnalyzeMarket(
            decimal marketRsi,
            ConfigurationRecord config,
            decimal bitcoinPrice)
        {
            if (config == null) throw new ArgumentNullException(nameof(config));
            if (bitcoinPrice <= 0) throw new ArgumentException("O preço do Bitcoin deve ser maior que zero");

            var lastOperation = GetLastOperation();
            var decision = DecisionEnum.Keep;

            if (ShouldTriggerStopLoss(lastOperation, bitcoinPrice, config.StopLoss))
            {
                decision = DecisionEnum.Sell;
            }
            else if (ShouldTakeProfit(lastOperation, bitcoinPrice, config.TakeProfit))
            {
                decision = DecisionEnum.Sell;
            }
            else if (ShouldSellBasedOnRsi(marketRsi, config.SellRsi, lastOperation, bitcoinPrice))
            {
                decision = DecisionEnum.Sell;
            }
            else if (ShouldBuyBasedOnRsi(marketRsi, config.BuyRsi))
            {
                decision = DecisionEnum.Buy;
            }

            return decision;
        }

        private OperationResultDomain GetLastOperation()
        {
            var lastOperation = _operation.ReadAll()?
                .OrderByDescending(x => x.OperationDate)
                .FirstOrDefault();

            return lastOperation?.Decision == 2 ? lastOperation : null;
        }

        private bool ShouldTriggerStopLoss(OperationResultDomain lastOperation, decimal currentPrice, int stopLoss)
        {
            if (lastOperation == null || lastOperation.BitcoinPrice == 0)
                return false;

            decimal percentageChange = (currentPrice - lastOperation.BitcoinPrice) / lastOperation.BitcoinPrice * 100;
            return percentageChange <= -stopLoss;
        }

        private bool ShouldTakeProfit(OperationResultDomain lastOperation, decimal currentPrice, int takeProfit)
        {
            if (lastOperation == null || lastOperation.BitcoinPrice == 0)
                return false;

            decimal percentageChange = (currentPrice - lastOperation.BitcoinPrice) / lastOperation.BitcoinPrice * 100;
            return percentageChange >= takeProfit;
        }

        private bool ShouldSellBasedOnRsi(decimal marketRsi, decimal sellRsi, OperationResultDomain lastOperation, decimal currentPrice)
        {
            if (lastOperation == null || lastOperation.BitcoinPrice == 0)
                return false;

            bool isProfitable = currentPrice > lastOperation.BitcoinPrice * 1.01m; 
            return marketRsi > sellRsi && isProfitable;
        }

        private bool ShouldBuyBasedOnRsi(decimal marketRsi, decimal buyRsi)
        {
            return marketRsi < buyRsi;
        }
    }
}