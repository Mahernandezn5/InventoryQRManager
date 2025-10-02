using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using InventoryQRManager.Models;
using InventoryQRManager.Services;
using InventoryQRManager.Data;

namespace InventoryQRManager.Views
{
    public partial class UserManagementForm : Form
    {
        private readonly AuthService _authService;
        private readonly DatabaseContext _context;
        private DataGridView _usersDataGrid;
        private Button _addUserButton;
        private Button _editUserButton;
        private Button _deleteUserButton;
        private Button _refreshButton;

        public UserManagementForm(AuthService authService)
        {
            _authService = authService;
            _context = new DatabaseContext();
            InitializeComponent();
            LoadUsers();
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();

            this.Text = "👥 Gestión de Usuarios - Inventory QR Manager";
            this.Size = new Size(800, 600);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.FromArgb(248, 249, 250);

            var mainPanel = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(20)
            };

            var titleLabel = new Label
            {
                Text = "👥 Gestión de Usuarios",
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                ForeColor = Color.FromArgb(52, 58, 64),
                Size = new Size(300, 30),
                Location = new Point(20, 20)
            };

            var buttonPanel = new Panel
            {
                Size = new Size(760, 50),
                Location = new Point(20, 60),
                BackColor = Color.Transparent
            };

            _addUserButton = new Button
            {
                Text = "➕ Agregar Usuario",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = Color.White,
                BackColor = Color.FromArgb(40, 167, 69),
                Size = new Size(150, 35),
                Location = new Point(0, 8),
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            _addUserButton.FlatAppearance.BorderSize = 0;

            _editUserButton = new Button
            {
                Text = "✏️ Editar Usuario",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = Color.White,
                BackColor = Color.FromArgb(255, 193, 7),
                Size = new Size(150, 35),
                Location = new Point(160, 8),
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            _editUserButton.FlatAppearance.BorderSize = 0;

            _deleteUserButton = new Button
            {
                Text = "🗑️ Eliminar Usuario",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = Color.White,
                BackColor = Color.FromArgb(220, 53, 69),
                Size = new Size(150, 35),
                Location = new Point(320, 8),
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            _deleteUserButton.FlatAppearance.BorderSize = 0;

            _refreshButton = new Button
            {
                Text = "🔄 Actualizar",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = Color.White,
                BackColor = Color.FromArgb(0, 123, 255),
                Size = new Size(120, 35),
                Location = new Point(640, 8),
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            _refreshButton.FlatAppearance.BorderSize = 0;

            buttonPanel.Controls.AddRange(new Control[] { _addUserButton, _editUserButton, _deleteUserButton, _refreshButton });

            _usersDataGrid = new DataGridView
            {
                Size = new Size(760, 400),
                Location = new Point(20, 120),
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.Fixed3D,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                Font = new Font("Segoe UI", 9)
            };

            _usersDataGrid.Columns.Add("Id", "ID");
            _usersDataGrid.Columns.Add("Username", "Usuario");
            _usersDataGrid.Columns.Add("Email", "Email");
            _usersDataGrid.Columns.Add("FullName", "Nombre Completo");
            _usersDataGrid.Columns.Add("Role", "Rol");
            _usersDataGrid.Columns.Add("CreatedDate", "Fecha Creación");
            _usersDataGrid.Columns.Add("LastLoginDate", "Último Login");

            _usersDataGrid.Columns["Id"].Visible = false;

            _addUserButton.Click += AddUserButton_Click;
            _editUserButton.Click += EditUserButton_Click;
            _deleteUserButton.Click += DeleteUserButton_Click;
            _refreshButton.Click += RefreshButton_Click;

            mainPanel.Controls.AddRange(new Control[] { titleLabel, buttonPanel, _usersDataGrid });
            this.Controls.Add(mainPanel);

            this.ResumeLayout(false);
        }

        private void LoadUsers()
        {
            try
            {
                _usersDataGrid.Rows.Clear();
                var users = _authService.GetAllUsers();

                foreach (var user in users)
                {
                    _usersDataGrid.Rows.Add(
                        user.Id,
                        user.Username,
                        user.Email,
                        user.FullName,
                        GetRoleDisplayName(user.Role),
                        user.CreatedDate.ToString("dd/MM/yyyy"),
                        user.LastLoginDate?.ToString("dd/MM/yyyy HH:mm") ?? "Nunca"
                    );
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error cargando usuarios: {ex.Message}", "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void AddUserButton_Click(object sender, EventArgs e)
        {
            var addUserForm = new AddEditUserForm();
            if (addUserForm.ShowDialog() == DialogResult.OK)
            {
                var newUser = addUserForm.GetUser();
                if (_authService.Register(newUser.Username, newUser.Email, "temp123", 
                    newUser.FirstName, newUser.LastName, newUser.Role))
                {
                    MessageBox.Show("✅ Usuario agregado exitosamente.", "Éxito", 
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadUsers();
                }
                else
                {
                    MessageBox.Show("❌ Error al agregar usuario. Verifique que el usuario no exista.", 
                        "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void EditUserButton_Click(object sender, EventArgs e)
        {
            if (_usersDataGrid.SelectedRows.Count == 0)
            {
                MessageBox.Show("⚠️ Por favor, seleccione un usuario para editar.", "Selección Requerida", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var selectedRow = _usersDataGrid.SelectedRows[0];
            var userId = (int)selectedRow.Cells["Id"].Value;
            var user = _authService.GetAllUsers().FirstOrDefault(u => u.Id == userId);

            if (user != null)
            {
                var editUserForm = new AddEditUserForm(user);
                if (editUserForm.ShowDialog() == DialogResult.OK)
                {
                    var updatedUser = editUserForm.GetUser();
                    updatedUser.Id = user.Id;
                    updatedUser.PasswordHash = user.PasswordHash; // Mantener la contraseña actual

                    if (_authService.UpdateUser(updatedUser))
                    {
                        MessageBox.Show("✅ Usuario actualizado exitosamente.", "Éxito", 
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                        LoadUsers();
                    }
                    else
                    {
                        MessageBox.Show("❌ Error al actualizar usuario.", "Error", 
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void DeleteUserButton_Click(object sender, EventArgs e)
        {
            if (_usersDataGrid.SelectedRows.Count == 0)
            {
                MessageBox.Show("⚠️ Por favor, seleccione un usuario para eliminar.", "Selección Requerida", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var selectedRow = _usersDataGrid.SelectedRows[0];
            var userId = (int)selectedRow.Cells["Id"].Value;
            var username = selectedRow.Cells["Username"].Value.ToString();

            var result = MessageBox.Show($"¿Está seguro de que desea eliminar al usuario '{username}'?\n\n" +
                                        "Esta acción no se puede deshacer.",
                "Confirmar Eliminación", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                if (_authService.DeleteUser(userId))
                {
                    MessageBox.Show("✅ Usuario eliminado exitosamente.", "Éxito", 
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadUsers();
                }
                else
                {
                    MessageBox.Show("❌ Error al eliminar usuario.", "Error", 
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void RefreshButton_Click(object sender, EventArgs e)
        {
            LoadUsers();
        }

        private string GetRoleDisplayName(UserRole role)
        {
            return role switch
            {
                UserRole.Admin => "Administrador",
                UserRole.Manager => "Manager",
                UserRole.User => "Usuario",
                _ => "Desconocido"
            };
        }
    }
}
