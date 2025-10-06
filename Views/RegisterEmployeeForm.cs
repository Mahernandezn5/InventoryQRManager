using System;
using System.Drawing;
using System.Windows.Forms;
using InventoryQRManager.Services;
using InventoryQRManager.Models;
using InventoryQRManager.Data;

namespace InventoryQRManager.Views
{
    public partial class RegisterEmployeeForm : Form
    {
        private readonly AuthService _authService;
        private readonly DatabaseContext _context;

        public RegisterEmployeeForm()
        {
            InitializeComponent();
            _context = new DatabaseContext();
            _authService = new AuthService(_context);
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();

            this.Text = "👥 Registrar Empleado - Inventory QR Manager";
            this.Size = new Size(450, 600);
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
                Text = "👥 Registrar Nuevo Empleado",
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                ForeColor = Color.FromArgb(52, 58, 64),
                TextAlign = ContentAlignment.MiddleCenter,
                Size = new Size(390, 40),
                Location = new Point(30, 20)
            };

            var firstNameLabel = new Label
            {
                Text = "👤 Nombre:",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = Color.FromArgb(52, 58, 64),
                Size = new Size(100, 25),
                Location = new Point(30, 80)
            };

            var firstNameTextBox = new TextBox
            {
                Font = new Font("Segoe UI", 10),
                Size = new Size(350, 30),
                Location = new Point(30, 105),
                PlaceholderText = "Ingrese el nombre del empleado"
            };

            var lastNameLabel = new Label
            {
                Text = "👤 Apellido:",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = Color.FromArgb(52, 58, 64),
                Size = new Size(100, 25),
                Location = new Point(30, 150)
            };

            var lastNameTextBox = new TextBox
            {
                Font = new Font("Segoe UI", 10),
                Size = new Size(350, 30),
                Location = new Point(30, 175),
                PlaceholderText = "Ingrese el apellido del empleado"
            };

            var usernameLabel = new Label
            {
                Text = "👤 Usuario:",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = Color.FromArgb(52, 58, 64),
                Size = new Size(100, 25),
                Location = new Point(30, 220)
            };

            var usernameTextBox = new TextBox
            {
                Font = new Font("Segoe UI", 10),
                Size = new Size(350, 30),
                Location = new Point(30, 245),
                PlaceholderText = "Ingrese el nombre de usuario"
            };

            var emailLabel = new Label
            {
                Text = "📧 Email:",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = Color.FromArgb(52, 58, 64),
                Size = new Size(100, 25),
                Location = new Point(30, 290)
            };

            var emailTextBox = new TextBox
            {
                Font = new Font("Segoe UI", 10),
                Size = new Size(350, 30),
                Location = new Point(30, 315),
                PlaceholderText = "Ingrese el email del empleado"
            };

            var passwordLabel = new Label
            {
                Text = "🔒 Contraseña:",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = Color.FromArgb(52, 58, 64),
                Size = new Size(100, 25),
                Location = new Point(30, 360)
            };

            var passwordTextBox = new TextBox
            {
                Font = new Font("Segoe UI", 10),
                Size = new Size(350, 30),
                Location = new Point(30, 385),
                PlaceholderText = "Ingrese la contraseña",
                UseSystemPasswordChar = true
            };

            var registerButton = new Button
            {
                Text = "✅ Registrar Empleado",
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                ForeColor = Color.White,
                BackColor = Color.FromArgb(40, 167, 69),
                Size = new Size(350, 40),
                Location = new Point(30, 440),
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            registerButton.FlatAppearance.BorderSize = 0;

            var cancelButton = new Button
            {
                Text = "❌ Cancelar",
                Font = new Font("Segoe UI", 10),
                ForeColor = Color.FromArgb(108, 117, 125),
                BackColor = Color.FromArgb(233, 236, 239),
                Size = new Size(350, 35),
                Location = new Point(30, 500),
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            cancelButton.FlatAppearance.BorderSize = 0;

            registerButton.Click += (s, e) => RegisterButton_Click(
                firstNameTextBox.Text, 
                lastNameTextBox.Text, 
                usernameTextBox.Text, 
                emailTextBox.Text, 
                passwordTextBox.Text);

            cancelButton.Click += (s, e) => this.Close();

            mainPanel.Controls.AddRange(new Control[] {
                titleLabel, firstNameLabel, firstNameTextBox, lastNameLabel, lastNameTextBox,
                usernameLabel, usernameTextBox, emailLabel, emailTextBox, passwordLabel, passwordTextBox,
                registerButton, cancelButton
            });

            this.Controls.Add(mainPanel);
            this.ResumeLayout(false);
        }

        private void RegisterButton_Click(string firstName, string lastName, string username, string email, string password)
        {
            if (string.IsNullOrWhiteSpace(firstName) || string.IsNullOrWhiteSpace(lastName) ||
                string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(email) ||
                string.IsNullOrWhiteSpace(password))
            {
                MessageBox.Show("⚠️ Por favor, complete todos los campos.", "Campos Requeridos", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (password.Length < 6)
            {
                MessageBox.Show("⚠️ La contraseña debe tener al menos 6 caracteres.", "Contraseña Inválida", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                if (_authService.Register(username, email, password, firstName, lastName, UserRole.Employee))
                {
                    MessageBox.Show($"✅ Empleado registrado exitosamente!\n\n" +
                                  $"Nombre: {firstName} {lastName}\n" +
                                  $"Usuario: {username}\n" +
                                  $"Email: {email}\n" +
                                  $"Rol: Empleado",
                        "Registro Exitoso", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
                else
                {
                    MessageBox.Show("❌ Error al registrar el empleado.\n\n" +
                                  "Verifique que el usuario y email no estén en uso.",
                        "Error de Registro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error durante el registro: {ex.Message}",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
