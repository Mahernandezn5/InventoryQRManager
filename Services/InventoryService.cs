using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using InventoryQRManager.Data;
using InventoryQRManager.Models;

namespace InventoryQRManager.Services
{
    public class InventoryService
    {
        private readonly DatabaseContext _dbContext;

        public InventoryService()
        {
            _dbContext = new DatabaseContext();
        }

        public List<InventoryItem> GetAllItems()
        {
            try
            {
                var items = new List<InventoryItem>();
                using var connection = _dbContext.GetConnection();
                connection.Open();

                var query = "SELECT * FROM InventoryItems WHERE IsActive = 1 ORDER BY Name";
                using var command = new SQLiteCommand(query, connection);
                using var reader = command.ExecuteReader();

                while (reader.Read())
                {
                    items.Add(MapReaderToInventoryItem(reader));
                }

                return items;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error obteniendo todos los items: {ex.Message}");
            }
        }

        public InventoryItem? GetItemById(int id)
        {
            try
            {
                using var connection = _dbContext.GetConnection();
                connection.Open();

                var query = "SELECT * FROM InventoryItems WHERE Id = @id AND IsActive = 1";
                using var command = new SQLiteCommand(query, connection);
                command.Parameters.AddWithValue("@id", id);
                using var reader = command.ExecuteReader();

                if (reader.Read())
                {
                    return MapReaderToInventoryItem(reader);
                }

                return null;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error obteniendo item por ID: {ex.Message}");
            }
        }

        public InventoryItem? GetItemBySKU(string sku)
        {
            try
            {
                using var connection = _dbContext.GetConnection();
                connection.Open();

                var query = "SELECT * FROM InventoryItems WHERE SKU = @sku AND IsActive = 1";
                using var command = new SQLiteCommand(query, connection);
                command.Parameters.AddWithValue("@sku", sku);
                using var reader = command.ExecuteReader();

                if (reader.Read())
                {
                    return MapReaderToInventoryItem(reader);
                }

                return null;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error obteniendo item por SKU: {ex.Message}");
            }
        }

        public InventoryItem? GetItemByQRCode(string qrCode)
        {
            using var connection = _dbContext.GetConnection();
            connection.Open();

            var query = "SELECT * FROM InventoryItems WHERE QRCode = @qrCode AND IsActive = 1";
            using var command = new SQLiteCommand(query, connection);
            command.Parameters.AddWithValue("@qrCode", qrCode);
            using var reader = command.ExecuteReader();

            if (reader.Read())
            {
                return MapReaderToInventoryItem(reader);
            }

            return null;
        }

        public bool AddItem(InventoryItem item)
        {
            try
            {
                if (item == null)
                {
                    throw new ArgumentNullException(nameof(item), "El item no puede ser null");
                }

                using var connection = _dbContext.GetConnection();
                connection.Open();

                var query = @"
                    INSERT INTO InventoryItems 
                    (Name, Description, SKU, QRCode, Quantity, Price, Category, Location, CreatedDate, IsActive)
                    VALUES 
                    (@name, @description, @sku, @qrCode, @quantity, @price, @category, @location, @createdDate, 1)";

                using var command = new SQLiteCommand(query, connection);
                command.Parameters.AddWithValue("@name", item.Name ?? string.Empty);
                command.Parameters.AddWithValue("@description", item.Description ?? string.Empty);
                command.Parameters.AddWithValue("@sku", item.SKU ?? string.Empty);
                command.Parameters.AddWithValue("@qrCode", item.QRCode ?? string.Empty);
                command.Parameters.AddWithValue("@quantity", item.Quantity);
                command.Parameters.AddWithValue("@price", item.Price);
                command.Parameters.AddWithValue("@category", item.Category ?? string.Empty);
                command.Parameters.AddWithValue("@location", item.Location ?? string.Empty);
                command.Parameters.AddWithValue("@createdDate", item.CreatedDate.ToString("yyyy-MM-dd HH:mm:ss"));

                return command.ExecuteNonQuery() > 0;
            }
            catch (Exception ex)
            {
                // Log the exception for debugging
                System.Diagnostics.Debug.WriteLine($"Error adding item: {ex.Message}");
                throw new Exception($"Error agregando item: {ex.Message}");
            }
        }

        public bool UpdateItem(InventoryItem item)
        {
            try
            {
                using var connection = _dbContext.GetConnection();
                connection.Open();

                var query = @"
                    UPDATE InventoryItems 
                    SET Name = @name, Description = @description, SKU = @sku, QRCode = @qrCode,
                        Quantity = @quantity, Price = @price, Category = @category, Location = @location,
                        LastUpdated = @lastUpdated
                    WHERE Id = @id";

                using var command = new SQLiteCommand(query, connection);
                command.Parameters.AddWithValue("@id", item.Id);
                command.Parameters.AddWithValue("@name", item.Name);
                command.Parameters.AddWithValue("@description", item.Description);
                command.Parameters.AddWithValue("@sku", item.SKU);
                command.Parameters.AddWithValue("@qrCode", item.QRCode);
                command.Parameters.AddWithValue("@quantity", item.Quantity);
                command.Parameters.AddWithValue("@price", item.Price);
                command.Parameters.AddWithValue("@category", item.Category);
                command.Parameters.AddWithValue("@location", item.Location);
                command.Parameters.AddWithValue("@lastUpdated", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));

                return command.ExecuteNonQuery() > 0;
            }
            catch (Exception ex)
            {
                // Log the exception for debugging
                System.Diagnostics.Debug.WriteLine($"Error updating item: {ex.Message}");
                throw; // Re-throw to allow proper error handling in the UI
            }
        }

        public bool DeleteItem(int id)
        {
            try
            {
                using var connection = _dbContext.GetConnection();
                connection.Open();

                var query = "UPDATE InventoryItems SET IsActive = 0 WHERE Id = @id";
                using var command = new SQLiteCommand(query, connection);
                command.Parameters.AddWithValue("@id", id);

                return command.ExecuteNonQuery() > 0;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public List<string> GetCategories()
        {
            var categories = new List<string>();
            using var connection = _dbContext.GetConnection();
            connection.Open();

            var query = "SELECT DISTINCT Category FROM InventoryItems WHERE IsActive = 1 AND Category IS NOT NULL AND Category != ''";
            using var command = new SQLiteCommand(query, connection);
            using var reader = command.ExecuteReader();

            while (reader.Read())
            {
                categories.Add(reader["Category"].ToString() ?? string.Empty);
            }

            return categories;
        }

        public List<string> GetLocations()
        {
            var locations = new List<string>();
            using var connection = _dbContext.GetConnection();
            connection.Open();

            var query = "SELECT DISTINCT Location FROM InventoryItems WHERE IsActive = 1 AND Location IS NOT NULL AND Location != ''";
            using var command = new SQLiteCommand(query, connection);
            using var reader = command.ExecuteReader();

            while (reader.Read())
            {
                locations.Add(reader["Location"].ToString() ?? string.Empty);
            }

            return locations;
        }

        private InventoryItem MapReaderToInventoryItem(SQLiteDataReader reader)
        {
            try
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
            catch (Exception ex)
            {
                throw new Exception($"Error mapeando datos del item: {ex.Message}");
            }
        }
    }
}

