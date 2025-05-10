using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Enum;

namespace Aplication.InternalServices
{
    public interface ICalculation
    {
        decimal GetRSI(List<decimal> precos, int periodo = 14);
        Task<int> DefinirQuantidadeDeCandles(KlineIntervalEnum intervalo, int periodo = 14);
    }
}
