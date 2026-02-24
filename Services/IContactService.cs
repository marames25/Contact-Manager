using System;
using System.Collections.Generic;
using ContactManagerCLI.Models;

namespace ContactManagerCLI.Services
{
    public interface IContactService
    {
        void AddContact(Contact contact);
        void EditContact(Guid id, string name, string phone, string email);
        void DeleteContact(Guid id);
        Contact? GetContactById(Guid id);
        List<Contact> GetAllContacts();
        List<Contact> SearchByField(string query); 
        void Save();
    }
}