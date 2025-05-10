using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Enum;
using Domain.Record;

namespace Aplication.InternalServices
{
    public interface IDecision
    {
        Task<DecisionEnum> Analise(decimal marketRsi, ConfigurationResult config);
    }
}
