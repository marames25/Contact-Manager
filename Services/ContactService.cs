using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Net.Mail;
using ContactManagerCLI.Models;
using ContactManagerCLI.Storage;

namespace ContactManagerCLI.Services
{
    public class ContactService : IContactService
    {
        private List<Contact> _contacts;
        private readonly IStorage _storage;
        private int _nextId;

        public ContactService(IStorage storage)
        {
            _storage = storage ?? throw new ArgumentNullException(nameof(storage));
            _contacts = _storage.Load() ?? new List<Contact>();

            _nextId = _contacts.Any() ? _contacts.Max(c => c.Id) + 1 : 1;
        }

        // VALIDATION
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

        // ADD
        public void AddContact(Contact contact)
        {
            ValidateContact(contact);

            contact.Email = contact.Email.Trim();

            if (_contacts.Any(c =>
                c.Email.Equals(contact.Email, StringComparison.OrdinalIgnoreCase)))
            {
                throw new Exception("A contact with this email already exists!");
            }

            contact.Id = _nextId++;
            _contacts.Add(contact);
        }

        // EDIT 
        public void EditContact(int id, string name, string phone, string email)
        {
            var contact = GetContactById(id);
            if (contact == null)
                throw new Exception("Contact not found.");

            if (!string.IsNullOrWhiteSpace(email))
            {
                email = email.Trim();

                if (!IsValidEmail(email))
                    throw new Exception("Email format is invalid.");

                if (_contacts.Any(c =>
                    c.Email.Equals(email, StringComparison.OrdinalIgnoreCase)
                    && c.Id != id))
                {
                    throw new Exception("Another contact already has this email!");
                }

                contact.Email = email;
            }

            if (!string.IsNullOrWhiteSpace(name))
                contact.Name = name;

            if (!string.IsNullOrWhiteSpace(phone))
            {
                if (!IsValidPhone(phone))
                    throw new Exception("Phone format is invalid.");

                contact.Phone = phone;
            }
        }

        // DELETE 
        public void DeleteContact(int id)
        {
            var contact = GetContactById(id);
            if (contact == null)
                throw new Exception("Contact not found.");

            _contacts.Remove(contact);
        }

        // GET 
        public Contact? GetContactById(int id)
        {
            return _contacts.FirstOrDefault(c => c.Id == id);
        }

        public List<Contact> GetAllContacts()
        {
            return _contacts;
        }

        // SEARCH / FILTER 
        public List<Contact> SearchByField(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
                return new List<Contact>();

            query = query.Trim();

            return _contacts.Where(c =>
                c.Name.Contains(query, StringComparison.OrdinalIgnoreCase) ||
                c.Phone.Contains(query, StringComparison.OrdinalIgnoreCase) ||
                c.Email.Contains(query, StringComparison.OrdinalIgnoreCase)
            ).ToList();
        }

        public List<Contact> FilterByDateRange(DateTime startDate, DateTime endDate)
        {
            return _contacts.Where(c => 
                c.CreationDate >= startDate && c.CreationDate <= endDate
            ).ToList();
        }

        // SAVE
        public void Save()
        {
            _storage.Save(_contacts);
        }
    }
}