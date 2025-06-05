using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text.Json;

namespace TxtDatabase
{
    public class OperationRepository<T> : IOperationRepository<T> where T : class
    {
        private readonly string FilePath;
        private readonly JsonSerializerOptions _jsonOptions;

        public OperationRepository()
        {
            _jsonOptions = new JsonSerializerOptions
            {
                WriteIndented = false,
                PropertyNameCaseInsensitive = true
            };

            // Determina o caminho do arquivo baseado no sistema operacional
            FilePath = GetDatabaseFilePath();
            EnsureFileExists();
        }

        private string GetDatabaseFilePath()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                return @"C:\Exodvs\TxtDatabase.txt";
            }
            else
            {
                string homeDirectory = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
                return Path.Combine(homeDirectory, "ExodvsBot", "TxtDatabase.txt");
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

        public void Create(T entity)
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
                throw new IOException($"Failed to create entity: {ex.Message}", ex);
            }
        }

        public IEnumerable<T> ReadAll()
        {
            try
            {
                var lines = File.ReadAllLines(FilePath);
                return lines.Where(line => !string.IsNullOrWhiteSpace(line))
                           .Select(line => JsonSerializer.Deserialize<T>(line, _jsonOptions));
            }
            catch (Exception ex)
            {
                throw new IOException($"Failed to read entities: {ex.Message}", ex);
            }
        }

        public T ReadById(Func<T, bool> predicate)
        {
            try
            {
                return ReadAll().FirstOrDefault(predicate);
            }
            catch (Exception ex)
            {
                throw new IOException($"Failed to read entity by ID: {ex.Message}", ex);
            }
        }

        public void Update(Func<T, bool> predicate, T newEntity)
        {
            try
            {
                var entities = ReadAll().ToList();
                var index = entities.FindIndex(new Predicate<T>(predicate));

                if (index != -1)
                {
                    entities[index] = newEntity;
                    SaveAllEntities(entities);
                }
            }
            catch (Exception ex)
            {
                throw new IOException($"Failed to update entity: {ex.Message}", ex);
            }
        }

        public void Delete(Func<T, bool> predicate)
        {
            try
            {
                var entities = ReadAll().ToList();
                entities.RemoveAll(new Predicate<T>(predicate));
                SaveAllEntities(entities);
            }
            catch (Exception ex)
            {
                throw new IOException($"Failed to delete entity: {ex.Message}", ex);
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
                throw new IOException($"Failed to clear database: {ex.Message}", ex);
            }
        }

        private void SaveAllEntities(IEnumerable<T> entities)
        {
            try
            {
                var jsonLines = entities.Select(entity => JsonSerializer.Serialize(entity, _jsonOptions));
                File.WriteAllLines(FilePath, jsonLines);
            }
            catch (Exception ex)
            {
                throw new IOException($"Failed to save entities: {ex.Message}", ex);
            }
        }
    }
}