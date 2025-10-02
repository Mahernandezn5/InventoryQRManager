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


            var createCategoriesTable = @"
                CREATE TABLE IF NOT EXISTS Categories (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Name TEXT NOT NULL UNIQUE,
                    Description TEXT,
                    Color TEXT DEFAULT '#007bff',
                    IsActive INTEGER DEFAULT 1,
                    CreatedDate TEXT NOT NULL
                )";


            var createLocationsTable = @"
                CREATE TABLE IF NOT EXISTS Locations (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Name TEXT NOT NULL UNIQUE,
                    Description TEXT,
                    Address TEXT,
                    IsActive INTEGER DEFAULT 1,
                    CreatedDate TEXT NOT NULL
                )";


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

            var createUsersTable = @"
                CREATE TABLE IF NOT EXISTS Users (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Username TEXT NOT NULL UNIQUE,
                    Email TEXT NOT NULL UNIQUE,
                    PasswordHash TEXT NOT NULL,
                    FirstName TEXT,
                    LastName TEXT,
                    Role INTEGER DEFAULT 3,
                    IsActive INTEGER DEFAULT 1,
                    CreatedDate TEXT NOT NULL,
                    LastLoginDate TEXT
                )";

            using var command1 = new SQLiteCommand(createCategoriesTable, connection);
            command1.ExecuteNonQuery();

            using var command2 = new SQLiteCommand(createLocationsTable, connection);
            command2.ExecuteNonQuery();

            using var command3 = new SQLiteCommand(createInventoryItemsTable, connection);
            command3.ExecuteNonQuery();

            using var command4 = new SQLiteCommand(createUsersTable, connection);
            command4.ExecuteNonQuery();

            
            InsertInitialData(connection);
        }

        private void InsertInitialData(SQLiteConnection connection)
        {
           
            var checkData = "SELECT COUNT(*) FROM Categories";
            using var checkCommand = new SQLiteCommand(checkData, connection);
            var count = Convert.ToInt32(checkCommand.ExecuteScalar());

            if (count == 0)
            {
              
                var insertCategories = @"
                    INSERT INTO Categories (Name, Description, Color, CreatedDate) VALUES
                    ('Electrónicos', 'Dispositivos electrónicos y tecnología', '#28a745', datetime('now')),
                    ('Ropa', 'Vestimenta y accesorios', '#dc3545', datetime('now')),
                    ('Hogar', 'Artículos para el hogar', '#ffc107', datetime('now')),
                    ('Libros', 'Libros y material educativo', '#17a2b8', datetime('now')),
                    ('Otros', 'Categoría general', '#6c757d', datetime('now'))";

             
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

        
        public List<User> Users => GetUsers();
        
        private List<User> GetUsers()
        {
            var users = new List<User>();
            using var connection = GetConnection();
            connection.Open();
            
            var query = "SELECT * FROM Users WHERE IsActive = 1";
            using var command = new SQLiteCommand(query, connection);
            using var reader = command.ExecuteReader();
            
            while (reader.Read())
            {
                users.Add(new User
                {
                    Id = reader.GetInt32(0),
                    Username = reader.GetString(1),
                    Email = reader.GetString(2),
                    PasswordHash = reader.GetString(3),
                    FirstName = reader.IsDBNull(4) ? "" : reader.GetString(4),
                    LastName = reader.IsDBNull(5) ? "" : reader.GetString(5),
                    Role = (UserRole)reader.GetInt32(6),
                    IsActive = reader.GetInt32(7) == 1,
                    CreatedDate = DateTime.Parse(reader.GetString(8)),
                    LastLoginDate = reader.IsDBNull(9) ? null : DateTime.Parse(reader.GetString(9))
                });
            }
            
            return users;
        }
        
        public void AddUser(User user)
        {
            using var connection = GetConnection();
            connection.Open();
            
            var query = @"INSERT INTO Users (Username, Email, PasswordHash, FirstName, LastName, Role, IsActive, CreatedDate, LastLoginDate)
                         VALUES (@username, @email, @passwordHash, @firstName, @lastName, @role, @isActive, @createdDate, @lastLoginDate)";
            
            using var command = new SQLiteCommand(query, connection);
            command.Parameters.AddWithValue("@username", user.Username);
            command.Parameters.AddWithValue("@email", user.Email);
            command.Parameters.AddWithValue("@passwordHash", user.PasswordHash);
            command.Parameters.AddWithValue("@firstName", user.FirstName);
            command.Parameters.AddWithValue("@lastName", user.LastName);
            command.Parameters.AddWithValue("@role", (int)user.Role);
            command.Parameters.AddWithValue("@isActive", user.IsActive ? 1 : 0);
            command.Parameters.AddWithValue("@createdDate", user.CreatedDate.ToString("yyyy-MM-dd HH:mm:ss"));
            command.Parameters.AddWithValue("@lastLoginDate", user.LastLoginDate?.ToString("yyyy-MM-dd HH:mm:ss"));
            
            command.ExecuteNonQuery();
        }
        
        public void UpdateUser(User user)
        {
            using var connection = GetConnection();
            connection.Open();
            
            var query = @"UPDATE Users SET Username = @username, Email = @email, PasswordHash = @passwordHash, 
                         FirstName = @firstName, LastName = @lastName, Role = @role, IsActive = @isActive, 
                         LastLoginDate = @lastLoginDate WHERE Id = @id";
            
            using var command = new SQLiteCommand(query, connection);
            command.Parameters.AddWithValue("@id", user.Id);
            command.Parameters.AddWithValue("@username", user.Username);
            command.Parameters.AddWithValue("@email", user.Email);
            command.Parameters.AddWithValue("@passwordHash", user.PasswordHash);
            command.Parameters.AddWithValue("@firstName", user.FirstName);
            command.Parameters.AddWithValue("@lastName", user.LastName);
            command.Parameters.AddWithValue("@role", (int)user.Role);
            command.Parameters.AddWithValue("@isActive", user.IsActive ? 1 : 0);
            command.Parameters.AddWithValue("@lastLoginDate", user.LastLoginDate?.ToString("yyyy-MM-dd HH:mm:ss"));
            
            command.ExecuteNonQuery();
        }
        
        public User? FindUser(string username)
        {
            using var connection = GetConnection();
            connection.Open();
            
            var query = "SELECT * FROM Users WHERE Username = @username AND IsActive = 1";
            using var command = new SQLiteCommand(query, connection);
            command.Parameters.AddWithValue("@username", username);
            using var reader = command.ExecuteReader();
            
            if (reader.Read())
            {
                return new User
                {
                    Id = reader.GetInt32(0),
                    Username = reader.GetString(1),
                    Email = reader.GetString(2),
                    PasswordHash = reader.GetString(3),
                    FirstName = reader.IsDBNull(4) ? "" : reader.GetString(4),
                    LastName = reader.IsDBNull(5) ? "" : reader.GetString(5),
                    Role = (UserRole)reader.GetInt32(6),
                    IsActive = reader.GetInt32(7) == 1,
                    CreatedDate = DateTime.Parse(reader.GetString(8)),
                    LastLoginDate = reader.IsDBNull(9) ? null : DateTime.Parse(reader.GetString(9))
                };
            }
            
            return null;
        }
    }
}

