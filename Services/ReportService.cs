using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using InventoryQRManager.Data;
using InventoryQRManager.Models;

namespace InventoryQRManager.Services
{
    public class ReportService
    {
        private readonly DatabaseContext _dbContext;

        public ReportService()
        {
            _dbContext = new DatabaseContext();
        }

        public class InventorySummary
        {
            public int TotalItems { get; set; }
            public int TotalQuantity { get; set; }
            public decimal TotalValue { get; set; }
            public int CategoriesCount { get; set; }
            public int LocationsCount { get; set; }
        }

        public class CategorySummary
        {
            public string Category { get; set; } = string.Empty;
            public int ItemsCount { get; set; }
            public int TotalQuantity { get; set; }
            public decimal TotalValue { get; set; }
            public decimal AveragePrice { get; set; }
        }

        public class LocationSummary
        {
            public string Location { get; set; } = string.Empty;
            public int ItemsCount { get; set; }
            public int TotalQuantity { get; set; }
            public decimal TotalValue { get; set; }
        }

        public class LowStockItem
        {
            public int Id { get; set; }
            public string Name { get; set; } = string.Empty;
            public string SKU { get; set; } = string.Empty;
            public int Quantity { get; set; }
            public string Category { get; set; } = string.Empty;
            public string Location { get; set; } = string.Empty;
        }

        public InventorySummary GetInventorySummary()
        {
            using var connection = _dbContext.GetConnection();
            connection.Open();

            var query = @"
                SELECT 
                    COUNT(*) as TotalItems,
                    SUM(Quantity) as TotalQuantity,
                    SUM(Quantity * Price) as TotalValue,
                    COUNT(DISTINCT Category) as CategoriesCount,
                    COUNT(DISTINCT Location) as LocationsCount
                FROM InventoryItems 
                WHERE IsActive = 1";

            using var command = new SQLiteCommand(query, connection);
            using var reader = command.ExecuteReader();

            if (reader.Read())
            {
                return new InventorySummary
                {
                    TotalItems = Convert.ToInt32(reader["TotalItems"]),
                    TotalQuantity = Convert.ToInt32(reader["TotalQuantity"] ?? 0),
                    TotalValue = Convert.ToDecimal(reader["TotalValue"] ?? 0),
                    CategoriesCount = Convert.ToInt32(reader["CategoriesCount"]),
                    LocationsCount = Convert.ToInt32(reader["LocationsCount"])
                };
            }

            return new InventorySummary();
        }

        public List<CategorySummary> GetCategorySummary()
        {
            var summaries = new List<CategorySummary>();
            using var connection = _dbContext.GetConnection();
            connection.Open();

            var query = @"
                SELECT 
                    Category,
                    COUNT(*) as ItemsCount,
                    SUM(Quantity) as TotalQuantity,
                    SUM(Quantity * Price) as TotalValue,
                    AVG(Price) as AveragePrice
                FROM InventoryItems 
                WHERE IsActive = 1 AND Category IS NOT NULL AND Category != ''
                GROUP BY Category
                ORDER BY TotalValue DESC";

            using var command = new SQLiteCommand(query, connection);
            using var reader = command.ExecuteReader();

            while (reader.Read())
            {
                summaries.Add(new CategorySummary
                {
                    Category = reader["Category"].ToString() ?? string.Empty,
                    ItemsCount = Convert.ToInt32(reader["ItemsCount"]),
                    TotalQuantity = Convert.ToInt32(reader["TotalQuantity"] ?? 0),
                    TotalValue = Convert.ToDecimal(reader["TotalValue"] ?? 0),
                    AveragePrice = Convert.ToDecimal(reader["AveragePrice"] ?? 0)
                });
            }

            return summaries;
        }

        public List<LocationSummary> GetLocationSummary()
        {
            var summaries = new List<LocationSummary>();
            using var connection = _dbContext.GetConnection();
            connection.Open();

            var query = @"
                SELECT 
                    Location,
                    COUNT(*) as ItemsCount,
                    SUM(Quantity) as TotalQuantity,
                    SUM(Quantity * Price) as TotalValue
                FROM InventoryItems 
                WHERE IsActive = 1 AND Location IS NOT NULL AND Location != ''
                GROUP BY Location
                ORDER BY TotalValue DESC";

            using var command = new SQLiteCommand(query, connection);
            using var reader = command.ExecuteReader();

            while (reader.Read())
            {
                summaries.Add(new LocationSummary
                {
                    Location = reader["Location"].ToString() ?? string.Empty,
                    ItemsCount = Convert.ToInt32(reader["ItemsCount"]),
                    TotalQuantity = Convert.ToInt32(reader["TotalQuantity"] ?? 0),
                    TotalValue = Convert.ToDecimal(reader["TotalValue"] ?? 0)
                });
            }

            return summaries;
        }

        public List<LowStockItem> GetLowStockItems(int threshold = 10)
        {
            var items = new List<LowStockItem>();
            using var connection = _dbContext.GetConnection();
            connection.Open();

            var query = @"
                SELECT Id, Name, SKU, Quantity, Category, Location
                FROM InventoryItems 
                WHERE IsActive = 1 AND Quantity <= @threshold
                ORDER BY Quantity ASC";

            using var command = new SQLiteCommand(query, connection);
            command.Parameters.AddWithValue("@threshold", threshold);
            using var reader = command.ExecuteReader();

            while (reader.Read())
            {
                items.Add(new LowStockItem
                {
                    Id = Convert.ToInt32(reader["Id"]),
                    Name = reader["Name"].ToString() ?? string.Empty,
                    SKU = reader["SKU"].ToString() ?? string.Empty,
                    Quantity = Convert.ToInt32(reader["Quantity"]),
                    Category = reader["Category"].ToString() ?? string.Empty,
                    Location = reader["Location"].ToString() ?? string.Empty
                });
            }

            return items;
        }

        public List<InventoryItem> GetItemsCreatedInPeriod(DateTime startDate, DateTime endDate)
        {
            var items = new List<InventoryItem>();
            using var connection = _dbContext.GetConnection();
            connection.Open();

            var query = @"
                SELECT * FROM InventoryItems 
                WHERE IsActive = 1 
                AND CreatedDate >= @startDate 
                AND CreatedDate <= @endDate
                ORDER BY CreatedDate DESC";

            using var command = new SQLiteCommand(query, connection);
            command.Parameters.AddWithValue("@startDate", startDate.ToString("yyyy-MM-dd HH:mm:ss"));
            command.Parameters.AddWithValue("@endDate", endDate.ToString("yyyy-MM-dd HH:mm:ss"));
            using var reader = command.ExecuteReader();

            while (reader.Read())
            {
                items.Add(MapReaderToInventoryItem(reader));
            }

            return items;
        }

        public List<InventoryItem> SearchItems(string searchTerm)
        {
            var items = new List<InventoryItem>();
            using var connection = _dbContext.GetConnection();
            connection.Open();

            var query = @"
                SELECT * FROM InventoryItems 
                WHERE IsActive = 1 
                AND (Name LIKE @searchTerm 
                     OR Description LIKE @searchTerm 
                     OR SKU LIKE @searchTerm 
                     OR Category LIKE @searchTerm 
                     OR Location LIKE @searchTerm)
                ORDER BY Name";

            using var command = new SQLiteCommand(query, connection);
            command.Parameters.AddWithValue("@searchTerm", $"%{searchTerm}%");
            using var reader = command.ExecuteReader();

            while (reader.Read())
            {
                items.Add(MapReaderToInventoryItem(reader));
            }

            return items;
        }

        private InventoryItem MapReaderToInventoryItem(SQLiteDataReader reader)
        {
            return new InventoryItem
            {
                Id = Convert.ToInt32(reader["Id"]),
                Name = reader["Name"].ToString() ?? string.Empty,
                Description = reader["Description"].ToString() ?? string.Empty,
                SKU = reader["SKU"].ToString() ?? string.Empty,
                QRCode = reader["QRCode"].ToString() ?? string.Empty,
                Quantity = Convert.ToInt32(reader["Quantity"]),
                Price = Convert.ToDecimal(reader["Price"]),
                Category = reader["Category"].ToString() ?? string.Empty,
                Location = reader["Location"].ToString() ?? string.Empty,
                CreatedDate = DateTime.Parse(reader["CreatedDate"].ToString() ?? DateTime.Now.ToString()),
                LastUpdated = reader["LastUpdated"] != DBNull.Value ? DateTime.Parse(reader["LastUpdated"].ToString()!) : null,
                IsActive = Convert.ToBoolean(reader["IsActive"])
            };
        }
    }
}
