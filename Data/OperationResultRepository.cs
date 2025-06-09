using System.Runtime.InteropServices;
using System.Text.Json;
using Domain.Class;

namespace Data;

public class OperationResultRepository : IOperationResultRepository
    {
        private readonly string FilePath;
        private readonly JsonSerializerOptions _jsonOptions;

        public OperationResultRepository()
        {
            _jsonOptions = new JsonSerializerOptions
            {
                WriteIndented = false,
                PropertyNameCaseInsensitive = true
            };

            FilePath = GetDatabaseFilePath();
            EnsureFileExists();
        }

        private string GetDatabaseFilePath()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                return @"C:\ExodvsBot\OperationResults.txt";
            }
            else
            {
                string homeDirectory = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
                return Path.Combine(homeDirectory, "ExodvsBot", "OperationResults.txt");
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
                    using (File.Create(FilePath)) { }
                }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to initialize database file: {ex.Message}", ex);
            }
        }

        public void Create(OperationResultDomain entity)
        {
            try
            {
                var json = JsonSerializer.Serialize(entity, _jsonOptions);
                var existingContent = File.ReadAllLines(FilePath).ToList();
                existingContent.Insert(0, json);
                File.WriteAllLines(FilePath, existingContent);
            }
            catch (Exception ex)
            {
                throw new IOException($"Failed to create operation result: {ex.Message}", ex);
            }
        }

        public IEnumerable<OperationResultDomain> ReadAll()
        {
            try
            {
                var lines = File.ReadAllLines(FilePath);
                return lines.Where(line => !string.IsNullOrWhiteSpace(line))
                           .Select(line => JsonSerializer.Deserialize<OperationResultDomain>(line, _jsonOptions))
                           .OrderByDescending(x => x.OperationDate); // Ordenação padrão por data
            }
            catch (Exception ex)
            {
                throw new IOException($"Failed to read operation results: {ex.Message}", ex);
            }
        }

        public OperationResultDomain ReadById(Func<OperationResultDomain, bool> predicate)
        {
            try
            {
                return ReadAll().FirstOrDefault(predicate);
            }
            catch (Exception ex)
            {
                throw new IOException($"Failed to read operation result: {ex.Message}", ex);
            }
        }

        public void Update(Func<OperationResultDomain, bool> predicate, OperationResultDomain newEntity)
        {
            try
            {
                var entities = ReadAll().ToList();
                var index = entities.FindIndex(new Predicate<OperationResultDomain>(predicate));

                if (index != -1)
                {
                    entities[index] = newEntity;
                    SaveAllEntities(entities);
                }
            }
            catch (Exception ex)
            {
                throw new IOException($"Failed to update operation result: {ex.Message}", ex);
            }
        }

        public void Delete(Func<OperationResultDomain, bool> predicate)
        {
            try
            {
                var entities = ReadAll().ToList();
                entities.RemoveAll(new Predicate<OperationResultDomain>(predicate));
                SaveAllEntities(entities);
            }
            catch (Exception ex)
            {
                throw new IOException($"Failed to delete operation result: {ex.Message}", ex);
            }
        }

        public void ClearAll()
        {
            try
            {
                File.WriteAllText(FilePath, string.Empty);
            }
            catch (Exception ex)
            {
                throw new IOException($"Failed to clear operation results: {ex.Message}", ex);
            }
        }

        private void SaveAllEntities(IEnumerable<OperationResultDomain> entities)
        {
            try
            {
                var jsonLines = entities.Select(entity => JsonSerializer.Serialize(entity, _jsonOptions));
                File.WriteAllLines(FilePath, jsonLines);
            }
            catch (Exception ex)
            {
                throw new IOException($"Failed to save operation results: {ex.Message}", ex);
            }
        }
    }