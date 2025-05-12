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
                    services.AddScoped<IConfigurationAnimation, ConfigurationAnimation>();
                    services.AddScoped<IIntroducaoAnimation, IntroducaoAnimation>();
                    services.AddScoped<ICalculationInternalService, CalculationInternalService>();
                    services.AddScoped<IDecisionInternalService, DecisionInternalService>();
                    services.AddScoped<IOperationResultInternalService, OperationResultServiceInternalService>();
                    services.AddScoped<IOperationRepository<OperationResultDomain>, OperationRepository<OperationResultDomain>>(); 
                    services.AddScoped<ITelegramMessageExternalService, TelegramMessageExternalService>();
                    services.AddScoped<IMonitoringAnimation, MonitoringAnimation>();
                    services.AddScoped<IVerifyConditionsToRunInternalService, VerifyConditionsToRunInternalService>();
                    services.AddScoped<IRunnerInternalService, RunnerInternalService>();
                })
                .Build();

            using (var scope = host.Services.CreateScope())
            {
                var runner = scope.ServiceProvider.GetRequiredService<IRunnerInternalService>();
                await runner.Run();
            }
        }
    }
}
