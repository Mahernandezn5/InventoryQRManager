using System;
using System.Drawing;
using System.Windows.Forms;
using InventoryQRManager.Models;

namespace InventoryQRManager.Views
{
    public partial class AddEditUserForm : Form
    {
        private TextBox _usernameTextBox = null!;
        private TextBox _emailTextBox = null!;
        private TextBox _firstNameTextBox = null!;
        private TextBox _lastNameTextBox = null!;
        private ComboBox _roleComboBox = null!;
        private Button _saveButton = null!;
        private Button _cancelButton = null!;
        private User? _user;

        public AddEditUserForm(User? user = null)
        {
            _user = user;
            InitializeComponent();
            
            if (_user != null)
            {
                LoadUserData();
                this.Text = "✏️ Editar Usuario - Inventory QR Manager";
            }
            else
            {
                this.Text = "➕ Agregar Usuario - Inventory QR Manager";
            }
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();

           
            this.Size = new Size(450, 400);
            this.StartPosition = FormStartPosition.CenterParent;
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
                Text = _user != null ? "✏️ Editar Usuario" : "➕ Agregar Usuario",
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                ForeColor = Color.FromArgb(52, 58, 64),
                TextAlign = ContentAlignment.MiddleCenter,
                Size = new Size(390, 30),
                Location = new Point(30, 20)
            };

            var usernameLabel = new Label
            {
                Text = "👤 Usuario:",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = Color.FromArgb(52, 58, 64),
                Size = new Size(100, 25),
                Location = new Point(30, 70)
            };

            _usernameTextBox = new TextBox
            {
                Font = new Font("Segoe UI", 10),
                Size = new Size(300, 30),
                Location = new Point(30, 95),
                PlaceholderText = "Nombre de usuario"
            };

            var emailLabel = new Label
            {
                Text = "📧 Email:",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = Color.FromArgb(52, 58, 64),
                Size = new Size(100, 25),
                Location = new Point(30, 140)
            };

            _emailTextBox = new TextBox
            {
                Font = new Font("Segoe UI", 10),
                Size = new Size(300, 30),
                Location = new Point(30, 165),
                PlaceholderText = "correo@ejemplo.com"
            };

            var firstNameLabel = new Label
            {
                Text = "👤 Nombre:",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = Color.FromArgb(52, 58, 64),
                Size = new Size(100, 25),
                Location = new Point(30, 210)
            };

            _firstNameTextBox = new TextBox
            {
                Font = new Font("Segoe UI", 10),
                Size = new Size(140, 30),
                Location = new Point(30, 235),
                PlaceholderText = "Nombre"
            };

            var lastNameLabel = new Label
            {
                Text = "👤 Apellido:",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = Color.FromArgb(52, 58, 64),
                Size = new Size(100, 25),
                Location = new Point(190, 210)
            };

            _lastNameTextBox = new TextBox
            {
                Font = new Font("Segoe UI", 10),
                Size = new Size(140, 30),
                Location = new Point(190, 235),
                PlaceholderText = "Apellido"
            };

            var roleLabel = new Label
            {
                Text = "👑 Rol:",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = Color.FromArgb(52, 58, 64),
                Size = new Size(100, 25),
                Location = new Point(30, 280)
            };

            _roleComboBox = new ComboBox
            {
                Font = new Font("Segoe UI", 10),
                Size = new Size(200, 30),
                Location = new Point(30, 305),
                DropDownStyle = ComboBoxStyle.DropDownList
            };

            _roleComboBox.Items.Add("Usuario");
            _roleComboBox.Items.Add("Manager");
            _roleComboBox.Items.Add("Administrador");
            _roleComboBox.SelectedIndex = 0;

            var buttonPanel = new Panel
            {
                Size = new Size(390, 50),
                Location = new Point(30, 350),
                BackColor = Color.Transparent
            };

            _saveButton = new Button
            {
                Text = "💾 Guardar",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = Color.White,
                BackColor = Color.FromArgb(40, 167, 69),
                Size = new Size(120, 35),
                Location = new Point(0, 8),
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            _saveButton.FlatAppearance.BorderSize = 0;

            _cancelButton = new Button
            {
                Text = "❌ Cancelar",
                Font = new Font("Segoe UI", 10),
                ForeColor = Color.FromArgb(108, 117, 125),
                BackColor = Color.FromArgb(233, 236, 239),
                Size = new Size(120, 35),
                Location = new Point(130, 8),
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            _cancelButton.FlatAppearance.BorderSize = 0;

            buttonPanel.Controls.AddRange(new Control[] { _saveButton, _cancelButton });

            _saveButton.Click += SaveButton_Click;
            _cancelButton.Click += (s, e) => this.Close();

            mainPanel.Controls.AddRange(new Control[] {
                titleLabel, usernameLabel, _usernameTextBox, emailLabel, _emailTextBox,
                firstNameLabel, _firstNameTextBox, lastNameLabel, _lastNameTextBox,
                roleLabel, _roleComboBox, buttonPanel
            });

            this.Controls.Add(mainPanel);
            this.ResumeLayout(false);
        }

        private void LoadUserData()
        {
            if (_user != null)
            {
                _usernameTextBox.Text = _user.Username;
                _emailTextBox.Text = _user.Email;
                _firstNameTextBox.Text = _user.FirstName;
                _lastNameTextBox.Text = _user.LastName;
                _roleComboBox.SelectedIndex = (int)_user.Role - 1;
            }
        }

        private void SaveButton_Click(object? sender, EventArgs e)
        {
            if (ValidateInput())
            {
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
        }

        private bool ValidateInput()
        {
            if (string.IsNullOrWhiteSpace(_usernameTextBox.Text))
            {
                MessageBox.Show("⚠️ El nombre de usuario es requerido.", "Validación", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                _usernameTextBox.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(_emailTextBox.Text))
            {
                MessageBox.Show("⚠️ El email es requerido.", "Validación", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                _emailTextBox.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(_firstNameTextBox.Text))
            {
                MessageBox.Show("⚠️ El nombre es requerido.", "Validación", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                _firstNameTextBox.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(_lastNameTextBox.Text))
            {
                MessageBox.Show("⚠️ El apellido es requerido.", "Validación", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                _lastNameTextBox.Focus();
                return false;
            }

            return true;
        }

        public User GetUser()
        {
            return new User
            {
                Username = _usernameTextBox.Text.Trim(),
                Email = _emailTextBox.Text.Trim(),
                FirstName = _firstNameTextBox.Text.Trim(),
                LastName = _lastNameTextBox.Text.Trim(),
                Role = (UserRole)(_roleComboBox.SelectedIndex + 1),
                IsActive = true,
                CreatedDate = _user?.CreatedDate ?? DateTime.Now
            };
        }
    }
}
