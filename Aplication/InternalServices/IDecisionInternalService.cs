using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Enum;
using Domain.Record;

namespace Aplication.InternalServices
{
    public interface IDecisionInternalService
    {
        Task<DecisionEnum> AnalyzeMarket(
           decimal marketRsi,
           ConfigurationResultRecord config,
           decimal bitcoinPrice);
    }
}
