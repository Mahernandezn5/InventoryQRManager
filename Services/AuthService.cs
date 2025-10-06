using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Data.SQLite;
using InventoryQRManager.Models;
using InventoryQRManager.Data;

namespace InventoryQRManager.Services
{
    public class AuthService
    {
        private readonly DatabaseContext _context;
        private User? _currentUser;

        public AuthService(DatabaseContext context)
        {
            _context = context;
            InitializeDefaultUsers();
        }

        public User? CurrentUser => _currentUser;
        public bool IsAuthenticated => _currentUser != null;

        private void InitializeDefaultUsers()
        {
            try
            {
                var existingUsers = _context.Users.ToList();
                System.Diagnostics.Debug.WriteLine($"Usuarios existentes: {existingUsers.Count}");
                
                if (existingUsers.Count == 0)
                {
                    System.Diagnostics.Debug.WriteLine("Creando usuarios por defecto...");
                    
                    var adminUser = new User
                    {
                        Username = "admin",
                        Email = "admin@inventory.com",
                        PasswordHash = HashPassword("admin123"),
                        FirstName = "Administrador",
                        LastName = "Sistema",
                        Role = UserRole.Admin,
                        IsActive = true,
                        CreatedDate = DateTime.Now
                    };

                    var employeeUser = new User
                    {
                        Username = "empleado",
                        Email = "empleado@inventory.com",
                        PasswordHash = HashPassword("empleado123"),
                        FirstName = "Empleado",
                        LastName = "Inventario",
                        Role = UserRole.Employee,
                        IsActive = true,
                        CreatedDate = DateTime.Now
                    };

                    _context.AddUser(adminUser);
                    _context.AddUser(employeeUser);
                    
                    System.Diagnostics.Debug.WriteLine("Usuarios por defecto creados exitosamente.");
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("Usuarios ya existen en la base de datos.");
                    foreach (var user in existingUsers)
                    {
                        System.Diagnostics.Debug.WriteLine($"Usuario: {user.Username}, Rol: {user.Role}, Activo: {user.IsActive}");
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error inicializando usuarios por defecto: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"Stack trace: {ex.StackTrace}");
            }
        }

        public bool Login(string username, string password)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine($"Intentando login para usuario: {username}");
                
                var user = _context.Users.FirstOrDefault(u => u.Username == username && u.IsActive);
                
                if (user == null)
                {
                    System.Diagnostics.Debug.WriteLine($"Usuario '{username}' no encontrado o inactivo");
                    return false;
                }
                
                System.Diagnostics.Debug.WriteLine($"Usuario encontrado: {user.Username}, Rol: {user.Role}, Activo: {user.IsActive}");
                
                var passwordHash = HashPassword(password);
                System.Diagnostics.Debug.WriteLine($"Hash de contraseña ingresada: {passwordHash}");
                System.Diagnostics.Debug.WriteLine($"Hash almacenado: {user.PasswordHash}");
                
                if (!VerifyPassword(password, user.PasswordHash))
                {
                    System.Diagnostics.Debug.WriteLine("Contraseña incorrecta");
                    return false;
                }

                _currentUser = user;
                user.LastLoginDate = DateTime.Now;
                _context.UpdateUser(user);
                
                System.Diagnostics.Debug.WriteLine($"Login exitoso para usuario: {user.Username}");
                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error en login: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"Stack trace: {ex.StackTrace}");
                return false;
            }
        }

        public void Logout()
        {
            _currentUser = null;
        }

        public bool Register(string username, string email, string password, string firstName, string lastName, UserRole role = UserRole.Employee)
        {
            try
            {
                if (_context.Users.Any(u => u.Username == username || u.Email == email))
                {
                    return false; 
                }

                var newUser = new User
                {
                    Username = username,
                    Email = email,
                    PasswordHash = HashPassword(password),
                    FirstName = firstName,
                    LastName = lastName,
                    Role = role,
                    IsActive = true,
                    CreatedDate = DateTime.Now
                };

                _context.AddUser(newUser);
                
                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error en registro: {ex.Message}");
                return false;
            }
        }

        public bool ChangePassword(string currentPassword, string newPassword)
        {
            if (_currentUser == null) return false;

            try
            {
                if (!VerifyPassword(currentPassword, _currentUser.PasswordHash))
                {
                    return false;
                }

                _currentUser.PasswordHash = HashPassword(newPassword);
                _context.UpdateUser(_currentUser);
                
                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error cambiando contraseña: {ex.Message}");
                return false;
            }
        }

        public List<User> GetAllUsers()
        {
            return _context.Users;
        }

        public bool UpdateUser(User user)
        {
            try
            {
                _context.UpdateUser(user);
                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error actualizando usuario: {ex.Message}");
                return false;
            }
        }

        public bool DeleteUser(int userId)
        {
            try
            {
                var user = _context.Users.FirstOrDefault(u => u.Id == userId);
                if (user == null) return false;

                user.IsActive = false;
                _context.UpdateUser(user);
                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error eliminando usuario: {ex.Message}");
                return false;
            }
        }

        private string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return Convert.ToBase64String(hashedBytes);
            }
        }

        private bool VerifyPassword(string password, string hash)
        {
            return HashPassword(password) == hash;
        }

        public bool ResetPassword(string email, string newPassword)
        {
            try
            {
                var user = _context.Users.FirstOrDefault(u => u.Email == email && u.IsActive);
                if (user == null)
                {
                    return false;
                }

                user.PasswordHash = HashPassword(newPassword);
                _context.UpdateUser(user);
                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error reseteando contraseña: {ex.Message}");
                return false;
            }
        }

        public bool ValidateEmail(string email)
        {
            try
            {
                return _context.Users.Any(u => u.Email == email && u.IsActive);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error validando email: {ex.Message}");
                return false;
            }
        }

        public User? GetUserByEmail(string email)
        {
            try
            {
                return _context.Users.FirstOrDefault(u => u.Email == email && u.IsActive);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error obteniendo usuario por email: {ex.Message}");
                return null;
            }
        }

        public void DebugUsers()
        {
            try
            {
                var users = _context.Users.ToList();
                System.Diagnostics.Debug.WriteLine($"Total users in database: {users.Count}");
                
                foreach (var user in users)
                {
                    System.Diagnostics.Debug.WriteLine($"User: {user.Username}, Email: {user.Email}, Role: {user.Role}, Active: {user.IsActive}");
                    System.Diagnostics.Debug.WriteLine($"Password Hash: {user.PasswordHash}");
                    System.Diagnostics.Debug.WriteLine($"---");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error debugging users: {ex.Message}");
            }
        }

        public void ResetDefaultUsers()
        {
            try
            {
                System.Diagnostics.Debug.WriteLine("Reseteando usuarios por defecto...");
                
                // Eliminar usuarios existentes
                var existingUsers = _context.Users.ToList();
                foreach (var user in existingUsers)
                {
                    user.IsActive = false;
                    _context.UpdateUser(user);
                }
                
                // Crear nuevos usuarios por defecto
                var adminUser = new User
                {
                    Username = "admin",
                    Email = "admin@inventory.com",
                    PasswordHash = HashPassword("admin123"),
                    FirstName = "Administrador",
                    LastName = "Sistema",
                    Role = UserRole.Admin,
                    IsActive = true,
                    CreatedDate = DateTime.Now
                };

                var employeeUser = new User
                {
                    Username = "empleado",
                    Email = "empleado@inventory.com",
                    PasswordHash = HashPassword("empleado123"),
                    FirstName = "Empleado",
                    LastName = "Inventario",
                    Role = UserRole.Employee,
                    IsActive = true,
                    CreatedDate = DateTime.Now
                };

                _context.AddUser(adminUser);
                _context.AddUser(employeeUser);
                
                System.Diagnostics.Debug.WriteLine("Usuarios por defecto reseteados exitosamente.");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error reseteando usuarios por defecto: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"Stack trace: {ex.StackTrace}");
            }
        }

        public void ClearAndRecreateUsers()
        {
            try
            {
                System.Diagnostics.Debug.WriteLine("Limpiando y recreando usuarios...");
                
                // Eliminar todos los usuarios de la base de datos
                using var connection = _context.GetConnection();
                connection.Open();
                
                var deleteQuery = "DELETE FROM Users";
                using var deleteCommand = new SQLiteCommand(deleteQuery, connection);
                deleteCommand.ExecuteNonQuery();
                
                System.Diagnostics.Debug.WriteLine("Usuarios eliminados de la base de datos.");
                
                // Crear nuevos usuarios por defecto
                var adminUser = new User
                {
                    Username = "admin",
                    Email = "admin@inventory.com",
                    PasswordHash = HashPassword("admin123"),
                    FirstName = "Administrador",
                    LastName = "Sistema",
                    Role = UserRole.Admin,
                    IsActive = true,
                    CreatedDate = DateTime.Now
                };

                var employeeUser = new User
                {
                    Username = "empleado",
                    Email = "empleado@inventory.com",
                    PasswordHash = HashPassword("empleado123"),
                    FirstName = "Empleado",
                    LastName = "Inventario",
                    Role = UserRole.Employee,
                    IsActive = true,
                    CreatedDate = DateTime.Now
                };

                _context.AddUser(adminUser);
                _context.AddUser(employeeUser);
                
                System.Diagnostics.Debug.WriteLine("Usuarios por defecto recreados exitosamente.");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error limpiando y recreando usuarios: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"Stack trace: {ex.StackTrace}");
            }
        }

        public bool HasPermission(UserRole requiredRole)
        {
            if (_currentUser == null) return false;
            
            // Admin (1) tiene acceso a todo, Employee (2) solo a funciones básicas
            return _currentUser.Role == UserRole.Admin || 
                   (_currentUser.Role == UserRole.Employee && requiredRole == UserRole.Employee);
        }
    }
}
