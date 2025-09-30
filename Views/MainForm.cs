using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using InventoryQRManager.Models;
using InventoryQRManager.Services;

namespace InventoryQRManager.Views
{
    public partial class MainForm : Form
    {
        private readonly InventoryService _inventoryService;
        private readonly QRCodeService _qrCodeService;
        private readonly ReportService _reportService;
        private readonly BackupService _backupService;
        private readonly SettingsService _settingsService;
        private List<InventoryItem> _inventoryItems;

        public MainForm()
        {
            _inventoryService = new InventoryService();
            _qrCodeService = new QRCodeService();
            _reportService = new ReportService();
            _backupService = new BackupService();
            _settingsService = new SettingsService();
            _inventoryItems = new List<InventoryItem>();
            
            InitializeComponent();
            
            // Cargar datos después de que se hayan configurado todos los controles
            LoadInventoryData();
            LoadSettings();
            
            // Aplicar tema profesional DESPUÉS de cargar los datos
            ApplyProfessionalTheme();
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            
            // Configurar el formulario principal con mejor diseño
            this.Text = "Inventory QR Manager - Sistema de Gestión de Inventario";
            this.Size = new Size(1400, 900);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.MinimumSize = new Size(1000, 700);
            this.BackColor = Color.FromArgb(236, 240, 241);
            this.Font = new Font("Segoe UI", 9F, FontStyle.Regular);
            this.WindowState = FormWindowState.Maximized;

            // Crear el menú principal
            CreateMenuStrip();
            
            // Crear la barra de herramientas
            CreateToolStrip();
            
            // Crear el DataGridView
            CreateDataGridView();
            
            // Crear el panel de información
            CreateInfoPanel();
            
            // Crear el panel de QR
            CreateQRPanel();

            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private void CreateMenuStrip()
        {
            var menuStrip = new MenuStrip();
            
            // Menú Archivo
            var fileMenu = new ToolStripMenuItem("&Archivo");
            fileMenu.DropDownItems.Add("&Importar", null, (s, e) => ImportData());
            fileMenu.DropDownItems.Add("&Exportar", null, (s, e) => ExportData());
            fileMenu.DropDownItems.Add(new ToolStripSeparator());
            fileMenu.DropDownItems.Add("&Salir", null, (s, e) => this.Close());
            
            // Menú Inventario (sin "Nuevo Item" ni funciones QR duplicadas)
            var inventoryMenu = new ToolStripMenuItem("&Inventario");
            inventoryMenu.DropDownItems.Add("&Ver Todos", null, (s, e) => LoadInventoryData());
            inventoryMenu.DropDownItems.Add("&Buscar Avanzada", null, (s, e) => ShowAdvancedSearch());
            inventoryMenu.DropDownItems.Add("&Reportes y Estadísticas", null, (s, e) => ShowReports());
            inventoryMenu.DropDownItems.Add(new ToolStripSeparator());
            inventoryMenu.DropDownItems.Add("&Stock Bajo", null, (s, e) => ShowLowStockItems());
            
            // Menú Códigos QR (mantener solo las funciones principales)
            var qrMenu = new ToolStripMenuItem("&Códigos QR");
            qrMenu.DropDownItems.Add("&Escanear QR", null, (s, e) => ScanQRCode());
            qrMenu.DropDownItems.Add("&Imprimir QR", null, (s, e) => PrintQRCode());
            
            // Menú Herramientas
            var toolsMenu = new ToolStripMenuItem("&Herramientas");
            toolsMenu.DropDownItems.Add("&Configuración", null, (s, e) => ShowSettings());
            toolsMenu.DropDownItems.Add("&Backup", null, (s, e) => ShowBackupDialog());
            
            // Menú Ayuda
            var helpMenu = new ToolStripMenuItem("&Ayuda");
            helpMenu.DropDownItems.Add("&Acerca de", null, (s, e) => ShowAbout());
            
            menuStrip.Items.AddRange(new ToolStripItem[] { fileMenu, inventoryMenu, qrMenu, toolsMenu, helpMenu });
            this.MainMenuStrip = menuStrip;
            this.Controls.Add(menuStrip);
        }

        private void CreateToolStrip()
        {
            var toolStrip = new ToolStrip();
            toolStrip.BackColor = Color.FromArgb(52, 73, 94); // Color unificado para el fondo
            toolStrip.ForeColor = Color.White;
            toolStrip.Font = new Font("Segoe UI", 9F, FontStyle.Regular);
            toolStrip.Padding = new Padding(10, 5, 10, 5);
            
            // Color unificado para todos los botones - más formal y profesional
            var primaryButtonColor = Color.FromArgb(41, 128, 185); // Azul profesional unificado
            
            // Botones principales con diseño unificado y más formal
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
            
            
            var scanButton = new ToolStripButton("📱 Escanear", null, (s, e) => ScanQRCode());
            scanButton.BackColor = primaryButtonColor;
            scanButton.ForeColor = Color.White;
            scanButton.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            
            var separator2 = new ToolStripSeparator();
            separator2.BackColor = Color.FromArgb(149, 165, 166);
            
            var refreshButton = new ToolStripButton("🔄 Actualizar", null, (s, e) => LoadInventoryData());
            refreshButton.BackColor = primaryButtonColor;
            refreshButton.ForeColor = Color.White;
            refreshButton.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            
            toolStrip.Items.AddRange(new ToolStripItem[] { 
                newButton, editButton, deleteButton, separator1, 
                scanButton, separator2, refreshButton 
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
            
            // Configurar propiedades básicas del DataGridView
            dataGridView.RowHeadersVisible = false;
            dataGridView.EnableHeadersVisualStyles = false;
            
            // CRÍTICO: Configurar encabezados ANTES de agregar columnas
            dataGridView.ColumnHeadersVisible = true;
            dataGridView.ColumnHeadersHeight = 50;
            
            // Configurar estilo de encabezados más moderno
            dataGridView.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(41, 128, 185);
            dataGridView.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dataGridView.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            dataGridView.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView.ColumnHeadersDefaultCellStyle.Padding = new Padding(8, 8, 8, 8);
            
            // Mejorar el centrado y apariencia con mejor espaciado
            dataGridView.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridView.DefaultCellStyle.Font = new Font("Segoe UI", 10F, FontStyle.Regular);
            dataGridView.DefaultCellStyle.Padding = new Padding(8, 4, 8, 4);
            dataGridView.GridColor = Color.FromArgb(220, 220, 220);
            dataGridView.BorderStyle = BorderStyle.None;
            dataGridView.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            dataGridView.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
            
            // Hacer los items más grandes y con mejor espaciado
            dataGridView.RowTemplate.Height = 50;
            
            // Agregar margen superior para separar del menú
            dataGridView.Margin = new Padding(10, 10, 10, 0);
            
            // Configurar columnas inmediatamente
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
            
            // Crear un panel principal con sombra
            var mainContainer = new Panel();
            mainContainer.Dock = DockStyle.Fill;
            mainContainer.BackColor = Color.White;
            mainContainer.Padding = new Padding(15);
            
            // Título del panel QR
            var qrLabel = new Label();
            qrLabel.Text = "🔍 Código QR";
            qrLabel.Dock = DockStyle.Top;
            qrLabel.TextAlign = ContentAlignment.MiddleCenter;
            qrLabel.Font = new Font("Segoe UI", 14, FontStyle.Bold);
            qrLabel.Height = 45;
            qrLabel.BackColor = Color.FromArgb(41, 128, 185);
            qrLabel.ForeColor = Color.White;
            qrLabel.Padding = new Padding(0, 10, 0, 10);
            
            // Panel para el código QR con mejor diseño
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
            
            // Panel de información del item mejorado
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
            
            // Agregar controles en orden
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
                // Asegurar que el DataGridView esté en modo None antes de configurar columnas
                dataGridView.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;
                
                dataGridView.Columns.Clear();
                
                // Agregar columnas con títulos descriptivos y claros
                dataGridView.Columns.Add("Id", "ID");
                dataGridView.Columns.Add("Name", "Nombre");
                dataGridView.Columns.Add("SKU", "Código SKU");
                dataGridView.Columns.Add("Quantity", "Cantidad");
                dataGridView.Columns.Add("Price", "Precio");
                dataGridView.Columns.Add("Category", "Categoría");
                dataGridView.Columns.Add("Location", "Ubicación");
                dataGridView.Columns.Add("CreatedDate", "Fecha");
                
                // Configurar cada columna individualmente para evitar errores de FillWeight
                foreach (DataGridViewColumn column in dataGridView.Columns)
                {
                    try
                    {
                        // Asegurar que cada columna tenga configuración segura
                        column.AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
                        column.FillWeight = 100; // Valor seguro por defecto
                        
                        // Configurar anchos específicos
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
                        // Ignorar errores individuales de columnas
                        System.Diagnostics.Debug.WriteLine($"Error configurando columna {column.Name}: {columnEx.Message}");
                    }
                }
                
                // CRÍTICO: Asegurar que los encabezados sean visibles DESPUÉS de configurar las columnas
                dataGridView.ColumnHeadersVisible = true;
                dataGridView.EnableHeadersVisualStyles = false;
                dataGridView.ColumnHeadersHeight = 50;
                
                // Reconfigurar estilos de encabezados para asegurar visibilidad
                dataGridView.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(41, 128, 185);
                dataGridView.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
                dataGridView.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
                dataGridView.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                
                // Forzar actualización visual
                dataGridView.Invalidate();
                dataGridView.Refresh();
                
                // Asegurar que el modo permanezca en None
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
                
                // Solo refrescar si el DataGridView está configurado
                if (dataGridView != null && dataGridView.Columns.Count > 0)
                {
                    // Asegurar que el DataGridView esté correctamente configurado
                    EnsureDataGridViewIsValid();
                    RefreshDataGridView();
                    UpdateInfoLabel();
                    
                    // Limpiar selección y panel QR automáticamente
                    ClearSelectionAndQRPanel();
                }
                else if (dataGridView != null && dataGridView.Columns.Count == 0)
                {
                    // Si el DataGridView existe pero no tiene columnas, configurarlas
                    SetupDataGridViewColumns();
                    RefreshDataGridView();
                    UpdateInfoLabel();
                    
                    // Limpiar selección y panel QR automáticamente
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
                // Limpiar selección del DataGridView
                if (dataGridView != null)
                {
                    dataGridView.ClearSelection();
                }
                
                // Limpiar panel QR
                if (qrPictureBox != null && qrPictureBox.Image != null)
                {
                    qrPictureBox.Image.Dispose();
                    qrPictureBox.Image = null;
                }
                
                // Limpiar información del item
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
            
            // Asegurar que el modo de redimensionamiento esté configurado correctamente
            if (dataGridView.AutoSizeColumnsMode != DataGridViewAutoSizeColumnsMode.None)
            {
                dataGridView.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;
            }
        }

        private void RefreshDataGridView()
        {
            if (dataGridView == null || dataGridView.Columns.Count == 0)
            {
                return; // No refrescar si el DataGridView no está configurado
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
                    return; // No actualizar si el DataGridView no está configurado
                }
                
                var selectedCount = dataGridView.SelectedRows.Count;
                var totalCount = _inventoryItems.Count;
                var activeItems = _inventoryItems.Count(item => item.IsActive);
                var totalValue = _inventoryItems.Where(item => item.IsActive).Sum(item => item.Price * item.Quantity);
                var lowStockItems = _inventoryItems.Count(item => item.IsActive && item.Quantity <= 5);
                
                // Actualizar el label de información
                var infoPanel = this.Controls.OfType<Panel>().FirstOrDefault(p => p.Dock == DockStyle.Bottom);
                if (infoPanel != null)
                {
                    var infoLabel = infoPanel.Controls.OfType<Label>().FirstOrDefault();
                    if (infoLabel != null)
                    {
                        infoLabel.Text = $"Total: {totalCount} items | Valor: ${totalValue:F2} | Stock Bajo: {lowStockItems} | Seleccionados: {selectedCount}";
                        
                        // Siempre usar color negro
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
                return; // No procesar si el DataGridView no está configurado
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
                return; // No procesar si el DataGridView no está configurado
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
                        // Generar código QR si no existe
                        if (string.IsNullOrEmpty(item.QRCode))
                        {
                            item.QRCode = $"ITEM_{item.Id}_{item.SKU}";
                            _inventoryService.UpdateItem(item);
                        }
                        
                        // Generar y mostrar el código QR automáticamente
                        var qrCodeImage = _qrCodeService.GenerateQRCode(item.QRCode);
                        if (qrPictureBox != null)
                        {
                            // Limpiar imagen anterior si existe
                            if (qrPictureBox.Image != null)
                            {
                                qrPictureBox.Image.Dispose();
                            }
                            
                            qrPictureBox.Image = qrCodeImage;
                            qrPictureBox.SizeMode = PictureBoxSizeMode.Zoom;
                            qrPictureBox.Refresh(); // Forzar actualización visual
                        }
                        
                        // Actualizar la información del item
                        UpdateQRPanelInfo(item);
                    }
                }
                catch (Exception ex)
                {
                    // Mostrar error en el panel de información
                    if (itemInfoLabel != null)
                    {
                        itemInfoLabel.Text = $"Error generando QR: {ex.Message}";
                    }
                    System.Diagnostics.Debug.WriteLine($"Error generando QR automático: {ex.Message}");
                }
            }
            else
            {
                // Limpiar el panel QR cuando no hay selección
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
                    // Actualizar datos cuando se cierre el formulario
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
                            // Actualizar datos cuando se cierre el formulario
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
                    if (_inventoryService.DeleteItem(itemId))
                    {
                        MessageBox.Show("Item eliminado correctamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
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
                        // Verificar si el item tiene código QR
                        if (string.IsNullOrEmpty(item.QRCode))
                        {
                            MessageBox.Show("Este item no tiene un código QR. Los códigos QR se generan automáticamente al crear o editar items.", "Información", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            return;
                        }
                        
                        // Generar la imagen del código QR existente
                        var qrCodeImage = _qrCodeService.GenerateQRCode(item.QRCode);
                        if (qrPictureBox != null)
                        {
                            // Limpiar imagen anterior si existe
                            if (qrPictureBox.Image != null)
                            {
                                qrPictureBox.Image.Dispose();
                            }
                            
                            qrPictureBox.Image = qrCodeImage;
                            qrPictureBox.Refresh(); // Forzar actualización visual
                        }
                        
                        // Actualizar el label del panel QR con información del item
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

                // Obtener el item seleccionado para el nombre del archivo
                InventoryItem? selectedItem = null;
                if (dataGridView?.SelectedRows.Count > 0)
                {
                    var selectedRow = dataGridView.SelectedRows[0];
                    var itemId = Convert.ToInt32(selectedRow.Cells["Id"].Value);
                    selectedItem = _inventoryService.GetItemById(itemId);
                }

                // Configurar el diálogo de guardar archivo
                using (var saveFileDialog = new SaveFileDialog())
                {
                    saveFileDialog.Filter = "Imagen PNG (*.png)|*.png|Imagen JPEG (*.jpg)|*.jpg|Imagen BMP (*.bmp)|*.bmp|Todos los archivos (*.*)|*.*";
                    saveFileDialog.FilterIndex = 1;
                    saveFileDialog.DefaultExt = "png";
                    
                    // Sugerir nombre de archivo basado en el item seleccionado
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
                        // Guardar la imagen QR
                        var imageFormat = ImageFormat.Png;
                        switch (saveFileDialog.FilterIndex)
                        {
                            case 1: imageFormat = ImageFormat.Png; break;
                            case 2: imageFormat = ImageFormat.Jpeg; break;
                            case 3: imageFormat = ImageFormat.Bmp; break;
                        }

                        // Crear una copia de la imagen para evitar problemas de acceso
                        using (var bitmap = new Bitmap(qrPictureBox.Image))
                        {
                            bitmap.Save(saveFileDialog.FileName, imageFormat);
                        }

                        // Mostrar mensaje de éxito
                        var message = $"Código QR guardado exitosamente en:\n{saveFileDialog.FileName}";
                        if (selectedItem != null)
                        {
                            message += $"\n\nItem: {selectedItem.Name}\nSKU: {selectedItem.SKU}";
                        }
                        
                        MessageBox.Show(message, "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);

                        // Opcional: Abrir la carpeta donde se guardó el archivo
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
                            // Importar desde JSON (backup)
                            _backupService.RestoreBackup(openDialog.FileName);
                            MessageBox.Show("Importación desde JSON completada exitosamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        else if (fileExtension == ".csv")
                        {
                            // Importar desde CSV
                            _backupService.ImportFromCSV(openDialog.FileName);
                            MessageBox.Show("Importación desde CSV completada exitosamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        else
                        {
                            MessageBox.Show("Formato de archivo no soportado. Use archivos JSON o CSV.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }
                        
                        // Actualizar la interfaz después de la importación
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
                        // Exportar como JSON (backup completo)
                        _backupService.CreateBackup(saveDialog.FileName);
                        MessageBox.Show("Exportación a JSON completada exitosamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else if (fileExtension == ".csv")
                    {
                        // Exportar como CSV
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
                    // Actualizar datos cuando se cierre el formulario
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
                
                // Aplicar configuración de la empresa
                if (!string.IsNullOrEmpty(settings.CompanyName))
                {
                    this.Text = $"Inventory QR Manager - {settings.CompanyName}";
                }
                
                // Actualizar información del panel solo si el DataGridView está configurado
                if (dataGridView != null && dataGridView.Columns.Count > 0)
                {
                    UpdateInfoLabel();
                }
            }
            catch (Exception ex)
            {
                // Error cargando configuración, usar valores por defecto
            }
        }

        private void ApplyProfessionalTheme()
        {
            // Aplicar tema profesional a todo el formulario
            ThemeService.ApplyTheme(this);
            
            // Aplicar estilos específicos adicionales
            ApplyCustomStyling();
        }

        private void ApplyCustomStyling()
        {
            try
            {
                var theme = ThemeService.CurrentTheme;
                
                // Personalizar el DataGridView con estilos adicionales más modernos
                if (dataGridView != null)
                {
                    // Asegurar que el modo esté en None ANTES de cualquier otra operación para evitar errores de FillWeight
                    dataGridView.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;
                    
                    // Configuración de espaciado (aplicar siempre)
                    dataGridView.RowHeadersWidth = 0;
                    dataGridView.ColumnHeadersHeight = 50;
                    dataGridView.RowTemplate.Height = 50;
                    
                    // Mejorar la apariencia general
                    dataGridView.BackgroundColor = Color.White;
                    dataGridView.GridColor = Color.FromArgb(220, 220, 220);
                    dataGridView.BorderStyle = BorderStyle.None;
                    dataGridView.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
                    dataGridView.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
                    
                    // Solo aplicar estilos si hay columnas configuradas
                    if (dataGridView.Columns.Count > 0)
                    {
                        // Estilo para filas alternadas
                        dataGridView.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(250, 250, 250);
                        dataGridView.AlternatingRowsDefaultCellStyle.ForeColor = Color.FromArgb(44, 62, 80);
                        
                        // Estilo para filas normales
                        dataGridView.DefaultCellStyle.BackColor = Color.White;
                        dataGridView.DefaultCellStyle.ForeColor = Color.FromArgb(44, 62, 80);
                        
                        // Estilo para filas seleccionadas
                        dataGridView.RowsDefaultCellStyle.SelectionBackColor = Color.FromArgb(52, 152, 219);
                        dataGridView.RowsDefaultCellStyle.SelectionForeColor = Color.White;
                        
                        // Agregar padding a las celdas
                        dataGridView.DefaultCellStyle.Padding = new Padding(8, 8, 8, 8);
                        
                        // Asegurar que no haya problemas con FillWeight - verificar cada columna individualmente
                        foreach (DataGridViewColumn column in dataGridView.Columns)
                        {
                            try
                            {
                                if (column.AutoSizeMode == DataGridViewAutoSizeColumnMode.Fill)
                                {
                                    column.AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
                                }
                                // Asegurar que FillWeight no sea 0
                                if (column.FillWeight <= 0)
                                {
                                    column.FillWeight = 100; // Valor por defecto seguro
                                }
                            }
                            catch (Exception columnEx)
                            {
                                // Ignorar errores individuales de columnas
                                System.Diagnostics.Debug.WriteLine($"Error configurando columna {column.Name}: {columnEx.Message}");
                            }
                        }
                    }
                }
                
                // Personalizar el PictureBox del QR
                if (qrPictureBox != null)
                {
                    qrPictureBox.BorderStyle = BorderStyle.None;
                }
                
                // Agregar sombras sutiles a los paneles principales
                AddShadowToPanels();
            }
            catch (Exception ex)
            {
                // Solo mostrar error en debug, no al usuario para evitar interrupciones
                System.Diagnostics.Debug.WriteLine($"Error aplicando estilos personalizados: {ex.Message}");
            }
        }

        private void AddShadowToPanels()
        {
            var theme = ThemeService.CurrentTheme;
            
            if (this.Controls == null)
            {
                return; // No agregar sombras si los controles no están configurados
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

        private void ShowAbout()
        {
            MessageBox.Show("Inventory QR Manager v1.0\n\nSistema de gestión de inventario con códigos QR\nDesarrollado con .NET 6 y Windows Forms", 
                "Acerca de", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void UpdateQRPanelInfo(InventoryItem item)
        {
            if (itemInfoLabel == null)
            {
                return; // No actualizar si el label no está configurado
            }
            
            var info = $"Item: {item.Name}\n" +
                      $"SKU: {item.SKU}\n" +
                      $"Cantidad: {item.Quantity}\n" +
                      $"Precio: {item.Price:C2}\n" +
                      $"Categoría: {item.Category}\n" +
                      $"Ubicación: {item.Location}";
            
            itemInfoLabel.Text = info;
        }

        // Controles
        private DataGridView dataGridView;
        private PictureBox qrPictureBox;
        private Label itemInfoLabel;
    }
}

