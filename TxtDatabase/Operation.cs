using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace TxtDatabase
{
    public class Operation<T> : IOperation<T> where T : class
    {
        private readonly string _filePath;
        private readonly JsonSerializerOptions _jsonOptions;

        public Operation()
        {
            // Configuração do caminho do arquivo
            string projectDirectory = Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.FullName;
            _filePath = Path.Combine(projectDirectory, "TxtDatabase.txt");

            // Configurações de serialização JSON
            _jsonOptions = new JsonSerializerOptions
            {
                WriteIndented = false,
                PropertyNameCaseInsensitive = true
            };

            // Garante que o arquivo existe
            EnsureFileExists();
        }

        public void EnsureFileExists()
        {
            if (!File.Exists(_filePath))
            {
                using (File.Create(_filePath)) { }
            }
        }

        public void Create(T entity)
        {
            var json = JsonSerializer.Serialize(entity, _jsonOptions);
            var existingContent = File.ReadAllLines(_filePath).ToList();
            existingContent.Insert(0, json);
            File.WriteAllLines(_filePath, existingContent);
        }

        public IEnumerable<T> ReadAll()
        {
            var lines = File.ReadAllLines(_filePath);
            return lines.Select(line => JsonSerializer.Deserialize<T>(line, _jsonOptions));
        }

        public T ReadById(Func<T, bool> predicate)
        {
            return ReadAll().FirstOrDefault(predicate);
        }

        public void Update(Func<T, bool> predicate, T newEntity)
        {
            var entities = ReadAll().ToList();
            var index = entities.FindIndex(new Predicate<T>(predicate));

            if (index != -1)
            {
                entities[index] = newEntity;
                SaveAllEntities(entities);
            }
        }

        public void Delete(Func<T, bool> predicate)
        {
            var entities = ReadAll().ToList();
            entities.RemoveAll(new Predicate<T>(predicate));
            SaveAllEntities(entities);
        }

        public void ClearAll()
        {
            File.WriteAllText(_filePath, string.Empty);
        }

        private void SaveAllEntities(IEnumerable<T> entities)
        {
            var jsonLines = entities.Select(entity => JsonSerializer.Serialize(entity, _jsonOptions));
            File.WriteAllLines(_filePath, jsonLines);
        }
    }
}