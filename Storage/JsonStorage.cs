using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
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

        // Sync load
        public List<Contact> LoadContacts()
        {
            if (!File.Exists(_filePath))
                return new List<Contact>();

            try
            {
                string json = File.ReadAllText(_filePath);
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                return JsonSerializer.Deserialize<List<Contact>>(json, options) ?? new List<Contact>();
            }
            catch
            {
                return new List<Contact>();
            }
        }

        // Async load
        public async Task<List<Contact>> LoadContactsAsync()
        {
            if (!File.Exists(_filePath))
                return new List<Contact>();

            try
            {
                string json = await File.ReadAllTextAsync(_filePath);
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                return JsonSerializer.Deserialize<List<Contact>>(json, options) ?? new List<Contact>();
            }
            catch
            {
                return new List<Contact>();
            }
        }

        // Sync save
        public void SaveContacts(List<Contact> contacts)
        {
            try
            {
                var options = new JsonSerializerOptions { WriteIndented = true };
                string json = JsonSerializer.Serialize(contacts, options);
                File.WriteAllText(_filePath, json);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving contacts: {ex.Message}");
            }
        }

        // Async save
        public async Task SaveContactsAsync(List<Contact> contacts)
        {
            try
            {
                var options = new JsonSerializerOptions { WriteIndented = true };
                string json = JsonSerializer.Serialize(contacts, options);
                await File.WriteAllTextAsync(_filePath, json);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving contacts: {ex.Message}");
            }
        }
    }
}