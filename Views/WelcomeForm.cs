using System;
using System.Drawing;
using System.Windows.Forms;
using InventoryQRManager.Services;

namespace InventoryQRManager.Views
{
    public partial class WelcomeForm : Form
    {
        private readonly SettingsService _settingsService;

        public WelcomeForm()
        {
            _settingsService = new SettingsService();
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            
            // Configurar el formulario
            this.Text = "Bienvenido - Inventory QR Manager";
            this.Size = new Size(900, 700);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.BackColor = Color.FromArgb(248, 249, 250);
            this.Font = new Font("Segoe UI", 9F, FontStyle.Regular);

            CreateControls();
            
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private void CreateControls()
        {
            // Panel principal
            var mainPanel = new Panel();
            mainPanel.Dock = DockStyle.Fill;
            mainPanel.BackColor = Color.White;
            this.Controls.Add(mainPanel);

            // Logo/Título
            var titleLabel = new Label();
            titleLabel.Text = "Inventory QR Manager";
            titleLabel.Font = new Font("Segoe UI", 28, FontStyle.Bold);
            titleLabel.ForeColor = Color.FromArgb(41, 128, 185);
            titleLabel.Location = new Point(200, 50);
            titleLabel.Size = new Size(500, 45);
            titleLabel.TextAlign = ContentAlignment.MiddleCenter;
            mainPanel.Controls.Add(titleLabel);

            var subtitleLabel = new Label();
            subtitleLabel.Text = "Sistema de Gestión de Inventario con Códigos QR";
            subtitleLabel.Font = new Font("Segoe UI", 16, FontStyle.Regular);
            subtitleLabel.ForeColor = Color.FromArgb(127, 140, 141);
            subtitleLabel.Location = new Point(200, 105);
            subtitleLabel.Size = new Size(500, 30);
            subtitleLabel.TextAlign = ContentAlignment.MiddleCenter;
            mainPanel.Controls.Add(subtitleLabel);

            // Panel de características
            var featuresPanel = new Panel();
            featuresPanel.Location = new Point(50, 150);
            featuresPanel.Size = new Size(700, 300);
            featuresPanel.BackColor = Color.LightGray;
            featuresPanel.BorderStyle = BorderStyle.FixedSingle;
            mainPanel.Controls.Add(featuresPanel);

            var featuresTitleLabel = new Label();
            featuresTitleLabel.Text = "Características Principales";
            featuresTitleLabel.Font = new Font("Arial", 16, FontStyle.Bold);
            featuresTitleLabel.Location = new Point(20, 20);
            featuresTitleLabel.Size = new Size(300, 25);
            featuresPanel.Controls.Add(featuresTitleLabel);

            // Lista de características
            var features = new[]
            {
                "✓ Gestión completa de inventario",
                "✓ Generación automática de códigos QR",
                "✓ Búsqueda y filtrado avanzado",
                "✓ Reportes y estadísticas detalladas",
                "✓ Sistema de backup y restauración",
                "✓ Importación y exportación de datos",
                "✓ Configuración personalizable",
                "✓ Alertas de stock bajo"
            };

            int yPos = 60;
            foreach (var feature in features)
            {
                var featureLabel = new Label();
                featureLabel.Text = feature;
                featureLabel.Font = new Font("Arial", 12);
                featureLabel.Location = new Point(30, yPos);
                featureLabel.Size = new Size(300, 20);
                featuresPanel.Controls.Add(featureLabel);
                yPos += 25;
            }

            // Panel de acciones rápidas
            var actionsPanel = new Panel();
            actionsPanel.Location = new Point(400, 60);
            actionsPanel.Size = new Size(250, 250);
            actionsPanel.BackColor = Color.White;
            actionsPanel.BorderStyle = BorderStyle.FixedSingle;
            featuresPanel.Controls.Add(actionsPanel);

            var actionsTitleLabel = new Label();
            actionsTitleLabel.Text = "Acciones Rápidas";
            actionsTitleLabel.Font = new Font("Arial", 14, FontStyle.Bold);
            actionsTitleLabel.Location = new Point(20, 20);
            actionsTitleLabel.Size = new Size(200, 25);
            actionsPanel.Controls.Add(actionsTitleLabel);

            // Botones de acciones rápidas
            var newItemButton = new Button();
            newItemButton.Text = "Nuevo Item";
            newItemButton.Location = new Point(20, 60);
            newItemButton.Size = new Size(200, 40);
            newItemButton.BackColor = Color.FromArgb(46, 204, 113);
            newItemButton.ForeColor = Color.White;
            newItemButton.FlatStyle = FlatStyle.Flat;
            newItemButton.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            newItemButton.Click += NewItemButton_Click;
            actionsPanel.Controls.Add(newItemButton);

            var searchButton = new Button();
            searchButton.Text = "Buscar Items";
            searchButton.Location = new Point(20, 105);
            searchButton.Size = new Size(200, 35);
            searchButton.BackColor = Color.Green;
            searchButton.ForeColor = Color.White;
            searchButton.FlatStyle = FlatStyle.Flat;
            searchButton.Click += SearchButton_Click;
            actionsPanel.Controls.Add(searchButton);

            var reportsButton = new Button();
            reportsButton.Text = "Ver Reportes";
            reportsButton.Location = new Point(20, 150);
            reportsButton.Size = new Size(200, 35);
            reportsButton.BackColor = Color.Orange;
            reportsButton.ForeColor = Color.White;
            reportsButton.FlatStyle = FlatStyle.Flat;
            reportsButton.Click += ReportsButton_Click;
            actionsPanel.Controls.Add(reportsButton);

            var settingsButton = new Button();
            settingsButton.Text = "Configuración";
            settingsButton.Location = new Point(20, 195);
            settingsButton.Size = new Size(200, 35);
            settingsButton.BackColor = Color.Purple;
            settingsButton.ForeColor = Color.White;
            settingsButton.FlatStyle = FlatStyle.Flat;
            settingsButton.Click += SettingsButton_Click;
            actionsPanel.Controls.Add(settingsButton);

            // Panel inferior
            var bottomPanel = new Panel();
            bottomPanel.Dock = DockStyle.Bottom;
            bottomPanel.Height = 80;
            bottomPanel.BackColor = Color.LightGray;
            mainPanel.Controls.Add(bottomPanel);

            // Checkbox para no mostrar más
            var dontShowCheckBox = new CheckBox();
            dontShowCheckBox.Text = "No mostrar esta pantalla al iniciar";
            dontShowCheckBox.Location = new Point(20, 20);
            dontShowCheckBox.Size = new Size(250, 20);
            dontShowCheckBox.CheckedChanged += DontShowCheckBox_CheckedChanged;
            bottomPanel.Controls.Add(dontShowCheckBox);

            // Botones
            var startButton = new Button();
            startButton.Text = "Comenzar";
            startButton.Location = new Point(600, 15);
            startButton.Size = new Size(100, 40);
            startButton.BackColor = Color.DarkGreen;
            startButton.ForeColor = Color.White;
            startButton.FlatStyle = FlatStyle.Flat;
            startButton.Click += StartButton_Click;
            bottomPanel.Controls.Add(startButton);

            var exitButton = new Button();
            exitButton.Text = "Salir";
            exitButton.Location = new Point(710, 15);
            exitButton.Size = new Size(70, 40);
            exitButton.BackColor = Color.DarkRed;
            exitButton.ForeColor = Color.White;
            exitButton.FlatStyle = FlatStyle.Flat;
            exitButton.Click += ExitButton_Click;
            bottomPanel.Controls.Add(exitButton);

            // Información de versión
            var versionLabel = new Label();
            versionLabel.Text = "Versión 1.0 - Desarrollado con .NET 9.0";
            versionLabel.Font = new Font("Arial", 9);
            versionLabel.ForeColor = Color.Gray;
            versionLabel.Location = new Point(20, 50);
            versionLabel.Size = new Size(300, 15);
            bottomPanel.Controls.Add(versionLabel);
        }

        private void NewItemButton_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            this.Tag = "NewItem";
            this.Close();
        }

        private void SearchButton_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            this.Tag = "Search";
            this.Close();
        }

        private void ReportsButton_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            this.Tag = "Reports";
            this.Close();
        }

        private void SettingsButton_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            this.Tag = "Settings";
            this.Close();
        }

        private void StartButton_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void ExitButton_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void DontShowCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                var checkBox = sender as CheckBox;
                if (checkBox != null && _settingsService != null)
                {
                    _settingsService.UpdateSetting("ShowWelcomeScreen", !checkBox.Checked);
                }
            }
            catch
            {
                // Error actualizando configuración
            }
        }
    }
}
