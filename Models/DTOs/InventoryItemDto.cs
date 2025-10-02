using System.ComponentModel.DataAnnotations;

namespace InventoryQRManager.Models.DTOs
{
    public class InventoryItemDto
    {
        public int Id { get; set; }
        
        [Required]
        [StringLength(200)]
        public string Name { get; set; } = string.Empty;
        
        [StringLength(500)]
        public string Description { get; set; } = string.Empty;
        
        [Required]
        [StringLength(50)]
        public string SKU { get; set; } = string.Empty;
        
        public string QRCode { get; set; } = string.Empty;
        
        [Range(0, int.MaxValue)]
        public int Quantity { get; set; }
        
        [Range(0, double.MaxValue)]
        public decimal Price { get; set; }
        
        [StringLength(100)]
        public string Category { get; set; } = string.Empty;
        
        [StringLength(100)]
        public string Location { get; set; } = string.Empty;
        
        public DateTime CreatedDate { get; set; }
        public DateTime? LastUpdated { get; set; }
        public bool IsActive { get; set; } = true;
    }

    public class CreateInventoryItemDto
    {
        [Required]
        [StringLength(200)]
        public string Name { get; set; } = string.Empty;
        
        [StringLength(500)]
        public string Description { get; set; } = string.Empty;
        
        [Required]
        [StringLength(50)]
        public string SKU { get; set; } = string.Empty;
        
        [Range(0, int.MaxValue)]
        public int Quantity { get; set; }
        
        [Range(0, double.MaxValue)]
        public decimal Price { get; set; }
        
        [StringLength(100)]
        public string Category { get; set; } = string.Empty;
        
        [StringLength(100)]
        public string Location { get; set; } = string.Empty;
    }

    public class UpdateInventoryItemDto
    {
        [Required]
        [StringLength(200)]
        public string Name { get; set; } = string.Empty;
        
        [StringLength(500)]
        public string Description { get; set; } = string.Empty;
        
        [Required]
        [StringLength(50)]
        public string SKU { get; set; } = string.Empty;
        
        [Range(0, int.MaxValue)]
        public int Quantity { get; set; }
        
        [Range(0, double.MaxValue)]
        public decimal Price { get; set; }
        
        [StringLength(100)]
        public string Category { get; set; } = string.Empty;
        
        [StringLength(100)]
        public string Location { get; set; } = string.Empty;
    }

    public class InventorySummaryDto
    {
        public int TotalItems { get; set; }
        public int TotalQuantity { get; set; }
        public decimal TotalValue { get; set; }
        public int CategoriesCount { get; set; }
        public int LocationsCount { get; set; }
        public int LowStockItems { get; set; }
    }

    public class ApiResponse<T>
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public T? Data { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.Now;
    }
}
