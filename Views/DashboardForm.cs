using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using InventoryQRManager.Services;
using InventoryQRManager.Models;

namespace InventoryQRManager.Views
{
    public partial class DashboardForm : Form
    {
        private readonly ReportService _reportService;
        private readonly InventoryService _inventoryService;
        private readonly AuthService _authService;
        private Chart _categoryChart;
        private Chart _locationChart;
        private Chart _trendChart;
        private Label _totalItemsLabel;
        private Label _totalValueLabel;
        private Label _lowStockLabel;
        private Label _categoriesLabel;
        private System.Windows.Forms.Timer _refreshTimer;

        public DashboardForm(ReportService reportService, InventoryService inventoryService, AuthService authService)
        {
            _reportService = reportService;
            _inventoryService = inventoryService;
            _authService = authService;
            InitializeComponent();
            LoadDashboardData();
            SetupAutoRefresh();
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();

            this.Text = "📊 Dashboard Ejecutivo - Inventory QR Manager";
            this.Size = new Size(1200, 800);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.FromArgb(248, 249, 250);

            var mainPanel = new Panel
            {
                Dock = DockStyle.Fill,
                AutoScroll = true,
                Padding = new Padding(20)
            };

            var titleLabel = new Label
            {
                Text = "📊 Dashboard Ejecutivo",
                Font = new Font("Segoe UI", 20, FontStyle.Bold),
                ForeColor = Color.FromArgb(52, 58, 64),
                Size = new Size(400, 40),
                Location = new Point(20, 20)
            };

            var metricsPanel = new Panel
            {
                Size = new Size(1160, 120),
                Location = new Point(20, 80),
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };

            var totalItemsPanel = CreateMetricPanel("📦 Total Items", "0", Color.FromArgb(40, 167, 69), new Point(20, 20));
            var totalValuePanel = CreateMetricPanel("💰 Valor Total", "$0", Color.FromArgb(255, 193, 7), new Point(300, 20));
            var lowStockPanel = CreateMetricPanel("⚠️ Stock Bajo", "0", Color.FromArgb(220, 53, 69), new Point(580, 20));
            var categoriesPanel = CreateMetricPanel("📂 Categorías", "0", Color.FromArgb(0, 123, 255), new Point(860, 20));

            _totalItemsLabel = (Label)totalItemsPanel.Controls[1];
            _totalValueLabel = (Label)totalItemsPanel.Controls[1];
            _lowStockLabel = (Label)lowStockPanel.Controls[1];
            _categoriesLabel = (Label)categoriesPanel.Controls[1];

            metricsPanel.Controls.AddRange(new Control[] { totalItemsPanel, totalValuePanel, lowStockPanel, categoriesPanel });

            var chartsPanel = new Panel
            {
                Size = new Size(1160, 500),
                Location = new Point(20, 220),
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };

            var categoryChartLabel = new Label
            {
                Text = "📊 Distribución por Categorías",
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                ForeColor = Color.FromArgb(52, 58, 64),
                Size = new Size(300, 25),
                Location = new Point(20, 20)
            };

            _categoryChart = new Chart
            {
                Size = new Size(350, 200),
                Location = new Point(20, 50),
                BackColor = Color.White
            };

            SetupCategoryChart();

            var locationChartLabel = new Label
            {
                Text = "📍 Distribución por Ubicaciones",
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                ForeColor = Color.FromArgb(52, 58, 64),
                Size = new Size(300, 25),
                Location = new Point(400, 20)
            };

            _locationChart = new Chart
            {
                Size = new Size(350, 200),
                Location = new Point(400, 50),
                BackColor = Color.White
            };

            SetupLocationChart();

            var trendChartLabel = new Label
            {
                Text = "📈 Tendencias de Inventario",
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                ForeColor = Color.FromArgb(52, 58, 64),
                Size = new Size(300, 25),
                Location = new Point(20, 280)
            };

            _trendChart = new Chart
            {
                Size = new Size(730, 200),
                Location = new Point(20, 310),
                BackColor = Color.White
            };

            SetupTrendChart();

            chartsPanel.Controls.AddRange(new Control[] {
                categoryChartLabel, _categoryChart, locationChartLabel, _locationChart,
                trendChartLabel, _trendChart
            });

            var userInfoPanel = new Panel
            {
                Size = new Size(1160, 60),
                Location = new Point(20, 740),
                BackColor = Color.FromArgb(233, 236, 239),
                BorderStyle = BorderStyle.FixedSingle
            };

            var userInfoLabel = new Label
            {
                Text = $"👤 Usuario: {_authService.CurrentUser?.FullName} | Rol: {GetRoleDisplayName(_authService.CurrentUser?.Role ?? UserRole.User)} | Última actualización: {DateTime.Now:HH:mm:ss}",
                Font = new Font("Segoe UI", 10),
                ForeColor = Color.FromArgb(52, 58, 64),
                Size = new Size(1000, 30),
                Location = new Point(20, 15)
            };

            userInfoPanel.Controls.Add(userInfoLabel);

            mainPanel.Controls.AddRange(new Control[] {
                titleLabel, metricsPanel, chartsPanel, userInfoPanel
            });

            this.Controls.Add(mainPanel);
            this.ResumeLayout(false);
        }

        private Panel CreateMetricPanel(string title, string value, Color color, Point location)
        {
            var panel = new Panel
            {
                Size = new Size(250, 80),
                Location = location,
                BackColor = color,
                BorderStyle = BorderStyle.FixedSingle
            };

            var titleLabel = new Label
            {
                Text = title,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = Color.White,
                Size = new Size(200, 25),
                Location = new Point(10, 10),
                TextAlign = ContentAlignment.MiddleLeft
            };

            var valueLabel = new Label
            {
                Text = value,
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                ForeColor = Color.White,
                Size = new Size(200, 35),
                Location = new Point(10, 35),
                TextAlign = ContentAlignment.MiddleLeft
            };

            panel.Controls.AddRange(new Control[] { titleLabel, valueLabel });
            return panel;
        }

        private void SetupCategoryChart()
        {
            var chartArea = new ChartArea("CategoryArea");
            chartArea.BackColor = Color.White;
            chartArea.AxisX.Title = "Categorías";
            chartArea.AxisY.Title = "Cantidad";
            chartArea.AxisX.MajorGrid.Enabled = false;
            chartArea.AxisY.MajorGrid.Enabled = true;
            chartArea.AxisY.MajorGrid.LineColor = Color.LightGray;

            _categoryChart.ChartAreas.Add(chartArea);

            var series = new Series("Categorías");
            series.ChartType = SeriesChartType.Column;
            series.Color = Color.FromArgb(40, 167, 69);
            series.BorderWidth = 2;

            _categoryChart.Series.Add(series);
            _categoryChart.Legends.Clear();
        }

        private void SetupLocationChart()
        {
            var chartArea = new ChartArea("LocationArea");
            chartArea.BackColor = Color.White;
            chartArea.AxisX.Title = "Ubicaciones";
            chartArea.AxisY.Title = "Cantidad";
            chartArea.AxisX.MajorGrid.Enabled = false;
            chartArea.AxisY.MajorGrid.Enabled = true;
            chartArea.AxisY.MajorGrid.LineColor = Color.LightGray;

            _locationChart.ChartAreas.Add(chartArea);

            var series = new Series("Ubicaciones");
            series.ChartType = SeriesChartType.Column;
            series.Color = Color.FromArgb(0, 123, 255);
            series.BorderWidth = 2;

            _locationChart.Series.Add(series);
            _locationChart.Legends.Clear();
        }

        private void SetupTrendChart()
        {
            var chartArea = new ChartArea("TrendArea");
            chartArea.BackColor = Color.White;
            chartArea.AxisX.Title = "Fecha";
            chartArea.AxisY.Title = "Cantidad";
            chartArea.AxisX.MajorGrid.Enabled = true;
            chartArea.AxisY.MajorGrid.Enabled = true;
            chartArea.AxisX.MajorGrid.LineColor = Color.LightGray;
            chartArea.AxisY.MajorGrid.LineColor = Color.LightGray;

            _trendChart.ChartAreas.Add(chartArea);

            var series = new Series("Tendencia");
            series.ChartType = SeriesChartType.Line;
            series.Color = Color.FromArgb(255, 193, 7);
            series.BorderWidth = 3;
            series.MarkerStyle = MarkerStyle.Circle;
            series.MarkerSize = 8;

            _trendChart.Series.Add(series);
            _trendChart.Legends.Clear();
        }

        private void LoadDashboardData()
        {
            try
            {
                var summary = _reportService.GetInventorySummary();
                var lowStockThreshold = 10; // Umbral de stock bajo
                var lowStockItems = _inventoryService.GetAllItems().Count(i => i.Quantity <= lowStockThreshold);

                _totalItemsLabel.Text = summary.TotalItems.ToString();
                _totalValueLabel.Text = $"${summary.TotalValue:F2}";
                _lowStockLabel.Text = lowStockItems.ToString();
                _categoriesLabel.Text = summary.CategoriesCount.ToString();

                LoadCategoryChart();

                LoadLocationChart();

                LoadTrendChart();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error cargando datos del dashboard: {ex.Message}", "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadCategoryChart()
        {
            _categoryChart.Series["Categorías"].Points.Clear();

            var categorySummary = _reportService.GetCategorySummary();
            foreach (var category in categorySummary)
            {
                _categoryChart.Series["Categorías"].Points.AddXY(category.Category, category.ItemsCount);
            }
        }

        private void LoadLocationChart()
        {
            _locationChart.Series["Ubicaciones"].Points.Clear();

            var locationSummary = _reportService.GetLocationSummary();
            foreach (var location in locationSummary)
            {
                _locationChart.Series["Ubicaciones"].Points.AddXY(location.Location, location.ItemsCount);
            }
        }

        private void LoadTrendChart()
        {
            _trendChart.Series["Tendencia"].Points.Clear();

            var random = new Random();
            var baseValue = 100;
            
            for (int i = 0; i < 7; i++)
            {
                var date = DateTime.Now.AddDays(-6 + i);
                var value = baseValue + random.Next(-20, 30);
                _trendChart.Series["Tendencia"].Points.AddXY(date.ToString("dd/MM"), value);
            }
        }

        private void SetupAutoRefresh()
        {
            _refreshTimer = new System.Windows.Forms.Timer();
            _refreshTimer.Interval = 30000; // 30 segundos
            _refreshTimer.Tick += (s, e) => LoadDashboardData();
            _refreshTimer.Start();
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

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            _refreshTimer?.Stop();
            _refreshTimer?.Dispose();
            base.OnFormClosed(e);
        }
    }
}
