using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using InventoryQRManager.Models;
using InventoryQRManager.Services;

namespace InventoryQRManager.Views
{
    public partial class SearchForm : Form
    {
        private readonly ReportService _reportService;
        private List<InventoryItem> _searchResults;

        public List<InventoryItem> SelectedItems { get; private set; }

        public SearchForm()
        {
            _reportService = new ReportService();
            _searchResults = new List<InventoryItem>();
            SelectedItems = new List<InventoryItem>();
            
            InitializeComponent();
            
            // Cargar categorías y ubicaciones después de que se hayan configurado los controles
            LoadCategoriesAndLocations();
            
            // Configurar evento Load para centrar contenido cuando el formulario esté listo
            this.Load += SearchForm_Load;
            
            // Cargar todos los items inicialmente
            LoadInitialData();
        }
        
        private void SearchForm_Load(object sender, EventArgs e)
        {
            // Centrar contenido cuando el formulario esté completamente cargado
            if (dataGridView != null && dataGridView.Rows.Count > 0)
            {
                CenterContent();
            }
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            
            // Configurar el formulario
            this.Text = "Búsqueda Avanzada de Inventario";
            this.Size = new Size(1000, 700);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            CreateControls();
            
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private void CreateControls()
        {
            // Panel de búsqueda
            var searchPanel = new Panel();
            searchPanel.Dock = DockStyle.Top;
            searchPanel.Height = 160;
            searchPanel.BackColor = Color.LightGray;
            searchPanel.Padding = new Padding(20);
            this.Controls.Add(searchPanel);

            // Título
            var titleLabel = new Label();
            titleLabel.Text = "Búsqueda Avanzada";
            titleLabel.Font = new Font("Arial", 14, FontStyle.Bold);
            titleLabel.Location = new Point(20, 15);
            titleLabel.Size = new Size(200, 25);
            searchPanel.Controls.Add(titleLabel);

            // Campo de búsqueda
            var searchLabel = new Label();
            searchLabel.Text = "Buscar:";
            searchLabel.Location = new Point(20, 55);
            searchLabel.Size = new Size(60, 20);
            searchLabel.Font = new Font("Arial", 9);
            searchPanel.Controls.Add(searchLabel);

            searchTextBox = new TextBox();
            searchTextBox.Location = new Point(90, 53);
            searchTextBox.Size = new Size(300, 22);
            searchTextBox.Font = new Font("Arial", 9);
            searchTextBox.TextChanged += SearchTextBox_TextChanged;
            searchPanel.Controls.Add(searchTextBox);

            // Botón buscar
            var searchButton = new Button();
            searchButton.Text = "Buscar";
            searchButton.Location = new Point(400, 51);
            searchButton.Size = new Size(80, 26);
            searchButton.Font = new Font("Arial", 9);
            searchButton.Click += SearchButton_Click;
            searchPanel.Controls.Add(searchButton);

            // Filtros
            var categoryLabel = new Label();
            categoryLabel.Text = "Categoría:";
            categoryLabel.Location = new Point(500, 55);
            categoryLabel.Size = new Size(70, 20);
            categoryLabel.Font = new Font("Arial", 9);
            searchPanel.Controls.Add(categoryLabel);

            categoryComboBox = new ComboBox();
            categoryComboBox.Location = new Point(575, 53);
            categoryComboBox.Size = new Size(150, 22);
            categoryComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            categoryComboBox.Font = new Font("Arial", 9);
            categoryComboBox.Items.Add("Todas");
            categoryComboBox.SelectedIndex = 0;
            categoryComboBox.SelectedIndexChanged += Filter_Changed;
            searchPanel.Controls.Add(categoryComboBox);

            var locationLabel = new Label();
            locationLabel.Text = "Ubicación:";
            locationLabel.Location = new Point(20, 95);
            locationLabel.Size = new Size(70, 20);
            locationLabel.Font = new Font("Arial", 9);
            searchPanel.Controls.Add(locationLabel);

            locationComboBox = new ComboBox();
            locationComboBox.Location = new Point(90, 93);
            locationComboBox.Size = new Size(150, 22);
            locationComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            locationComboBox.Font = new Font("Arial", 9);
            locationComboBox.Items.Add("Todas");
            locationComboBox.SelectedIndex = 0;
            locationComboBox.SelectedIndexChanged += Filter_Changed;
            searchPanel.Controls.Add(locationComboBox);

            // Stock bajo
            lowStockCheckBox = new CheckBox();
            lowStockCheckBox.Text = "Solo stock bajo";
            lowStockCheckBox.Location = new Point(250, 95);
            lowStockCheckBox.Size = new Size(120, 20);
            lowStockCheckBox.Font = new Font("Arial", 9);
            lowStockCheckBox.CheckedChanged += Filter_Changed;
            searchPanel.Controls.Add(lowStockCheckBox);

            // Botón limpiar
            var clearButton = new Button();
            clearButton.Text = "Limpiar";
            clearButton.Location = new Point(500, 91);
            clearButton.Size = new Size(80, 26);
            clearButton.Font = new Font("Arial", 9);
            clearButton.Click += ClearButton_Click;
            searchPanel.Controls.Add(clearButton);

            // Panel de resultados
            var resultsPanel = new Panel();
            resultsPanel.Dock = DockStyle.Fill;
            resultsPanel.BackColor = Color.White;
            resultsPanel.Padding = new Padding(10, 5, 10, 5);
            this.Controls.Add(resultsPanel);

            // DataGridView
            dataGridView = new DataGridView();
            dataGridView.Dock = DockStyle.Fill;
            dataGridView.Margin = new Padding(5);
            dataGridView.AllowUserToAddRows = false;
            dataGridView.AllowUserToDeleteRows = false;
            dataGridView.ReadOnly = true;
            dataGridView.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridView.MultiSelect = true;
            dataGridView.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;
            dataGridView.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            
            // Mejorar el centrado y apariencia
            dataGridView.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView.RowHeadersVisible = false;
            dataGridView.GridColor = Color.LightGray;
            dataGridView.BorderStyle = BorderStyle.None;
            dataGridView.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            dataGridView.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single;
            dataGridView.EnableHeadersVisualStyles = false;
            
            // Asegurar visibilidad
            dataGridView.Visible = true;
            dataGridView.BackgroundColor = Color.White;
            dataGridView.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(240, 240, 240);
            
            // Configurar para mostrar todas las filas correctamente
            dataGridView.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.None;
            dataGridView.RowTemplate.Height = 25;
            dataGridView.DefaultCellStyle.SelectionBackColor = Color.FromArgb(41, 128, 185);
            dataGridView.DefaultCellStyle.SelectionForeColor = Color.White;
            
            // Configurar columnas inmediatamente
            SetupDataGridViewColumns();
            
            resultsPanel.Controls.Add(dataGridView);

            // Panel de botones
            var buttonPanel = new Panel();
            buttonPanel.Dock = DockStyle.Bottom;
            buttonPanel.Height = 50;
            buttonPanel.BackColor = Color.LightGray;
            this.Controls.Add(buttonPanel);

            // Botones
            var selectButton = new Button();
            selectButton.Text = "Seleccionar";
            selectButton.Location = new Point(20, 12);
            selectButton.Size = new Size(100, 30);
            selectButton.Click += SelectButton_Click;
            buttonPanel.Controls.Add(selectButton);

            var exportButton = new Button();
            exportButton.Text = "Exportar";
            exportButton.Location = new Point(130, 12);
            exportButton.Size = new Size(100, 30);
            exportButton.Click += ExportButton_Click;
            buttonPanel.Controls.Add(exportButton);

            var closeButton = new Button();
            closeButton.Text = "Cerrar";
            closeButton.Location = new Point(890, 12);
            closeButton.Size = new Size(80, 30);
            closeButton.Click += CloseButton_Click;
            buttonPanel.Controls.Add(closeButton);

            // Label de resultados
            resultsLabel = new Label();
            resultsLabel.Text = "Resultados: 0 items";
            resultsLabel.Location = new Point(240, 18);
            resultsLabel.Size = new Size(200, 20);
            resultsLabel.Font = new Font("Arial", 10, FontStyle.Bold);
            buttonPanel.Controls.Add(resultsLabel);
        }

        private void SetupDataGridViewColumns()
        {
            dataGridView.Columns.Clear();
            
            dataGridView.Columns.Add("Id", "ID");
            dataGridView.Columns.Add("Name", "Nombre");
            dataGridView.Columns.Add("SKU", "SKU");
            dataGridView.Columns.Add("Quantity", "Cantidad");
            dataGridView.Columns.Add("Price", "Precio");
            dataGridView.Columns.Add("Category", "Categoría");
            dataGridView.Columns.Add("Location", "Ubicación");
            dataGridView.Columns.Add("CreatedDate", "Fecha Creación");
            
            // Ocultar columna ID
            dataGridView.Columns["Id"].Visible = false;
            
            // Formatear columnas
            dataGridView.Columns["Price"].DefaultCellStyle.Format = "C2";
            dataGridView.Columns["CreatedDate"].DefaultCellStyle.Format = "dd/MM/yyyy";
            
            // Configurar anchos fijos para evitar problemas con FillWeight
            dataGridView.Columns["Name"].Width = 200;
            dataGridView.Columns["SKU"].Width = 100;
            dataGridView.Columns["Quantity"].Width = 80;
            dataGridView.Columns["Price"].Width = 100;
            dataGridView.Columns["Category"].Width = 120;
            dataGridView.Columns["Location"].Width = 120;
            dataGridView.Columns["CreatedDate"].Width = 100;
        }

        private void LoadCategoriesAndLocations()
        {
            if (categoryComboBox == null || locationComboBox == null)
            {
                return; // No cargar si los controles no están configurados
            }
            
            var inventoryService = new InventoryService();
            
            // Cargar categorías
            categoryComboBox.Items.Clear();
            categoryComboBox.Items.Add("Todas");
            var categories = inventoryService.GetCategories();
            categoryComboBox.Items.AddRange(categories.ToArray());
            categoryComboBox.SelectedIndex = 0;

            // Cargar ubicaciones
            locationComboBox.Items.Clear();
            locationComboBox.Items.Add("Todas");
            var locations = inventoryService.GetLocations();
            locationComboBox.Items.AddRange(locations.ToArray());
            locationComboBox.SelectedIndex = 0;
        }

        private void LoadInitialData()
        {
            try
            {
                var inventoryService = new InventoryService();
                _searchResults = inventoryService.GetAllItems();
                
                // Refrescar la tabla con los datos iniciales
                if (dataGridView != null && dataGridView.Columns.Count > 0)
                {
                    RefreshDataGridView();
                    UpdateResultsLabel();
                    
                    // Asegurar que la tabla muestre desde el inicio
                    EnsureTableVisible();
                    
                    // Centrar el contenido después de un breve delay (solo si el control está listo)
                    if (this.IsHandleCreated)
                    {
                        this.BeginInvoke(new Action(() => CenterContent()));
                    }
                    else
                    {
                        // Si el handle no está creado, centrar directamente
                        CenterContent();
                    }
                    
                    // Forzar actualización completa del formulario
                    this.Invalidate();
                    this.Update();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error cargando datos iniciales: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        
        private void EnsureTableVisible()
        {
            try
            {
                if (dataGridView != null && dataGridView.Rows.Count > 0)
                {
                    // Asegurar que la primera fila sea visible
                    dataGridView.FirstDisplayedScrollingRowIndex = 0;
                    
                    // Limpiar cualquier selección previa
                    dataGridView.ClearSelection();
                    
                    // Seleccionar la primera fila si es posible
                    if (dataGridView.Rows[0].Cells.Count > 0)
                    {
                        dataGridView.Rows[0].Selected = true;
                        dataGridView.CurrentCell = dataGridView.Rows[0].Cells[0];
                    }
                    
                    // Forzar actualización del layout y scroll
                    dataGridView.PerformLayout();
                    dataGridView.Refresh();
                    dataGridView.Update();
                    
                    // Asegurar que el scroll esté en la posición correcta
                    dataGridView.FirstDisplayedScrollingRowIndex = 0;
                    
                    // Forzar redibujado del panel contenedor
                    if (dataGridView.Parent != null)
                    {
                        dataGridView.Parent.Invalidate();
                        dataGridView.Parent.Update();
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error asegurando visibilidad de tabla: {ex.Message}");
            }
        }
        
        private void CenterContent()
        {
            try
            {
                // Verificar que el DataGridView esté completamente inicializado
                if (dataGridView != null && dataGridView.IsHandleCreated && dataGridView.Rows.Count > 0)
                {
                    // Asegurar que la primera fila sea visible
                    dataGridView.FirstDisplayedScrollingRowIndex = 0;
                    
                    // Limpiar selección
                    dataGridView.ClearSelection();
                    
                    // Seleccionar primera fila
                    if (dataGridView.Rows[0].Cells.Count > 0)
                    {
                        dataGridView.Rows[0].Selected = true;
                        dataGridView.CurrentCell = dataGridView.Rows[0].Cells[0];
                    }
                    
                    // Forzar actualización visual
                    dataGridView.PerformLayout();
                    dataGridView.Refresh();
                    
                    // Asegurar que el panel contenedor también se actualice
                    if (dataGridView.Parent != null && dataGridView.Parent.IsHandleCreated)
                    {
                        dataGridView.Parent.PerformLayout();
                        dataGridView.Parent.Refresh();
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error centrando contenido: {ex.Message}");
            }
        }

        private void SearchTextBox_TextChanged(object sender, EventArgs e)
        {
            // Búsqueda en tiempo real
            if (dataGridView != null && dataGridView.Columns.Count > 0)
            {
                PerformSearch();
            }
        }

        private void Filter_Changed(object sender, EventArgs e)
        {
            try
            {
                if (dataGridView != null && dataGridView.Columns.Count > 0)
                {
                    // Aplicar todos los filtros (incluyendo búsqueda de texto)
                    ApplyFilters();
                    
                    // Refrescar la tabla
                    RefreshDataGridView();
                    UpdateResultsLabel();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error aplicando filtros: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void SearchButton_Click(object sender, EventArgs e)
        {
            if (dataGridView != null && dataGridView.Columns.Count > 0)
            {
                PerformSearch();
            }
        }

        private void ClearButton_Click(object sender, EventArgs e)
        {
            if (searchTextBox == null || categoryComboBox == null || locationComboBox == null || lowStockCheckBox == null)
            {
                return; // No limpiar si los controles no están configurados
            }
            
            searchTextBox.Clear();
            categoryComboBox.SelectedIndex = 0;
            locationComboBox.SelectedIndex = 0;
            lowStockCheckBox.Checked = false;
            PerformSearch();
        }

        private void SelectButton_Click(object sender, EventArgs e)
        {
            try
            {
                if (dataGridView == null || dataGridView.Columns.Count == 0)
                {
                    MessageBox.Show("El DataGridView no está configurado correctamente.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                
                SelectedItems.Clear();
                
                foreach (DataGridViewRow row in dataGridView.SelectedRows)
                {
                    if (row.Index < _searchResults.Count)
                    {
                        SelectedItems.Add(_searchResults[row.Index]);
                    }
                }

                if (SelectedItems.Count > 0)
                {
                    MessageBox.Show($"{SelectedItems.Count} item(s) seleccionado(s) correctamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show("Por favor seleccione al menos un item.", "Información", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al seleccionar items: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ExportButton_Click(object sender, EventArgs e)
        {
            if (dataGridView == null || dataGridView.Columns.Count == 0)
            {
                MessageBox.Show("El DataGridView no está configurado correctamente.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            
            if (_searchResults.Count == 0)
            {
                MessageBox.Show("No hay resultados para exportar.", "Información", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var saveDialog = new SaveFileDialog();
            saveDialog.Filter = "Archivos CSV (*.csv)|*.csv";
            saveDialog.FileName = $"busqueda_inventario_{DateTime.Now:yyyyMMdd_HHmmss}.csv";

            if (saveDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    ExportToCSV(_searchResults, saveDialog.FileName);
                    MessageBox.Show($"Resultados exportados exitosamente a {saveDialog.FileName}", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error al exportar: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void CloseButton_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void PerformSearch()
        {
            try
            {
                // Aplicar todos los filtros (incluyendo búsqueda de texto)
                ApplyFilters();

                // Solo refrescar si el DataGridView está configurado
                if (dataGridView != null && dataGridView.Columns.Count > 0)
                {
                    RefreshDataGridView();
                    UpdateResultsLabel();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error en la búsqueda: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ApplyFilters()
        {
            try
            {
                if (categoryComboBox == null || locationComboBox == null || lowStockCheckBox == null)
                {
                    return; // No aplicar filtros si los controles no están configurados
                }
                
                // Obtener todos los items para aplicar filtros desde cero
                var inventoryService = new InventoryService();
                var allItems = inventoryService.GetAllItems();
                
                // Aplicar búsqueda de texto si existe
                var searchTerm = searchTextBox?.Text?.Trim() ?? string.Empty;
                if (!string.IsNullOrEmpty(searchTerm))
                {
                    allItems = allItems.Where(item => 
                        item.Name.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                        item.SKU.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                        item.Description.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)
                    ).ToList();
                }
                
                // Aplicar filtros de categoría
                if (categoryComboBox.SelectedIndex > 0)
                {
                    var selectedCategory = categoryComboBox.SelectedItem.ToString();
                    allItems = allItems.Where(item => item.Category == selectedCategory).ToList();
                }

                // Aplicar filtros de ubicación
                if (locationComboBox.SelectedIndex > 0)
                {
                    var selectedLocation = locationComboBox.SelectedItem.ToString();
                    allItems = allItems.Where(item => item.Location == selectedLocation).ToList();
                }

                // Aplicar filtro de stock bajo
                if (lowStockCheckBox.Checked)
                {
                    var settingsService = new SettingsService();
                    var threshold = settingsService.GetSetting<int>("LowStockThreshold");
                    allItems = allItems.Where(item => item.Quantity <= threshold).ToList();
                }
                
                // Actualizar los resultados finales
                _searchResults = allItems;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error aplicando filtros: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void RefreshDataGridView()
        {
            try
            {
                if (dataGridView == null || dataGridView.Columns.Count == 0)
                {
                    return; // No refrescar si el DataGridView no está configurado
                }
                
                // Limpiar filas existentes
                dataGridView.Rows.Clear();
                
                // Agregar filas con los resultados
                foreach (var item in _searchResults)
                {
                    if (item != null)
                    {
                        dataGridView.Rows.Add(
                            item.Id,
                            item.Name ?? string.Empty,
                            item.SKU ?? string.Empty,
                            item.Quantity,
                            item.Price,
                            item.Category ?? string.Empty,
                            item.Location ?? string.Empty,
                            item.CreatedDate
                        );
                    }
                }
                
                // Forzar actualización visual y layout
                dataGridView.Refresh();
                dataGridView.Update();
                dataGridView.Invalidate();
                
                // Asegurar que la primera fila sea visible si hay resultados
                if (_searchResults.Count > 0 && dataGridView.Rows.Count > 0)
                {
                    try
                    {
                        // Forzar scroll a la primera fila
                        dataGridView.FirstDisplayedScrollingRowIndex = 0;
                        
                        // Limpiar selección actual
                        dataGridView.ClearSelection();
                        
                        // Solo establecer la celda actual si hay filas y columnas válidas
                        if (dataGridView.Rows[0].Cells.Count > 0)
                        {
                            dataGridView.CurrentCell = dataGridView.Rows[0].Cells[0];
                            dataGridView.Rows[0].Selected = true;
                        }
                        
                        // Forzar actualización del scroll
                        dataGridView.PerformLayout();
                    }
                    catch (Exception ex)
                    {
                        // Si hay error al establecer la celda actual, solo logear y continuar
                        System.Diagnostics.Debug.WriteLine($"Error estableciendo celda actual: {ex.Message}");
                    }
                }
                
                // Forzar redibujado del panel contenedor
                if (dataGridView.Parent != null)
                {
                    dataGridView.Parent.Invalidate();
                    dataGridView.Parent.Update();
                }
                
                // Centrar el contenido después de refrescar (solo si el control está listo)
                if (this.IsHandleCreated)
                {
                    this.BeginInvoke(new Action(() => CenterContent()));
                }
                else
                {
                    // Si el handle no está creado, centrar directamente
                    CenterContent();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error refrescando la tabla: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void UpdateResultsLabel()
        {
            if (resultsLabel == null)
            {
                return; // No actualizar si el label no está configurado
            }
            
            resultsLabel.Text = $"Resultados: {_searchResults.Count} items";
        }

        private void ExportToCSV(List<InventoryItem> items, string filePath)
        {
            if (items == null || items.Count == 0)
            {
                return; // No exportar si no hay items
            }
            
            using var writer = new System.IO.StreamWriter(filePath);
            
            // Escribir encabezados
            writer.WriteLine("Nombre,Descripción,SKU,Código QR,Cantidad,Precio,Categoría,Ubicación,Fecha Creación");

            // Escribir datos
            foreach (var item in items)
            {
                writer.WriteLine($"{item.Name},{item.Description},{item.SKU},{item.QRCode},{item.Quantity},{item.Price},{item.Category},{item.Location},{item.CreatedDate:yyyy-MM-dd HH:mm:ss}");
            }
        }

        // Controles
        private TextBox searchTextBox;
        private ComboBox categoryComboBox;
        private ComboBox locationComboBox;
        private CheckBox lowStockCheckBox;
        private DataGridView dataGridView;
        private Label resultsLabel;
    }
}
