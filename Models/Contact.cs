using System;
namespace ContactManagerCLI.Models
{
    public class Contact
    {
        public Guid Id { get; set; }

        public required string Name { get; set; }
        public required string Phone { get; set; }
        public required string Email { get; set; }
        public DateTime CreationDate { get; set; }

        public Contact()
        {
            Id = Guid.NewGuid();
            CreationDate = DateTime.Now;
        }
    }
}