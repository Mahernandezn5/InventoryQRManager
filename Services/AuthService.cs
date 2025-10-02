using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
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
            if (!_context.Users.Any())
            {
                
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

                
                var managerUser = new User
                {
                    Username = "manager",
                    Email = "manager@inventory.com",
                    PasswordHash = HashPassword("manager123"),
                    FirstName = "Manager",
                    LastName = "Inventario",
                    Role = UserRole.Manager,
                    IsActive = true,
                    CreatedDate = DateTime.Now
                };

               
                var regularUser = new User
                {
                    Username = "user",
                    Email = "user@inventory.com",
                    PasswordHash = HashPassword("user123"),
                    FirstName = "Usuario",
                    LastName = "Regular",
                    Role = UserRole.User,
                    IsActive = true,
                    CreatedDate = DateTime.Now
                };

                _context.AddUser(adminUser);
                _context.AddUser(managerUser);
                _context.AddUser(regularUser);
            }
        }

        public bool Login(string username, string password)
        {
            try
            {
                var user = _context.Users.FirstOrDefault(u => u.Username == username && u.IsActive);
                
                if (user == null || !VerifyPassword(password, user.PasswordHash))
                {
                    return false;
                }

                _currentUser = user;
                user.LastLoginDate = DateTime.Now;
                _context.UpdateUser(user);
                
                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error en login: {ex.Message}");
                return false;
            }
        }

        public void Logout()
        {
            _currentUser = null;
        }

        public bool Register(string username, string email, string password, string firstName, string lastName, UserRole role = UserRole.User)
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

        public bool HasPermission(UserRole requiredRole)
        {
            if (_currentUser == null) return false;
            
            return _currentUser.Role <= requiredRole; 
        }
    }
}
