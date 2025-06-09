using Domain.Record;

namespace Data
{
    public interface IConfigurationRepository
    {
        void EnsureFileExists();
        void SaveConfiguration(ConfigurationRecord config);
        ConfigurationRecord LoadConfiguration();
    }
}