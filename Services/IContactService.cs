using System;
using System.Collections.Generic;
using ContactManagerCLI.Models;

namespace ContactManagerCLI.Services
{
    public interface IContactService
    {
        void AddContact(Contact contact);
        void EditContact(int id, string name, string phone, string email);
        void DeleteContact(int id);
        Contact? GetContactById(int id);
        List<Contact> GetAllContacts();
        List<Contact> SearchByField(string query); 
        List<Contact> FilterByDateRange(DateTime startDate, DateTime endDate);
        void Save();
    }
}