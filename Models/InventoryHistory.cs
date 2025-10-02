using System;

namespace InventoryQRManager.Models
{
    /// <summary>
    
    /// </summary>
    public class InventoryHistory
    {
        public int Id { get; set; }
        public int ItemId { get; set; }
        public string ItemName { get; set; } = string.Empty;
        public string ItemSKU { get; set; } = string.Empty;
        public HistoryAction Action { get; set; }
        public DateTime Timestamp { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string Details { get; set; } = string.Empty;
        public string OldValue { get; set; } = string.Empty;
        public string NewValue { get; set; } = string.Empty;
        public int QuantityChange { get; set; }
        public decimal PriceChange { get; set; }
        public string Category { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
    }

    /// <summary>
   
    /// </summary>
    public enum HistoryAction
    {
        CREATE,         
        UPDATE,        
        DELETE,         
        QUANTITY_ADD,  
        QUANTITY_REMOVE, 
        PRICE_CHANGE,   
        LOCATION_CHANGE, 
        CATEGORY_CHANGE 
    }
}

