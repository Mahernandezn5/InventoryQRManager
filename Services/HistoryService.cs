using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using InventoryQRManager.Models;

namespace InventoryQRManager.Services
{

    public class HistoryService
    {
        private readonly string _connectionString;
        private readonly string _currentUser;

        public HistoryService()
        {
            _connectionString = "Data Source=inventory.db;Version=3;";
            _currentUser = Environment.UserName;
            InitializeDatabase();
        }


        private void InitializeDatabase()
        {
            try
            {
                using var connection = new SQLiteConnection(_connectionString);
                connection.Open();

                var createTableQuery = @"
                    CREATE TABLE IF NOT EXISTS InventoryHistory (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        ItemId INTEGER NOT NULL,
                        ItemName TEXT NOT NULL,
                        ItemSKU TEXT NOT NULL,
                        Action TEXT NOT NULL,
                        Timestamp DATETIME NOT NULL,
                        UserName TEXT NOT NULL,
                        Details TEXT,
                        OldValue TEXT,
                        NewValue TEXT,
                        QuantityChange INTEGER DEFAULT 0,
                        PriceChange DECIMAL DEFAULT 0,
                        Category TEXT,
                        Location TEXT
                    )";

                using var command = new SQLiteCommand(createTableQuery, connection);
                command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error inicializando tabla de historial: {ex.Message}");
            }
        }


        public bool RecordAction(InventoryItem item, HistoryAction action, string details = "", 
            string oldValue = "", string newValue = "", int quantityChange = 0, decimal priceChange = 0)
        {
            try
            {
                using var connection = new SQLiteConnection(_connectionString);
                connection.Open();

                var insertQuery = @"
                    INSERT INTO InventoryHistory 
                    (ItemId, ItemName, ItemSKU, Action, Timestamp, UserName, Details, 
                     OldValue, NewValue, QuantityChange, PriceChange, Category, Location)
                    VALUES 
                    (@ItemId, @ItemName, @ItemSKU, @Action, @Timestamp, @UserName, @Details,
                     @OldValue, @NewValue, @QuantityChange, @PriceChange, @Category, @Location)";

                using var command = new SQLiteCommand(insertQuery, connection);
                command.Parameters.AddWithValue("@ItemId", item.Id);
                command.Parameters.AddWithValue("@ItemName", item.Name);
                command.Parameters.AddWithValue("@ItemSKU", item.SKU);
                command.Parameters.AddWithValue("@Action", action.ToString());
                command.Parameters.AddWithValue("@Timestamp", DateTime.Now);
                command.Parameters.AddWithValue("@UserName", _currentUser);
                command.Parameters.AddWithValue("@Details", details);
                command.Parameters.AddWithValue("@OldValue", oldValue);
                command.Parameters.AddWithValue("@NewValue", newValue);
                command.Parameters.AddWithValue("@QuantityChange", quantityChange);
                command.Parameters.AddWithValue("@PriceChange", priceChange);
                command.Parameters.AddWithValue("@Category", item.Category);
                command.Parameters.AddWithValue("@Location", item.Location);

                command.ExecuteNonQuery();
                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error registrando acción en historial: {ex.Message}");
                return false;
            }
        }


        public List<InventoryHistory> GetAllHistory()
        {
            var history = new List<InventoryHistory>();

            try
            {
                using var connection = new SQLiteConnection(_connectionString);
                connection.Open();

                var query = @"
                    SELECT Id, ItemId, ItemName, ItemSKU, Action, Timestamp, UserName, 
                           Details, OldValue, NewValue, QuantityChange, PriceChange, 
                           Category, Location
                    FROM InventoryHistory 
                    ORDER BY Timestamp DESC";

                using var command = new SQLiteCommand(query, connection);
                using var reader = command.ExecuteReader();

                while (reader.Read())
                {
                    history.Add(new InventoryHistory
                    {
                        Id = reader.GetInt32("Id"),
                        ItemId = reader.GetInt32("ItemId"),
                        ItemName = reader.GetString("ItemName"),
                        ItemSKU = reader.GetString("ItemSKU"),
                        Action = Enum.Parse<HistoryAction>(reader.GetString("Action")),
                        Timestamp = reader.GetDateTime("Timestamp"),
                        UserName = reader.GetString("UserName"),
                        Details = reader.IsDBNull("Details") ? "" : reader.GetString("Details"),
                        OldValue = reader.IsDBNull("OldValue") ? "" : reader.GetString("OldValue"),
                        NewValue = reader.IsDBNull("NewValue") ? "" : reader.GetString("NewValue"),
                        QuantityChange = reader.GetInt32("QuantityChange"),
                        PriceChange = reader.GetDecimal("PriceChange"),
                        Category = reader.IsDBNull("Category") ? "" : reader.GetString("Category"),
                        Location = reader.IsDBNull("Location") ? "" : reader.GetString("Location")
                    });
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error obteniendo historial: {ex.Message}");
            }

            return history;
        }


        public List<InventoryHistory> GetItemHistory(int itemId)
        {
            var history = new List<InventoryHistory>();

            try
            {
                using var connection = new SQLiteConnection(_connectionString);
                connection.Open();

                var query = @"
                    SELECT Id, ItemId, ItemName, ItemSKU, Action, Timestamp, UserName, 
                           Details, OldValue, NewValue, QuantityChange, PriceChange, 
                           Category, Location
                    FROM InventoryHistory 
                    WHERE ItemId = @ItemId
                    ORDER BY Timestamp DESC";

                using var command = new SQLiteCommand(query, connection);
                command.Parameters.AddWithValue("@ItemId", itemId);
                using var reader = command.ExecuteReader();

                while (reader.Read())
                {
                    history.Add(new InventoryHistory
                    {
                        Id = reader.GetInt32("Id"),
                        ItemId = reader.GetInt32("ItemId"),
                        ItemName = reader.GetString("ItemName"),
                        ItemSKU = reader.GetString("ItemSKU"),
                        Action = Enum.Parse<HistoryAction>(reader.GetString("Action")),
                        Timestamp = reader.GetDateTime("Timestamp"),
                        UserName = reader.GetString("UserName"),
                        Details = reader.IsDBNull("Details") ? "" : reader.GetString("Details"),
                        OldValue = reader.IsDBNull("OldValue") ? "" : reader.GetString("OldValue"),
                        NewValue = reader.IsDBNull("NewValue") ? "" : reader.GetString("NewValue"),
                        QuantityChange = reader.GetInt32("QuantityChange"),
                        PriceChange = reader.GetDecimal("PriceChange"),
                        Category = reader.IsDBNull("Category") ? "" : reader.GetString("Category"),
                        Location = reader.IsDBNull("Location") ? "" : reader.GetString("Location")
                    });
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error obteniendo historial del item: {ex.Message}");
            }

            return history;
        }


        public List<InventoryHistory> GetHistoryByDateRange(DateTime startDate, DateTime endDate)
        {
            var history = new List<InventoryHistory>();

            try
            {
                using var connection = new SQLiteConnection(_connectionString);
                connection.Open();

                var query = @"
                    SELECT Id, ItemId, ItemName, ItemSKU, Action, Timestamp, UserName, 
                           Details, OldValue, NewValue, QuantityChange, PriceChange, 
                           Category, Location
                    FROM InventoryHistory 
                    WHERE Timestamp BETWEEN @StartDate AND @EndDate
                    ORDER BY Timestamp DESC";

                using var command = new SQLiteCommand(query, connection);
                command.Parameters.AddWithValue("@StartDate", startDate);
                command.Parameters.AddWithValue("@EndDate", endDate);
                using var reader = command.ExecuteReader();

                while (reader.Read())
                {
                    history.Add(new InventoryHistory
                    {
                        Id = reader.GetInt32("Id"),
                        ItemId = reader.GetInt32("ItemId"),
                        ItemName = reader.GetString("ItemName"),
                        ItemSKU = reader.GetString("ItemSKU"),
                        Action = Enum.Parse<HistoryAction>(reader.GetString("Action")),
                        Timestamp = reader.GetDateTime("Timestamp"),
                        UserName = reader.GetString("UserName"),
                        Details = reader.IsDBNull("Details") ? "" : reader.GetString("Details"),
                        OldValue = reader.IsDBNull("OldValue") ? "" : reader.GetString("OldValue"),
                        NewValue = reader.IsDBNull("NewValue") ? "" : reader.GetString("NewValue"),
                        QuantityChange = reader.GetInt32("QuantityChange"),
                        PriceChange = reader.GetDecimal("PriceChange"),
                        Category = reader.IsDBNull("Category") ? "" : reader.GetString("Category"),
                        Location = reader.IsDBNull("Location") ? "" : reader.GetString("Location")
                    });
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error obteniendo historial por fecha: {ex.Message}");
            }

            return history;
        }


        public Dictionary<string, int> GetHistoryStats()
        {
            var stats = new Dictionary<string, int>();

            try
            {
                using var connection = new SQLiteConnection(_connectionString);
                connection.Open();

                var query = @"
                    SELECT Action, COUNT(*) as Count
                    FROM InventoryHistory 
                    GROUP BY Action";

                using var command = new SQLiteCommand(query, connection);
                using var reader = command.ExecuteReader();

                while (reader.Read())
                {
                    stats[reader.GetString("Action")] = reader.GetInt32("Count");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error obteniendo estadísticas: {ex.Message}");
            }

            return stats;
        }


        public bool CleanOldHistory(DateTime beforeDate)
        {
            try
            {
                using var connection = new SQLiteConnection(_connectionString);
                connection.Open();

                var query = "DELETE FROM InventoryHistory WHERE Timestamp < @BeforeDate";
                using var command = new SQLiteCommand(query, connection);
                command.Parameters.AddWithValue("@BeforeDate", beforeDate);
                
                int rowsAffected = command.ExecuteNonQuery();
                return rowsAffected > 0;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error limpiando historial: {ex.Message}");
                return false;
            }
        }
    }
}

