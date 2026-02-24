using System;
using ContactManagerCLI.Models;
using ContactManagerCLI.Services;
using ContactManagerCLI.Storage;

class Program
{
    static void Main()
    {
        IStorage storage = new JsonStorage("contacts.json");
        IContactService service = new ContactService(storage);

        while (true)
        {
            Console.WriteLine("\n--- Contact Manager CLI ---");
            Console.WriteLine("1. Add Contact");
            Console.WriteLine("2. Edit Contact");
            Console.WriteLine("3. Delete Contact");
            Console.WriteLine("4. View Contact");
            Console.WriteLine("5. List Contacts");
            Console.WriteLine("6. Search Contacts");
            Console.WriteLine("7. Filter by Date");
            Console.WriteLine("8. Save Contacts");
            Console.WriteLine("9. Exit");
            Console.Write("Enter choice: ");
            string choice = Console.ReadLine()?.Trim() ?? "";

            switch (choice)
            {
                case "1":
                    AddContactCLI(service);
                    break;
                case "2":
                    EditContactCLI(service);
                    break;
                case "3":
                    DeleteContactCLI(service);
                    break;
                case "4":
                    ViewContactCLI(service);
                    break;
                case "5":
                    ListContactsCLI(service);
                    break;
                case "6":
                    SearchContactsCLI(service);
                    break;
                case "7":
                    FilterContactsByDateCLI(service);
                    break;
                case "8":
                    service.Save();
                    Console.WriteLine("Contacts saved!");
                    break;
                case "9":
                    return;
                default:
                    Console.WriteLine("Invalid choice.");
                    break;
            }
        }
    }

    static void AddContactCLI(IContactService service)
    {
        Console.Write("Name: ");
        string name = Console.ReadLine()?.Trim() ?? "";
        Console.Write("Phone: ");
        string phone = Console.ReadLine()?.Trim() ?? "";
        Console.Write("Email: ");
        string email = Console.ReadLine()?.Trim() ?? "";

        try
        {
            var contact = new Contact
            {
                Name = name,
                Phone = phone,
                Email = email
            };
            service.AddContact(contact);
            Console.WriteLine("Contact added!");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
    }

    static void EditContactCLI(IContactService service)
    {
        Console.Write("Enter Contact Id: ");
        string idStr = Console.ReadLine()?.Trim() ?? "";
        if (!int.TryParse(idStr, out int id))
        {
            Console.WriteLine("Invalid Id format.");
            return;
        }

        var contact = service.GetContactById(id);
        if (contact == null)
        {
            Console.WriteLine("Contact not found.");
            return;
        }

        Console.Write("New Name (leave blank to keep): ");
        string name = Console.ReadLine()?.Trim() ?? "";
        Console.Write("New Phone (leave blank to keep): ");
        string phone = Console.ReadLine()?.Trim() ?? "";
        Console.Write("New Email (leave blank to keep): ");
        string email = Console.ReadLine()?.Trim() ?? "";

        try
        {
            service.EditContact(id, name, phone, email);
            Console.WriteLine("Contact updated!");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
    }

    static void DeleteContactCLI(IContactService service)
    {
        Console.Write("Enter Contact Id: ");
        string idStr = Console.ReadLine()?.Trim() ?? "";
        if (!int.TryParse(idStr, out int id))
        {
            Console.WriteLine("Invalid Id format.");
            return;
        }

        var contact = service.GetContactById(id);
        if (contact == null)
        {
            Console.WriteLine("Contact not found.");
            return;
        }

        service.DeleteContact(id);
        Console.WriteLine("Contact deleted!");
    }

    static void ViewContactCLI(IContactService service)
    {
        Console.Write("Enter Contact Id: ");
        string idStr = Console.ReadLine()?.Trim() ?? "";
        if (!int.TryParse(idStr, out int id))
        {
            Console.WriteLine("Invalid Id format.");
            return;
        }

        var contact = service.GetContactById(id);
        if (contact == null)
        {
            Console.WriteLine("Contact not found.");
            return;
        }

        Console.WriteLine($"\nId: {contact.Id}");
        Console.WriteLine($"Name: {contact.Name}");
        Console.WriteLine($"Phone: {contact.Phone}");
        Console.WriteLine($"Email: {contact.Email}");
        Console.WriteLine($"Created: {contact.CreationDate}");
    }

    static void ListContactsCLI(IContactService service)
    {
        var contacts = service.GetAllContacts();
        if (contacts.Count == 0)
        {
            Console.WriteLine("No contacts found.");
            return;
        }

        Console.WriteLine($"\n--- All Contacts ({contacts.Count}) ---");
        foreach (var c in contacts)
        {
            Console.WriteLine($"{c.Id} | {c.Name} | {c.Phone} | {c.Email} | {c.CreationDate}");
        }
    }

    static void SearchContactsCLI(IContactService service)
    {
        Console.Write("Enter search query: ");
        string query = Console.ReadLine()?.Trim() ?? "";

        var results = service.SearchByField(query);
        if (results.Count == 0)
        {
            Console.WriteLine("No contacts found.");
            return;
        }

        Console.WriteLine($"\n--- Search Results ({results.Count}) ---");
        foreach (var c in results)
        {
            Console.WriteLine($"{c.Id} | {c.Name} | {c.Phone} | {c.Email} | {c.CreationDate}");
        }
    }

    static void FilterContactsByDateCLI(IContactService service)
    {
        Console.Write("Enter start date (yyyy-MM-dd): ");
        string startStr = Console.ReadLine()?.Trim() ?? "";
        Console.Write("Enter end date (yyyy-MM-dd): ");
        string endStr = Console.ReadLine()?.Trim() ?? "";

        if (!DateTime.TryParse(startStr, out DateTime startDate))
        {
            Console.WriteLine("Invalid start date format.");
            return;
        }

        if (!DateTime.TryParse(endStr, out DateTime endDate))
        {
            Console.WriteLine("Invalid end date format.");
            return;
        }

        endDate = endDate.Date.AddDays(1).AddSeconds(-1);

        var results = service.FilterByDateRange(startDate, endDate);
        if (results.Count == 0)
        {
            Console.WriteLine("No contacts found in the specified date range.");
            return;
        }

        Console.WriteLine($"\n--- Filtered Contacts ({results.Count}) ---");
        foreach (var c in results)
        {
            Console.WriteLine($"{c.Id} | {c.Name} | {c.Phone} | {c.Email} | {c.CreationDate}");
        }
    }
}
