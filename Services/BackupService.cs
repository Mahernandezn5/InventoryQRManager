using System;
using System.IO;
using System.Text.Json;
using System.Collections.Generic;
using InventoryQRManager.Models;
using InventoryQRManager.Data;
using System.Data.SQLite;

namespace InventoryQRManager.Services
{
    public class BackupService
    {
        private readonly DatabaseContext _dbContext;

        public BackupService()
        {
            _dbContext = new DatabaseContext();
        }

        public class BackupData
        {
            public DateTime BackupDate { get; set; }
            public List<InventoryItem> Items { get; set; } = new();
            public string Version { get; set; } = "1.0";
        }

        public bool CreateBackup(string filePath)
        {
            try
            {
                var backupData = new BackupData
                {
                    BackupDate = DateTime.Now,
                    Items = GetAllItemsForBackup()
                };

                var json = JsonSerializer.Serialize(backupData, new JsonSerializerOptions 
                { 
                    WriteIndented = true 
                });

                File.WriteAllText(filePath, json);
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error creando backup: {ex.Message}");
            }
        }

        public bool RestoreBackup(string filePath)
        {
            try
            {
                if (!File.Exists(filePath))
                {
                    throw new Exception("El archivo de backup no existe.");
                }

                var json = File.ReadAllText(filePath);
                var backupData = JsonSerializer.Deserialize<BackupData>(json);

                if (backupData == null || backupData.Items == null)
                {
                    throw new Exception("El archivo de backup está corrupto o vacío.");
                }

                // Crear backup de seguridad antes de restaurar
                var safetyBackupPath = Path.Combine(
                    Path.GetDirectoryName(filePath) ?? "",
                    $"safety_backup_{DateTime.Now:yyyyMMdd_HHmmss}.json"
                );
                CreateBackup(safetyBackupPath);

                // Limpiar datos existentes
                ClearAllData();

                // Restaurar datos
                foreach (var item in backupData.Items)
                {
                    InsertItemFromBackup(item);
                }

                return true;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error restaurando backup: {ex.Message}");
            }
        }

        public bool ExportToCSV(string filePath)
        {
            try
            {
                var items = GetAllItemsForBackup();
                
                using var writer = new StreamWriter(filePath);
                
                // Escribir encabezados
                writer.WriteLine("ID,Nombre,Descripción,SKU,Código QR,Cantidad,Precio,Categoría,Ubicación,Fecha Creación,Última Actualización");

                // Escribir datos
                foreach (var item in items)
                {
                    writer.WriteLine($"{item.Id},\"{item.Name}\",\"{item.Description}\",{item.SKU},{item.QRCode},{item.Quantity},{item.Price},\"{item.Category}\",\"{item.Location}\",{item.CreatedDate:yyyy-MM-dd HH:mm:ss},{item.LastUpdated:yyyy-MM-dd HH:mm:ss}");
                }

                return true;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error exportando a CSV: {ex.Message}");
            }
        }

        public bool ImportFromCSV(string filePath)
        {
            try
            {
                if (!File.Exists(filePath))
                {
                    throw new Exception("El archivo CSV no existe.");
                }

                var lines = File.ReadAllLines(filePath);
                if (lines.Length < 2)
                {
                    throw new Exception("El archivo CSV está vacío o no tiene el formato correcto.");
                }

                // Crear backup antes de importar
                var backupPath = Path.Combine(
                    Path.GetDirectoryName(filePath) ?? "",
                    $"pre_import_backup_{DateTime.Now:yyyyMMdd_HHmmss}.json"
                );
                CreateBackup(backupPath);

                var importedCount = 0;
                var errorCount = 0;

                // Procesar cada línea (saltar encabezados)
                for (int i = 1; i < lines.Length; i++)
                {
                    try
                    {
                        var values = ParseCSVLine(lines[i]);
                        if (values.Length >= 10)
                        {
                            var item = new InventoryItem
                            {
                                Name = values[1],
                                Description = values[2],
                                SKU = values[3],
                                QRCode = values[4],
                                Quantity = int.TryParse(values[5], out var qty) ? qty : 0,
                                Price = decimal.TryParse(values[6], out var price) ? price : 0,
                                Category = values[7],
                                Location = values[8],
                                CreatedDate = DateTime.TryParse(values[9], out var created) ? created : DateTime.Now,
                                LastUpdated = DateTime.TryParse(values[10], out var updated) ? updated : null,
                                IsActive = true
                            };

                            if (InsertItemFromBackup(item))
                            {
                                importedCount++;
                            }
                        }
                    }
                    catch
                    {
                        errorCount++;
                    }
                }

                if (errorCount > 0)
                {
                    throw new Exception($"Importación completada con {importedCount} items exitosos y {errorCount} errores.");
                }

                return true;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error importando desde CSV: {ex.Message}");
            }
        }

        public List<string> GetBackupFiles(string directory)
        {
            var backupFiles = new List<string>();
            
            if (Directory.Exists(directory))
            {
                var files = Directory.GetFiles(directory, "*.json");
                foreach (var file in files)
                {
                    var fileName = Path.GetFileName(file);
                    if (fileName.Contains("backup") || fileName.Contains("safety_backup"))
                    {
                        var fileInfo = new FileInfo(file);
                        backupFiles.Add($"{fileName} - {fileInfo.CreationTime:yyyy-MM-dd HH:mm:ss}");
                    }
                }
            }

            return backupFiles;
        }

        private List<InventoryItem> GetAllItemsForBackup()
        {
            var items = new List<InventoryItem>();
            using var connection = _dbContext.GetConnection();
            connection.Open();

            var query = "SELECT * FROM InventoryItems WHERE IsActive = 1 ORDER BY CreatedDate";
            using var command = new SQLiteCommand(query, connection);
            using var reader = command.ExecuteReader();

            while (reader.Read())
            {
                items.Add(new InventoryItem
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
                });
            }

            return items;
        }

        private void ClearAllData()
        {
            using var connection = _dbContext.GetConnection();
            connection.Open();

            var query = "DELETE FROM InventoryItems";
            using var command = new SQLiteCommand(query, connection);
            command.ExecuteNonQuery();
        }

        private bool InsertItemFromBackup(InventoryItem item)
        {
            try
            {
                using var connection = _dbContext.GetConnection();
                connection.Open();

                var query = @"
                    INSERT INTO InventoryItems 
                    (Name, Description, SKU, QRCode, Quantity, Price, Category, Location, CreatedDate, LastUpdated, IsActive)
                    VALUES 
                    (@name, @description, @sku, @qrCode, @quantity, @price, @category, @location, @createdDate, @lastUpdated, 1)";

                using var command = new SQLiteCommand(query, connection);
                command.Parameters.AddWithValue("@name", item.Name);
                command.Parameters.AddWithValue("@description", item.Description);
                command.Parameters.AddWithValue("@sku", item.SKU);
                command.Parameters.AddWithValue("@qrCode", item.QRCode);
                command.Parameters.AddWithValue("@quantity", item.Quantity);
                command.Parameters.AddWithValue("@price", item.Price);
                command.Parameters.AddWithValue("@category", item.Category);
                command.Parameters.AddWithValue("@location", item.Location);
                command.Parameters.AddWithValue("@createdDate", item.CreatedDate.ToString("yyyy-MM-dd HH:mm:ss"));
                command.Parameters.AddWithValue("@lastUpdated", item.LastUpdated?.ToString("yyyy-MM-dd HH:mm:ss") ?? (object)DBNull.Value);

                return command.ExecuteNonQuery() > 0;
            }
            catch
            {
                return false;
            }
        }

        private string[] ParseCSVLine(string line)
        {
            var values = new List<string>();
            var current = "";
            var inQuotes = false;

            for (int i = 0; i < line.Length; i++)
            {
                var c = line[i];

                if (c == '"')
                {
                    inQuotes = !inQuotes;
                }
                else if (c == ',' && !inQuotes)
                {
                    values.Add(current.Trim());
                    current = "";
                }
                else
                {
                    current += c;
                }
            }

            values.Add(current.Trim());
            return values.ToArray();
        }
    }
}
