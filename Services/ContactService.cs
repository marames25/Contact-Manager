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

        public ContactService(IStorage storage)
        {
            _storage = storage ?? throw new ArgumentNullException(nameof(storage));
            _contacts = _storage.Load() ?? new List<Contact>();
        }

        #region Validations
        private void ValidateContact(Contact contact)
        {
            if (string.IsNullOrWhiteSpace(contact.Name))
                throw new ArgumentException("Name cannot be empty.");

            if (string.IsNullOrWhiteSpace(contact.Phone))
                throw new ArgumentException("Phone cannot be empty.");

            if (string.IsNullOrWhiteSpace(contact.Email))
                throw new ArgumentException("Email cannot be empty.");

            if (!IsValidEmail(contact.Email))
                throw new ArgumentException("Email format is invalid.");

            if (!IsValidPhone(contact.Phone))
                throw new ArgumentException("Phone format is invalid.");
        }

        private bool IsValidEmail(string email)
        {
            try
            {
                var addr = new MailAddress(email);
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
        #endregion

        #region Add / Edit / Delete
        public void AddContact(Contact contact)
        {
            ValidateContact(contact);

            // Trim email to avoid accidental duplicates
            contact.Email = contact.Email.Trim();

            if (_contacts.Any(c => c.Email.Equals(contact.Email, StringComparison.OrdinalIgnoreCase)))
            {
                throw new Exception("A contact with this email already exists!");
            }

            _contacts.Add(contact);
        }

        public void EditContact(Guid id, string name, string phone, string email)
        {
            var contact = GetContactById(id);
            if (contact == null)
                throw new Exception("Contact not found.");

            // Update Email with duplicate check
            if (!string.IsNullOrWhiteSpace(email))
            {
                string trimmedEmail = email.Trim();
                if (_contacts.Any(c => c.Email.Equals(trimmedEmail, StringComparison.OrdinalIgnoreCase) && c.Id != id))
                    throw new Exception("Another contact already has this email!");
                if (!IsValidEmail(trimmedEmail))
                    throw new Exception("Email format is invalid.");
                contact.Email = trimmedEmail;
            }

            // Update Name and Phone
            if (!string.IsNullOrWhiteSpace(name)) contact.Name = name;
            if (!string.IsNullOrWhiteSpace(phone))
            {
                if (!IsValidPhone(phone))
                    throw new Exception("Phone format is invalid.");
                contact.Phone = phone;
            }
        }

        public void DeleteContact(Guid id)
        {
            var c = _contacts.FirstOrDefault(x => x.Id == id);
            if (c != null)
                _contacts.Remove(c);
        }
        #endregion

        #region Get / Search
        public Contact? GetContactById(Guid id)
        {
            return _contacts.FirstOrDefault(c => c.Id == id);
        }

        public List<Contact> GetAllContacts()
        {
            return _contacts;
        }

        public List<Contact> SearchByField(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
                return new List<Contact>();

            query = query.Trim();

            return _contacts
                .Where(c =>
                    (!string.IsNullOrEmpty(c.Name) && c.Name.Contains(query, StringComparison.OrdinalIgnoreCase)) ||
                    (!string.IsNullOrEmpty(c.Phone) && c.Phone.Contains(query, StringComparison.OrdinalIgnoreCase)) ||
                    (!string.IsNullOrEmpty(c.Email) && c.Email.Contains(query, StringComparison.OrdinalIgnoreCase))
                ).ToList();
        }
        #endregion

        #region Save
        public void Save()
        {
            _storage.Save(_contacts);
        }
        #endregion
    }
}