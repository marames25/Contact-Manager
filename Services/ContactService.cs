using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Net.Mail;
using System.Threading.Tasks;
using ContactManagerCLI.Models;
using ContactManagerCLI.Storage;

namespace ContactManagerCLI.Services
{
    public class ContactService : IContactService
    {
        private List<Contact> _contacts;
        private readonly IStorage _storage;
        private int _nextId;

        // Indexes for fast search
        private Dictionary<string, Contact> _emailIndex;
        private Dictionary<string, List<Contact>> _phoneIndex;
        private Dictionary<string, List<Contact>> _nameIndex;

        public ContactService(IStorage storage)
        {
            _storage = storage ?? throw new ArgumentNullException(nameof(storage));
            _contacts = new List<Contact>();

            _emailIndex = new Dictionary<string, Contact>();
            _phoneIndex = new Dictionary<string, List<Contact>>();
            _nameIndex = new Dictionary<string, List<Contact>>();

            _nextId = 1; // will be updated after load
        }

        // Async initialization
        public async Task InitializeAsync()
        {
            _contacts = await _storage.LoadContactsAsync() ?? new List<Contact>();
            _nextId = _contacts.Any() ? _contacts.Max(c => c.Id) + 1 : 1;

            // Build indexes
            _emailIndex = _contacts.ToDictionary(c => c.Email.ToLower(), c => c);
            _phoneIndex = _contacts.GroupBy(c => c.Phone)
                                   .ToDictionary(g => g.Key, g => g.ToList());

            _nameIndex = new Dictionary<string, List<Contact>>();
            foreach (var c in _contacts)
            {
                var key = c.Name.ToLower();
                if (!_nameIndex.ContainsKey(key))
                    _nameIndex[key] = new List<Contact>();
                _nameIndex[key].Add(c);
            }
        }

        private void ValidateContact(Contact contact)
        {
            if (string.IsNullOrWhiteSpace(contact.Name))
                throw new Exception("Name cannot be empty.");
            if (string.IsNullOrWhiteSpace(contact.Phone))
                throw new Exception("Phone cannot be empty.");
            if (string.IsNullOrWhiteSpace(contact.Email))
                throw new Exception("Email cannot be empty.");
            if (!IsValidEmail(contact.Email))
                throw new Exception("Email format is invalid.");
            if (!IsValidPhone(contact.Phone))
                throw new Exception("Phone format is invalid.");
        }

        private bool IsValidEmail(string email)
        {
            try
            {
                var addr = new MailAddress(email.Trim());
                return addr.Address == email.Trim();
            }
            catch
            {
                return false;
            }
        }

        private bool IsValidPhone(string phone)
        {
            return Regex.IsMatch(phone.Trim(), @"^\d{8,15}$");
        }

        // Add contact
        public void AddContact(Contact contact)
        {
            ValidateContact(contact);
            contact.Email = contact.Email.Trim();

            if (_emailIndex.ContainsKey(contact.Email.ToLower()))
                throw new Exception("A contact with this email already exists!");

            contact.Id = _nextId++;
            _contacts.Add(contact);

            // Update indexes
            _emailIndex[contact.Email.ToLower()] = contact;

            if (!_phoneIndex.ContainsKey(contact.Phone))
                _phoneIndex[contact.Phone] = new List<Contact>();
            _phoneIndex[contact.Phone].Add(contact);

            var nameKey = contact.Name.ToLower();
            if (!_nameIndex.ContainsKey(nameKey))
                _nameIndex[nameKey] = new List<Contact>();
            _nameIndex[nameKey].Add(contact);
        }

        // Edit contact
        public void EditContact(int id, string name, string phone, string email)
        {
            var contact = GetContactById(id);
            if (contact == null) throw new Exception("Contact not found.");

            // Email
            if (!string.IsNullOrWhiteSpace(email))
            {
                email = email.Trim();
                if (!IsValidEmail(email))
                    throw new Exception("Email format is invalid.");

                if (_emailIndex.ContainsKey(email.ToLower()) && _emailIndex[email.ToLower()].Id != id)
                    throw new Exception("Another contact already has this email!");

                _emailIndex.Remove(contact.Email.ToLower());
                contact.Email = email;
                _emailIndex[email.ToLower()] = contact;
            }

            // Name
            if (!string.IsNullOrWhiteSpace(name))
            {
                var oldKey = contact.Name.ToLower();
                _nameIndex[oldKey].Remove(contact);
                if (_nameIndex[oldKey].Count == 0)
                    _nameIndex.Remove(oldKey);

                contact.Name = name;
                var newKey = name.ToLower();
                if (!_nameIndex.ContainsKey(newKey))
                    _nameIndex[newKey] = new List<Contact>();
                _nameIndex[newKey].Add(contact);
            }

            // Phone
            if (!string.IsNullOrWhiteSpace(phone))
            {
                if (!IsValidPhone(phone))
                    throw new Exception("Phone format is invalid.");

                _phoneIndex[contact.Phone].Remove(contact);
                if (_phoneIndex[contact.Phone].Count == 0)
                    _phoneIndex.Remove(contact.Phone);

                contact.Phone = phone;
                if (!_phoneIndex.ContainsKey(phone))
                    _phoneIndex[phone] = new List<Contact>();
                _phoneIndex[phone].Add(contact);
            }
        }

        // Delete contact
        public void DeleteContact(int id)
        {
            var contact = GetContactById(id);
            if (contact == null) throw new Exception("Contact not found.");

            _contacts.Remove(contact);

            _emailIndex.Remove(contact.Email.ToLower());
            _phoneIndex[contact.Phone].Remove(contact);
            if (_phoneIndex[contact.Phone].Count == 0)
                _phoneIndex.Remove(contact.Phone);

            var nameKey = contact.Name.ToLower();
            _nameIndex[nameKey].Remove(contact);
            if (_nameIndex[nameKey].Count == 0)
                _nameIndex.Remove(nameKey);
        }

        // Get by ID
        public Contact? GetContactById(int id) => _contacts.FirstOrDefault(c => c.Id == id);

        public List<Contact> GetAllContacts() => _contacts;

        // Search
        public List<Contact> SearchByField(string query)
        {
            if (string.IsNullOrWhiteSpace(query)) return new List<Contact>();

            query = query.Trim().ToLower();
            var results = new HashSet<Contact>();

            // Email
            if (_emailIndex.ContainsKey(query))
                results.Add(_emailIndex[query]);

            // Phone
            if (_phoneIndex.ContainsKey(query))
                foreach (var c in _phoneIndex[query])
                    results.Add(c);

            // Name contains
            foreach (var kvp in _nameIndex)
            {
                if (kvp.Key.Contains(query))
                    foreach (var c in kvp.Value)
                        results.Add(c);
            }

            return results.ToList();
        }

        // Filter by date
        public List<Contact> FilterByDateRange(DateTime startDate, DateTime endDate)
            => _contacts.Where(c => c.CreationDate >= startDate && c.CreationDate <= endDate).ToList();

        // Save async
        public Task SaveAsync() => _storage.SaveContactsAsync(_contacts);
    }
}