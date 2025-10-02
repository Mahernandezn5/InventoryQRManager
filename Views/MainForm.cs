using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using InventoryQRManager.Models;
using InventoryQRManager.Services;
using InventoryQRManager.Data;

namespace InventoryQRManager.Views
{
    public partial class MainForm : Form
    {
        private readonly InventoryService _inventoryService;
        private readonly QRCodeService _qrCodeService;
        private readonly HistoryService _historyService;
        private readonly ReportService _reportService;
        private readonly BackupService _backupService;
        private readonly SettingsService _settingsService;
        private readonly ApiTestService _apiTestService;
        private readonly AuthService _authService;
        private readonly DatabaseContext _context;
        private List<InventoryItem> _inventoryItems;

        public MainForm()
        {
            _context = new DatabaseContext();
            _authService = new AuthService(_context);
            _inventoryService = new InventoryService();
            _qrCodeService = new QRCodeService();
            _historyService = new HistoryService();
            _reportService = new ReportService();
            _backupService = new BackupService();
            _settingsService = new SettingsService();
            _apiTestService = new ApiTestService();
            _inventoryItems = new List<InventoryItem>();
            
            if (!ShowLogin())
            {
                Application.Exit();
                return;
            }
            
            InitializeComponent();
            
            LoadInventoryData();
            LoadSettings();
            
            ApplyProfessionalTheme();
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            
            this.Text = $"Inventory QR Manager - Sistema de Gestión de Inventario | Usuario: {_authService.CurrentUser?.FullName ?? "Desconocido"}";
            this.Size = new Size(1400, 900);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.MinimumSize = new Size(1000, 700);
            this.BackColor = Color.FromArgb(236, 240, 241);
            this.Font = new Font("Segoe UI", 9F, FontStyle.Regular);
            this.WindowState = FormWindowState.Maximized;

            CreateMenuStrip();
            
            CreateToolStrip();
            
            CreateDataGridView();
            
            CreateInfoPanel();
            
            CreateQRPanel();

            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private void CreateMenuStrip()
        {
            var menuStrip = new MenuStrip();
            
            var fileMenu = new ToolStripMenuItem("&Archivo");
            fileMenu.DropDownItems.Add("&Importar", null, (s, e) => ImportData());
            fileMenu.DropDownItems.Add("&Exportar", null, (s, e) => ExportData());
            fileMenu.DropDownItems.Add(new ToolStripSeparator());
            fileMenu.DropDownItems.Add("&Salir", null, (s, e) => this.Close());
            
            var inventoryMenu = new ToolStripMenuItem("&Inventario");
            inventoryMenu.DropDownItems.Add("&Ver Todos", null, (s, e) => LoadInventoryData());
            inventoryMenu.DropDownItems.Add("&Buscar Avanzada", null, (s, e) => ShowAdvancedSearch());
            inventoryMenu.DropDownItems.Add("&Reportes y Estadísticas", null, (s, e) => ShowReports());
            inventoryMenu.DropDownItems.Add(new ToolStripSeparator());
            inventoryMenu.DropDownItems.Add("&Stock Bajo", null, (s, e) => ShowLowStockItems());
            
            var qrMenu = new ToolStripMenuItem("&Códigos QR");
            qrMenu.DropDownItems.Add("&Escanear QR", null, (s, e) => ScanQRCode());
            qrMenu.DropDownItems.Add("&Imprimir QR", null, (s, e) => PrintQRCode());
            
            var toolsMenu = new ToolStripMenuItem("&Herramientas");
            toolsMenu.DropDownItems.Add("📊 &Dashboard Ejecutivo", null, (s, e) => ShowDashboard());
            toolsMenu.DropDownItems.Add(new ToolStripSeparator());
            toolsMenu.DropDownItems.Add("👥 &Gestión de Usuarios", null, (s, e) => ShowUserManagement());
            toolsMenu.DropDownItems.Add("🔐 &Cambiar Usuario", null, (s, e) => ChangeUser());
            toolsMenu.DropDownItems.Add(new ToolStripSeparator());
            toolsMenu.DropDownItems.Add("&Configuración", null, (s, e) => ShowSettings());
            toolsMenu.DropDownItems.Add("&Backup", null, (s, e) => ShowBackupDialog());
            toolsMenu.DropDownItems.Add(new ToolStripSeparator());
            toolsMenu.DropDownItems.Add("🌐 &Probar API REST", null, (s, e) => TestApiConnection());
            toolsMenu.DropDownItems.Add("📋 &Historial de Movimientos", null, (s, e) => ShowHistory());
            
            var helpMenu = new ToolStripMenuItem("&Ayuda");
            helpMenu.DropDownItems.Add("&Acerca de", null, (s, e) => ShowAbout());
            
            menuStrip.Items.AddRange(new ToolStripItem[] { fileMenu, inventoryMenu, qrMenu, toolsMenu, helpMenu });
            this.MainMenuStrip = menuStrip;
            this.Controls.Add(menuStrip);
        }

        private void CreateToolStrip()
        {
            var toolStrip = new ToolStrip();
            toolStrip.BackColor = Color.FromArgb(52, 73, 94); 
            toolStrip.ForeColor = Color.White;
            toolStrip.Font = new Font("Segoe UI", 9F, FontStyle.Regular);
            toolStrip.Padding = new Padding(10, 5, 10, 5);
            
            var primaryButtonColor = Color.FromArgb(41, 128, 185); o
            
            var newButton = new ToolStripButton("➕ Nuevo Item", null, (s, e) => ShowAddItemForm());
            newButton.BackColor = primaryButtonColor;
            newButton.ForeColor = Color.White;
            newButton.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            
            var editButton = new ToolStripButton("✏️ Editar", null, (s, e) => EditSelectedItem());
            editButton.BackColor = primaryButtonColor;
            editButton.ForeColor = Color.White;
            editButton.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            
            var deleteButton = new ToolStripButton("🗑️ Eliminar", null, (s, e) => DeleteSelectedItem());
            deleteButton.BackColor = primaryButtonColor;
            deleteButton.ForeColor = Color.White;
            deleteButton.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            
            var separator1 = new ToolStripSeparator();
            separator1.BackColor = Color.FromArgb(149, 165, 166);
            
            
            var historyButton = new ToolStripButton("📋 Historial", null, (s, e) => ShowHistory());
            historyButton.BackColor = Color.FromArgb(155, 89, 182);
            historyButton.ForeColor = Color.White;
            historyButton.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            
            var separator2 = new ToolStripSeparator();
            separator2.BackColor = Color.FromArgb(149, 165, 166);
            
            var refreshButton = new ToolStripButton("🔄 Actualizar", null, (s, e) => LoadInventoryData());
            refreshButton.BackColor = primaryButtonColor;
            refreshButton.ForeColor = Color.White;
            refreshButton.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            
            toolStrip.Items.AddRange(new ToolStripItem[] { 
                newButton, editButton, deleteButton, separator1, 
                historyButton, separator2, refreshButton 
            });
            
            this.Controls.Add(toolStrip);
        }

        private void CreateDataGridView()
        {
            dataGridView = new DataGridView();
            dataGridView.Dock = DockStyle.Fill;
            dataGridView.AllowUserToAddRows = false;
            dataGridView.AllowUserToDeleteRows = false;
            dataGridView.ReadOnly = true;
            dataGridView.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridView.MultiSelect = false;
            dataGridView.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;
            dataGridView.CellDoubleClick += DataGridView_CellDoubleClick;
            dataGridView.SelectionChanged += DataGridView_SelectionChanged;
            
            dataGridView.RowHeadersVisible = false;
            dataGridView.EnableHeadersVisualStyles = false;
            
            dataGridView.ColumnHeadersVisible = true;
            dataGridView.ColumnHeadersHeight = 50;
            
            dataGridView.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(41, 128, 185);
            dataGridView.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dataGridView.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            dataGridView.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView.ColumnHeadersDefaultCellStyle.Padding = new Padding(8, 8, 8, 8);
            
            dataGridView.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridView.DefaultCellStyle.Font = new Font("Segoe UI", 10F, FontStyle.Regular);
            dataGridView.DefaultCellStyle.Padding = new Padding(8, 4, 8, 4);
            dataGridView.GridColor = Color.FromArgb(220, 220, 220);
            dataGridView.BorderStyle = BorderStyle.None;
            dataGridView.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            dataGridView.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
            
            dataGridView.RowTemplate.Height = 50;
            
            dataGridView.Margin = new Padding(10, 10, 10, 0);
            
            SetupDataGridViewColumns();
            
            this.Controls.Add(dataGridView);
        }

        private void CreateInfoPanel()
        {
            var infoPanel = new Panel();
            infoPanel.Dock = DockStyle.Bottom;
            infoPanel.Height = 60;
            infoPanel.BackColor = Color.FromArgb(52, 73, 94);
            infoPanel.Padding = new Padding(15, 10, 15, 10);
            
            var infoLabel = new Label();
            infoLabel.Text = "Total de items: 0 | Items seleccionados: 0";
            infoLabel.Dock = DockStyle.Fill;
            infoLabel.TextAlign = ContentAlignment.MiddleCenter;
            infoLabel.Font = new Font("Segoe UI", 11, FontStyle.Bold);
            infoLabel.ForeColor = Color.White;
            
            infoPanel.Controls.Add(infoLabel);
            this.Controls.Add(infoPanel);
        }

        private void CreateQRPanel()
        {
            var qrPanel = new Panel();
            qrPanel.Dock = DockStyle.Right;
            qrPanel.Width = 320;
            qrPanel.BackColor = Color.White;
            qrPanel.Padding = new Padding(0);
            
            var mainContainer = new Panel();
            mainContainer.Dock = DockStyle.Fill;
            mainContainer.BackColor = Color.White;
            mainContainer.Padding = new Padding(15);
            
            var qrLabel = new Label();
            qrLabel.Text = "🔍 Código QR";
            qrLabel.Dock = DockStyle.Top;
            qrLabel.TextAlign = ContentAlignment.MiddleCenter;
            qrLabel.Font = new Font("Segoe UI", 14, FontStyle.Bold);
            qrLabel.Height = 45;
            qrLabel.BackColor = Color.FromArgb(41, 128, 185);
            qrLabel.ForeColor = Color.White;
            qrLabel.Padding = new Padding(0, 10, 0, 10);
            
            var qrImagePanel = new Panel();
            qrImagePanel.Dock = DockStyle.Fill;
            qrImagePanel.BackColor = Color.FromArgb(248, 249, 250);
            qrImagePanel.Padding = new Padding(20);
            
            qrPictureBox = new PictureBox();
            qrPictureBox.Dock = DockStyle.Fill;
            qrPictureBox.SizeMode = PictureBoxSizeMode.Zoom;
            qrPictureBox.BorderStyle = BorderStyle.None;
            qrPictureBox.BackColor = Color.White;
            qrPictureBox.Padding = new Padding(10);
            
            var itemInfoPanel = new Panel();
            itemInfoPanel.Dock = DockStyle.Bottom;
            itemInfoPanel.Height = 160;
            itemInfoPanel.BackColor = Color.FromArgb(52, 73, 94);
            itemInfoPanel.Padding = new Padding(15, 10, 15, 10);
            
            itemInfoLabel = new Label();
            itemInfoLabel.Text = "Seleccione un item para ver su código QR";
            itemInfoLabel.Dock = DockStyle.Fill;
            itemInfoLabel.TextAlign = ContentAlignment.MiddleCenter;
            itemInfoLabel.Font = new Font("Segoe UI", 10, FontStyle.Regular);
            itemInfoLabel.ForeColor = Color.White;
            itemInfoLabel.Padding = new Padding(8);
            
            itemInfoPanel.Controls.Add(itemInfoLabel);
            
            qrImagePanel.Controls.Add(qrPictureBox);
            mainContainer.Controls.Add(qrImagePanel);
            mainContainer.Controls.Add(itemInfoPanel);
            mainContainer.Controls.Add(qrLabel);
            
            qrPanel.Controls.Add(mainContainer);
            this.Controls.Add(qrPanel);
        }

        private void SetupDataGridViewColumns()
        {
            try
            {
                dataGridView.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;
                
                dataGridView.Columns.Clear();
                
                dataGridView.Columns.Add("Id", "ID");
                dataGridView.Columns.Add("Name", "Nombre");
                dataGridView.Columns.Add("SKU", "Código SKU");
                dataGridView.Columns.Add("Quantity", "Cantidad");
                dataGridView.Columns.Add("Price", "Precio");
                dataGridView.Columns.Add("Category", "Categoría");
                dataGridView.Columns.Add("Location", "Ubicación");
                dataGridView.Columns.Add("CreatedDate", "Fecha");
                
                foreach (DataGridViewColumn column in dataGridView.Columns)
                {
                    try
                    {
                        column.AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
                        column.FillWeight = 100; 
                        
                        switch (column.Name)
                        {
                            case "Id":
                                column.Visible = false;
                                column.Width = 0;
                                break;
                            case "Name":
                                column.Width = 280;
                                break;
                            case "SKU":
                                column.Width = 120;
                                break;
                            case "Quantity":
                                column.Width = 90;
                                column.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                                break;
                            case "Price":
                                column.Width = 110;
                                column.DefaultCellStyle.Format = "C2";
                                column.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                                break;
                            case "Category":
                                column.Width = 130;
                                break;
                            case "Location":
                                column.Width = 130;
                                break;
                            case "CreatedDate":
                                column.Width = 120;
                                column.DefaultCellStyle.Format = "dd/MM/yyyy";
                                column.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                                break;
                        }
                    }
                    catch (Exception columnEx)
                    {
                        System.Diagnostics.Debug.WriteLine($"Error configurando columna {column.Name}: {columnEx.Message}");
                    }
                }
                
                dataGridView.ColumnHeadersVisible = true;
                dataGridView.EnableHeadersVisualStyles = false;
                dataGridView.ColumnHeadersHeight = 50;
                
                dataGridView.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(41, 128, 185);
                dataGridView.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
                dataGridView.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
                dataGridView.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                
                dataGridView.Invalidate();
                dataGridView.Refresh();
                
                dataGridView.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error configurando columnas del DataGridView: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadInventoryData()
        {
            try
            {
                _inventoryItems = _inventoryService.GetAllItems();
                
                if (dataGridView != null && dataGridView.Columns.Count > 0)
                {
                    EnsureDataGridViewIsValid();
                    RefreshDataGridView();
                    UpdateInfoLabel();
                    
                    ClearSelectionAndQRPanel();
                }
                else if (dataGridView != null && dataGridView.Columns.Count == 0)
                {
                    SetupDataGridViewColumns();
                    RefreshDataGridView();
                    UpdateInfoLabel();
                    
                    ClearSelectionAndQRPanel();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error cargando datos: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        
        private void ClearSelectionAndQRPanel()
        {
            try
            {
                if (dataGridView != null)
                {
                    dataGridView.ClearSelection();
                }
                
                if (qrPictureBox != null && qrPictureBox.Image != null)
                {
                    qrPictureBox.Image.Dispose();
                    qrPictureBox.Image = null;
                }
                
                if (itemInfoLabel != null)
                {
                    itemInfoLabel.Text = "Seleccione un item para ver detalles";
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error limpiando selección: {ex.Message}");
            }
        }
        
        private void EnsureDataGridViewIsValid()
        {
            if (dataGridView == null || dataGridView.Columns.Count == 0)
            {
                return;
            }
            
            if (dataGridView.AutoSizeColumnsMode != DataGridViewAutoSizeColumnsMode.None)
            {
                dataGridView.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;
            }
        }

        private void RefreshDataGridView()
        {
            if (dataGridView == null || dataGridView.Columns.Count == 0)
            {
                return; 
            }
            
            dataGridView.Rows.Clear();
            
            foreach (var item in _inventoryItems)
            {
                dataGridView.Rows.Add(
                    item.Id,
                    item.Name,
                    item.SKU,
                    item.Quantity,
                    item.Price,
                    item.Category,
                    item.Location,
                    item.CreatedDate
                );
            }
        }

        private void UpdateInfoLabel()
        {
            try
            {
                if (dataGridView == null)
                {
                    return; 
                }
                
                var selectedCount = dataGridView.SelectedRows.Count;
                var totalCount = _inventoryItems.Count;
                var activeItems = _inventoryItems.Count(item => item.IsActive);
                var totalValue = _inventoryItems.Where(item => item.IsActive).Sum(item => item.Price * item.Quantity);
                var lowStockItems = _inventoryItems.Count(item => item.IsActive && item.Quantity <= 5);
                
                var infoPanel = this.Controls.OfType<Panel>().FirstOrDefault(p => p.Dock == DockStyle.Bottom);
                if (infoPanel != null)
                {
                    var infoLabel = infoPanel.Controls.OfType<Label>().FirstOrDefault();
                    if (infoLabel != null)
                    {
                        infoLabel.Text = $"Total: {totalCount} items | Valor: ${totalValue:F2} | Stock Bajo: {lowStockItems} | Seleccionados: {selectedCount}";
                        
                        infoLabel.ForeColor = Color.Black;
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error actualizando label de información: {ex.Message}");
            }
        }

        private void DataGridView_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (dataGridView == null || dataGridView.Columns.Count == 0)
            {
                return; 
            }
            
            if (e.RowIndex >= 0)
            {
                EditSelectedItem();
            }
        }

        private void DataGridView_SelectionChanged(object sender, EventArgs e)
        {
            if (dataGridView == null || dataGridView.Columns.Count == 0)
            {
                return; 
            }
            
            if (dataGridView.SelectedRows.Count > 0)
            {
                try
                {
                    var selectedRow = dataGridView.SelectedRows[0];
                    var itemId = Convert.ToInt32(selectedRow.Cells["Id"].Value);
                    var item = _inventoryService.GetItemById(itemId);
                    
                    if (item != null)
                    {
                        
                        if (string.IsNullOrEmpty(item.QRCode))
                        {
                            item.QRCode = $"ITEM_{item.Id}_{item.SKU}";
                            _inventoryService.UpdateItem(item);
                        }
                        
                        
                        var qrCodeImage = _qrCodeService.GenerateQRCode(item.QRCode);
                        if (qrPictureBox != null)
                        {
                            
                            if (qrPictureBox.Image != null)
                            {
                                qrPictureBox.Image.Dispose();
                            }
                            
                            qrPictureBox.Image = qrCodeImage;
                            qrPictureBox.SizeMode = PictureBoxSizeMode.Zoom;
                            qrPictureBox.Refresh();
                        }
                        
                       
                        UpdateQRPanelInfo(item);
                    }
                }
                catch (Exception ex)
                {
                    if (itemInfoLabel != null)
                    {
                        itemInfoLabel.Text = $"Error generando QR: {ex.Message}";
                    }
                    System.Diagnostics.Debug.WriteLine($"Error generando QR automático: {ex.Message}");
                }
            }
            else
            {
                if (qrPictureBox != null)
                {
                    if (qrPictureBox.Image != null)
                    {
                        qrPictureBox.Image.Dispose();
                    }
                    qrPictureBox.Image = null;
                    qrPictureBox.Refresh();
                }
                if (itemInfoLabel != null)
                {
                    itemInfoLabel.Text = "Seleccione un item para ver su código QR";
                }
            }
        }

        public void ShowAddItemForm()
        {
            try
            {
                var addForm = new AddEditItemForm();
                addForm.FormClosed += (sender, e) => {
                    LoadInventoryData();
                };
                addForm.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error abriendo formulario de nuevo item: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void EditSelectedItem()
        {
            try
            {
                if (dataGridView == null || dataGridView.Columns.Count == 0)
                {
                    MessageBox.Show("El DataGridView no está configurado correctamente.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                
                if (dataGridView.SelectedRows.Count > 0)
                {
                    var selectedRow = dataGridView.SelectedRows[0];
                    var itemId = Convert.ToInt32(selectedRow.Cells["Id"].Value);
                    var item = _inventoryService.GetItemById(itemId);
                    
                    if (item != null)
                    {
                        var editForm = new AddEditItemForm(item);
                        editForm.FormClosed += (sender, e) => {
                            
                            LoadInventoryData();
                        };
                        editForm.Show();
                    }
                }
                else
                {
                    MessageBox.Show("Por favor seleccione un item para editar.", "Información", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error editando item: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void DeleteSelectedItem()
        {
            if (dataGridView == null || dataGridView.Columns.Count == 0)
            {
                MessageBox.Show("El DataGridView no está configurado correctamente.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            
            if (dataGridView.SelectedRows.Count > 0)
            {
                var selectedRow = dataGridView.SelectedRows[0];
                var itemId = Convert.ToInt32(selectedRow.Cells["Id"].Value);
                var itemName = selectedRow.Cells["Name"].Value.ToString();
                
                var result = MessageBox.Show($"¿Está seguro de que desea eliminar el item '{itemName}'?", 
                    "Confirmar eliminación", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                
                if (result == DialogResult.Yes)
                {
                    
                    var item = _inventoryService.GetItemById(itemId);
                    
                    if (_inventoryService.DeleteItem(itemId))
                    {
                       
                        if (item != null)
                        {
                            _historyService.RecordAction(item, HistoryAction.DELETE, "Eliminación manual");
                        }
                        
                        MessageBox.Show("Item eliminado correctamente y registrado en historial.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        LoadInventoryData();
                    }
                    else
                    {
                        MessageBox.Show("Error al eliminar el item.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            else
            {
                MessageBox.Show("Por favor seleccione un item para eliminar.", "Información", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void GenerateQRForSelected()
        {
            if (dataGridView == null || dataGridView.Columns.Count == 0)
            {
                MessageBox.Show("El DataGridView no está configurado correctamente.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            
            if (dataGridView.SelectedRows.Count > 0)
            {
                var selectedRow = dataGridView.SelectedRows[0];
                var itemId = Convert.ToInt32(selectedRow.Cells["Id"].Value);
                var item = _inventoryService.GetItemById(itemId);
                
                if (item != null)
                {
                    try
                    {
                        if (string.IsNullOrEmpty(item.QRCode))
                        {
                            MessageBox.Show("Este item no tiene un código QR. Los códigos QR se generan automáticamente al crear o editar items.", "Información", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            return;
                        }
                        
                        var qrCodeImage = _qrCodeService.GenerateQRCode(item.QRCode);
                        if (qrPictureBox != null)
                        {
                            if (qrPictureBox.Image != null)
                            {
                                qrPictureBox.Image.Dispose();
                            }
                            
                            qrPictureBox.Image = qrCodeImage;
                            qrPictureBox.Refresh(); 
                        }
                        
                        UpdateQRPanelInfo(item);
                        
                        MessageBox.Show("Código QR mostrado correctamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error mostrando código QR: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else
                {
                    MessageBox.Show("No se pudo encontrar el item seleccionado.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("Por favor seleccione un item para mostrar el código QR.", "Información", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void ScanQRCode()
        {
            MessageBox.Show("Funcionalidad de escaneo de QR en desarrollo.", "Información", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void PrintQRCode()
        {
            try
            {
                if (qrPictureBox?.Image == null)
                {
                    MessageBox.Show("No hay código QR para imprimir. Por favor seleccione un item primero.", "Información", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                InventoryItem? selectedItem = null;
                if (dataGridView?.SelectedRows.Count > 0)
                {
                    var selectedRow = dataGridView.SelectedRows[0];
                    var itemId = Convert.ToInt32(selectedRow.Cells["Id"].Value);
                    selectedItem = _inventoryService.GetItemById(itemId);
                }

                using (var saveFileDialog = new SaveFileDialog())
                {
                    saveFileDialog.Filter = "Imagen PNG (*.png)|*.png|Imagen JPEG (*.jpg)|*.jpg|Imagen BMP (*.bmp)|*.bmp|Todos los archivos (*.*)|*.*";
                    saveFileDialog.FilterIndex = 1;
                    saveFileDialog.DefaultExt = "png";
                    
                    if (selectedItem != null)
                    {
                        var fileName = $"QR_{selectedItem.Name?.Replace(" ", "_") ?? "Item"}_{selectedItem.SKU ?? selectedItem.Id.ToString()}";
                        saveFileDialog.FileName = fileName;
                    }
                    else
                    {
                        saveFileDialog.FileName = $"QR_Item_{DateTime.Now:yyyyMMdd_HHmmss}";
                    }

                    if (saveFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        var imageFormat = ImageFormat.Png;
                        switch (saveFileDialog.FilterIndex)
                        {
                            case 1: imageFormat = ImageFormat.Png; break;
                            case 2: imageFormat = ImageFormat.Jpeg; break;
                            case 3: imageFormat = ImageFormat.Bmp; break;
                        }

                        using (var bitmap = new Bitmap(qrPictureBox.Image))
                        {
                            bitmap.Save(saveFileDialog.FileName, imageFormat);
                        }

                        var message = $"Código QR guardado exitosamente en:\n{saveFileDialog.FileName}";
                        if (selectedItem != null)
                        {
                            message += $"\n\nItem: {selectedItem.Name}\nSKU: {selectedItem.SKU}";
                        }
                        
                        MessageBox.Show(message, "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);

                        var result = MessageBox.Show("¿Desea abrir la carpeta donde se guardó el archivo?", "Abrir carpeta", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                        if (result == DialogResult.Yes)
                        {
                            System.Diagnostics.Process.Start("explorer.exe", $"/select,\"{saveFileDialog.FileName}\"");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al guardar el código QR: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ImportData()
        {
            try
            {
                var openDialog = new OpenFileDialog();
                openDialog.Filter = "Archivos JSON (*.json)|*.json|Archivos CSV (*.csv)|*.csv|Todos los archivos (*.*)|*.*";
                openDialog.Title = "Seleccionar archivo para importar";
                
                if (openDialog.ShowDialog() == DialogResult.OK)
                {
                    var fileExtension = Path.GetExtension(openDialog.FileName).ToLower();
                    
                    var result = MessageBox.Show($"¿Está seguro de que desea importar desde {Path.GetFileName(openDialog.FileName)}?\n\n" +
                                                "Esta acción puede agregar nuevos items al inventario o reemplazar los existentes.", 
                                                "Confirmar Importación", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    
                    if (result == DialogResult.Yes)
                    {
                        if (fileExtension == ".json")
                        {
                            _backupService.RestoreBackup(openDialog.FileName);
                            MessageBox.Show("Importación desde JSON completada exitosamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        else if (fileExtension == ".csv")
                        {
                            _backupService.ImportFromCSV(openDialog.FileName);
                            MessageBox.Show("Importación desde CSV completada exitosamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        else
                        {
                            MessageBox.Show("Formato de archivo no soportado. Use archivos JSON o CSV.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }
                        
                        LoadInventoryData();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error durante la importación: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ExportData()
        {
            try
            {
                var saveDialog = new SaveFileDialog();
                saveDialog.Filter = "Archivos JSON (*.json)|*.json|Archivos CSV (*.csv)|*.csv";
                saveDialog.Title = "Guardar archivo de exportación";
                saveDialog.FileName = $"inventario_{DateTime.Now:yyyyMMdd_HHmmss}";
                
                if (saveDialog.ShowDialog() == DialogResult.OK)
                {
                    var fileExtension = Path.GetExtension(saveDialog.FileName).ToLower();
                    
                    if (fileExtension == ".json")
                    {
                        _backupService.CreateBackup(saveDialog.FileName);
                        MessageBox.Show("Exportación a JSON completada exitosamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else if (fileExtension == ".csv")
                    {
                        _backupService.ExportToCSV(saveDialog.FileName);
                        MessageBox.Show("Exportación a CSV completada exitosamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        MessageBox.Show("Formato de archivo no soportado. Use archivos JSON o CSV.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error durante la exportación: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public void ShowAdvancedSearch()
        {
            try
            {
                var searchForm = new SearchForm();
                searchForm.FormClosed += (sender, e) => {
                    LoadInventoryData();
                };
                searchForm.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error abriendo formulario de búsqueda: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public void ShowReports()
        {
            try
            {
                var reportsForm = new ReportsForm();
                reportsForm.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error abriendo formulario de reportes: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ShowLowStockItems()
        {
            try
            {
                var lowStockItems = _reportService.GetLowStockItems();
                var threshold = _settingsService.GetSetting<int>("LowStockThreshold");
                
                if (lowStockItems.Count == 0)
                {
                    MessageBox.Show($"No hay items con stock bajo (≤ {threshold} unidades).", "Información", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    var message = $"Items con stock bajo (≤ {threshold} unidades):\n\n";
                    foreach (var item in lowStockItems.Take(10))
                    {
                        message += $"• {item.Name} ({item.SKU}) - {item.Quantity} unidades\n";
                    }
                    
                    if (lowStockItems.Count > 10)
                    {
                        message += $"\n... y {lowStockItems.Count - 10} items más.";
                    }
                    
                    MessageBox.Show(message, "Stock Bajo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error obteniendo items con stock bajo: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public void ShowSettings()
        {
            var settingsForm = new SettingsForm();
            if (settingsForm.ShowDialog() == DialogResult.OK)
            {
                LoadSettings();
                LoadInventoryData();
            }
        }

        private void ShowBackupDialog()
        {
            var result = MessageBox.Show("¿Desea crear un backup de la base de datos actual?", "Backup", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            
            if (result == DialogResult.Yes)
            {
                var saveDialog = new SaveFileDialog();
                saveDialog.Filter = "Archivos de Backup (*.json)|*.json";
                saveDialog.FileName = $"backup_{DateTime.Now:yyyyMMdd_HHmmss}.json";
                
                if (saveDialog.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        _backupService.CreateBackup(saveDialog.FileName);
                        MessageBox.Show("Backup creado exitosamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error creando backup: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void ImportFromCSV()
        {
            var openDialog = new OpenFileDialog();
            openDialog.Filter = "Archivos CSV (*.csv)|*.csv";
            
            if (openDialog.ShowDialog() == DialogResult.OK)
            {
                var result = MessageBox.Show("¿Está seguro de que desea importar desde CSV? Esta acción agregará nuevos items al inventario.", 
                    "Confirmar Importación", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                
                if (result == DialogResult.Yes)
                {
                    try
                    {
                        _backupService.ImportFromCSV(openDialog.FileName);
                        MessageBox.Show("Importación completada exitosamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        LoadInventoryData();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error importando desde CSV: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void ExportToCSV()
        {
            var saveDialog = new SaveFileDialog();
            saveDialog.Filter = "Archivos CSV (*.csv)|*.csv";
            saveDialog.FileName = $"inventario_{DateTime.Now:yyyyMMdd_HHmmss}.csv";
            
            if (saveDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    _backupService.ExportToCSV(saveDialog.FileName);
                    MessageBox.Show("Exportación completada exitosamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error exportando a CSV: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void LoadSettings()
        {
            try
            {
                var settings = _settingsService.GetSettings();
                
                if (!string.IsNullOrEmpty(settings.CompanyName))
                {
                    this.Text = $"Inventory QR Manager - {settings.CompanyName}";
                }
                
                if (dataGridView != null && dataGridView.Columns.Count > 0)
                {
                    UpdateInfoLabel();
                }
            }
            catch (Exception ex)
            {
            }
        }

        private void ApplyProfessionalTheme()
        {
            ThemeService.ApplyTheme(this);
            
            ApplyCustomStyling();
        }

        private void ApplyCustomStyling()
        {
            try
            {
                var theme = ThemeService.CurrentTheme;
                
                if (dataGridView != null)
                {
                    dataGridView.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;
                    
                    dataGridView.RowHeadersWidth = 0;
                    dataGridView.ColumnHeadersHeight = 50;
                    dataGridView.RowTemplate.Height = 50;
                    
                    dataGridView.BackgroundColor = Color.White;
                    dataGridView.GridColor = Color.FromArgb(220, 220, 220);
                    dataGridView.BorderStyle = BorderStyle.None;
                    dataGridView.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
                    dataGridView.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
                    
                    if (dataGridView.Columns.Count > 0)
                    {
                        dataGridView.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(250, 250, 250);
                        dataGridView.AlternatingRowsDefaultCellStyle.ForeColor = Color.FromArgb(44, 62, 80);
                        
                        dataGridView.DefaultCellStyle.BackColor = Color.White;
                        dataGridView.DefaultCellStyle.ForeColor = Color.FromArgb(44, 62, 80);
                        
                        dataGridView.RowsDefaultCellStyle.SelectionBackColor = Color.FromArgb(52, 152, 219);
                        dataGridView.RowsDefaultCellStyle.SelectionForeColor = Color.White;
                        
                        dataGridView.DefaultCellStyle.Padding = new Padding(8, 8, 8, 8);
                        
                        foreach (DataGridViewColumn column in dataGridView.Columns)
                        {
                            try
                            {
                                if (column.AutoSizeMode == DataGridViewAutoSizeColumnMode.Fill)
                                {
                                    column.AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
                                }
                                if (column.FillWeight <= 0)
                                {
                                    column.FillWeight = 100; 
                                }
                            }
                            catch (Exception columnEx)
                            {
                                System.Diagnostics.Debug.WriteLine($"Error configurando columna {column.Name}: {columnEx.Message}");
                            }
                        }
                    }
                }
                
                if (qrPictureBox != null)
                {
                    qrPictureBox.BorderStyle = BorderStyle.None;
                }
                
                AddShadowToPanels();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error aplicando estilos personalizados: {ex.Message}");
            }
        }

        private void AddShadowToPanels()
        {
            var theme = ThemeService.CurrentTheme;
            
            if (this.Controls == null)
            {
                return; 
            }
            
            foreach (Control control in this.Controls)
            {
                if (control is Panel panel && panel.Name.Contains("Panel"))
                {
                    panel.Paint += (sender, e) =>
                    {
                        var rect = new Rectangle(0, 0, panel.Width - 1, panel.Height - 1);
                        using (var pen = new Pen(theme.BorderColor, 1))
                        {
                            e.Graphics.DrawRectangle(pen, rect);
                        }
                    };
                }
            }
        }

        private void ShowHistory()
        {
            try
            {
                var historyForm = new HistoryForm();
                historyForm.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error abriendo formulario de historial: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        private async void TestApiConnection()
        {
            try
            {
                var isConnected = await _apiTestService.TestApiConnection();
                
                if (isConnected)
                {
                    var summary = await _apiTestService.GetInventorySummary();
                    
                    if (summary != null && summary.Success)
                    {
                        var message = $"✅ API REST funcionando correctamente!\n\n" +
                                    $"📊 Resumen del inventario:\n" +
                                    $"• Total items: {summary.Data?.TotalItems}\n" +
                                    $"• Total cantidad: {summary.Data?.TotalQuantity}\n" +
                                    $"• Valor total: ${summary.Data?.TotalValue:F2}\n" +
                                    $"• Categorías: {summary.Data?.CategoriesCount}\n" +
                                    $"• Ubicaciones: {summary.Data?.LocationsCount}\n" +
                                    $"• Stock bajo: {summary.Data?.LowStockItems}\n\n" +
                                    $"🌐 API disponible en: http://localhost:5000\n" +
                                    $"📚 Documentación: http://localhost:5000/api-docs";
                        
                        MessageBox.Show(message, "API REST - Conexión Exitosa", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        MessageBox.Show("✅ API REST funcionando, pero hay un problema con los datos.", 
                            "API REST - Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
                else
                {
                    MessageBox.Show("❌ No se pudo conectar con la API REST.\n\n" +
                                  "Verifique que:\n" +
                                  "• La aplicación esté ejecutándose\n" +
                                  "• El puerto 5000 esté disponible\n" +
                                  "• No haya problemas de firewall", 
                                  "API REST - Error de Conexión", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error probando conexión API: {ex.Message}", 
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private bool ShowLogin()
        {
            var loginForm = new LoginForm();
            var result = loginForm.ShowDialog();
            
            if (result == DialogResult.OK)
            {
                return true;
            }
            
            return false;
        }

        private void ShowDashboard()
        {
            try
            {
                var dashboardForm = new DashboardForm(_reportService, _inventoryService, _authService);
                dashboardForm.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error abriendo dashboard: {ex.Message}", "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ShowUserManagement()
        {
            try
            {
                if (!_authService.HasPermission(UserRole.Manager))
                {
                    MessageBox.Show("⚠️ No tiene permisos para gestionar usuarios.\n\n" +
                                  "Solo los Managers y Administradores pueden acceder a esta función.",
                        "Permisos Insuficientes", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                var userManagementForm = new UserManagementForm(_authService);
                userManagementForm.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error abriendo gestión de usuarios: {ex.Message}", "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ChangeUser()
        {
            var result = MessageBox.Show("¿Desea cambiar de usuario?\n\n" +
                                       "Se cerrará la sesión actual y deberá iniciar sesión nuevamente.",
                "Cambiar Usuario", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                _authService.Logout();
                this.Close();
                
                Application.Restart();
            }
        }

        private void ShowAbout()
        {
            var userInfo = _authService.CurrentUser != null 
                ? $"Usuario: {_authService.CurrentUser.FullName}\nRol: {GetRoleDisplayName(_authService.CurrentUser.Role)}\n\n"
                : "";

            MessageBox.Show($"Inventory QR Manager v3.0\n\n{userInfo}" +
                          "Sistema de gestión de inventario con códigos QR\n" +
                          "📋 Incluye Historial de Movimientos\n" +
                          "🌐 API REST integrada\n" +
                          "👥 Sistema de usuarios\n" +
                          "📊 Dashboard ejecutivo\n" +
                          "Desarrollado con .NET 9 y Windows Forms", 
                "Acerca de", MessageBoxButtons.OK, MessageBoxIcon.Information);
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

        private void UpdateQRPanelInfo(InventoryItem item)
        {
            if (itemInfoLabel == null)
            {
                return; 
            }
            
            var info = $"Item: {item.Name}\n" +
                      $"SKU: {item.SKU}\n" +
                      $"Cantidad: {item.Quantity}\n" +
                      $"Precio: {item.Price:C2}\n" +
                      $"Categoría: {item.Category}\n" +
                      $"Ubicación: {item.Location}";
            
            itemInfoLabel.Text = info;
        }

        
        private DataGridView dataGridView;
        private PictureBox qrPictureBox;
        private Label itemInfoLabel;
    }
}

