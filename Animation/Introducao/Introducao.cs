using System;
using System.Threading;

namespace Animation.Introducao
{
    public class Introducao : IIntroducao
    {
        public void ExibirTutorial()
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("=========================================");
            Console.WriteLine("📢 EXODVS BOT - Introdução");
            Console.WriteLine("=========================================");
            Console.WriteLine("Pressione qualquer tecla para ver a introdução.");
            Console.WriteLine("Ou pressione ESC para pular.");
            Console.WriteLine("=========================================");
            Console.ResetColor();

            var key = Console.ReadKey(intercept: true).Key;
            if (key == ConsoleKey.Escape)
            {
                Console.Clear();
                return;
            }

            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Cyan;

            PrintSection("📈 Bem-vindo ao EXODVS Bot!");

            PrintLine("Este é um bot automatizado que compra e vende Bitcoin com base no indicador RSI.");
            PrintLine("Ele analisa o mercado a cada minuto utilizando os dados da Binance com o intervalo de velas (Kline) de 4 horas.");

            Thread.Sleep(2000);
            Console.WriteLine();
            PrintSection("🔑 Como funciona?");

            PrintLine("1. Você fornece suas chaves de API da Binance.");
            PrintLine("2. O bot se conecta à sua conta e executa ordens automaticamente.");
            PrintLine("3. A lógica de compra é baseada no RSI configurável: o bot compra quando o RSI está abaixo de um valor definido e vende quando está acima de outro valor.");
            PrintLine("4. Os valores padrão são: Compra (RSI < 30) e Venda (RSI > 70), mas você pode configurá-los.");
            PrintLine("5. Também é possível configurar o Stop Loss e o Take Profit em porcentagem. O valor padrão é 10%, mas você pode alterar.");

            Thread.Sleep(2000);
            Console.WriteLine();
            PrintSection("💬 Integração com Telegram (opcional)");

            PrintLine("Você pode conectar o bot a um canal do Telegram.");
            PrintLine("Assim, todas as operações realizadas serão notificadas diretamente a você em tempo real.");
            PrintLine("Basta fornecer a chave do bot e o chat ID ao configurar.");

            Thread.Sleep(2000);
            Console.WriteLine();
            PrintSection("🛡️ Segurança");

            PrintLine("🔐 Para máxima segurança, recomendamos que você limite as permissões da API da Binance.");
            PrintLine("Além disso, configure a API para funcionar apenas no IP do seu computador.");
            PrintLine("Dessa forma, mesmo que suas chaves sejam expostas, apenas sua máquina poderá usá-las.");

            Thread.Sleep(2000);
            Console.WriteLine();
            PrintSection("💰 Resultados");

            PrintLine("Diversas pessoas já utilizam o EXODVS Bot para aproveitar as oscilações do mercado e obter lucros de forma passiva.");

            Thread.Sleep(2000);
            Console.WriteLine();
            PrintSection("🌐 Mais informações");

            PrintLine("Acesse o site oficial para atualizações, tutoriais e suporte:");
            PrintLine("➡️ https://exodvsbot.xyz");

            Thread.Sleep(2000);
            Console.WriteLine();
            PrintSection("💡 Privacidade e Gratuidade");

            PrintLine("O EXODVS Bot é totalmente gratuito e NÃO COLETA NENHUM DADO do usuário.");
            PrintLine("Ele roda **offline** no seu computador e todas as informações e chaves fornecidas ficam **localmente** armazenadas.");
            PrintLine("Ou seja, NENHUM dado sensível é compartilhado ou enviado para servidores externos.");

            Thread.Sleep(2000);
            Console.WriteLine();
            PrintSection("🔓 Open Source");

            PrintLine("O EXODVS Bot é um projeto **Open Source**, o que significa que qualquer pessoa pode visualizar e contribuir para o código.");
            PrintLine("O repositório estará disponível em breve no link: https://xpto");

            Thread.Sleep(2000);
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine("Pressione qualquer tecla para continuar...");
            Console.ResetColor();
            Console.ReadKey();
            Console.Clear();
        }

        private void PrintSection(string title)
        {
            Console.WriteLine("=========================================");
            Console.WriteLine(title);
            Console.WriteLine("=========================================");
        }

        private void PrintLine(string message)
        {
            Console.WriteLine("→ " + message);
        }
    }
}
