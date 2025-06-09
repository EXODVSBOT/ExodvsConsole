using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text.Json;
using Domain.Record;

namespace Data
{
    public class ConfigurationRepository : IConfigurationRepository
    {
        private readonly string FilePath;
        private readonly JsonSerializerOptions _jsonOptions;

        public ConfigurationRepository()
        {
            _jsonOptions = new JsonSerializerOptions
            {
                WriteIndented = true,  
                PropertyNameCaseInsensitive = true
            };

            FilePath = GetConfigurationFilePath();
            EnsureFileExists();
        }

        private string GetConfigurationFilePath()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                return @"C:\ExodvsBot\Configuration.txt";
            }
            else
            {
                string homeDirectory = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
                return Path.Combine(homeDirectory, "ExodvsBot", "Configuration.txt");
            }
        }

        public void EnsureFileExists()
        {
            try
            {
                var directory = Path.GetDirectoryName(FilePath);
                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                if (!File.Exists(FilePath))
                {
                    File.WriteAllText(FilePath, "{}"); // Cria com um JSON vazio
                }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to initialize configuration file: {ex.Message}", ex);
            }
        }

        public void SaveConfiguration(ConfigurationRecord config)
        {
            try
            {
                var json = JsonSerializer.Serialize(config, _jsonOptions);
                File.WriteAllText(FilePath, json); // Sobrescreve todo o conteúdo
            }
            catch (Exception ex)
            {
                throw new IOException($"Failed to save configuration: {ex.Message}", ex);
            }
        }

        public ConfigurationRecord LoadConfiguration()
        {
            try
            {
                var json = File.ReadAllText(FilePath);
                return JsonSerializer.Deserialize<ConfigurationRecord>(json, _jsonOptions);
            }
            catch (Exception ex)
            {
                throw new IOException($"Failed to load configuration: {ex.Message}", ex);
            }
        }
    }
}