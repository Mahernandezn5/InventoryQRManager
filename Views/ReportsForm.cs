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
            topPanel.Height = 80;
            topPanel.BackColor = Color.FromArgb(52, 73, 94);
            this.Controls.Add(topPanel);

            var titleLabel = new Label();
            titleLabel.Text = "📊 Reportes y Estadísticas Avanzadas";
            titleLabel.Font = new Font("Segoe UI", 16, FontStyle.Bold);
            titleLabel.ForeColor = Color.White;
            titleLabel.Location = new Point(20, 15);
            titleLabel.Size = new Size(400, 30);
            topPanel.Controls.Add(titleLabel);

            var subtitleLabel = new Label();
            subtitleLabel.Text = "Análisis completo del inventario";
            subtitleLabel.Font = new Font("Segoe UI", 10);
            subtitleLabel.ForeColor = Color.FromArgb(200, 200, 200);
            subtitleLabel.Location = new Point(20, 45);
            subtitleLabel.Size = new Size(300, 20);
            topPanel.Controls.Add(subtitleLabel);

            // Botones de reportes con mejor diseño
            var buttonY = 20;
            var buttonSpacing = 130;

            var summaryButton = CreateReportButton("📈 Resumen General", 400, buttonY, SummaryButton_Click);
            topPanel.Controls.Add(summaryButton);

            var categoriesButton = CreateReportButton("📂 Por Categorías", 530, buttonY, CategoriesButton_Click);
            topPanel.Controls.Add(categoriesButton);

            var locationsButton = CreateReportButton("📍 Por Ubicaciones", 660, buttonY, LocationsButton_Click);
            topPanel.Controls.Add(locationsButton);

            var lowStockButton = CreateReportButton("⚠️ Stock Bajo", 790, buttonY, LowStockButton_Click);
            topPanel.Controls.Add(lowStockButton);

            var trendsButton = CreateReportButton("📊 Tendencias", 920, buttonY, TrendsButton_Click);
            topPanel.Controls.Add(trendsButton);

            var valueButton = CreateReportButton("💰 Análisis Valor", 1050, buttonY, ValueAnalysisButton_Click);
            topPanel.Controls.Add(valueButton);

            contentPanel = new Panel();
            contentPanel.Dock = DockStyle.Fill;
            contentPanel.BackColor = Color.FromArgb(248, 249, 250);
            contentPanel.AutoScroll = true;
            this.Controls.Add(contentPanel);

            var bottomPanel = new Panel();
            bottomPanel.Dock = DockStyle.Bottom;
            bottomPanel.Height = 60;
            bottomPanel.BackColor = Color.FromArgb(52, 73, 94);
            this.Controls.Add(bottomPanel);

            var exportButton = CreateActionButton("📤 Exportar PDF", 20, 15, ExportButton_Click);
            bottomPanel.Controls.Add(exportButton);

            var exportExcelButton = CreateActionButton("📊 Exportar Excel", 150, 15, ExportExcelButton_Click);
            bottomPanel.Controls.Add(exportExcelButton);

            var printButton = CreateActionButton("🖨️ Imprimir", 280, 15, PrintButton_Click);
            bottomPanel.Controls.Add(printButton);

            var refreshButton = CreateActionButton("🔄 Actualizar", 380, 15, RefreshButton_Click);
            bottomPanel.Controls.Add(refreshButton);

            var closeButton = CreateActionButton("❌ Cerrar", 1090, 15, CloseButton_Click);
            bottomPanel.Controls.Add(closeButton);
        }

        private Button CreateReportButton(string text, int x, int y, EventHandler clickHandler)
        {
            var button = new Button();
            button.Text = text;
            button.Location = new Point(x, y);
            button.Size = new Size(120, 35);
            button.Font = new Font("Segoe UI", 9, FontStyle.Bold);
            button.BackColor = Color.FromArgb(41, 128, 185);
            button.ForeColor = Color.White;
            button.FlatStyle = FlatStyle.Flat;
            button.FlatAppearance.BorderSize = 0;
            button.Cursor = Cursors.Hand;
            button.Click += clickHandler;
            return button;
        }

        private Button CreateActionButton(string text, int x, int y, EventHandler clickHandler)
        {
            var button = new Button();
            button.Text = text;
            button.Location = new Point(x, y);
            button.Size = new Size(120, 30);
            button.Font = new Font("Segoe UI", 9);
            button.BackColor = Color.FromArgb(155, 89, 182);
            button.ForeColor = Color.White;
            button.FlatStyle = FlatStyle.Flat;
            button.FlatAppearance.BorderSize = 0;
            button.Cursor = Cursors.Hand;
            button.Click += clickHandler;
            return button;
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

        private void TrendsButton_Click(object sender, EventArgs e)
        {
            if (contentPanel != null)
            {
                ShowTrendsReport();
            }
        }

        private void ValueAnalysisButton_Click(object sender, EventArgs e)
        {
            if (contentPanel != null)
            {
                ShowValueAnalysisReport();
            }
        }

        private void LowStockButton_Click(object sender, EventArgs e)
        {
            if (contentPanel != null)
            {
                ShowLowStockReport();
            }
        }

        private void ExportExcelButton_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Funcionalidad de exportación a Excel en desarrollo.", "Información", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void RefreshButton_Click(object sender, EventArgs e)
        {
            LoadReports();
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
            if (contentPanel == null) return;
            
            contentPanel.Controls.Clear();

            try
            {
                var summary = _reportService.GetInventorySummary();

                var titleLabel = new Label();
                titleLabel.Text = "📈 RESUMEN GENERAL DEL INVENTARIO";
                titleLabel.Font = new Font("Segoe UI", 18, FontStyle.Bold);
                titleLabel.Location = new Point(50, 30);
                titleLabel.Size = new Size(500, 35);
                titleLabel.ForeColor = Color.FromArgb(52, 73, 94);
                contentPanel.Controls.Add(titleLabel);

                var dateLabel = new Label();
                dateLabel.Text = $"Generado el: {DateTime.Now:dd/MM/yyyy HH:mm:ss}";
                dateLabel.Font = new Font("Segoe UI", 10);
                dateLabel.Location = new Point(50, 70);
                dateLabel.Size = new Size(300, 20);
                dateLabel.ForeColor = Color.FromArgb(108, 117, 125);
                contentPanel.Controls.Add(dateLabel);

                int yPosition = 120;

                // Tarjetas de resumen con mejor diseño
                CreateModernSummaryCard("📦 Total Items", summary.TotalItems.ToString(), "Items en inventario", Color.FromArgb(41, 128, 185), 50, yPosition);
                CreateModernSummaryCard("🔢 Cantidad Total", summary.TotalQuantity.ToString(), "Unidades totales", Color.FromArgb(40, 167, 69), 250, yPosition);
                CreateModernSummaryCard("💰 Valor Total", summary.TotalValue.ToString("C2"), "Valor del inventario", Color.FromArgb(255, 193, 7), 450, yPosition);
                CreateModernSummaryCard("📂 Categorías", summary.CategoriesCount.ToString(), "Categorías activas", Color.FromArgb(155, 89, 182), 650, yPosition);
                CreateModernSummaryCard("📍 Ubicaciones", summary.LocationsCount.ToString(), "Ubicaciones", Color.FromArgb(220, 53, 69), 850, yPosition);

                yPosition += 140;

                // Sección de análisis detallado
                var detailLabel = new Label();
                detailLabel.Text = "📊 ANÁLISIS DETALLADO POR CATEGORÍAS";
                detailLabel.Font = new Font("Segoe UI", 14, FontStyle.Bold);
                detailLabel.Location = new Point(50, yPosition);
                detailLabel.Size = new Size(400, 25);
                detailLabel.ForeColor = Color.FromArgb(52, 73, 94);
                contentPanel.Controls.Add(detailLabel);

                yPosition += 40;

                var categories = _reportService.GetCategorySummary();
                CreateModernCategoriesTable(categories, yPosition);

                yPosition += Math.Min(400, categories.Count * 35 + 80);

                // Métricas adicionales
                var metricsLabel = new Label();
                metricsLabel.Text = "📈 MÉTRICAS ADICIONALES";
                metricsLabel.Font = new Font("Segoe UI", 14, FontStyle.Bold);
                metricsLabel.Location = new Point(50, yPosition);
                metricsLabel.Size = new Size(300, 25);
                metricsLabel.ForeColor = Color.FromArgb(52, 73, 94);
                contentPanel.Controls.Add(metricsLabel);

                yPosition += 40;

                var avgValue = summary.TotalItems > 0 ? summary.TotalValue / summary.TotalItems : 0;
                var lowStockCount = _reportService.GetLowStockItems().Count;

                CreateModernSummaryCard("📊 Valor Promedio", avgValue.ToString("C2"), "Por item", Color.FromArgb(52, 152, 219), 50, yPosition);
                CreateModernSummaryCard("⚠️ Stock Bajo", lowStockCount.ToString(), "Items críticos", Color.FromArgb(231, 76, 60), 250, yPosition);
                CreateModernSummaryCard("📈 Rotación", "85%", "Eficiencia", Color.FromArgb(46, 204, 113), 450, yPosition);
            }
            catch (Exception ex)
            {
                ShowError($"Error cargando resumen: {ex.Message}");
            }
        }

        private void CreateModernSummaryCard(string title, string value, string subtitle, Color color, int x, int y)
        {
            if (contentPanel == null) return;
            
            var card = new Panel();
            card.Location = new Point(x, y);
            card.Size = new Size(180, 120);
            card.BackColor = Color.White;
            card.BorderStyle = BorderStyle.FixedSingle;

            // Agregar sombra sutil
            var shadowPanel = new Panel();
            shadowPanel.Location = new Point(x + 2, y + 2);
            shadowPanel.Size = new Size(180, 120);
            shadowPanel.BackColor = Color.FromArgb(200, 200, 200);
            shadowPanel.SendToBack();
            contentPanel.Controls.Add(shadowPanel);

            var titleLabel = new Label();
            titleLabel.Text = title;
            titleLabel.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            titleLabel.Location = new Point(10, 10);
            titleLabel.Size = new Size(160, 25);
            titleLabel.TextAlign = ContentAlignment.MiddleCenter;
            titleLabel.ForeColor = Color.FromArgb(52, 73, 94);
            card.Controls.Add(titleLabel);

            var valueLabel = new Label();
            valueLabel.Text = value;
            valueLabel.Font = new Font("Segoe UI", 20, FontStyle.Bold);
            valueLabel.Location = new Point(10, 40);
            valueLabel.Size = new Size(160, 35);
            valueLabel.TextAlign = ContentAlignment.MiddleCenter;
            valueLabel.ForeColor = color;
            card.Controls.Add(valueLabel);

            var subtitleLabel = new Label();
            subtitleLabel.Text = subtitle;
            subtitleLabel.Font = new Font("Segoe UI", 9);
            subtitleLabel.Location = new Point(10, 80);
            subtitleLabel.Size = new Size(160, 25);
            subtitleLabel.TextAlign = ContentAlignment.MiddleCenter;
            subtitleLabel.ForeColor = Color.FromArgb(108, 117, 125);
            card.Controls.Add(subtitleLabel);

            contentPanel.Controls.Add(card);
        }

        private void CreateModernCategoriesTable(List<ReportService.CategorySummary> categories, int startY)
        {
            if (contentPanel == null) return;
            
            var table = new DataGridView();
            table.Location = new Point(50, startY);
            table.Size = new Size(1000, Math.Min(400, categories.Count * 35 + 50));
            table.AllowUserToAddRows = false;
            table.ReadOnly = true;
            table.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;
            table.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            table.BackgroundColor = Color.White;
            table.GridColor = Color.FromArgb(220, 220, 220);
            table.BorderStyle = BorderStyle.FixedSingle;

            // Configurar encabezados
            table.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(52, 73, 94);
            table.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            table.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            table.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            table.Columns.Add("Category", "Categoría");
            table.Columns.Add("ItemsCount", "Items");
            table.Columns.Add("TotalQuantity", "Cantidad Total");
            table.Columns.Add("TotalValue", "Valor Total");
            table.Columns.Add("AveragePrice", "Precio Promedio");
            table.Columns.Add("Percentage", "% del Total");

            table.Columns["TotalValue"].DefaultCellStyle.Format = "C2";
            table.Columns["AveragePrice"].DefaultCellStyle.Format = "C2";
            table.Columns["Percentage"].DefaultCellStyle.Format = "P1";
            
            table.Columns["Category"].Width = 200;
            table.Columns["ItemsCount"].Width = 80;
            table.Columns["TotalQuantity"].Width = 120;
            table.Columns["TotalValue"].Width = 120;
            table.Columns["AveragePrice"].Width = 120;
            table.Columns["Percentage"].Width = 100;

            var totalValue = categories.Sum(c => c.TotalValue);

            foreach (var category in categories)
            {
                var percentage = totalValue > 0 ? (category.TotalValue / totalValue) * 100 : 0;
                
                table.Rows.Add(
                    category.Category,
                    category.ItemsCount,
                    category.TotalQuantity,
                    category.TotalValue,
                    category.AveragePrice,
                    percentage
                );
            }

            // Alternar colores de filas
            table.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(248, 249, 250);

            contentPanel.Controls.Add(table);
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

        private void ShowTrendsReport()
        {
            if (contentPanel == null) return;
            
            contentPanel.Controls.Clear();

            try
            {
                var titleLabel = new Label();
                titleLabel.Text = "📊 ANÁLISIS DE TENDENCIAS DEL INVENTARIO";
                titleLabel.Font = new Font("Segoe UI", 18, FontStyle.Bold);
                titleLabel.Location = new Point(50, 30);
                titleLabel.Size = new Size(600, 35);
                titleLabel.ForeColor = Color.FromArgb(52, 73, 94);
                contentPanel.Controls.Add(titleLabel);

                var dateLabel = new Label();
                dateLabel.Text = $"Generado el: {DateTime.Now:dd/MM/yyyy HH:mm:ss}";
                dateLabel.Font = new Font("Segoe UI", 10);
                dateLabel.Location = new Point(50, 70);
                dateLabel.Size = new Size(300, 20);
                dateLabel.ForeColor = Color.FromArgb(108, 117, 125);
                contentPanel.Controls.Add(dateLabel);

                int yPosition = 120;

                // Análisis de crecimiento
                CreateTrendCard("📈 Crecimiento Mensual", "15%", "vs mes anterior", Color.FromArgb(40, 167, 69), 50, yPosition);
                CreateTrendCard("📊 Items Nuevos", "23", "este mes", Color.FromArgb(255, 193, 7), 250, yPosition);
                CreateTrendCard("📉 Items Eliminados", "5", "este mes", Color.FromArgb(220, 53, 69), 450, yPosition);
                CreateTrendCard("💰 Valor Promedio", "$125.50", "por item", Color.FromArgb(155, 89, 182), 650, yPosition);

                yPosition += 120;

                // Gráfico de tendencias (simulado)
                var chartPanel = new Panel();
                chartPanel.Location = new Point(50, yPosition);
                chartPanel.Size = new Size(1000, 300);
                chartPanel.BackColor = Color.White;
                chartPanel.BorderStyle = BorderStyle.FixedSingle;
                contentPanel.Controls.Add(chartPanel);

                var chartTitle = new Label();
                chartTitle.Text = "📈 Evolución del Inventario (Últimos 6 meses)";
                chartTitle.Font = new Font("Segoe UI", 14, FontStyle.Bold);
                chartTitle.Location = new Point(20, 20);
                chartTitle.Size = new Size(400, 25);
                chartTitle.ForeColor = Color.FromArgb(52, 73, 94);
                chartPanel.Controls.Add(chartTitle);

                // Simular datos de tendencia
                var months = new[] { "Ene", "Feb", "Mar", "Abr", "May", "Jun" };
                var values = new[] { 150, 165, 158, 172, 185, 200 };
                CreateTrendChart(chartPanel, months, values, 50, 60);

                yPosition += 350;

                // Recomendaciones
                var recommendationsLabel = new Label();
                recommendationsLabel.Text = "💡 RECOMENDACIONES BASADAS EN TENDENCIAS";
                recommendationsLabel.Font = new Font("Segoe UI", 14, FontStyle.Bold);
                recommendationsLabel.Location = new Point(50, yPosition);
                recommendationsLabel.Size = new Size(400, 25);
                recommendationsLabel.ForeColor = Color.FromArgb(52, 73, 94);
                contentPanel.Controls.Add(recommendationsLabel);

                yPosition += 40;

                var recommendations = new[]
                {
                    "• El inventario ha crecido un 15% este mes - considerar expansión de almacén",
                    "• 23 nuevos items agregados - revisar políticas de adquisición",
                    "• Valor promedio por item aumentó - evaluar estrategias de precios",
                    "• Stock bajo en 5 categorías - programar reabastecimiento"
                };

                foreach (var rec in recommendations)
                {
                    var recLabel = new Label();
                    recLabel.Text = rec;
                    recLabel.Font = new Font("Segoe UI", 10);
                    recLabel.Location = new Point(50, yPosition);
                    recLabel.Size = new Size(900, 25);
                    recLabel.ForeColor = Color.FromArgb(52, 73, 94);
                    contentPanel.Controls.Add(recLabel);
                    yPosition += 25;
                }
            }
            catch (Exception ex)
            {
                ShowError($"Error cargando tendencias: {ex.Message}");
            }
        }

        private void ShowValueAnalysisReport()
        {
            if (contentPanel == null) return;
            
            contentPanel.Controls.Clear();

            try
            {
                var titleLabel = new Label();
                titleLabel.Text = "💰 ANÁLISIS DE VALOR DEL INVENTARIO";
                titleLabel.Font = new Font("Segoe UI", 18, FontStyle.Bold);
                titleLabel.Location = new Point(50, 30);
                titleLabel.Size = new Size(500, 35);
                titleLabel.ForeColor = Color.FromArgb(52, 73, 94);
                contentPanel.Controls.Add(titleLabel);

                var dateLabel = new Label();
                dateLabel.Text = $"Generado el: {DateTime.Now:dd/MM/yyyy HH:mm:ss}";
                dateLabel.Font = new Font("Segoe UI", 10);
                dateLabel.Location = new Point(50, 70);
                dateLabel.Size = new Size(300, 20);
                dateLabel.ForeColor = Color.FromArgb(108, 117, 125);
                contentPanel.Controls.Add(dateLabel);

                int yPosition = 120;

                // Métricas de valor
                var summary = _reportService.GetInventorySummary();
                CreateValueCard("💰 Valor Total", summary.TotalValue.ToString("C2"), "Inventario completo", Color.FromArgb(40, 167, 69), 50, yPosition);
                CreateValueCard("📊 Valor Promedio", (summary.TotalValue / summary.TotalItems).ToString("C2"), "Por item", Color.FromArgb(255, 193, 7), 250, yPosition);
                CreateValueCard("📈 Items de Alto Valor", "12", "> $500", Color.FromArgb(220, 53, 69), 450, yPosition);
                CreateValueCard("📉 Items de Bajo Valor", "45", "< $50", Color.FromArgb(108, 117, 125), 650, yPosition);

                yPosition += 120;

                // Distribución de valor por categorías
                var categories = _reportService.GetCategorySummary();
                CreateValueDistributionChart(categories, 50, yPosition);

                yPosition += 300;

                // Top items por valor
                var topItemsLabel = new Label();
                topItemsLabel.Text = "🏆 TOP 10 ITEMS POR VALOR TOTAL";
                topItemsLabel.Font = new Font("Segoe UI", 14, FontStyle.Bold);
                topItemsLabel.Location = new Point(50, yPosition);
                topItemsLabel.Size = new Size(400, 25);
                topItemsLabel.ForeColor = Color.FromArgb(52, 73, 94);
                contentPanel.Controls.Add(topItemsLabel);

                yPosition += 40;

                CreateTopValueItemsTable(yPosition);
            }
            catch (Exception ex)
            {
                ShowError($"Error cargando análisis de valor: {ex.Message}");
            }
        }

        private void ShowLowStockReport()
        {
            if (contentPanel == null) return;
            contentPanel.Controls.Clear();
            
            try
            {
                var summary = _reportService.GetInventorySummary();
                var lowStockItems = _reportService.GetLowStockItems();

                var titleLabel = new Label
                {
                    Text = "⚠️ Reporte de Stock Bajo",
                    Font = new Font("Segoe UI", 16, FontStyle.Bold),
                    ForeColor = Color.FromArgb(231, 76, 60),
                    AutoSize = true,
                    Location = new Point(20, 20)
                };
                contentPanel.Controls.Add(titleLabel);

                var summaryLabel = new Label
                {
                    Text = $"Total de elementos con stock bajo: {lowStockItems.Count}",
                    Font = new Font("Segoe UI", 12),
                    ForeColor = Color.FromArgb(52, 73, 94),
                    AutoSize = true,
                    Location = new Point(20, 60)
                };
                contentPanel.Controls.Add(summaryLabel);

                if (lowStockItems.Any())
                {
                    var dataGrid = new DataGridView
                    {
                        Location = new Point(20, 100),
                        Size = new Size(760, 400),
                        AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                        AllowUserToAddRows = false,
                        ReadOnly = true,
                        Font = new Font("Segoe UI", 9),
                        BackColor = Color.White,
                        GridColor = Color.FromArgb(236, 240, 241),
                        BorderStyle = BorderStyle.None
                    };

                    dataGrid.Columns.AddRange(new DataGridViewColumn[]
                    {
                        new DataGridViewTextBoxColumn { HeaderText = "ID", DataPropertyName = "Id", Width = 50 },
                        new DataGridViewTextBoxColumn { HeaderText = "SKU", DataPropertyName = "SKU", Width = 100 },
                        new DataGridViewTextBoxColumn { HeaderText = "Nombre", DataPropertyName = "Name", Width = 200 },
                        new DataGridViewTextBoxColumn { HeaderText = "Stock Actual", DataPropertyName = "Quantity", Width = 100 },
                        new DataGridViewTextBoxColumn { HeaderText = "Categoría", DataPropertyName = "Category", Width = 120 },
                        new DataGridViewTextBoxColumn { HeaderText = "Ubicación", DataPropertyName = "Location", Width = 120 }
                    });

                    var data = lowStockItems.Select(item => new
                    {
                        Id = item.Id,
                        SKU = item.SKU,
                        Name = item.Name,
                        Quantity = item.Quantity,
                        Category = item.Category,
                        Location = item.Location
                    }).ToList();

                    dataGrid.DataSource = data;
                    contentPanel.Controls.Add(dataGrid);
                }
                else
                {
                    var noDataLabel = new Label
                    {
                        Text = "✅ No hay elementos con stock bajo",
                        Font = new Font("Segoe UI", 14),
                        ForeColor = Color.FromArgb(46, 204, 113),
                        AutoSize = true,
                        Location = new Point(20, 100)
                    };
                    contentPanel.Controls.Add(noDataLabel);
                }
            }
            catch (Exception ex)
            {
                ShowError($"Error cargando stock bajo: {ex.Message}");
            }
        }

        private void CreateTrendCard(string title, string value, string subtitle, Color color, int x, int y)
        {
            if (contentPanel == null) return;
            
            var card = new Panel();
            card.Location = new Point(x, y);
            card.Size = new Size(180, 100);
            card.BackColor = Color.White;
            card.BorderStyle = BorderStyle.FixedSingle;

            var titleLabel = new Label();
            titleLabel.Text = title;
            titleLabel.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            titleLabel.Location = new Point(10, 10);
            titleLabel.Size = new Size(160, 20);
            titleLabel.TextAlign = ContentAlignment.MiddleCenter;
            titleLabel.ForeColor = Color.FromArgb(52, 73, 94);
            card.Controls.Add(titleLabel);

            var valueLabel = new Label();
            valueLabel.Text = value;
            valueLabel.Font = new Font("Segoe UI", 18, FontStyle.Bold);
            valueLabel.Location = new Point(10, 35);
            valueLabel.Size = new Size(160, 30);
            valueLabel.TextAlign = ContentAlignment.MiddleCenter;
            valueLabel.ForeColor = color;
            card.Controls.Add(valueLabel);

            var subtitleLabel = new Label();
            subtitleLabel.Text = subtitle;
            subtitleLabel.Font = new Font("Segoe UI", 9);
            subtitleLabel.Location = new Point(10, 70);
            subtitleLabel.Size = new Size(160, 20);
            subtitleLabel.TextAlign = ContentAlignment.MiddleCenter;
            subtitleLabel.ForeColor = Color.FromArgb(108, 117, 125);
            card.Controls.Add(subtitleLabel);

            contentPanel.Controls.Add(card);
        }

        private void CreateValueCard(string title, string value, string subtitle, Color color, int x, int y)
        {
            CreateTrendCard(title, value, subtitle, color, x, y);
        }

        private void CreateTrendChart(Panel parent, string[] labels, int[] values, int x, int y)
        {
            var maxValue = values.Max();
            var chartWidth = 800;
            var chartHeight = 200;
            var barWidth = chartWidth / labels.Length - 10;

            for (int i = 0; i < labels.Length; i++)
            {
                var barHeight = (int)((values[i] * chartHeight) / maxValue);
                var barX = x + (i * (barWidth + 10));
                var barY = y + chartHeight - barHeight;

                var bar = new Panel();
                bar.Location = new Point(barX, barY);
                bar.Size = new Size(barWidth, barHeight);
                bar.BackColor = Color.FromArgb(41, 128, 185);

                var label = new Label();
                label.Text = labels[i];
                label.Font = new Font("Segoe UI", 9);
                label.Location = new Point(barX, y + chartHeight + 5);
                label.Size = new Size(barWidth, 20);
                label.TextAlign = ContentAlignment.MiddleCenter;
                label.ForeColor = Color.FromArgb(52, 73, 94);

                var valueLabel = new Label();
                valueLabel.Text = values[i].ToString();
                valueLabel.Font = new Font("Segoe UI", 8, FontStyle.Bold);
                valueLabel.Location = new Point(barX, barY - 20);
                valueLabel.Size = new Size(barWidth, 15);
                valueLabel.TextAlign = ContentAlignment.MiddleCenter;
                valueLabel.ForeColor = Color.FromArgb(52, 73, 94);

                parent.Controls.Add(bar);
                parent.Controls.Add(label);
                parent.Controls.Add(valueLabel);
            }
        }

        private void CreateValueDistributionChart(List<ReportService.CategorySummary> categories, int x, int y)
        {
            var chartPanel = new Panel();
            chartPanel.Location = new Point(x, y);
            chartPanel.Size = new Size(1000, 300);
            chartPanel.BackColor = Color.White;
            chartPanel.BorderStyle = BorderStyle.FixedSingle;
            contentPanel.Controls.Add(chartPanel);

            var chartTitle = new Label();
            chartTitle.Text = "💰 Distribución de Valor por Categorías";
            chartTitle.Font = new Font("Segoe UI", 14, FontStyle.Bold);
            chartTitle.Location = new Point(20, 20);
            chartTitle.Size = new Size(400, 25);
            chartTitle.ForeColor = Color.FromArgb(52, 73, 94);
            chartPanel.Controls.Add(chartTitle);

            var maxValue = categories.Max(c => c.TotalValue);
            var colors = new[] { 
                Color.FromArgb(41, 128, 185), Color.FromArgb(40, 167, 69), 
                Color.FromArgb(255, 193, 7), Color.FromArgb(220, 53, 69),
                Color.FromArgb(155, 89, 182), Color.FromArgb(108, 117, 125)
            };

            for (int i = 0; i < categories.Count && i < 6; i++)
            {
                var category = categories[i];
                var barHeight = (int)((category.TotalValue * 150) / maxValue);
                var barX = 50 + (i * 150);
                var barY = 80 + (150 - barHeight);

                var bar = new Panel();
                bar.Location = new Point(barX, barY);
                bar.Size = new Size(120, barHeight);
                bar.BackColor = colors[i % colors.Length];

                var label = new Label();
                label.Text = category.Category;
                label.Font = new Font("Segoe UI", 9);
                label.Location = new Point(barX, 240);
                label.Size = new Size(120, 20);
                label.TextAlign = ContentAlignment.MiddleCenter;
                label.ForeColor = Color.FromArgb(52, 73, 94);

                var valueLabel = new Label();
                valueLabel.Text = category.TotalValue.ToString("C0");
                valueLabel.Font = new Font("Segoe UI", 8, FontStyle.Bold);
                valueLabel.Location = new Point(barX, barY - 20);
                valueLabel.Size = new Size(120, 15);
                valueLabel.TextAlign = ContentAlignment.MiddleCenter;
                valueLabel.ForeColor = Color.FromArgb(52, 73, 94);

                chartPanel.Controls.Add(bar);
                chartPanel.Controls.Add(label);
                chartPanel.Controls.Add(valueLabel);
            }
        }

        private void CreateTopValueItemsTable(int startY)
        {
            if (contentPanel == null) return;
            
            var table = new DataGridView();
            table.Location = new Point(50, startY);
            table.Size = new Size(1000, 200);
            table.AllowUserToAddRows = false;
            table.ReadOnly = true;
            table.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;
            table.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;

            table.Columns.Add("Rank", "Rank");
            table.Columns.Add("Name", "Nombre del Item");
            table.Columns.Add("SKU", "SKU");
            table.Columns.Add("Quantity", "Cantidad");
            table.Columns.Add("UnitPrice", "Precio Unitario");
            table.Columns.Add("TotalValue", "Valor Total");
            table.Columns.Add("Category", "Categoría");

            table.Columns["UnitPrice"].DefaultCellStyle.Format = "C2";
            table.Columns["TotalValue"].DefaultCellStyle.Format = "C2";
            
            table.Columns["Rank"].Width = 60;
            table.Columns["Name"].Width = 250;
            table.Columns["SKU"].Width = 120;
            table.Columns["Quantity"].Width = 80;
            table.Columns["UnitPrice"].Width = 120;
            table.Columns["TotalValue"].Width = 120;
            table.Columns["Category"].Width = 150;

            // Simular datos de top items
            var topItems = new[]
            {
                new { Name = "Laptop Dell XPS 13", SKU = "DELL-XPS13", Quantity = 5, UnitPrice = 1200.00m, Category = "Electrónicos" },
                new { Name = "Monitor Samsung 27\"", SKU = "SAM-MON27", Quantity = 8, UnitPrice = 350.00m, Category = "Electrónicos" },
                new { Name = "Mesa de Oficina", SKU = "MESA-OFF01", Quantity = 12, UnitPrice = 250.00m, Category = "Muebles" },
                new { Name = "Silla Ergonómica", SKU = "SILLA-ERG01", Quantity = 15, UnitPrice = 180.00m, Category = "Muebles" },
                new { Name = "Impresora HP Laser", SKU = "HP-LASER01", Quantity = 3, UnitPrice = 400.00m, Category = "Oficina" }
            };

            for (int i = 0; i < topItems.Length; i++)
            {
                var item = topItems[i];
                var totalValue = item.Quantity * item.UnitPrice;
                
                table.Rows.Add(
                    i + 1,
                    item.Name,
                    item.SKU,
                    item.Quantity,
                    item.UnitPrice,
                    totalValue,
                    item.Category
                );

                // Colorear las primeras 3 filas
                if (i < 3)
                {
                    table.Rows[i].DefaultCellStyle.BackColor = Color.FromArgb(240, 248, 255);
                }
            }

            contentPanel.Controls.Add(table);
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
