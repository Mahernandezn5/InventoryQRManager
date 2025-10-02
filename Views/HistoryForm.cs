using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using InventoryQRManager.Models;
using InventoryQRManager.Services;

namespace InventoryQRManager.Views
{
    public partial class HistoryForm : Form
    {
        private readonly HistoryService _historyService;
        private DataGridView _historyDataGrid;
        private ComboBox _filterComboBox;
        private DateTimePicker _startDatePicker;
        private DateTimePicker _endDatePicker;
        private Button _filterButton;
        private Button _clearButton;
        private Button _exportButton;
        private Button _closeButton;
        private Label _statsLabel;

        public HistoryForm()
        {
            _historyService = new HistoryService();
            InitializeComponent();
            LoadHistoryData();
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();

            this.Text = "Historial de Movimientos";
            this.Size = new Size(1200, 700);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.BackColor = Color.FromArgb(236, 240, 241);

            var filterPanel = new Panel
            {
                Size = new Size(1160, 60),
                Location = new Point(20, 20),
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };

            var actionLabel = new Label
            {
                Text = "Acción:",
                Location = new Point(20, 20),
                Size = new Size(50, 20),
                Font = new Font("Segoe UI", 9F, FontStyle.Bold)
            };

            _filterComboBox = new ComboBox
            {
                Location = new Point(80, 18),
                Size = new Size(120, 25),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            _filterComboBox.Items.AddRange(new string[] { "Todas", "CREATE", "UPDATE", "DELETE", "QUANTITY_ADD", "QUANTITY_REMOVE", "PRICE_CHANGE", "LOCATION_CHANGE", "CATEGORY_CHANGE" });
            _filterComboBox.SelectedIndex = 0;

            var dateLabel = new Label
            {
                Text = "Desde:",
                Location = new Point(220, 20),
                Size = new Size(40, 20),
                Font = new Font("Segoe UI", 9F, FontStyle.Bold)
            };

            _startDatePicker = new DateTimePicker
            {
                Location = new Point(270, 18),
                Size = new Size(120, 25),
                Format = DateTimePickerFormat.Short
            };

            var toLabel = new Label
            {
                Text = "Hasta:",
                Location = new Point(410, 20),
                Size = new Size(40, 20),
                Font = new Font("Segoe UI", 9F, FontStyle.Bold)
            };

            _endDatePicker = new DateTimePicker
            {
                Location = new Point(460, 18),
                Size = new Size(120, 25),
                Format = DateTimePickerFormat.Short,
                Value = DateTime.Now
            };

            _filterButton = new Button
            {
                Text = "🔍 Filtrar",
                Location = new Point(600, 16),
                Size = new Size(80, 30),
                BackColor = Color.FromArgb(52, 152, 219),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat
            };
            _filterButton.Click += FilterButton_Click;

            _clearButton = new Button
            {
                Text = "🗑️ Limpiar",
                Location = new Point(690, 16),
                Size = new Size(80, 30),
                BackColor = Color.FromArgb(149, 165, 166),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat
            };
            _clearButton.Click += ClearButton_Click;

            filterPanel.Controls.AddRange(new Control[] {
                actionLabel, _filterComboBox, dateLabel, _startDatePicker, toLabel, _endDatePicker,
                _filterButton, _clearButton
            });

            _historyDataGrid = new DataGridView
            {
                Location = new Point(20, 100),
                Size = new Size(1160, 450),
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                Font = new Font("Segoe UI", 9F)
            };

            _historyDataGrid.Columns.AddRange(new DataGridViewColumn[] {
                new DataGridViewTextBoxColumn { Name = "Timestamp", HeaderText = "Fecha/Hora", Width = 120 },
                new DataGridViewTextBoxColumn { Name = "Action", HeaderText = "Acción", Width = 100 },
                new DataGridViewTextBoxColumn { Name = "ItemName", HeaderText = "Item", Width = 150 },
                new DataGridViewTextBoxColumn { Name = "ItemSKU", HeaderText = "SKU", Width = 100 },
                new DataGridViewTextBoxColumn { Name = "UserName", HeaderText = "Usuario", Width = 100 },
                new DataGridViewTextBoxColumn { Name = "Details", HeaderText = "Detalles", Width = 200 },
                new DataGridViewTextBoxColumn { Name = "OldValue", HeaderText = "Valor Anterior", Width = 120 },
                new DataGridViewTextBoxColumn { Name = "NewValue", HeaderText = "Valor Nuevo", Width = 120 },
                new DataGridViewTextBoxColumn { Name = "QuantityChange", HeaderText = "Cambio Cantidad", Width = 100 },
                new DataGridViewTextBoxColumn { Name = "PriceChange", HeaderText = "Cambio Precio", Width = 100 },
                new DataGridViewTextBoxColumn { Name = "Category", HeaderText = "Categoría", Width = 100 },
                new DataGridViewTextBoxColumn { Name = "Location", HeaderText = "Ubicación", Width = 100 }
            });

            var buttonPanel = new Panel
            {
                Size = new Size(1160, 50),
                Location = new Point(20, 570),
                BackColor = Color.Transparent
            };

            _exportButton = new Button
            {
                Text = "📊 Exportar",
                Location = new Point(20, 10),
                Size = new Size(100, 35),
                BackColor = Color.FromArgb(46, 204, 113),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat
            };
            _exportButton.Click += ExportButton_Click;

            _closeButton = new Button
            {
                Text = "❌ Cerrar",
                Location = new Point(130, 10),
                Size = new Size(100, 35),
                BackColor = Color.FromArgb(231, 76, 60),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat
            };
            _closeButton.Click += CloseButton_Click;

            _statsLabel = new Label
            {
                Location = new Point(250, 15),
                Size = new Size(500, 25),
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                ForeColor = Color.FromArgb(52, 73, 94)
            };

            buttonPanel.Controls.AddRange(new Control[] { _exportButton, _closeButton, _statsLabel });

            this.Controls.AddRange(new Control[] { filterPanel, _historyDataGrid, buttonPanel });

            this.ResumeLayout(false);
        }

        private void LoadHistoryData()
        {
            try
            {
                var history = _historyService.GetAllHistory();
                _historyDataGrid.Rows.Clear();

                foreach (var record in history)
                {
                    _historyDataGrid.Rows.Add(
                        record.Timestamp.ToString("dd/MM/yyyy HH:mm"),
                        GetActionDisplayName(record.Action),
                        record.ItemName,
                        record.ItemSKU,
                        record.UserName,
                        record.Details,
                        record.OldValue,
                        record.NewValue,
                        record.QuantityChange != 0 ? record.QuantityChange.ToString() : "",
                        record.PriceChange != 0 ? record.PriceChange.ToString("C") : "",
                        record.Category,
                        record.Location
                    );
                }

                UpdateStats();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error cargando historial: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void FilterButton_Click(object sender, EventArgs e)
        {
            try
            {
                var startDate = _startDatePicker.Value.Date;
                var endDate = _endDatePicker.Value.Date.AddDays(1).AddSeconds(-1);
                var selectedAction = _filterComboBox.SelectedItem.ToString();

                var history = _historyService.GetHistoryByDateRange(startDate, endDate);

                if (selectedAction != "Todas")
                {
                    history = history.Where(h => h.Action.ToString() == selectedAction).ToList();
                }

                _historyDataGrid.Rows.Clear();

                foreach (var record in history)
                {
                    _historyDataGrid.Rows.Add(
                        record.Timestamp.ToString("dd/MM/yyyy HH:mm"),
                        GetActionDisplayName(record.Action),
                        record.ItemName,
                        record.ItemSKU,
                        record.UserName,
                        record.Details,
                        record.OldValue,
                        record.NewValue,
                        record.QuantityChange != 0 ? record.QuantityChange.ToString() : "",
                        record.PriceChange != 0 ? record.PriceChange.ToString("C") : "",
                        record.Category,
                        record.Location
                    );
                }

                UpdateStats();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error filtrando historial: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ClearButton_Click(object sender, EventArgs e)
        {
            _filterComboBox.SelectedIndex = 0;
            _startDatePicker.Value = DateTime.Now.AddMonths(-1);
            _endDatePicker.Value = DateTime.Now;
            LoadHistoryData();
        }

        private void ExportButton_Click(object sender, EventArgs e)
        {
            try
            {
                var saveDialog = new SaveFileDialog
                {
                    Filter = "Archivos CSV (*.csv)|*.csv|Archivos de texto (*.txt)|*.txt",
                    Title = "Exportar historial"
                };

                if (saveDialog.ShowDialog() == DialogResult.OK)
                {
                    var lines = new List<string>();
                    lines.Add("Fecha/Hora,Acción,Item,SKU,Usuario,Detalles,Valor Anterior,Valor Nuevo,Cambio Cantidad,Cambio Precio,Categoría,Ubicación");

                    foreach (DataGridViewRow row in _historyDataGrid.Rows)
                    {
                        if (!row.IsNewRow)
                        {
                            var line = string.Join(",", row.Cells.Cast<DataGridViewCell>().Select(cell => $"\"{cell.Value}\""));
                            lines.Add(line);
                        }
                    }

                    System.IO.File.WriteAllLines(saveDialog.FileName, lines);
                    MessageBox.Show("Historial exportado exitosamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error exportando historial: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void CloseButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void UpdateStats()
        {
            try
            {
                var stats = _historyService.GetHistoryStats();
                var totalRecords = _historyDataGrid.Rows.Count;
                var statsText = $"Total de registros: {totalRecords}";

                if (stats.Any())
                {
                    statsText += " | ";
                    statsText += string.Join(", ", stats.Select(s => $"{s.Key}: {s.Value}"));
                }

                _statsLabel.Text = statsText;
            }
            catch (Exception ex)
            {
                _statsLabel.Text = "Error cargando estadísticas";
                System.Diagnostics.Debug.WriteLine($"Error actualizando estadísticas: {ex.Message}");
            }
        }

        private string GetActionDisplayName(HistoryAction action)
        {
            return action switch
            {
                HistoryAction.CREATE => "Crear",
                HistoryAction.UPDATE => "Actualizar",
                HistoryAction.DELETE => "Eliminar",
                HistoryAction.QUANTITY_ADD => "Aumentar Stock",
                HistoryAction.QUANTITY_REMOVE => "Disminuir Stock",
                HistoryAction.PRICE_CHANGE => "Cambiar Precio",
                HistoryAction.LOCATION_CHANGE => "Cambiar Ubicación",
                HistoryAction.CATEGORY_CHANGE => "Cambiar Categoría",
                _ => action.ToString()
            };
        }
    }
}
