using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Enum;
using Domain.Record;

namespace Aplication.InternalServices
{
    public class Decision : IDecision
    {
        public async Task<DecisionEnum> Analise(decimal marketRsi, ConfigurationResult config)
        {
            var decision = DecisionEnum.Keep;
            
            if(marketRsi > config.SellRsi)
            {
                decision = DecisionEnum.Sell;
            }

            if (marketRsi < config.BuyRsi)
            {
                decision = DecisionEnum.Buy;
            }

            return decision;
        }
    }
}
