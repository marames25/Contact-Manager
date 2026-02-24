using System.Collections.Generic;
using ContactManagerCLI.Models;

namespace ContactManagerCLI.Storage
{
    public interface IStorage
    {
        List<Contact>? Load();
        void Save(List<Contact> contacts);
    }
}