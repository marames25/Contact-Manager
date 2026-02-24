# Microsoft Internship 2026 - Contact Manager CLI

## Overview
This is a **Command Line Contact Manager** application built in **C#**, designed for the Microsoft Summer Internship 2026 project.  
The app allows adding, editing, deleting, viewing, listing, searching, and saving contacts using a **JSON storage system**.  

---

## Features

- Add new contacts with Name, Phone, Email
- Edit existing contacts
- Delete contacts
- View a single contact
- List all contacts
- Search contacts by Name, Phone, or Email
- Save contacts to JSON file
- Validations:
  - Duplicate emails are not allowed
  - Name, Phone, Email cannot be empty
  - Phone format: 8–15 digits
  - Email format validated

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
cd "your folder"
```

3. Build and run
```bash
dotnet run
```

4. Follow the CLI menu to manage contacts
