using System;
using Animation.Configuration;
using Animation.Introducao;
using Animation.Monitoring;
using Animation.Start;
using Aplication.ExternalServices;
using Aplication.InternalServices;
using Aplication.InternalServices.Helper;
using Data;
using Domain.Class;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace MeuConsoleApp
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            using IHost host = Host.CreateDefaultBuilder(args)
                .ConfigureServices((context, services) =>
                {
                    ConfigureAnimations(services);
                    ConfigureServices(services);
                    ConfigureRepositories(services);
                    services.AddSingleton(new CommandLineArgs(args));
                })
                .Build();

            using (var scope = host.Services.CreateScope())
            {
                var runner = scope.ServiceProvider.GetRequiredService<IRunnerInternalService>();
                await runner.Run();
            }
        }

        private static void ConfigureAnimations(IServiceCollection services)
        {
            services.AddScoped<IStarterAnimation, StarterAnimation>();
            services.AddScoped<IConfigurationAnimation, ConfigurationAnimation>();
            services.AddScoped<IIntroducaoAnimation, IntroducaoAnimation>();
            services.AddScoped<IMonitoringAnimation, MonitoringAnimation>();
        }

        private static void ConfigureServices(IServiceCollection services)
        {
            services.AddScoped<ICalculationInternalService, CalculationInternalService>();
            services.AddScoped<IDecisionInternalService, DecisionInternalService>();
            services.AddScoped<IOperationResultInternalService, OperationResultServiceInternalService>();
            services.AddScoped<IConfigurationInternalService, ConfigurationInternalService>();
            services.AddScoped<IVerifyConditionsToRunInternalService, VerifyConditionsToRunInternalService>();
            services.AddScoped<IRunnerInternalService, RunnerInternalService>();
            services.AddScoped<ITelegramMessageExternalService, TelegramMessageExternalService>();
        }

        private static void ConfigureRepositories(IServiceCollection services)
        {
            services.AddScoped<IOperationResultRepository, OperationResultRepository>();
            services.AddScoped<IConfigurationRepository, ConfigurationRepository>();
        }
    }
}