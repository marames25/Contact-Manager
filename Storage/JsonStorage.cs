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

        public JsonStorage(string filePath = "contacts.json")
        {
            _filePath = filePath;
        }

        public List<Contact> Load()
        {
            if (!File.Exists(_filePath))
                return new List<Contact>();

            try
            {
                string json = File.ReadAllText(_filePath);
                return JsonSerializer.Deserialize<List<Contact>>(json) ?? new List<Contact>();
            }
            catch
            {
                Console.WriteLine("Warning: contacts.json is invalid. Starting with empty list.");
                return new List<Contact>();
            }
        }

        public void Save(List<Contact> contacts)
        {
            string json = JsonSerializer.Serialize(contacts, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(_filePath, json);
        }
    }
}