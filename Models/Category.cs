using System;

namespace InventoryQRManager.Models
{
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Color { get; set; } = "#007bff"; 
        public bool IsActive { get; set; } = true;
        public DateTime CreatedDate { get; set; }

        public Category()
        {
            CreatedDate = DateTime.Now;
        }

        public override string ToString()
        {
            return Name;
        }
    }
}

