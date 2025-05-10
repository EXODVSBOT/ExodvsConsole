using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Domain.Class;
using TxtDatabase;

namespace Animation.Monitoring
{
    public class Monitoring : IMonitoring
    {
        private readonly ConsoleColor[] _colorScheme =
        {
            ConsoleColor.Cyan,
            ConsoleColor.Blue,
            ConsoleColor.DarkGray,
            ConsoleColor.Gray,
            ConsoleColor.White
        };

        private bool _isPaused;
        public bool IsPaused => _isPaused;

        private bool _isRunning = true;
        private OperationResult _lastResult;
        private readonly Queue<OperationResult> _resultsHistory = new Queue<OperationResult>();
        private const int MaxHistorySize = 1000;
        private readonly IOperation<OperationResult> _operation;

        public Monitoring(IOperation<OperationResult> operation)
        {
            _operation = operation;
        }

        public void Initialize()
        {
            Console.Title = "EXODVS - Crypto Trading Bot Monitor 🤑";
            Console.CursorVisible = false;
            Task.Run(CheckInput);
            DrawStaticLayout();
        }

        public void UpdateData(OperationResult result)
        {
            if (_isPaused) return;

            _lastResult = result;

            // Adiciona ao histórico e mantém apenas os últimos 1000 registros
            _resultsHistory.Enqueue(result);
            if (_resultsHistory.Count > MaxHistorySize)
            {
                _resultsHistory.Dequeue();
            }

            UpdateDynamicContent();
        }

        private void DrawStaticLayout()
        {
            Console.Clear();

            // Cabeçalho
            Console.ForegroundColor = _colorScheme[0];
            Console.WriteLine(new string('═', Console.WindowWidth));
            Console.WriteLine(" EXODVS - Crypto Trading Bot Monitor 🤑 ".PadBoth(Console.WindowWidth));
            Console.WriteLine(new string('═', Console.WindowWidth));

            // Colunas
            DrawColumn(0, "Status do Mercado");
            DrawColumn(1, "Configurações");
            DrawColumn(2, "Controles");

            Console.ResetColor();
        }

        private void DrawColumn(int columnIndex, string title)
        {
            var columnWidth = Console.WindowWidth / 3;
            var xPosition = columnIndex * columnWidth;

            Console.SetCursorPosition(xPosition, 5);
            Console.ForegroundColor = _colorScheme[2];
            Console.Write(new string('─', columnWidth));

            Console.SetCursorPosition(xPosition + (columnWidth - title.Length) / 2, 6);
            Console.ForegroundColor = _colorScheme[3];
            Console.Write(title);
        }

        private void UpdateDynamicContent()
        {
            var columnWidth = Console.WindowWidth / 3;

            // Status do Mercado
            UpdateColumn(0, 8, new[]
            {
                ($"Preço BTC: ", $"{_lastResult.BitcoinPrice:C}"),
                ($"RSI Atual: ", $"{_lastResult.MarketRsi:F2}"),
                ($"Saldo USDT: ", $"{_lastResult.UsdtBalance:F2}"),
                ($"Última Operação: ", $"{_lastResult.OperationDate:HH:mm:ss}"),
                ($"Intervalo Kline: ", GetKlineIntervalDescription(_lastResult.KlineInterval)),
                ($"Histórico: ", $"{_resultsHistory.Count}/{MaxHistorySize}")
            });

            // Configurações
            UpdateColumn(1, 8, new[]
            {
                ($"Compra RSI: ", _lastResult.BuyRsi.ToString()),
                ($"Venda RSI: ", _lastResult.SellRsi.ToString()),
                ($"Take Profit: ", $"{_lastResult.TakeProfit}%"),
                ($"Stop Loss: ", $"{_lastResult.StopLoss}%")
            });

            // Controles
            UpdateColumn(2, 8, new[]
            {
                ("Status: ", _isPaused ? "PAUSADO [X]" : "ATIVO [✔]"),
                ("", "Pressione SPACE para pausar"),
                ("", "Pressione Q para sair"),
                ("", "Pressione R para relatório"),
                ("", "Pressione L para listar operações")
            });

            // Footer
            Console.SetCursorPosition(0, Console.WindowHeight - 3);
            Console.ForegroundColor = _colorScheme[4];
            Console.Write(new string('═', Console.WindowWidth));
            Console.WriteLine($" Ciclo atualizado em: {DateTime.Now:HH:mm:ss} ".PadBoth(Console.WindowWidth));
            Console.Write(new string('═', Console.WindowWidth));
        }

        private void UpdateColumn(int columnIndex, int startLine, (string Label, string Value)[] items)
        {
            var columnWidth = Console.WindowWidth / 3;
            var xPosition = columnIndex * columnWidth;

            for (int i = 0; i < items.Length; i++)
            {
                Console.SetCursorPosition(xPosition + 2, startLine + i);
                Console.ForegroundColor = _colorScheme[3];
                Console.Write(items[i].Label);

                Console.ForegroundColor = _colorScheme[4];
                Console.Write(items[i].Value.PadRight(columnWidth - items[i].Label.Length - 2));
            }
        }

        private async Task CheckInput()
        {
            while (_isRunning)
            {
                if (Console.KeyAvailable)
                {
                    var key = Console.ReadKey(true).Key;

                    if (key == ConsoleKey.Spacebar)
                    {
                        _isPaused = !_isPaused;
                        if (_isPaused) ShowPauseScreen();
                        else DrawStaticLayout();
                    }
                    else if (key == ConsoleKey.Q)
                    {
                        _isRunning = false;
                        Environment.Exit(0);
                    }
                    else if (key == ConsoleKey.R)
                    {
                        ShowReport();
                    }
                    else if (key == ConsoleKey.L)
                    {
                        ShowOperationsList();
                    }
                }
                await Task.Delay(100);
            }
        }

        private void ShowPauseScreen()
        {
            Console.Clear();
            Console.ForegroundColor = _colorScheme[0];

            var art = @"
             _____                _____ 
            |  __ \              / ____|
            | |__) __ _ ___  ___| |     
            |  ___/ _` / __|/ _ \ |     
            | |  | (_| \__ \  __/ |____ 
            |_|   \__,_|___/\___|\_____|
            ";

            Console.SetCursorPosition(
                (Console.WindowWidth - art.Split('\n')[2].Length) / 2,
                Console.WindowHeight / 2 - 3
            );

            foreach (var line in art.Split('\n'))
            {
                Console.WriteLine(line.PadLeft(Console.WindowWidth / 2 + line.Length / 2));
            }

            Console.ForegroundColor = _colorScheme[4];
            Console.SetCursorPosition(
                (Console.WindowWidth - 30) / 2,
                Console.WindowHeight - 4
            );
            Console.WriteLine("Pressione SPACE para retomar as operações");
        }

        private void ShowReport()
        {
            _isPaused = true;
            Console.Clear();
            Console.ForegroundColor = _colorScheme[0];

            Console.WriteLine(new string('═', Console.WindowWidth));
            Console.WriteLine(" RELATÓRIO DE OPERAÇÕES ".PadBoth(Console.WindowWidth));
            Console.WriteLine(new string('═', Console.WindowWidth));
            Console.WriteLine();

            if (!_resultsHistory.Any())
            {
                Console.WriteLine("Nenhum dado histórico disponível.");
                Console.WriteLine("\nPressione qualquer tecla para voltar...");
                Console.ReadKey();
                _isPaused = false;
                DrawStaticLayout();
                return;
            }

            // Estatísticas básicas
            Console.ForegroundColor = _colorScheme[3];
            Console.WriteLine("ESTATÍSTICAS GERAIS:");
            Console.ForegroundColor = _colorScheme[4];
            Console.WriteLine($"Período analisado: {_resultsHistory.First().OperationDate} a {_resultsHistory.Last().OperationDate}");
            Console.WriteLine($"Total de operações registradas: {_resultsHistory.Count}");
            Console.WriteLine($"Saldo inicial USDT: {_resultsHistory.First().UsdtBalance:F2}");
            Console.WriteLine($"Saldo atual USDT: {_resultsHistory.Last().UsdtBalance:F2}");
            Console.WriteLine($"Variação percentual: {((_resultsHistory.Last().UsdtBalance - _resultsHistory.First().UsdtBalance) / _resultsHistory.First().UsdtBalance) * 100:F2}%");

            // Preço do BTC
            Console.WriteLine();
            Console.ForegroundColor = _colorScheme[3];
            Console.WriteLine("PREÇO DO BITCOIN:");
            Console.ForegroundColor = _colorScheme[4];
            Console.WriteLine($"Preço mínimo: {_resultsHistory.Min(r => r.BitcoinPrice):C}");
            Console.WriteLine($"Preço máximo: {_resultsHistory.Max(r => r.BitcoinPrice):C}");
            Console.WriteLine($"Preço médio: {_resultsHistory.Average(r => r.BitcoinPrice):C}");

            // RSI
            Console.WriteLine();
            Console.ForegroundColor = _colorScheme[3];
            Console.WriteLine("RSI DO MERCADO:");
            Console.ForegroundColor = _colorScheme[4];
            Console.WriteLine($"RSI mínimo: {_resultsHistory.Min(r => r.MarketRsi):F2}");
            Console.WriteLine($"RSI máximo: {_resultsHistory.Max(r => r.MarketRsi):F2}");
            Console.WriteLine($"RSI médio: {_resultsHistory.Average(r => r.MarketRsi):F2}");

            Console.WriteLine();
            Console.ForegroundColor = _colorScheme[2];
            Console.WriteLine(new string('─', Console.WindowWidth));
            Console.ForegroundColor = _colorScheme[4];
            Console.WriteLine("\nPressione qualquer tecla para voltar...");

            Console.ReadKey();
            _isPaused = false;
            DrawStaticLayout();
        }

        private void ShowOperationsList()
        {
            _isPaused = true;
            Console.Clear();
            Console.ForegroundColor = _colorScheme[0];

            Console.WriteLine(new string('═', Console.WindowWidth));
            Console.WriteLine(" LISTA DE OPERAÇÕES REALIZADAS ".PadBoth(Console.WindowWidth));
            Console.WriteLine(new string('═', Console.WindowWidth));
            Console.WriteLine();

            var operations = _operation.ReadAll()?
                .OrderByDescending(o => o.OperationDate)
                .ToList();

            if (operations == null || !operations.Any())
            {
                Console.WriteLine("Nenhuma operação registrada.");
                Console.WriteLine("\nPressione qualquer tecla para voltar...");
                Console.ReadKey();
                _isPaused = false;
                DrawStaticLayout();
                return;
            }

            // Cabeçalho da tabela
            Console.ForegroundColor = _colorScheme[3];
            Console.WriteLine("Data/Hora".PadRight(20) + " | " +
                              "Preço BTC".PadRight(15) + " | " +
                              "RSI".PadRight(10) + " | " +
                              "Executada".PadRight(10) + " | " +
                              "Saldo USDT".PadRight(15));
            Console.WriteLine(new string('─', Console.WindowWidth));

            // Linhas da tabela
            Console.ForegroundColor = _colorScheme[4];
            foreach (var op in operations)
            {
                Console.WriteLine(
                    op.OperationDate.ToString("dd/MM HH:mm:ss").PadRight(20) + " | " +
                    op.BitcoinPrice.ToString("C").PadRight(15) + " | " +
                    op.MarketRsi.ToString("F2").PadRight(10) + " | " +
                    (op.Executed ? "Sim" : "Não").PadRight(10) + " | " +
                    op.UsdtBalance.ToString("F2").PadRight(15));
            }

            Console.WriteLine();
            Console.ForegroundColor = _colorScheme[2];
            Console.WriteLine(new string('─', Console.WindowWidth));
            Console.ForegroundColor = _colorScheme[4];
            Console.WriteLine($"Total de operações: {operations.Count}");
            Console.WriteLine("\nPressione qualquer tecla para voltar...");

            Console.ReadKey();
            _isPaused = false;
            DrawStaticLayout();
        }

        private string GetKlineIntervalDescription(int intervalValue)
        {
            return intervalValue switch
            {
                1 => "1 segundo",
                2 => "1 minuto",
                3 => "3 minutos",
                4 => "5 minutos",
                5 => "15 minutos",
                6 => "30 minutos",
                7 => "1 hora",
                8 => "2 horas",
                9 => "4 horas",
                10 => "6 horas",
                11 => "8 horas",
                12 => "12 horas",
                13 => "1 dia",
                14 => "3 dias",
                15 => "1 semana",
                16 => "1 mês",
                _ => "4 horas (padrão)"
            };
        }
    }

    public static class StringExtensions
    {
        public static string PadBoth(this string str, int length)
        {
            int spaces = length - str.Length;
            int padLeft = spaces / 2 + str.Length;
            return str.PadLeft(padLeft).PadRight(length);
        }
    }
}