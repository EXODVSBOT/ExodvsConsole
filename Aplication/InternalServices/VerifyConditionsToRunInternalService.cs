using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Aplication.ExternalServices;
using Domain.Record;

namespace Aplication.InternalServices
{
    public class VerifyConditionsToRunInternalService : IVerifyConditionsToRunInternalService
    {
        private ConditionsToRunRecord _conditions;
        
        public async Task<ConditionsToRunRecord> StartVerification(ConfigurationResultRecord configuration)
        {
            _conditions.isAbleToStart = true;
            _conditions.errors = new List<string>();
            await VerifyInternetConnection();
            await VerifyBinanceKeys(configuration);
            await ApplyRequiredConsole();
            return _conditions;
        }

        private async Task ApplyRequiredConsole()
        {
            if (!_conditions.isAbleToStart) 
            {
                Console.Clear();
                Console.BackgroundColor = ConsoleColor.Red;

                foreach (var item in _conditions.errors)
                {
                    Console.WriteLine(item);
                }
            }
        }

        public async Task<ConditionsToRunRecord> VerifyInternetConnection()
        {
            try
            {
                using (var client = new HttpClient())
                {
                    client.Timeout = TimeSpan.FromSeconds(1);

                    var response = await client.SendAsync(new HttpRequestMessage(HttpMethod.Head, "http://www.google.com"));

                    if (response.IsSuccessStatusCode)
                    {
                        _conditions.isAbleToStart = true;
                    }
                    else
                    {
                        _conditions.isAbleToStart = false;
                        _conditions.errors.Add("Falha ao verificar conexão com a internet");
                    }
                }
            }
            catch (Exception ex)
            {
                _conditions.isAbleToStart = false;
                _conditions.errors.Add($"Erro ao verificar conexão com a internet: {ex.Message}");
            }

            return _conditions;
        }

        public async Task<ConditionsToRunRecord> VerifyBinanceKeys(ConfigurationResultRecord configuration)
        {
           var binanceInfo = new BinanceInfoExternalService(configuration.BinanceKey, configuration.BinanceSecret);

           var isValid = await binanceInfo.VerifyBinanceKeys();
            if (isValid) 
            { 
                _conditions.isAbleToStart = isValid;
            }
            if (!isValid) 
            {
                _conditions.isAbleToStart = isValid;
                _conditions.errors.Add("Chaves Binance incorretas, favor verificar.");
            }

            return _conditions;
        }
    }
}
