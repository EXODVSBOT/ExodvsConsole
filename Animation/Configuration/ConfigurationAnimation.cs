using System;
using System.Threading;
using Domain.Record;

namespace Animation.Configuration
{
    public class ConfigurationAnimation : IConfigurationAnimation
    {
        public string BinanceKey { get; private set; } = string.Empty;
        public string BinanceSecret { get; private set; } = string.Empty;
        public string TelegramKey { get; private set; } = string.Empty;
        public string TelegramChatId { get; private set; } = string.Empty;
        public int BuyRsi { get; private set; } = 30;
        public int SellRsi { get; private set; } = 70;
        public int StopLoss { get; private set; } = 10;
        public int TakeProfit { get; private set; } = 10;
        public int KlineInterval { get; private set; } = 9; // Padrão: 4 horas
        public int RunInterval { get; private set; } = 1; //Padrão 1 segundo

        public ConfigurationResultRecord GetConfiguration()
        {
            Console.ForegroundColor = ConsoleColor.Cyan;

            BinanceKey = ReadRequired("Digite sua Binance API Key (obrigatório): ");
            BinanceSecret = ReadRequired("Digite sua Binance API Secret (obrigatório): ");

            TelegramKey = ReadOptional("Digite sua Telegram Key (opcional): ");
            TelegramChatId = ReadOptional("Digite seu Telegram ChatId (opcional): ");

            BuyRsi = ReadIntOrDefault($"RSI para compra (padrão: {BuyRsi}): ", BuyRsi);
            SellRsi = ReadIntOrDefault($"RSI para venda (padrão: {SellRsi}): ", SellRsi);
            StopLoss = ReadIntOrDefault($"Stop Loss em % (padrão: {StopLoss}): ", StopLoss);
            TakeProfit = ReadIntOrDefault($"Take Profit em % (padrão: {TakeProfit}): ", TakeProfit);

            KlineInterval = SelectKlineInterval();

            RunInterval = SelectRunInterval();

            Thread.Sleep(1000);
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine("\n✅ Parâmetros configurados com sucesso!");
            Thread.Sleep(1000);
            Console.ResetColor();
            Console.Clear();

            return new ConfigurationResultRecord(
                BinanceKey,
                BinanceSecret,
                TelegramKey,
                TelegramChatId,
                KlineInterval,
                BuyRsi,
                SellRsi,
                StopLoss,
                TakeProfit,
                RunInterval
            );
        }

        private int SelectRunInterval()
        {
            Console.WriteLine("\nSelecione o intervalo para análise dos candles:");
            Console.WriteLine("╔═══════╦═══════════════════════════════════╗");
            Console.WriteLine("║ Opção ║         Intervalo de Análise      ║");
            Console.WriteLine("╠═══════╬═══════════════════════════════════╣");
            Console.WriteLine("║   1   ║ 1 análise por segundo             ║");
            Console.WriteLine("║   2   ║ 1 análise a cada 5 segundos       ║");
            Console.WriteLine("║   3   ║ 1 análise a cada 10 segundos      ║");
            Console.WriteLine("║   4   ║ 1 análise a cada 30 segundos      ║");
            Console.WriteLine("║   5   ║ 1 análise a cada minuto           ║");
            Console.WriteLine("║   6   ║ 1 análise a cada 10 minutos       ║");
            Console.WriteLine("║   7   ║ 1 análise a cada hora             ║");
            Console.WriteLine("╚═══════╩═══════════════════════════════════╝");

            while (true)
            {
                Console.Write("\nDigite o NÚMERO da opção desejada (1-7, padrão: 1): ");
                string input = Console.ReadLine();

                if (string.IsNullOrWhiteSpace(input))
                    return 1; // Valor padrão se nada for digitado

                if (int.TryParse(input, out int result) && result >= 1 && result <= 7)
                    return result;

                Console.WriteLine("Opção inválida! Por favor, digite um número entre 1 e 7.");
            }
        }

        private int SelectKlineInterval()
        {
            Console.WriteLine("\nSelecione o intervalo para análise dos candles:");
            Console.WriteLine("╔════════════════════════════════════╗");
            Console.WriteLine("║ Opção │ Intervalo Kline            ║");
            Console.WriteLine("╠═══════╪════════════════════════════╣");
            Console.WriteLine("║   1   │ 1 segundo                  ║");
            Console.WriteLine("║   2   │ 1 minuto                   ║");
            Console.WriteLine("║   3   │ 3 minutos                  ║");
            Console.WriteLine("║   4   │ 5 minutos                  ║");
            Console.WriteLine("║   5   │ 15 minutos                 ║");
            Console.WriteLine("║   6   │ 30 minutos                 ║");
            Console.WriteLine("║   7   │ 1 hora                     ║");
            Console.WriteLine("║   8   │ 2 horas                    ║");
            Console.WriteLine("║   9   │ 4 horas (RECOMENDADO)      ║");
            Console.WriteLine("║  10   │ 6 horas                    ║");
            Console.WriteLine("║  11   │ 8 horas                    ║");
            Console.WriteLine("║  12   │ 12 horas                   ║");
            Console.WriteLine("║  13   │ 1 dia                      ║");
            Console.WriteLine("║  14   │ 3 dias                     ║");
            Console.WriteLine("║  15   │ 1 semana                   ║");
            Console.WriteLine("║  16   │ 1 mês                      ║");
            Console.WriteLine("╚═══════╧════════════════════════════╝");

            Console.Write("\nDigite o NÚMERO da opção desejada (padrão: 9 - 4 horas): ");

            string input = Console.ReadLine();
            int selectedOption = int.TryParse(input, out int result) ? result : 9;

            return selectedOption;
        }

        private static string ReadRequired(string prompt)
        {
            string? input;
            do
            {
                Console.Write(prompt);
                input = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(input))
                {
                    Console.WriteLine("❌ Este campo é obrigatório.\n");
                }
            } while (string.IsNullOrWhiteSpace(input));
            return input.Trim();
        }

        private static string ReadOptional(string prompt)
        {
            Console.Write(prompt);
            return Console.ReadLine()?.Trim() ?? string.Empty;
        }

        private static int ReadIntOrDefault(string prompt, int defaultValue)
        {
            Console.Write(prompt);
            string? input = Console.ReadLine();
            return int.TryParse(input, out int result) ? result : defaultValue;
        }
    }
}