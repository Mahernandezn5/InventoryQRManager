using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using InventoryQRManager.Services;

namespace InventoryQRManager.Views
{
    public partial class ReportsForm : Form
    {
        private readonly ReportService _reportService;

        public ReportsForm()
        {
            _reportService = new ReportService();
            InitializeComponent();
            this.Load += (s, e) => LoadReports();
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            
            this.Text = "Reportes y Estadísticas";
            this.Size = new Size(1200, 800);
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
            var topPanel = new Panel();
            topPanel.Dock = DockStyle.Top;
            topPanel.Height = 60;
            topPanel.BackColor = Color.LightGray;
            this.Controls.Add(topPanel);

            var titleLabel = new Label();
            titleLabel.Text = "Reportes y Estadísticas";
            titleLabel.Font = new Font("Arial", 16, FontStyle.Bold);
            titleLabel.Location = new Point(20, 15);
            titleLabel.Size = new Size(300, 30);
            topPanel.Controls.Add(titleLabel);

            var summaryButton = new Button();
            summaryButton.Text = "Resumen General";
            summaryButton.Location = new Point(400, 15);
            summaryButton.Size = new Size(120, 30);
            summaryButton.Click += SummaryButton_Click;
            topPanel.Controls.Add(summaryButton);

            var categoriesButton = new Button();
            categoriesButton.Text = "Por Categorías";
            categoriesButton.Location = new Point(530, 15);
            categoriesButton.Size = new Size(120, 30);
            categoriesButton.Click += CategoriesButton_Click;
            topPanel.Controls.Add(categoriesButton);

            var locationsButton = new Button();
            locationsButton.Text = "Por Ubicaciones";
            locationsButton.Location = new Point(660, 15);
            locationsButton.Size = new Size(120, 30);
            locationsButton.Click += LocationsButton_Click;
            topPanel.Controls.Add(locationsButton);

            var lowStockButton = new Button();
            lowStockButton.Text = "Stock Bajo";
            lowStockButton.Location = new Point(790, 15);
            lowStockButton.Size = new Size(120, 30);
            lowStockButton.Click += LowStockButton_Click;
            topPanel.Controls.Add(lowStockButton);

            contentPanel = new Panel();
            contentPanel.Dock = DockStyle.Fill;
            contentPanel.BackColor = Color.White;
            this.Controls.Add(contentPanel);

            var bottomPanel = new Panel();
            bottomPanel.Dock = DockStyle.Bottom;
            bottomPanel.Height = 50;
            bottomPanel.BackColor = Color.LightGray;
            this.Controls.Add(bottomPanel);

            var exportButton = new Button();
            exportButton.Text = "Exportar Reporte";
            exportButton.Location = new Point(20, 10);
            exportButton.Size = new Size(120, 30);
            exportButton.Click += ExportButton_Click;
            bottomPanel.Controls.Add(exportButton);

            var printButton = new Button();
            printButton.Text = "Imprimir";
            printButton.Location = new Point(150, 10);
            printButton.Size = new Size(80, 30);
            printButton.Click += PrintButton_Click;
            bottomPanel.Controls.Add(printButton);

            var closeButton = new Button();
            closeButton.Text = "Cerrar";
            closeButton.Location = new Point(1090, 10);
            closeButton.Size = new Size(80, 30);
            closeButton.Click += CloseButton_Click;
            bottomPanel.Controls.Add(closeButton);
        }

        private void LoadReports()
        {
            if (contentPanel != null)
            {
                ShowSummaryReport();
            }
        }

        private void SummaryButton_Click(object sender, EventArgs e)
        {
            if (contentPanel != null)
            {
                ShowSummaryReport();
            }
        }

        private void CategoriesButton_Click(object sender, EventArgs e)
        {
            if (contentPanel != null)
            {
                ShowCategoriesReport();
            }
        }

        private void LocationsButton_Click(object sender, EventArgs e)
        {
            if (contentPanel != null)
            {
                ShowLocationsReport();
            }
        }

        private void LowStockButton_Click(object sender, EventArgs e)
        {
            if (contentPanel != null)
            {
                ShowLowStockReport();
            }
        }

        private void ExportButton_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Funcionalidad de exportación en desarrollo.", "Información", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void PrintButton_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Funcionalidad de impresión en desarrollo.", "Información", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void CloseButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void ShowSummaryReport()
        {
            if (contentPanel == null)
            {
                return; 
            }
            
            contentPanel.Controls.Clear();

            try
            {
                var summary = _reportService.GetInventorySummary();

                var titleLabel = new Label();
                titleLabel.Text = "RESUMEN GENERAL DEL INVENTARIO";
                titleLabel.Font = new Font("Arial", 18, FontStyle.Bold);
                titleLabel.Location = new Point(50, 30);
                titleLabel.Size = new Size(500, 35);
                titleLabel.ForeColor = Color.DarkBlue;
                contentPanel.Controls.Add(titleLabel);

                var dateLabel = new Label();
                dateLabel.Text = $"Generado el: {DateTime.Now:dd/MM/yyyy HH:mm:ss}";
                dateLabel.Font = new Font("Arial", 10);
                dateLabel.Location = new Point(50, 70);
                dateLabel.Size = new Size(300, 20);
                contentPanel.Controls.Add(dateLabel);

                int yPosition = 120;

                CreateSummaryCard("Total de Items", summary.TotalItems.ToString(), Color.Blue, 50, yPosition);
                CreateSummaryCard("Cantidad Total", summary.TotalQuantity.ToString(), Color.Green, 250, yPosition);
                CreateSummaryCard("Valor Total", summary.TotalValue.ToString("C2"), Color.Orange, 450, yPosition);
                CreateSummaryCard("Categorías", summary.CategoriesCount.ToString(), Color.Purple, 650, yPosition);
                CreateSummaryCard("Ubicaciones", summary.LocationsCount.ToString(), Color.Red, 850, yPosition);

                yPosition += 120;

                var detailLabel = new Label();
                detailLabel.Text = "DETALLES POR CATEGORÍA";
                detailLabel.Font = new Font("Arial", 14, FontStyle.Bold);
                detailLabel.Location = new Point(50, yPosition);
                detailLabel.Size = new Size(300, 25);
                contentPanel.Controls.Add(detailLabel);

                yPosition += 40;

                var categories = _reportService.GetCategorySummary();
                CreateCategoriesTable(categories, yPosition);
            }
            catch (Exception ex)
            {
                ShowError($"Error cargando resumen: {ex.Message}");
            }
        }

        private void ShowCategoriesReport()
        {
            if (contentPanel == null)
            {
                return; 
            }
            
            contentPanel.Controls.Clear();

            try
            {
                var categories = _reportService.GetCategorySummary();

                var titleLabel = new Label();
                titleLabel.Text = "REPORTE POR CATEGORÍAS";
                titleLabel.Font = new Font("Arial", 18, FontStyle.Bold);
                titleLabel.Location = new Point(50, 30);
                titleLabel.Size = new Size(400, 35);
                titleLabel.ForeColor = Color.DarkBlue;
                contentPanel.Controls.Add(titleLabel);

                var dateLabel = new Label();
                dateLabel.Text = $"Generado el: {DateTime.Now:dd/MM/yyyy HH:mm:ss}";
                dateLabel.Font = new Font("Arial", 10);
                dateLabel.Location = new Point(50, 70);
                dateLabel.Size = new Size(300, 20);
                contentPanel.Controls.Add(dateLabel);

                CreateCategoriesTable(categories, 120);
            }
            catch (Exception ex)
            {
                ShowError($"Error cargando categorías: {ex.Message}");
            }
        }

        private void ShowLocationsReport()
        {
            if (contentPanel == null)
            {
                return; 
            }
            
            contentPanel.Controls.Clear();

            try
            {
                var locations = _reportService.GetLocationSummary();

                var titleLabel = new Label();
                titleLabel.Text = "REPORTE POR UBICACIONES";
                titleLabel.Font = new Font("Arial", 18, FontStyle.Bold);
                titleLabel.Location = new Point(50, 30);
                titleLabel.Size = new Size(400, 35);
                titleLabel.ForeColor = Color.DarkBlue;
                contentPanel.Controls.Add(titleLabel);

                var dateLabel = new Label();
                dateLabel.Text = $"Generado el: {DateTime.Now:dd/MM/yyyy HH:mm:ss}";
                dateLabel.Font = new Font("Arial", 10);
                dateLabel.Location = new Point(50, 70);
                dateLabel.Size = new Size(300, 20);
                contentPanel.Controls.Add(dateLabel);

                CreateLocationsTable(locations, 120);
            }
            catch (Exception ex)
            {
                ShowError($"Error cargando ubicaciones: {ex.Message}");
            }
        }

        private void ShowLowStockReport()
        {
            if (contentPanel == null)
            {
                return; 
            }
            
            contentPanel.Controls.Clear();

            try
            {
                var lowStockItems = _reportService.GetLowStockItems();

                var titleLabel = new Label();
                titleLabel.Text = "ITEMS CON STOCK BAJO";
                titleLabel.Font = new Font("Arial", 18, FontStyle.Bold);
                titleLabel.Location = new Point(50, 30);
                titleLabel.Size = new Size(400, 35);
                titleLabel.ForeColor = Color.Red;
                contentPanel.Controls.Add(titleLabel);

                var dateLabel = new Label();
                dateLabel.Text = $"Generado el: {DateTime.Now:dd/MM/yyyy HH:mm:ss}";
                dateLabel.Font = new Font("Arial", 10);
                dateLabel.Location = new Point(50, 70);
                dateLabel.Size = new Size(300, 20);
                contentPanel.Controls.Add(dateLabel);

                CreateLowStockTable(lowStockItems, 120);
            }
            catch (Exception ex)
            {
                ShowError($"Error cargando stock bajo: {ex.Message}");
            }
        }

        private void CreateSummaryCard(string title, string value, Color color, int x, int y)
        {
            if (contentPanel == null)
            {
                return; 
            }
            
            var card = new Panel();
            card.Location = new Point(x, y);
            card.Size = new Size(150, 80);
            card.BackColor = Color.White;
            card.BorderStyle = BorderStyle.FixedSingle;

            var titleLabel = new Label();
            titleLabel.Text = title;
            titleLabel.Font = new Font("Arial", 10, FontStyle.Bold);
            titleLabel.Location = new Point(10, 10);
            titleLabel.Size = new Size(130, 20);
            titleLabel.TextAlign = ContentAlignment.MiddleCenter;
            card.Controls.Add(titleLabel);

            var valueLabel = new Label();
            valueLabel.Text = value;
            valueLabel.Font = new Font("Arial", 16, FontStyle.Bold);
            valueLabel.Location = new Point(10, 35);
            valueLabel.Size = new Size(130, 30);
            valueLabel.TextAlign = ContentAlignment.MiddleCenter;
            valueLabel.ForeColor = color;
            card.Controls.Add(valueLabel);

            contentPanel.Controls.Add(card);
        }

        private void CreateCategoriesTable(List<ReportService.CategorySummary> categories, int startY)
        {
            if (contentPanel == null)
            {
                return; 
            }
            
            var table = new DataGridView();
            table.Location = new Point(50, startY);
            table.Size = new Size(1000, Math.Min(400, categories.Count * 30 + 50));
            table.AllowUserToAddRows = false;
            table.ReadOnly = true;
            table.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;
            table.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;

            table.Columns.Add("Category", "Categoría");
            table.Columns.Add("ItemsCount", "Items");
            table.Columns.Add("TotalQuantity", "Cantidad Total");
            table.Columns.Add("TotalValue", "Valor Total");
            table.Columns.Add("AveragePrice", "Precio Promedio");

            table.Columns["TotalValue"].DefaultCellStyle.Format = "C2";
            table.Columns["AveragePrice"].DefaultCellStyle.Format = "C2";
            
            
            table.Columns["Category"].Width = 200;
            table.Columns["ItemsCount"].Width = 80;
            table.Columns["TotalQuantity"].Width = 120;
            table.Columns["TotalValue"].Width = 120;
            table.Columns["AveragePrice"].Width = 120;

            foreach (var category in categories)
            {
                table.Rows.Add(
                    category.Category,
                    category.ItemsCount,
                    category.TotalQuantity,
                    category.TotalValue,
                    category.AveragePrice
                );
            }

            contentPanel.Controls.Add(table);
        }

        private void CreateLocationsTable(List<ReportService.LocationSummary> locations, int startY)
        {
            if (contentPanel == null)
            {
                return; 
            }
            
            var table = new DataGridView();
            table.Location = new Point(50, startY);
            table.Size = new Size(1000, Math.Min(400, locations.Count * 30 + 50));
            table.AllowUserToAddRows = false;
            table.ReadOnly = true;
            table.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;
            table.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;

            table.Columns.Add("Location", "Ubicación");
            table.Columns.Add("ItemsCount", "Items");
            table.Columns.Add("TotalQuantity", "Cantidad Total");
            table.Columns.Add("TotalValue", "Valor Total");

            table.Columns["TotalValue"].DefaultCellStyle.Format = "C2";
            
            
            table.Columns["Location"].Width = 200;
            table.Columns["ItemsCount"].Width = 80;
            table.Columns["TotalQuantity"].Width = 120;
            table.Columns["TotalValue"].Width = 120;

            foreach (var location in locations)
            {
                table.Rows.Add(
                    location.Location,
                    location.ItemsCount,
                    location.TotalQuantity,
                    location.TotalValue
                );
            }

            contentPanel.Controls.Add(table);
        }

        private void CreateLowStockTable(List<ReportService.LowStockItem> items, int startY)
        {
            if (contentPanel == null)
            {
                return; 
            }
            
            var table = new DataGridView();
            table.Location = new Point(50, startY);
            table.Size = new Size(1000, Math.Min(400, items.Count * 30 + 50));
            table.AllowUserToAddRows = false;
            table.ReadOnly = true;
            table.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;
            table.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;

            table.Columns.Add("Name", "Nombre");
            table.Columns.Add("SKU", "SKU");
            table.Columns.Add("Quantity", "Cantidad");
            table.Columns.Add("Category", "Categoría");
            table.Columns.Add("Location", "Ubicación");

            foreach (var item in items)
            {
                var row = table.Rows.Add(
                    item.Name,
                    item.SKU,
                    item.Quantity,
                    item.Category,
                    item.Location
                );

                if (item.Quantity <= 5)
                {
                    table.Rows[row].DefaultCellStyle.BackColor = Color.LightPink;
                }
                else if (item.Quantity <= 10)
                {
                    table.Rows[row].DefaultCellStyle.BackColor = Color.LightYellow;
                }
            }

            contentPanel.Controls.Add(table);
        }

        private void ShowError(string message)
        {
            if (contentPanel == null)
            {
                return; 
            }
            
            var errorLabel = new Label();
            errorLabel.Text = message;
            errorLabel.Font = new Font("Arial", 12, FontStyle.Bold);
            errorLabel.ForeColor = Color.Red;
            errorLabel.Location = new Point(50, 100);
            errorLabel.Size = new Size(800, 50);
            contentPanel.Controls.Add(errorLabel);
        }

        private Panel contentPanel;
    }
}
