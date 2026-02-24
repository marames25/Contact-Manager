using System.Collections.Generic;
using System.Threading.Tasks;
using ContactManagerCLI.Models;

namespace ContactManagerCLI.Storage
{
    public interface IStorage
    {
        List<Contact> LoadContacts();              //  sync method
        Task<List<Contact>> LoadContactsAsync();   // async method
        void SaveContacts(List<Contact> contacts); // sync save
        Task SaveContactsAsync(List<Contact> contacts); // async save
    }
}