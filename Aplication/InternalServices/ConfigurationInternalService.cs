using Data;
using Domain.Record;

namespace Aplication.InternalServices;

public class ConfigurationInternalService : IConfigurationInternalService
{
    private readonly IConfigurationRepository _configurationRepository;

    public ConfigurationInternalService(IConfigurationRepository configurationRepository)
    {
        _configurationRepository = configurationRepository;
    }

    public void SaveFileAsync(ConfigurationRecord configuration)
    {
       _configurationRepository.SaveConfiguration(configuration);
    }

    public ConfigurationRecord GetConfiguration()
    {
        return _configurationRepository.LoadConfiguration();
    }
}