using System;
using System.Drawing;
using System.Windows.Forms;
using InventoryQRManager.Services;

namespace InventoryQRManager.Views
{
    public partial class SettingsForm : Form
    {
        private readonly SettingsService _settingsService;
        private SettingsService.AppSettings _settings;

        public SettingsForm()
        {
            _settingsService = new SettingsService();
            _settings = _settingsService.GetSettings();
            
            InitializeComponent();
            LoadSettings();
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            
            this.Text = "Configuración";
            this.Size = new Size(600, 700);
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
            var tabControl = new TabControl();
            tabControl.Dock = DockStyle.Fill;
            this.Controls.Add(tabControl);

            var generalTab = new TabPage("General");
            tabControl.TabPages.Add(generalTab);
            CreateGeneralTab(generalTab);

            var inventoryTab = new TabPage("Inventario");
            tabControl.TabPages.Add(inventoryTab);
            CreateInventoryTab(inventoryTab);

            var backupTab = new TabPage("Backup");
            tabControl.TabPages.Add(backupTab);
            CreateBackupTab(backupTab);

            var categoriesTab = new TabPage("Categorías y Ubicaciones");
            tabControl.TabPages.Add(categoriesTab);
            CreateCategoriesTab(categoriesTab);

            var buttonPanel = new Panel();
            buttonPanel.Dock = DockStyle.Bottom;
            buttonPanel.Height = 50;
            buttonPanel.BackColor = Color.LightGray;
            this.Controls.Add(buttonPanel);

            var saveButton = new Button();
            saveButton.Text = "Guardar";
            saveButton.Location = new Point(400, 10);
            saveButton.Size = new Size(80, 30);
            saveButton.Click += SaveButton_Click;
            buttonPanel.Controls.Add(saveButton);

            var cancelButton = new Button();
            cancelButton.Text = "Cancelar";
            cancelButton.Location = new Point(490, 10);
            cancelButton.Size = new Size(80, 30);
            cancelButton.Click += CancelButton_Click;
            buttonPanel.Controls.Add(cancelButton);

            var resetButton = new Button();
            resetButton.Text = "Restaurar Defecto";
            resetButton.Location = new Point(20, 10);
            resetButton.Size = new Size(120, 30);
            resetButton.Click += ResetButton_Click;
            buttonPanel.Controls.Add(resetButton);
        }

        private void CreateGeneralTab(TabPage tab)
        {
            int yPosition = 20;
            int labelWidth = 150;
            int controlWidth = 300;

            var companyLabel = new Label();
            companyLabel.Text = "Información de la Empresa";
            companyLabel.Font = new Font("Arial", 12, FontStyle.Bold);
            companyLabel.Location = new Point(20, yPosition);
            companyLabel.Size = new Size(200, 20);
            tab.Controls.Add(companyLabel);

            yPosition += 30;

            var companyNameLabel = new Label();
            companyNameLabel.Text = "Nombre de la Empresa:";
            companyNameLabel.Location = new Point(20, yPosition);
            companyNameLabel.Size = new Size(labelWidth, 20);
            tab.Controls.Add(companyNameLabel);

            companyNameTextBox = new TextBox();
            companyNameTextBox.Location = new Point(180, yPosition);
            companyNameTextBox.Size = new Size(controlWidth, 20);
            tab.Controls.Add(companyNameTextBox);

            yPosition += 40;

            var languageLabel = new Label();
            languageLabel.Text = "Idioma:";
            languageLabel.Location = new Point(20, yPosition);
            languageLabel.Size = new Size(labelWidth, 20);
            tab.Controls.Add(languageLabel);

            languageComboBox = new ComboBox();
            languageComboBox.Location = new Point(180, yPosition);
            languageComboBox.Size = new Size(controlWidth, 20);
            languageComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            languageComboBox.Items.AddRange(new[] { "Español", "English" });
            tab.Controls.Add(languageComboBox);

            yPosition += 40;

            var themeLabel = new Label();
            themeLabel.Text = "Tema:";
            themeLabel.Location = new Point(20, yPosition);
            themeLabel.Size = new Size(labelWidth, 20);
            tab.Controls.Add(themeLabel);

            themeComboBox = new ComboBox();
            themeComboBox.Location = new Point(180, yPosition);
            themeComboBox.Size = new Size(controlWidth, 20);
            themeComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            themeComboBox.Items.AddRange(new[] { "Claro", "Oscuro" });
            tab.Controls.Add(themeComboBox);

            yPosition += 40;

            var interfaceLabel = new Label();
            interfaceLabel.Text = "Configuración de Interfaz";
            interfaceLabel.Font = new Font("Arial", 12, FontStyle.Bold);
            interfaceLabel.Location = new Point(20, yPosition);
            interfaceLabel.Size = new Size(200, 20);
            tab.Controls.Add(interfaceLabel);

            yPosition += 30;

            showWelcomeScreenCheckBox = new CheckBox();
            showWelcomeScreenCheckBox.Text = "Mostrar pantalla de bienvenida";
            showWelcomeScreenCheckBox.Location = new Point(20, yPosition);
            showWelcomeScreenCheckBox.Size = new Size(250, 20);
            tab.Controls.Add(showWelcomeScreenCheckBox);

            yPosition += 30;

            showNotificationsCheckBox = new CheckBox();
            showNotificationsCheckBox.Text = "Mostrar notificaciones";
            showNotificationsCheckBox.Location = new Point(20, yPosition);
            showNotificationsCheckBox.Size = new Size(250, 20);
            tab.Controls.Add(showNotificationsCheckBox);

            yPosition += 40;

            var gridPageSizeLabel = new Label();
            gridPageSizeLabel.Text = "Tamaño de página en tablas:";
            gridPageSizeLabel.Location = new Point(20, yPosition);
            gridPageSizeLabel.Size = new Size(labelWidth, 20);
            tab.Controls.Add(gridPageSizeLabel);

            gridPageSizeNumericUpDown = new NumericUpDown();
            gridPageSizeNumericUpDown.Location = new Point(180, yPosition);
            gridPageSizeNumericUpDown.Size = new Size(100, 20);
            gridPageSizeNumericUpDown.Minimum = 10;
            gridPageSizeNumericUpDown.Maximum = 1000;
            gridPageSizeNumericUpDown.Value = 50;
            tab.Controls.Add(gridPageSizeNumericUpDown);
        }

        private void CreateInventoryTab(TabPage tab)
        {
            int yPosition = 20;
            int labelWidth = 150;
            int controlWidth = 300;

            var inventoryLabel = new Label();
            inventoryLabel.Text = "Configuración de Inventario";
            inventoryLabel.Font = new Font("Arial", 12, FontStyle.Bold);
            inventoryLabel.Location = new Point(20, yPosition);
            inventoryLabel.Size = new Size(200, 20);
            tab.Controls.Add(inventoryLabel);

            yPosition += 30;

            var defaultCategoryLabel = new Label();
            defaultCategoryLabel.Text = "Categoría por defecto:";
            defaultCategoryLabel.Location = new Point(20, yPosition);
            defaultCategoryLabel.Size = new Size(labelWidth, 20);
            tab.Controls.Add(defaultCategoryLabel);

            defaultCategoryComboBox = new ComboBox();
            defaultCategoryComboBox.Location = new Point(180, yPosition);
            defaultCategoryComboBox.Size = new Size(controlWidth, 20);
            defaultCategoryComboBox.DropDownStyle = ComboBoxStyle.DropDown;
            tab.Controls.Add(defaultCategoryComboBox);

            yPosition += 40;

            var defaultLocationLabel = new Label();
            defaultLocationLabel.Text = "Ubicación por defecto:";
            defaultLocationLabel.Location = new Point(20, yPosition);
            defaultLocationLabel.Size = new Size(labelWidth, 20);
            tab.Controls.Add(defaultLocationLabel);

            defaultLocationComboBox = new ComboBox();
            defaultLocationComboBox.Location = new Point(180, yPosition);
            defaultLocationComboBox.Size = new Size(controlWidth, 20);
            defaultLocationComboBox.DropDownStyle = ComboBoxStyle.DropDown;
            tab.Controls.Add(defaultLocationComboBox);

            yPosition += 40;

            autoGenerateQRCheckBox = new CheckBox();
            autoGenerateQRCheckBox.Text = "Generar código QR automáticamente";
            autoGenerateQRCheckBox.Location = new Point(20, yPosition);
            autoGenerateQRCheckBox.Size = new Size(250, 20);
            tab.Controls.Add(autoGenerateQRCheckBox);

            yPosition += 40;

            var lowStockLabel = new Label();
            lowStockLabel.Text = "Umbral de stock bajo:";
            lowStockLabel.Location = new Point(20, yPosition);
            lowStockLabel.Size = new Size(labelWidth, 20);
            tab.Controls.Add(lowStockLabel);

            lowStockThresholdNumericUpDown = new NumericUpDown();
            lowStockThresholdNumericUpDown.Location = new Point(180, yPosition);
            lowStockThresholdNumericUpDown.Size = new Size(100, 20);
            lowStockThresholdNumericUpDown.Minimum = 1;
            lowStockThresholdNumericUpDown.Maximum = 1000;
            lowStockThresholdNumericUpDown.Value = 10;
            tab.Controls.Add(lowStockThresholdNumericUpDown);

            yPosition += 40;

            enableAuditLogCheckBox = new CheckBox();
            enableAuditLogCheckBox.Text = "Habilitar registro de auditoría";
            enableAuditLogCheckBox.Location = new Point(20, yPosition);
            enableAuditLogCheckBox.Size = new Size(250, 20);
            tab.Controls.Add(enableAuditLogCheckBox);
        }

        private void CreateBackupTab(TabPage tab)
        {
            int yPosition = 20;
            int labelWidth = 150;
            int controlWidth = 300;

            var backupLabel = new Label();
            backupLabel.Text = "Configuración de Backup";
            backupLabel.Font = new Font("Arial", 12, FontStyle.Bold);
            backupLabel.Location = new Point(20, yPosition);
            backupLabel.Size = new Size(200, 20);
            tab.Controls.Add(backupLabel);

            yPosition += 30;

            autoBackupCheckBox = new CheckBox();
            autoBackupCheckBox.Text = "Backup automático";
            autoBackupCheckBox.Location = new Point(20, yPosition);
            autoBackupCheckBox.Size = new Size(250, 20);
            tab.Controls.Add(autoBackupCheckBox);

            yPosition += 40;

            var backupIntervalLabel = new Label();
            backupIntervalLabel.Text = "Intervalo de backup (días):";
            backupIntervalLabel.Location = new Point(20, yPosition);
            backupIntervalLabel.Size = new Size(labelWidth, 20);
            tab.Controls.Add(backupIntervalLabel);

            backupIntervalNumericUpDown = new NumericUpDown();
            backupIntervalNumericUpDown.Location = new Point(180, yPosition);
            backupIntervalNumericUpDown.Size = new Size(100, 20);
            backupIntervalNumericUpDown.Minimum = 1;
            backupIntervalNumericUpDown.Maximum = 365;
            backupIntervalNumericUpDown.Value = 7;
            tab.Controls.Add(backupIntervalNumericUpDown);

            yPosition += 40;

            var backupLocationLabel = new Label();
            backupLocationLabel.Text = "Ubicación de backup:";
            backupLocationLabel.Location = new Point(20, yPosition);
            backupLocationLabel.Size = new Size(labelWidth, 20);
            tab.Controls.Add(backupLocationLabel);

            backupLocationTextBox = new TextBox();
            backupLocationTextBox.Location = new Point(180, yPosition);
            backupLocationTextBox.Size = new Size(250, 20);
            tab.Controls.Add(backupLocationTextBox);

            var browseButton = new Button();
            browseButton.Text = "Examinar";
            browseButton.Location = new Point(440, yPosition);
            browseButton.Size = new Size(80, 25);
            browseButton.Click += BrowseButton_Click;
            tab.Controls.Add(browseButton);

            yPosition += 60;

            var manualBackupLabel = new Label();
            manualBackupLabel.Text = "Backup Manual";
            manualBackupLabel.Font = new Font("Arial", 12, FontStyle.Bold);
            manualBackupLabel.Location = new Point(20, yPosition);
            manualBackupLabel.Size = new Size(200, 20);
            tab.Controls.Add(manualBackupLabel);

            yPosition += 30;

            var createBackupButton = new Button();
            createBackupButton.Text = "Crear Backup";
            createBackupButton.Location = new Point(20, yPosition);
            createBackupButton.Size = new Size(100, 30);
            createBackupButton.Click += CreateBackupButton_Click;
            tab.Controls.Add(createBackupButton);

            var restoreBackupButton = new Button();
            restoreBackupButton.Text = "Restaurar Backup";
            restoreBackupButton.Location = new Point(130, yPosition);
            restoreBackupButton.Size = new Size(120, 30);
            restoreBackupButton.Click += RestoreBackupButton_Click;
            tab.Controls.Add(restoreBackupButton);
        }

        private void CreateCategoriesTab(TabPage tab)
        {
            int yPosition = 20;

            var categoriesLabel = new Label();
            categoriesLabel.Text = "Categorías Personalizadas";
            categoriesLabel.Font = new Font("Arial", 12, FontStyle.Bold);
            categoriesLabel.Location = new Point(20, yPosition);
            categoriesLabel.Size = new Size(200, 20);
            tab.Controls.Add(categoriesLabel);

            yPosition += 30;

            customCategoriesListBox = new ListBox();
            customCategoriesListBox.Location = new Point(20, yPosition);
            customCategoriesListBox.Size = new Size(200, 150);
            tab.Controls.Add(customCategoriesListBox);

            var addCategoryButton = new Button();
            addCategoryButton.Text = "Agregar";
            addCategoryButton.Location = new Point(230, yPosition);
            addCategoryButton.Size = new Size(80, 25);
            addCategoryButton.Click += AddCategoryButton_Click;
            tab.Controls.Add(addCategoryButton);

            var removeCategoryButton = new Button();
            removeCategoryButton.Text = "Eliminar";
            removeCategoryButton.Location = new Point(230, yPosition + 30);
            removeCategoryButton.Size = new Size(80, 25);
            removeCategoryButton.Click += RemoveCategoryButton_Click;
            tab.Controls.Add(removeCategoryButton);

            newCategoryTextBox = new TextBox();
            newCategoryTextBox.Location = new Point(20, yPosition + 160);
            newCategoryTextBox.Size = new Size(200, 20);
            newCategoryTextBox.PlaceholderText = "Nueva categoría";
            tab.Controls.Add(newCategoryTextBox);

            yPosition += 200;

            var locationsLabel = new Label();
            locationsLabel.Text = "Ubicaciones Personalizadas";
            locationsLabel.Font = new Font("Arial", 12, FontStyle.Bold);
            locationsLabel.Location = new Point(20, yPosition);
            locationsLabel.Size = new Size(200, 20);
            tab.Controls.Add(locationsLabel);

            yPosition += 30;

            customLocationsListBox = new ListBox();
            customLocationsListBox.Location = new Point(20, yPosition);
            customLocationsListBox.Size = new Size(200, 150);
            tab.Controls.Add(customLocationsListBox);

            var addLocationButton = new Button();
            addLocationButton.Text = "Agregar";
            addLocationButton.Location = new Point(230, yPosition);
            addLocationButton.Size = new Size(80, 25);
            addLocationButton.Click += AddLocationButton_Click;
            tab.Controls.Add(addLocationButton);

            var removeLocationButton = new Button();
            removeLocationButton.Text = "Eliminar";
            removeLocationButton.Location = new Point(230, yPosition + 30);
            removeLocationButton.Size = new Size(80, 25);
            removeLocationButton.Click += RemoveLocationButton_Click;
            tab.Controls.Add(removeLocationButton);

            newLocationTextBox = new TextBox();
            newLocationTextBox.Location = new Point(20, yPosition + 160);
            newLocationTextBox.Size = new Size(200, 20);
            newLocationTextBox.PlaceholderText = "Nueva ubicación";
            tab.Controls.Add(newLocationTextBox);
        }

        private void LoadSettings()
        {
            if (companyNameTextBox == null || languageComboBox == null || themeComboBox == null || 
                showWelcomeScreenCheckBox == null || showNotificationsCheckBox == null || 
                gridPageSizeNumericUpDown == null || defaultCategoryComboBox == null || 
                defaultLocationComboBox == null || autoGenerateQRCheckBox == null || 
                lowStockThresholdNumericUpDown == null || enableAuditLogCheckBox == null || 
                autoBackupCheckBox == null || backupIntervalNumericUpDown == null || 
                backupLocationTextBox == null)
            {
                return; 
            }
            
            companyNameTextBox.Text = _settings.CompanyName;
            languageComboBox.SelectedIndex = _settings.Language == "es" ? 0 : 1;
            themeComboBox.SelectedIndex = _settings.Theme == "Light" ? 0 : 1;
            showWelcomeScreenCheckBox.Checked = _settings.ShowWelcomeScreen;
            showNotificationsCheckBox.Checked = _settings.ShowNotifications;
            gridPageSizeNumericUpDown.Value = _settings.GridPageSize;

            LoadCategoriesAndLocations();
            defaultCategoryComboBox.Text = _settings.DefaultCategory;
            defaultLocationComboBox.Text = _settings.DefaultLocation;
            autoGenerateQRCheckBox.Checked = _settings.AutoGenerateQR;
            lowStockThresholdNumericUpDown.Value = _settings.LowStockThreshold;
            enableAuditLogCheckBox.Checked = _settings.EnableAuditLog;

            autoBackupCheckBox.Checked = _settings.AutoBackup;
            backupIntervalNumericUpDown.Value = _settings.BackupIntervalDays;
            backupLocationTextBox.Text = _settings.BackupLocation;

            LoadCustomCategoriesAndLocations();
        }

        private void LoadCategoriesAndLocations()
        {
            if (defaultCategoryComboBox == null || defaultLocationComboBox == null)
            {
                return;
            }
            
            var allCategories = _settingsService.GetAllCategories();
            defaultCategoryComboBox.Items.Clear();
            defaultCategoryComboBox.Items.AddRange(allCategories.ToArray());

            var allLocations = _settingsService.GetAllLocations();
            defaultLocationComboBox.Items.Clear();
            defaultLocationComboBox.Items.AddRange(allLocations.ToArray());
        }

        private void LoadCustomCategoriesAndLocations()
        {
            if (customCategoriesListBox == null || customLocationsListBox == null)
            {
                return; 
            }
            
            customCategoriesListBox.Items.Clear();
            customCategoriesListBox.Items.AddRange(_settings.CustomCategories.ToArray());

            customLocationsListBox.Items.Clear();
            customLocationsListBox.Items.AddRange(_settings.CustomLocations.ToArray());
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            if (companyNameTextBox == null || languageComboBox == null || themeComboBox == null || 
                showWelcomeScreenCheckBox == null || showNotificationsCheckBox == null || 
                gridPageSizeNumericUpDown == null || defaultCategoryComboBox == null || 
                defaultLocationComboBox == null || autoGenerateQRCheckBox == null || 
                lowStockThresholdNumericUpDown == null || enableAuditLogCheckBox == null || 
                autoBackupCheckBox == null || backupIntervalNumericUpDown == null || 
                backupLocationTextBox == null)
            {
                MessageBox.Show("Los controles no están configurados correctamente.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            
            try
            {
                _settings.CompanyName = companyNameTextBox.Text;
                _settings.Language = languageComboBox.SelectedIndex == 0 ? "es" : "en";
                _settings.Theme = themeComboBox.SelectedIndex == 0 ? "Light" : "Dark";
                _settings.ShowWelcomeScreen = showWelcomeScreenCheckBox.Checked;
                _settings.ShowNotifications = showNotificationsCheckBox.Checked;
                _settings.GridPageSize = (int)gridPageSizeNumericUpDown.Value;

                _settings.DefaultCategory = defaultCategoryComboBox.Text;
                _settings.DefaultLocation = defaultLocationComboBox.Text;
                _settings.AutoGenerateQR = autoGenerateQRCheckBox.Checked;
                _settings.LowStockThreshold = (int)lowStockThresholdNumericUpDown.Value;
                _settings.EnableAuditLog = enableAuditLogCheckBox.Checked;

                _settings.AutoBackup = autoBackupCheckBox.Checked;
                _settings.BackupIntervalDays = (int)backupIntervalNumericUpDown.Value;
                _settings.BackupLocation = backupLocationTextBox.Text;

                _settingsService.SaveSettings(_settings);

                MessageBox.Show("Configuración guardada exitosamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error guardando configuración: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void CancelButton_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void ResetButton_Click(object sender, EventArgs e)
        {
            var result = MessageBox.Show("¿Está seguro de que desea restaurar la configuración por defecto?", 
                "Confirmar", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            
            if (result == DialogResult.Yes)
            {
                _settingsService.ResetToDefaults();
                _settings = _settingsService.GetSettings();
                LoadSettings();
            }
        }

        private void BrowseButton_Click(object sender, EventArgs e)
        {
            if (backupLocationTextBox == null)
            {
                MessageBox.Show("El control de ubicación de backup no está configurado correctamente.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            
            var folderDialog = new FolderBrowserDialog();
            if (folderDialog.ShowDialog() == DialogResult.OK)
            {
                backupLocationTextBox.Text = folderDialog.SelectedPath;
            }
        }

        private void CreateBackupButton_Click(object sender, EventArgs e)
        {
            var saveDialog = new SaveFileDialog();
            saveDialog.Filter = "Archivos de Backup (*.json)|*.json";
            saveDialog.FileName = $"backup_{DateTime.Now:yyyyMMdd_HHmmss}.json";

            if (saveDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    var backupService = new BackupService();
                    backupService.CreateBackup(saveDialog.FileName);
                    MessageBox.Show("Backup creado exitosamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error creando backup: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void RestoreBackupButton_Click(object sender, EventArgs e)
        {
            var openDialog = new OpenFileDialog();
            openDialog.Filter = "Archivos de Backup (*.json)|*.json";

            if (openDialog.ShowDialog() == DialogResult.OK)
            {
                var result = MessageBox.Show("¿Está seguro de que desea restaurar este backup? Esta acción reemplazará todos los datos actuales.", 
                    "Confirmar", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                
                if (result == DialogResult.Yes)
                {
                    try
                    {
                        var backupService = new BackupService();
                        backupService.RestoreBackup(openDialog.FileName);
                        MessageBox.Show("Backup restaurado exitosamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error restaurando backup: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void AddCategoryButton_Click(object sender, EventArgs e)
        {
            if (newCategoryTextBox == null)
            {
                MessageBox.Show("El control de nueva categoría no está configurado correctamente.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            
            if (!string.IsNullOrWhiteSpace(newCategoryTextBox.Text))
            {
                _settingsService.AddCustomCategory(newCategoryTextBox.Text);
                LoadCustomCategoriesAndLocations();
                LoadCategoriesAndLocations();
                newCategoryTextBox.Clear();
            }
        }

        private void RemoveCategoryButton_Click(object sender, EventArgs e)
        {
            if (customCategoriesListBox == null)
            {
                MessageBox.Show("El control de categorías personalizadas no está configurado correctamente.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            
            if (customCategoriesListBox.SelectedItem != null)
            {
                _settingsService.RemoveCustomCategory(customCategoriesListBox.SelectedItem.ToString()!);
                LoadCustomCategoriesAndLocations();
                LoadCategoriesAndLocations();
            }
        }

        private void AddLocationButton_Click(object sender, EventArgs e)
        {
            if (newLocationTextBox == null)
            {
                MessageBox.Show("El control de nueva ubicación no está configurado correctamente.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            
            if (!string.IsNullOrWhiteSpace(newLocationTextBox.Text))
            {
                _settingsService.AddCustomLocation(newLocationTextBox.Text);
                LoadCustomCategoriesAndLocations();
                LoadCategoriesAndLocations();
                newLocationTextBox.Clear();
            }
        }

        private void RemoveLocationButton_Click(object sender, EventArgs e)
        {
            if (customLocationsListBox == null)
            {
                MessageBox.Show("El control de ubicaciones personalizadas no está configurado correctamente.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            
            if (customLocationsListBox.SelectedItem != null)
            {
                _settingsService.RemoveCustomLocation(customLocationsListBox.SelectedItem.ToString()!);
                LoadCustomCategoriesAndLocations();
                LoadCategoriesAndLocations();
            }
        }

        private TextBox companyNameTextBox;
        private ComboBox languageComboBox;
        private ComboBox themeComboBox;
        private CheckBox showWelcomeScreenCheckBox;
        private CheckBox showNotificationsCheckBox;
        private NumericUpDown gridPageSizeNumericUpDown;
        private ComboBox defaultCategoryComboBox;
        private ComboBox defaultLocationComboBox;
        private CheckBox autoGenerateQRCheckBox;
        private NumericUpDown lowStockThresholdNumericUpDown;
        private CheckBox enableAuditLogCheckBox;
        private CheckBox autoBackupCheckBox;
        private NumericUpDown backupIntervalNumericUpDown;
        private TextBox backupLocationTextBox;
        private ListBox customCategoriesListBox;
        private ListBox customLocationsListBox;
        private TextBox newCategoryTextBox;
        private TextBox newLocationTextBox;
    }
}
