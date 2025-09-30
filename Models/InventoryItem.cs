using System;

namespace InventoryQRManager.Models
{
    public class InventoryItem
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string SKU { get; set; } = string.Empty;
        public string QRCode { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public string Category { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; }
        public DateTime? LastUpdated { get; set; }
        public bool IsActive { get; set; } = true;

        public InventoryItem()
        {
            CreatedDate = DateTime.Now;
        }

        public override string ToString()
        {
            return $"{Name} (SKU: {SKU}) - Cantidad: {Quantity}";
        }
    }
}

