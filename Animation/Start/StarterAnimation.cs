using System;
using System.Collections.Generic;
using System.Threading;

namespace Animation.Start
{
    public class StarterAnimation : IStarterAnimation
    {
        private readonly string[] logo = new[]
        {
            @"███████╗██╗  ██╗ ██████╗ ██████╗ ██╗   ██╗███████╗",
            @"██╔════╝╚██╗██╔╝██╔═══██╗██╔══██╗██║   ██║██╔════╝",
            @"█████╗   ╚███╔╝ ██║   ██║██║  ██║██║   ██║███████╗",
            @"██╔══╝   ██╔██╗ ██║   ██║██║  ██║██║   ██║╚════██║",
            @"███████╗██╔╝ ██╗╚██████╔╝██████╔╝╚██████╔╝███████║",
            @"╚══════╝╚═╝  ╚═╝ ╚═════╝ ╚═════╝  ╚═════╝ ╚══════╝"
        };

        private readonly List<ConsoleColor> discordColors = new List<ConsoleColor>
        {
            ConsoleColor.Cyan,
            ConsoleColor.Blue,
            ConsoleColor.DarkGray,
            ConsoleColor.Gray,
            ConsoleColor.White
        };

        public void PlayAnimation()
        {
            int durationInSeconds = 5;
            int frameDelay = 500; // ms
            int totalFrames = durationInSeconds * 1000 / frameDelay;
            int colorIndex = 0;

            Console.BackgroundColor = ConsoleColor.Black;
            Console.Clear();

            for (int i = 0; i < totalFrames; i++)
            {
                Console.Clear();
                Console.ForegroundColor = discordColors[colorIndex];

                int consoleWidth = Console.WindowWidth;
                int consoleHeight = Console.WindowHeight;
                int logoWidth = logo[0].Length;
                int logoHeight = logo.Length;

                int topPadding = Math.Max((consoleHeight - logoHeight) / 2, 0);
                int leftPadding = Math.Max((consoleWidth - logoWidth) / 2, 0);

                // Espaçamento vertical
                for (int j = 0; j < topPadding; j++)
                    Console.WriteLine();

                // Imprimir linhas centralizadas
                foreach (var line in logo)
                {
                    Console.WriteLine(new string(' ', leftPadding) + line);
                }

                Console.ResetColor();
                Thread.Sleep(frameDelay);
                colorIndex = (colorIndex + 1) % discordColors.Count;
            }

            // Finalização com estilo
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.DarkCyan;
            Console.WriteLine(new string(' ', (Console.WindowWidth - 30) / 2) + ">> EXODVS CRYPTO PRONTO CHEFIA <<");
            Console.ResetColor();
            Thread.Sleep(1000);
        }
    }
}
