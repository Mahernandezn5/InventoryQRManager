using System;
using System.Data.SQLite;
using System.IO;
using System.Windows.Forms;
using InventoryQRManager.Models;

namespace InventoryQRManager.Data
{
    public class DatabaseContext
    {
        private readonly string _connectionString;
        private readonly string _databasePath;

        public DatabaseContext()
        {
            _databasePath = Path.Combine(Application.StartupPath, "inventory.db");
            _connectionString = $"Data Source={_databasePath};Version=3;";
            InitializeDatabase();
        }

        private void InitializeDatabase()
        {
            if (!File.Exists(_databasePath))
            {
                SQLiteConnection.CreateFile(_databasePath);
            }

            using var connection = new SQLiteConnection(_connectionString);
            connection.Open();

            // Crear tabla de categorías
            var createCategoriesTable = @"
                CREATE TABLE IF NOT EXISTS Categories (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Name TEXT NOT NULL UNIQUE,
                    Description TEXT,
                    Color TEXT DEFAULT '#007bff',
                    IsActive INTEGER DEFAULT 1,
                    CreatedDate TEXT NOT NULL
                )";

            // Crear tabla de ubicaciones
            var createLocationsTable = @"
                CREATE TABLE IF NOT EXISTS Locations (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Name TEXT NOT NULL UNIQUE,
                    Description TEXT,
                    Address TEXT,
                    IsActive INTEGER DEFAULT 1,
                    CreatedDate TEXT NOT NULL
                )";

            // Crear tabla de items de inventario
            var createInventoryItemsTable = @"
                CREATE TABLE IF NOT EXISTS InventoryItems (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Name TEXT NOT NULL,
                    Description TEXT,
                    SKU TEXT NOT NULL UNIQUE,
                    QRCode TEXT NOT NULL UNIQUE,
                    Quantity INTEGER DEFAULT 0,
                    Price REAL DEFAULT 0,
                    Category TEXT,
                    Location TEXT,
                    CreatedDate TEXT NOT NULL,
                    LastUpdated TEXT,
                    IsActive INTEGER DEFAULT 1
                )";

            using var command1 = new SQLiteCommand(createCategoriesTable, connection);
            command1.ExecuteNonQuery();

            using var command2 = new SQLiteCommand(createLocationsTable, connection);
            command2.ExecuteNonQuery();

            using var command3 = new SQLiteCommand(createInventoryItemsTable, connection);
            command3.ExecuteNonQuery();

            // Insertar datos iniciales
            InsertInitialData(connection);
        }

        private void InsertInitialData(SQLiteConnection connection)
        {
            // Verificar si ya hay datos
            var checkData = "SELECT COUNT(*) FROM Categories";
            using var checkCommand = new SQLiteCommand(checkData, connection);
            var count = Convert.ToInt32(checkCommand.ExecuteScalar());

            if (count == 0)
            {
                // Insertar categorías por defecto
                var insertCategories = @"
                    INSERT INTO Categories (Name, Description, Color, CreatedDate) VALUES
                    ('Electrónicos', 'Dispositivos electrónicos y tecnología', '#28a745', datetime('now')),
                    ('Ropa', 'Vestimenta y accesorios', '#dc3545', datetime('now')),
                    ('Hogar', 'Artículos para el hogar', '#ffc107', datetime('now')),
                    ('Libros', 'Libros y material educativo', '#17a2b8', datetime('now')),
                    ('Otros', 'Categoría general', '#6c757d', datetime('now'))";

                // Insertar ubicaciones por defecto
                var insertLocations = @"
                    INSERT INTO Locations (Name, Description, Address, CreatedDate) VALUES
                    ('Almacén Principal', 'Almacén principal de la empresa', 'Calle Principal 123', datetime('now')),
                    ('Oficina', 'Oficina central', 'Calle Oficina 456', datetime('now')),
                    ('Tienda', 'Tienda física', 'Calle Tienda 789', datetime('now'))";

                using var command1 = new SQLiteCommand(insertCategories, connection);
                command1.ExecuteNonQuery();

                using var command2 = new SQLiteCommand(insertLocations, connection);
                command2.ExecuteNonQuery();
            }
        }

        public SQLiteConnection GetConnection()
        {
            return new SQLiteConnection(_connectionString);
        }
    }
}

