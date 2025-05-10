using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Enum;

namespace Aplication.ExternalServices
{
    public interface IBinanceOperation
    {
        Task<bool> Buy(decimal usdtBalance);
        Task<bool> Sell();
        Task<bool> Operate(DecisionEnum decision, decimal ballance);
    }
}
