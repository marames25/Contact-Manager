using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using ContactManagerCLI.Models;

namespace ContactManagerCLI.Storage
{
    public class JsonStorage : IStorage
    {
        private readonly string _filePath;

        public JsonStorage(string filePath)
        {
            _filePath = filePath;
        }

        public List<Contact>? Load()
        {
            if (!File.Exists(_filePath))
                return new List<Contact>();

            try
            {
                string json = File.ReadAllText(_filePath);
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };
                return JsonSerializer.Deserialize<List<Contact>>(json, options) ?? new List<Contact>();
            }
            catch
            {
                return new List<Contact>();
            }
        }

        public void Save(List<Contact> contacts)
        {
            try
            {
                var options = new JsonSerializerOptions
                {
                    WriteIndented = true
                };
                string json = JsonSerializer.Serialize(contacts, options);
                File.WriteAllText(_filePath, json);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving contacts: {ex.Message}");
            }
        }
    }
}