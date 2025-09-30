using System;

namespace InventoryQRManager.Models
{
    public class Location
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;
        public DateTime CreatedDate { get; set; }

        public Location()
        {
            CreatedDate = DateTime.Now;
        }

        public override string ToString()
        {
            return Name;
        }
    }
}

