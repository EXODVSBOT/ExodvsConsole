using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Enum;

namespace Aplication.InternalServices
{
    public class Calculation : ICalculation
    {
        public decimal GetRSI(List<decimal> precos, int periodo = 14)
        {
            if (precos == null || precos.Count < periodo + 1)
                return 50m; // Valor neutro quando não há dados suficientes

            List<decimal> ganhos = new List<decimal>();
            List<decimal> perdas = new List<decimal>();

            // Cálculo de ganhos e perdas individuais
            for (int i = 1; i < precos.Count; i++)
            {
                decimal variacao = precos[i] - precos[i - 1];
                ganhos.Add(variacao > 0 ? variacao : 0);
                perdas.Add(variacao < 0 ? Math.Abs(variacao) : 0);
            }

            // Média inicial (SMA dos primeiros 'periodo' valores)
            decimal mediaGanhos = ganhos.Take(periodo).Average();
            decimal mediaPerdas = perdas.Take(periodo).Average();

            // Aplicação da EMA nos ganhos e perdas (suavização)
            for (int i = periodo; i < ganhos.Count; i++)
            {
                mediaGanhos = (mediaGanhos * (periodo - 1) + ganhos[i]) / periodo;
                mediaPerdas = (mediaPerdas * (periodo - 1) + perdas[i]) / periodo;
            }

            // Evita divisão por zero
            if (mediaPerdas == 0) return 100m;
            if (mediaGanhos == 0) return 0m;

            decimal rs = mediaGanhos / mediaPerdas;
            return 100 - (100 / (1 + rs));
        }
        public async Task<int> DefinirQuantidadeDeCandles(KlineIntervalEnum intervalo, int periodo = 14)
        {
            int fatorMultiplicador = 7; 

            switch (intervalo)
            {
                case KlineIntervalEnum.OneSecond: return periodo * fatorMultiplicador;   // 14 * 7 = 98 segundos (~2 min)
                case KlineIntervalEnum.OneMinute: return periodo * fatorMultiplicador;   // 14 * 7 = 98 minutos (~1,5h)
                case KlineIntervalEnum.ThreeMinutes: return periodo * fatorMultiplicador;// 14 * 7 = 98 * 3 min (~5h)
                case KlineIntervalEnum.FiveMinutes: return periodo * fatorMultiplicador; // 14 * 7 = 98 * 5 min (~8h)
                case KlineIntervalEnum.FifteenMinutes: return periodo * fatorMultiplicador;// ~24h
                case KlineIntervalEnum.ThirtyMinutes: return periodo * fatorMultiplicador;// ~2 dias
                case KlineIntervalEnum.OneHour: return periodo * fatorMultiplicador;      // ~4 dias
                case KlineIntervalEnum.TwoHour: return periodo * fatorMultiplicador;      // ~8 dias
                case KlineIntervalEnum.FourHour: return periodo * fatorMultiplicador;     // ~16 dias
                case KlineIntervalEnum.SixHour: return periodo * fatorMultiplicador;      // ~24 dias
                case KlineIntervalEnum.EightHour: return periodo * fatorMultiplicador;    // ~32 dias
                case KlineIntervalEnum.TwelveHour: return periodo * fatorMultiplicador;   // ~42 dias
                case KlineIntervalEnum.OneDay: return periodo * fatorMultiplicador;       // ~3 meses
                case KlineIntervalEnum.ThreeDay: return periodo * fatorMultiplicador;     // ~9 meses
                default: return periodo * fatorMultiplicador; // Padrão
            }
        }
    }
}
