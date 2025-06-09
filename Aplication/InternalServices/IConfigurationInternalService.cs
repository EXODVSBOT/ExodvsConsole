using Domain.Record;

namespace Aplication.InternalServices;

public interface IConfigurationInternalService
{
    void SaveFileAsync(ConfigurationRecord configuration);
    ConfigurationRecord GetConfiguration();
}