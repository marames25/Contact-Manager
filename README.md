# Microsoft Internship 2026 - Contact Manager CLI

## Overview
This is a **Command Line Contact Manager** application built in **C#**, designed for the Microsoft Summer Internship 2026 project.  
The app allows adding, editing, deleting, viewing, listing, searching, filtering, and saving contacts using a **JSON storage system**.  

---

## Features

- Add new contacts with **Name, Phone, Email**  
- Edit existing contacts  
- Delete contacts  
- View a single contact by **Id**  
- List all contacts  
- Search contacts by **Name, Phone, or Email**  
- **Filter contacts by creation date**  
- Save contacts to JSON file  

### Validations
- Duplicate emails are **not allowed**  
- Name, Phone, Email cannot be empty  
- Phone format: **8–15 digits**  
- Email format validated  

---

## CLI Menu (Updated)

1. **Add Contact**  
2. **Edit Contact**  
3. **Delete Contact**  
4. **View Contact**  
5. **List Contacts**  
6. **Search Contacts**  
7. **Filter Contacts by Date**  
8. **Save Contacts**  
9. **Exit**  

---

## Requirements

- .NET 10.0 or above  
- Visual Studio Code or any C# IDE  
- JSON storage file is automatically created (`contacts.json`)  

---

## How to Run

1. Clone the repository:

```bash
git clone https://github.com/marames25/Contact-Manager.git
```

2. Navigate to the project folder
```bash
cd Contact-Manager
```

3. Build and run
```bash
dotnet run
```

4. Follow the CLI menu to manage contacts
