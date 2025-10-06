using System;
using System.Drawing;
using System.Windows.Forms;
using InventoryQRManager.Services;
using InventoryQRManager.Data;

namespace InventoryQRManager.Views
{
    public partial class ForgotPasswordForm : Form
    {
        private readonly AuthService _authService;
        private readonly DatabaseContext _context;

        public ForgotPasswordForm()
        {
            InitializeComponent();
            _context = new DatabaseContext();
            _authService = new AuthService(_context);
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();

            this.Text = "🔐 Recuperar Contraseña - Inventory QR Manager";
            this.Size = new Size(400, 400);
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
                Text = "🔐 Recuperar Contraseña",
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                ForeColor = Color.FromArgb(52, 58, 64),
                TextAlign = ContentAlignment.MiddleCenter,
                Size = new Size(340, 40),
                Location = new Point(30, 20)
            };

            var infoLabel = new Label
            {
                Text = "Ingrese su email para recibir una nueva contraseña temporal:",
                Font = new Font("Segoe UI", 10),
                ForeColor = Color.FromArgb(108, 117, 125),
                Size = new Size(340, 40),
                Location = new Point(30, 70),
                TextAlign = ContentAlignment.MiddleLeft
            };

            var emailLabel = new Label
            {
                Text = "📧 Email:",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = Color.FromArgb(52, 58, 64),
                Size = new Size(100, 25),
                Location = new Point(30, 120)
            };

            var emailTextBox = new TextBox
            {
                Font = new Font("Segoe UI", 10),
                Size = new Size(300, 30),
                Location = new Point(30, 145),
                PlaceholderText = "Ingrese su email registrado"
            };

            var resetButton = new Button
            {
                Text = "🔄 Generar Nueva Contraseña",
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                ForeColor = Color.White,
                BackColor = Color.FromArgb(255, 193, 7),
                Size = new Size(300, 40),
                Location = new Point(30, 200),
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            resetButton.FlatAppearance.BorderSize = 0;

            var cancelButton = new Button
            {
                Text = "❌ Cancelar",
                Font = new Font("Segoe UI", 10),
                ForeColor = Color.FromArgb(108, 117, 125),
                BackColor = Color.FromArgb(233, 236, 239),
                Size = new Size(300, 35),
                Location = new Point(30, 260),
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            cancelButton.FlatAppearance.BorderSize = 0;

            var backToLoginButton = new Button
            {
                Text = "🔙 Volver al Login",
                Font = new Font("Segoe UI", 10),
                ForeColor = Color.FromArgb(52, 144, 220),
                BackColor = Color.Transparent,
                Size = new Size(300, 35),
                Location = new Point(30, 310),
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            backToLoginButton.FlatAppearance.BorderSize = 0;

            resetButton.Click += (s, e) => ResetButton_Click(emailTextBox.Text);
            cancelButton.Click += (s, e) => this.Close();
            backToLoginButton.Click += (s, e) => {
                this.DialogResult = DialogResult.Cancel;
                this.Close();
            };

            emailTextBox.KeyPress += (s, e) => { 
                if (e.KeyChar == (char)Keys.Enter) ResetButton_Click(emailTextBox.Text); 
            };

            mainPanel.Controls.AddRange(new Control[] {
                titleLabel, infoLabel, emailLabel, emailTextBox,
                resetButton, cancelButton, backToLoginButton
            });

            this.Controls.Add(mainPanel);
            this.ResumeLayout(false);
        }

        private void ResetButton_Click(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                MessageBox.Show("⚠️ Por favor, ingrese su email.", "Email Requerido", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!IsValidEmail(email))
            {
                MessageBox.Show("⚠️ Por favor, ingrese un email válido.", "Email Inválido", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                if (_authService.ValidateEmail(email))
                {
                    var user = _authService.GetUserByEmail(email);
                    if (user != null)
                    {
                        var newPassword = GenerateTemporaryPassword();
                        
                        if (_authService.ResetPassword(email, newPassword))
                        {
                            MessageBox.Show($"✅ Contraseña restablecida exitosamente!\n\n" +
                                          $"Usuario: {user.Username}\n" +
                                          $"Nueva contraseña temporal: {newPassword}\n\n" +
                                          $"⚠️ IMPORTANTE: Cambie esta contraseña en su próximo inicio de sesión.",
                            "Contraseña Restablecida", MessageBoxButtons.OK, MessageBoxIcon.Information);

                            this.DialogResult = DialogResult.OK;
                            this.Close();
                        }
                        else
                        {
                            MessageBox.Show("❌ Error al restablecer la contraseña.\n\n" +
                                          "Intente nuevamente o contacte al administrador.",
                                "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
                else
                {
                    MessageBox.Show("❌ No se encontró una cuenta con ese email.\n\n" +
                                  "Verifique el email e intente nuevamente.",
                        "Email No Encontrado", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error durante el restablecimiento: {ex.Message}",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        private string GenerateTemporaryPassword()
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            var random = new Random();
            var password = new char[8];
            
            for (int i = 0; i < password.Length; i++)
            {
                password[i] = chars[random.Next(chars.Length)];
            }
            
            return new string(password);
        }
    }
}
