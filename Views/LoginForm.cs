using System;
using System.Drawing;
using System.Windows.Forms;
using InventoryQRManager.Services;
using InventoryQRManager.Models;
using InventoryQRManager.Data;

namespace InventoryQRManager.Views
{
    public partial class LoginForm : Form
    {
        private readonly AuthService _authService;
        private readonly DatabaseContext _context;

        public LoginForm()
        {
            InitializeComponent();
            _context = new DatabaseContext();
            _authService = new AuthService(_context);
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();

            this.Text = "🔐 Iniciar Sesión - Inventory QR Manager";
            this.Size = new Size(400, 500);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.BackColor = Color.FromArgb(248, 249, 250);

            var mainPanel = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(30)
            };

            var titleLabel = new Label
            {
                Text = "🔐 Iniciar Sesión",
                Font = new Font("Segoe UI", 18, FontStyle.Bold),
                ForeColor = Color.FromArgb(52, 58, 64),
                TextAlign = ContentAlignment.MiddleCenter,
                Size = new Size(340, 40),
                Location = new Point(30, 20)
            };

            var usernameLabel = new Label
            {
                Text = "👤 Usuario:",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = Color.FromArgb(52, 58, 64),
                Size = new Size(100, 25),
                Location = new Point(30, 80)
            };

            var usernameTextBox = new TextBox
            {
                Font = new Font("Segoe UI", 10),
                Size = new Size(300, 30),
                Location = new Point(30, 105),
                PlaceholderText = "Ingrese su nombre de usuario"
            };

            var passwordLabel = new Label
            {
                Text = "🔒 Contraseña:",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = Color.FromArgb(52, 58, 64),
                Size = new Size(100, 25),
                Location = new Point(30, 150)
            };

            var passwordTextBox = new TextBox
            {
                Font = new Font("Segoe UI", 10),
                Size = new Size(300, 30),
                Location = new Point(30, 175),
                PlaceholderText = "Ingrese su contraseña",
                UseSystemPasswordChar = true
            };

            var loginButton = new Button
            {
                Text = "🚀 Iniciar Sesión",
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                ForeColor = Color.White,
                BackColor = Color.FromArgb(40, 167, 69),
                Size = new Size(300, 40),
                Location = new Point(30, 230),
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            loginButton.FlatAppearance.BorderSize = 0;

            var infoLabel = new Label
            {
                Text = "👥 Usuarios por defecto:\n" +
                      "• admin / admin123 (Administrador)\n" +
                      "• empleado / empleado123 (Empleado)",
                Font = new Font("Segoe UI", 9),
                ForeColor = Color.FromArgb(108, 117, 125),
                Size = new Size(340, 60),
                Location = new Point(30, 290),
                TextAlign = ContentAlignment.MiddleLeft
            };

            var forgotPasswordButton = new Button
            {
                Text = "🔐 ¿Olvidaste tu contraseña?",
                Font = new Font("Segoe UI", 9),
                ForeColor = Color.FromArgb(52, 144, 220),
                BackColor = Color.Transparent,
                Size = new Size(300, 30),
                Location = new Point(30, 360),
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            forgotPasswordButton.FlatAppearance.BorderSize = 0;


            var cancelButton = new Button
            {
                Text = "❌ Cancelar",
                Font = new Font("Segoe UI", 10),
                ForeColor = Color.FromArgb(108, 117, 125),
                BackColor = Color.FromArgb(233, 236, 239),
                Size = new Size(300, 35),
                Location = new Point(30, 400),
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            cancelButton.FlatAppearance.BorderSize = 0;

            loginButton.Click += (s, e) => LoginButton_Click(usernameTextBox.Text, passwordTextBox.Text);
            forgotPasswordButton.Click += (s, e) => ShowForgotPasswordForm();
            cancelButton.Click += (s, e) => this.Close();
            
            usernameTextBox.KeyPress += (s, e) => { if (e.KeyChar == (char)Keys.Enter) passwordTextBox.Focus(); };
            passwordTextBox.KeyPress += (s, e) => { if (e.KeyChar == (char)Keys.Enter) LoginButton_Click(usernameTextBox.Text, passwordTextBox.Text); };

            mainPanel.Controls.AddRange(new Control[] {
                titleLabel, usernameLabel, usernameTextBox, passwordLabel, passwordTextBox,
                loginButton, infoLabel, forgotPasswordButton, cancelButton
            });

            this.Controls.Add(mainPanel);
            this.ResumeLayout(false);
        }

        private void LoginButton_Click(string username, string password)
        {
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                MessageBox.Show("⚠️ Por favor, complete todos los campos.", "Campos Requeridos", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                // Debug: Mostrar información de usuarios
                _authService.DebugUsers();
                
                if (_authService.Login(username, password))
                {
                    var currentUser = _authService.CurrentUser;
                    MessageBox.Show($"✅ ¡Bienvenido, {currentUser?.FullName}!\n\n" +
                                  $"Rol: {GetRoleDisplayName(currentUser?.Role ?? UserRole.Employee)}\n" +
                                  $"Usuario: {currentUser?.Username}",
                        "Login Exitoso", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
                else
                {
                    // Debug: Mostrar información adicional en caso de error
                    var context = new DatabaseContext();
                    var allUsers = context.Users.ToList();
                    var userExists = allUsers.Any(u => u.Username == username);
                    var userInfo = userExists ? 
                        allUsers.First(u => u.Username == username) : null;
                    
                    var errorMessage = $"❌ Usuario o contraseña incorrectos.\n\n" +
                                      $"Debug Info:\n" +
                                      $"Usuario existe: {userExists}\n";
                    
                    if (userInfo != null)
                    {
                        errorMessage += $"Usuario encontrado: {userInfo.Username}\n" +
                                      $"Rol: {userInfo.Role}\n" +
                                      $"Activo: {userInfo.IsActive}\n" +
                                      $"Hash almacenado: {userInfo.PasswordHash}\n";
                    }
                    
                    errorMessage += $"\nCredenciales por defecto:\n" +
                                  $"👑 Administrador: admin / admin123\n" +
                                  $"👤 Empleado: empleado / empleado123";
                    
                    MessageBox.Show(errorMessage,
                        "Error de Login", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error durante el login: {ex.Message}",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ShowForgotPasswordForm()
        {
            try
            {
                var forgotPasswordForm = new ForgotPasswordForm();
                forgotPasswordForm.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error abriendo formulario de recuperación: {ex.Message}",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        private string GetRoleDisplayName(UserRole role)
        {
            return role switch
            {
                UserRole.Admin => "Administrador",
                UserRole.Employee => "Empleado",
                _ => "Desconocido"
            };
        }

        public AuthService GetAuthService() => _authService;
    }
}
