using System;
using Animation.Configuration;
using Animation.Introducao;
using Animation.Monitoring;
using Animation.Start;
using Aplication.ExternalServices;
using Aplication.InternalServices;
using Domain.Class;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using TxtDatabase;

namespace MeuConsoleApp
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            using IHost host = Host.CreateDefaultBuilder(args)
                .ConfigureServices((context, services) =>
                {
                    services.AddScoped<IStarterAnimation, StarterAnimation>();
                    services.AddScoped<IConfiguration, Configuration>();
                    services.AddScoped<IIntroducao, Introducao>();
                    services.AddScoped<ICalculation, Calculation>();
                    services.AddScoped<IDecision, Decision>();
                    services.AddScoped<IOperationResultService, OperationResultService>();
                    services.AddScoped<IOperation<OperationResult>, Operation<OperationResult>>(); 
                    services.AddScoped<ITelegramMessage, TelegramMessage>();
                    services.AddScoped<IMonitoring, Monitoring>();
                    services.AddScoped<IVerifyConditionsToRun, VerifyConditionsToRun>();
                    services.AddScoped<IRunner, Runner>();
                })
                .Build();

            using (var scope = host.Services.CreateScope())
            {
                var runner = scope.ServiceProvider.GetRequiredService<IRunner>();
                await runner.Run();
            }
        }
    }
}
