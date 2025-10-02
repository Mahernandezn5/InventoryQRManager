using System;
using System.Drawing;
using System.Windows.Forms;
using InventoryQRManager.Models;
using InventoryQRManager.Services;

namespace InventoryQRManager.Views
{
    public partial class AddEditItemForm : Form
    {
        private readonly InventoryService _inventoryService;
        private readonly QRCodeService _qrCodeService;
        private readonly HistoryService _historyService;
        private readonly InventoryItem? _item;
        private readonly bool _isEdit;

        public AddEditItemForm(InventoryItem? item = null)
        {
            _inventoryService = new InventoryService();
            _qrCodeService = new QRCodeService();
            _historyService = new HistoryService();
            _item = item;
            _isEdit = item != null;
            
            InitializeComponent();
            
           
            ApplyProfessionalTheme();
            
            LoadData();
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            
            
            this.Text = _isEdit ? "Editar Item - Inventory QR Manager" : "Nuevo Item - Inventory QR Manager";
            this.Size = new Size(550, 650);
            this.StartPosition = FormStartPosition.CenterParent;
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
            int yPosition = 20;
            int labelWidth = 100;
            int controlWidth = 300;
            int spacing = 30;

            
            var nameLabel = new Label();
            nameLabel.Text = "Nombre:";
            nameLabel.Location = new Point(20, yPosition);
            nameLabel.Size = new Size(labelWidth, 20);
            this.Controls.Add(nameLabel);

            nameTextBox = new TextBox();
            nameTextBox.Location = new Point(130, yPosition);
            nameTextBox.Size = new Size(controlWidth, 20);
            this.Controls.Add(nameTextBox);

            yPosition += spacing;

           
            var descriptionLabel = new Label();
            descriptionLabel.Text = "Descripción:";
            descriptionLabel.Location = new Point(20, yPosition);
            descriptionLabel.Size = new Size(labelWidth, 20);
            this.Controls.Add(descriptionLabel);

            descriptionTextBox = new TextBox();
            descriptionTextBox.Location = new Point(130, yPosition);
            descriptionTextBox.Size = new Size(controlWidth, 20);
            this.Controls.Add(descriptionTextBox);

            yPosition += spacing;

            
            var skuLabel = new Label();
            skuLabel.Text = "SKU:";
            skuLabel.Location = new Point(20, yPosition);
            skuLabel.Size = new Size(labelWidth, 20);
            this.Controls.Add(skuLabel);

            skuTextBox = new TextBox();
            skuTextBox.Location = new Point(130, yPosition);
            skuTextBox.Size = new Size(controlWidth, 20);
            this.Controls.Add(skuTextBox);

            yPosition += spacing;

            
            var quantityLabel = new Label();
            quantityLabel.Text = "Cantidad:";
            quantityLabel.Location = new Point(20, yPosition);
            quantityLabel.Size = new Size(labelWidth, 20);
            this.Controls.Add(quantityLabel);

            quantityNumericUpDown = new NumericUpDown();
            quantityNumericUpDown.Location = new Point(130, yPosition);
            quantityNumericUpDown.Size = new Size(100, 20);
            quantityNumericUpDown.Minimum = 0;
            quantityNumericUpDown.Maximum = 999999;
            this.Controls.Add(quantityNumericUpDown);

            yPosition += spacing;

            
            var priceLabel = new Label();
            priceLabel.Text = "Precio:";
            priceLabel.Location = new Point(20, yPosition);
            priceLabel.Size = new Size(labelWidth, 20);
            this.Controls.Add(priceLabel);

            priceNumericUpDown = new NumericUpDown();
            priceNumericUpDown.Location = new Point(130, yPosition);
            priceNumericUpDown.Size = new Size(100, 20);
            priceNumericUpDown.DecimalPlaces = 2;
            priceNumericUpDown.Minimum = 0;
            priceNumericUpDown.Maximum = 999999.99m;
            this.Controls.Add(priceNumericUpDown);

            yPosition += spacing;

            
            var categoryLabel = new Label();
            categoryLabel.Text = "Categoría:";
            categoryLabel.Location = new Point(20, yPosition);
            categoryLabel.Size = new Size(labelWidth, 20);
            this.Controls.Add(categoryLabel);

            categoryComboBox = new ComboBox();
            categoryComboBox.Location = new Point(130, yPosition);
            categoryComboBox.Size = new Size(controlWidth, 20);
            categoryComboBox.DropDownStyle = ComboBoxStyle.DropDown;
            this.Controls.Add(categoryComboBox);

            yPosition += spacing;

            
            var locationLabel = new Label();
            locationLabel.Text = "Ubicación:";
            locationLabel.Location = new Point(20, yPosition);
            locationLabel.Size = new Size(labelWidth, 20);
            this.Controls.Add(locationLabel);

            locationComboBox = new ComboBox();
            locationComboBox.Location = new Point(130, yPosition);
            locationComboBox.Size = new Size(controlWidth, 20);
            locationComboBox.DropDownStyle = ComboBoxStyle.DropDown;
            this.Controls.Add(locationComboBox);

            yPosition += spacing;

            
            var qrLabel = new Label();
            qrLabel.Text = "Código QR:";
            qrLabel.Location = new Point(20, yPosition);
            qrLabel.Size = new Size(labelWidth, 20);
            this.Controls.Add(qrLabel);

            qrTextBox = new TextBox();
            qrTextBox.Location = new Point(130, yPosition);
            qrTextBox.Size = new Size(200, 20);
            qrTextBox.ReadOnly = true;
            this.Controls.Add(qrTextBox);

            var generateQRButton = new Button();
            generateQRButton.Text = "Generar";
            generateQRButton.Location = new Point(340, yPosition);
            generateQRButton.Size = new Size(80, 25);
            generateQRButton.Click += GenerateQRButton_Click;
            this.Controls.Add(generateQRButton);

            yPosition += spacing + 10;

            
            var saveButton = new Button();
            saveButton.Text = "Guardar";
            saveButton.Location = new Point(200, yPosition);
            saveButton.Size = new Size(80, 30);
            saveButton.Click += SaveButton_Click;
            this.Controls.Add(saveButton);

            var cancelButton = new Button();
            cancelButton.Text = "Cancelar";
            cancelButton.Location = new Point(290, yPosition);
            cancelButton.Size = new Size(80, 30);
            cancelButton.Click += CancelButton_Click;
            this.Controls.Add(cancelButton);
        }

        private void LoadData()
        {
            if (categoryComboBox == null || locationComboBox == null || nameTextBox == null || 
                descriptionTextBox == null || skuTextBox == null || quantityNumericUpDown == null || 
                priceNumericUpDown == null || qrTextBox == null)
            {
                return; 
            }
            
            
            var categories = _inventoryService.GetCategories();
            categoryComboBox.Items.Clear();
            categoryComboBox.Items.AddRange(categories.ToArray());

            
            var locations = _inventoryService.GetLocations();
            locationComboBox.Items.Clear();
            locationComboBox.Items.AddRange(locations.ToArray());

            
            if (_isEdit && _item != null)
            {
                nameTextBox.Text = _item.Name;
                descriptionTextBox.Text = _item.Description;
                skuTextBox.Text = _item.SKU;
                quantityNumericUpDown.Value = _item.Quantity;
                priceNumericUpDown.Value = _item.Price;
                categoryComboBox.Text = _item.Category;
                locationComboBox.Text = _item.Location;
                qrTextBox.Text = _item.QRCode;
            }
            else
            {
                
                GenerateQRCode();
            }
        }

        private void GenerateQRButton_Click(object sender, EventArgs e)
        {
            GenerateQRCode();
        }

        private void GenerateQRCode()
        {
            if (skuTextBox == null || nameTextBox == null || qrTextBox == null)
            {
                return; 
            }
            
            try
            {
                var sku = skuTextBox.Text.Trim();
                var name = nameTextBox.Text.Trim();

                if (string.IsNullOrEmpty(sku) || string.IsNullOrEmpty(name))
                {
                    MessageBox.Show("Por favor ingrese SKU y Nombre para generar el código QR.", "Información", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                var qrCode = _qrCodeService.GenerateUniqueQRCode(sku, name);
                qrTextBox.Text = qrCode;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error generando código QR: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            try
            {
                if (ValidateInput())
                {
                    var item = _isEdit ? _item! : new InventoryItem();
                    
                    item.Name = nameTextBox.Text.Trim();
                    item.Description = descriptionTextBox.Text.Trim();
                    item.SKU = skuTextBox.Text.Trim();
                    item.Quantity = (int)quantityNumericUpDown.Value;
                    item.Price = priceNumericUpDown.Value;
                    item.Category = categoryComboBox.Text.Trim();
                    item.Location = locationComboBox.Text.Trim();
                    item.QRCode = qrTextBox.Text.Trim();

                    bool success;
                    if (_isEdit)
                    {
                       
                        success = _inventoryService.UpdateItem(item);
                        if (success)
                        {
                            _historyService.RecordAction(item, HistoryAction.UPDATE, "Actualización manual");
                        }
                    }
                    else
                    {
                        
                        success = _inventoryService.AddItem(item);
                        if (success)
                        {
                            _historyService.RecordAction(item, HistoryAction.CREATE, "Item creado desde interfaz");
                        }
                    }

                    if (success)
                    {
                        MessageBox.Show(_isEdit ? "Item actualizado correctamente." : "Item agregado correctamente.", 
                            "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        
                        
                        if (!_isEdit)
                        {
                            ClearFields();
                            GenerateQRCode();
                        }
                        else
                        {
                            this.DialogResult = DialogResult.OK;
                            this.Close();
                        }
                    }
                    else
                    {
                        MessageBox.Show("Error al guardar el item.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al guardar: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void CancelButton_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private bool ValidateInput()
        {
            if (nameTextBox == null || skuTextBox == null || qrTextBox == null)
            {
                MessageBox.Show("Los controles no están configurados correctamente.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            
            if (string.IsNullOrWhiteSpace(nameTextBox.Text))
            {
                MessageBox.Show("El nombre es requerido.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                nameTextBox.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(skuTextBox.Text))
            {
                MessageBox.Show("El SKU es requerido.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                skuTextBox.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(qrTextBox.Text))
            {
                MessageBox.Show("El código QR es requerido.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            
            if (!_isEdit)
            {
                var existingItem = _inventoryService.GetItemBySKU(skuTextBox.Text.Trim());
                if (existingItem != null)
                {
                    MessageBox.Show("Ya existe un item con este SKU.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    skuTextBox.Focus();
                    return false;
                }
            }

            return true;
        }

        private void ApplyProfessionalTheme()
        {
            
            ThemeService.ApplyTheme(this);
            
            
            ApplyButtonStyles();
        }

        private void ApplyButtonStyles()
        {
            var theme = ThemeService.CurrentTheme;
            
            if (this.Controls == null)
            {
                return; 
            }
            
            
            foreach (Control control in this.Controls)
            {
                if (control is Button button)
                {
                    button.FlatStyle = FlatStyle.Flat;
                    button.Font = new Font("Segoe UI", 9F, FontStyle.Regular);
                    
                    if (button.Text.Contains("Guardar"))
                    {
                        button.BackColor = theme.SuccessColor;
                        button.ForeColor = Color.White;
                        button.FlatAppearance.BorderColor = theme.SuccessColor;
                        button.FlatAppearance.MouseOverBackColor = Color.FromArgb(39, 174, 96);
                    }
                    else if (button.Text.Contains("Cancelar"))
                    {
                        button.BackColor = theme.DangerColor;
                        button.ForeColor = Color.White;
                        button.FlatAppearance.BorderColor = theme.DangerColor;
                        button.FlatAppearance.MouseOverBackColor = Color.FromArgb(192, 57, 43);
                    }
                    else if (button.Text.Contains("Generar"))
                    {
                        button.BackColor = theme.PrimaryColor;
                        button.ForeColor = Color.White;
                        button.FlatAppearance.BorderColor = theme.PrimaryColor;
                        button.FlatAppearance.MouseOverBackColor = Color.FromArgb(52, 152, 219);
                    }
                }
            }
        }

        private void ClearFields()
        {
            if (nameTextBox == null || descriptionTextBox == null || skuTextBox == null || 
                quantityNumericUpDown == null || priceNumericUpDown == null || 
                categoryComboBox == null || locationComboBox == null || qrTextBox == null)
            {
                return; 
            }
            
            nameTextBox.Clear();
            descriptionTextBox.Clear();
            skuTextBox.Clear();
            quantityNumericUpDown.Value = 0;
            priceNumericUpDown.Value = 0;
            categoryComboBox.SelectedIndex = -1;
            locationComboBox.SelectedIndex = -1;
            qrTextBox.Clear();
        }

        
        private TextBox nameTextBox;
        private TextBox descriptionTextBox;
        private TextBox skuTextBox;
        private NumericUpDown quantityNumericUpDown;
        private NumericUpDown priceNumericUpDown;
        private ComboBox categoryComboBox;
        private ComboBox locationComboBox;
        private TextBox qrTextBox;
    }
}

