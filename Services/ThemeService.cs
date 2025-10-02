using System;
using System.Drawing;
using System.Collections.Generic;

namespace InventoryQRManager.Services
{
    public class ThemeService
    {
        public class ProfessionalTheme
        {
           
            public Color PrimaryColor { get; set; } = Color.FromArgb(41, 128, 185);      // Azul profesional
            public Color SecondaryColor { get; set; } = Color.FromArgb(52, 73, 94);     // Gris oscuro elegante
            public Color AccentColor { get; set; } = Color.FromArgb(46, 204, 113);      // Verde éxito
            public Color WarningColor { get; set; } = Color.FromArgb(230, 126, 34);     // Naranja advertencia
            public Color DangerColor { get; set; } = Color.FromArgb(231, 76, 60);       // Rojo peligro
            
            
            public Color BackgroundColor { get; set; } = Color.FromArgb(248, 249, 250); // Gris muy claro
            public Color PanelBackgroundColor { get; set; } = Color.FromArgb(255, 255, 255); // Blanco
            public Color HeaderBackgroundColor { get; set; } = Color.FromArgb(52, 73, 94); // Gris oscuro
            public Color MenuBackgroundColor { get; set; } = Color.FromArgb(236, 240, 241); // Gris claro
            
            
            public Color TextColor { get; set; } = Color.FromArgb(44, 62, 80);          // Azul oscuro
            public Color HeaderTextColor { get; set; } = Color.FromArgb(255, 255, 255); // Blanco
            public Color SecondaryTextColor { get; set; } = Color.FromArgb(127, 140, 141); // Gris medio
            public Color LinkColor { get; set; } = Color.FromArgb(41, 128, 185);        // Azul profesional
            
            
            public Color BorderColor { get; set; } = Color.FromArgb(189, 195, 199);     // Gris claro
            public Color SeparatorColor { get; set; } = Color.FromArgb(149, 165, 166);  // Gris medio
            
           
            public Color SuccessColor { get; set; } = Color.FromArgb(46, 204, 113);     // Verde
            public Color InfoColor { get; set; } = Color.FromArgb(52, 152, 219);        // Azul claro
            public Color LightColor { get; set; } = Color.FromArgb(236, 240, 241);      // Gris muy claro
            public Color DarkColor { get; set; } = Color.FromArgb(44, 62, 80);          // Azul oscuro
            
            
            public Color HoverColor { get; set; } = Color.FromArgb(189, 195, 199);      // Gris claro
            public Color SelectedColor { get; set; } = Color.FromArgb(41, 128, 185);    // Azul profesional
            public Color FocusColor { get; set; } = Color.FromArgb(52, 152, 219);       // Azul claro
        }

        private static ProfessionalTheme _currentTheme;
        
        public static ProfessionalTheme CurrentTheme
        {
            get
            {
                if (_currentTheme == null)
                {
                    _currentTheme = new ProfessionalTheme();
                }
                return _currentTheme;
            }
        }

        public static void ApplyTheme(Form form)
        {
            var theme = CurrentTheme;
            
            
            form.BackColor = theme.BackgroundColor;
            form.ForeColor = theme.TextColor;
            
            
            ApplyThemeToControls(form.Controls, theme);
        }

        public static void ApplyThemeToControls(Control.ControlCollection controls, ProfessionalTheme theme)
        {
            foreach (Control control in controls)
            {
                ApplyThemeToControl(control, theme);
                
                
                if (control.HasChildren)
                {
                    ApplyThemeToControls(control.Controls, theme);
                }
            }
        }

        public static void ApplyThemeToControl(Control control, ProfessionalTheme theme)
        {
            switch (control)
            {
                case Panel panel:
                    if (panel.Name.Contains("header") || panel.Name.Contains("Header"))
                    {
                        panel.BackColor = theme.HeaderBackgroundColor;
                        panel.ForeColor = theme.HeaderTextColor;
                    }
                    else if (panel.Name.Contains("menu") || panel.Name.Contains("Menu"))
                    {
                        panel.BackColor = theme.MenuBackgroundColor;
                        panel.ForeColor = theme.TextColor;
                    }
                    else
                    {
                        panel.BackColor = theme.PanelBackgroundColor;
                        panel.ForeColor = theme.TextColor;
                    }
                    panel.BorderStyle = BorderStyle.FixedSingle;
                    break;
                    
                case Label label:
                    if (label.Name.Contains("title") || label.Name.Contains("Title"))
                    {
                        label.ForeColor = theme.PrimaryColor;
                        label.Font = new Font(label.Font.FontFamily, label.Font.Size, FontStyle.Bold);
                    }
                    else if (label.Name.Contains("header") || label.Name.Contains("Header"))
                    {
                        label.ForeColor = theme.HeaderTextColor;
                        label.Font = new Font(label.Font.FontFamily, label.Font.Size, FontStyle.Bold);
                    }
                    else
                    {
                        label.ForeColor = theme.TextColor;
                    }
                    break;
                    
                case Button button:
                    ApplyButtonTheme(button, theme);
                    break;
                    
                case TextBox textBox:
                    textBox.BackColor = theme.PanelBackgroundColor;
                    textBox.ForeColor = theme.TextColor;
                    textBox.BorderStyle = BorderStyle.FixedSingle;
                    break;
                    
                case ComboBox comboBox:
                    comboBox.BackColor = theme.PanelBackgroundColor;
                    comboBox.ForeColor = theme.TextColor;
                    comboBox.FlatStyle = FlatStyle.Flat;
                    break;
                    
                case DataGridView dataGrid:
                    ApplyDataGridViewTheme(dataGrid, theme);
                    break;
                    
                case MenuStrip menuStrip:
                    ApplyMenuStripTheme(menuStrip, theme);
                    break;
                    
                case ToolStrip toolStrip:
                    ApplyToolStripTheme(toolStrip, theme);
                    break;
                    
                case TabControl tabControl:
                    tabControl.BackColor = theme.PanelBackgroundColor;
                    tabControl.ForeColor = theme.TextColor;
                    break;
            }
        }

        private static void ApplyButtonTheme(Button button, ProfessionalTheme theme)
        {
            button.FlatStyle = FlatStyle.Flat;
            button.FlatAppearance.BorderSize = 1;
            button.FlatAppearance.BorderColor = theme.BorderColor;
            button.Font = new Font(button.Font.FontFamily, button.Font.Size, FontStyle.Regular);
            
            
            if (button.Text.Contains("Guardar") || button.Text.Contains("Crear") || button.Text.Contains("Agregar"))
            {
                button.BackColor = theme.SuccessColor;
                button.ForeColor = Color.White;
                button.FlatAppearance.BorderColor = theme.SuccessColor;
            }
            else if (button.Text.Contains("Eliminar") || button.Text.Contains("Borrar") || button.Text.Contains("Cancelar"))
            {
                button.BackColor = theme.DangerColor;
                button.ForeColor = Color.White;
                button.FlatAppearance.BorderColor = theme.DangerColor;
            }
            else if (button.Text.Contains("Editar") || button.Text.Contains("Modificar") || button.Text.Contains("Actualizar"))
            {
                button.BackColor = theme.WarningColor;
                button.ForeColor = Color.White;
                button.FlatAppearance.BorderColor = theme.WarningColor;
            }
            else if (button.Text.Contains("Buscar") || button.Text.Contains("Generar") || button.Text.Contains("Exportar"))
            {
                button.BackColor = theme.PrimaryColor;
                button.ForeColor = Color.White;
                button.FlatAppearance.BorderColor = theme.PrimaryColor;
            }
            else
            {
                button.BackColor = theme.PanelBackgroundColor;
                button.ForeColor = theme.TextColor;
                button.FlatAppearance.BorderColor = theme.BorderColor;
            }
            
            
            button.FlatAppearance.MouseOverBackColor = theme.HoverColor;
            button.FlatAppearance.MouseDownBackColor = theme.SelectedColor;
        }

        private static void ApplyDataGridViewTheme(DataGridView dataGrid, ProfessionalTheme theme)
        {
            dataGrid.BackgroundColor = theme.PanelBackgroundColor;
            dataGrid.ForeColor = theme.TextColor;
            dataGrid.BorderStyle = BorderStyle.FixedSingle;
            dataGrid.GridColor = theme.BorderColor;
            dataGrid.DefaultCellStyle.BackColor = theme.PanelBackgroundColor;
            dataGrid.DefaultCellStyle.ForeColor = theme.TextColor;
            dataGrid.DefaultCellStyle.SelectionBackColor = theme.SelectedColor;
            dataGrid.DefaultCellStyle.SelectionForeColor = Color.White;
            dataGrid.ColumnHeadersDefaultCellStyle.BackColor = theme.HeaderBackgroundColor;
            dataGrid.ColumnHeadersDefaultCellStyle.ForeColor = theme.HeaderTextColor;
            dataGrid.ColumnHeadersDefaultCellStyle.Font = new Font(dataGrid.Font.FontFamily, dataGrid.Font.Size, FontStyle.Bold);
            dataGrid.EnableHeadersVisualStyles = false;
            dataGrid.RowHeadersDefaultCellStyle.BackColor = theme.HeaderBackgroundColor;
            dataGrid.RowHeadersDefaultCellStyle.ForeColor = theme.HeaderTextColor;
        }

        private static void ApplyMenuStripTheme(MenuStrip menuStrip, ProfessionalTheme theme)
        {
            menuStrip.BackColor = theme.HeaderBackgroundColor;
            menuStrip.ForeColor = theme.HeaderTextColor;
            menuStrip.Renderer = new ProfessionalMenuRenderer(theme);
        }

        private static void ApplyToolStripTheme(ToolStrip toolStrip, ProfessionalTheme theme)
        {
            toolStrip.BackColor = theme.MenuBackgroundColor;
            toolStrip.ForeColor = theme.TextColor;
            toolStrip.Renderer = new ProfessionalToolStripRenderer(theme);
        }

        public static void CreateProfessionalGradient(Control control, Color startColor, Color endColor)
        {
            control.Paint += (sender, e) =>
            {
                var rect = new Rectangle(0, 0, control.Width, control.Height);
                using (var brush = new System.Drawing.Drawing2D.LinearGradientBrush(rect, startColor, endColor, 90f))
                {
                    e.Graphics.FillRectangle(brush, rect);
                }
            };
        }

        public static void AddShadowEffect(Control control)
        {
            control.Paint += (sender, e) =>
            {
                var rect = new Rectangle(0, 0, control.Width - 1, control.Height - 1);
                using (var pen = new Pen(Color.FromArgb(100, 0, 0, 0), 1))
                {
                    e.Graphics.DrawRectangle(pen, rect);
                }
            };
        }
    }

    
    public class ProfessionalMenuRenderer : ToolStripProfessionalRenderer
    {
        private readonly ThemeService.ProfessionalTheme _theme;

        public ProfessionalMenuRenderer(ThemeService.ProfessionalTheme theme)
        {
            _theme = theme;
        }

        protected override void OnRenderMenuItemBackground(ToolStripItemRenderEventArgs e)
        {
            if (e.Item.Selected)
            {
                using (var brush = new SolidBrush(_theme.HoverColor))
                {
                    e.Graphics.FillRectangle(brush, e.Item.ContentRectangle);
                }
            }
            else
            {
                base.OnRenderMenuItemBackground(e);
            }
        }

        protected override void OnRenderToolStripBackground(ToolStripRenderEventArgs e)
        {
            using (var brush = new SolidBrush(_theme.HeaderBackgroundColor))
            {
                e.Graphics.FillRectangle(brush, e.AffectedBounds);
            }
        }
    }

    
    public class ProfessionalToolStripRenderer : ToolStripProfessionalRenderer
    {
        private readonly ThemeService.ProfessionalTheme _theme;

        public ProfessionalToolStripRenderer(ThemeService.ProfessionalTheme theme)
        {
            _theme = theme;
        }

        protected override void OnRenderButtonBackground(ToolStripItemRenderEventArgs e)
        {
            if (e.Item.Selected || e.Item.Pressed)
            {
                using (var brush = new SolidBrush(_theme.HoverColor))
                {
                    e.Graphics.FillRectangle(brush, e.Item.ContentRectangle);
                }
            }
            else
            {
                base.OnRenderButtonBackground(e);
            }
        }

        protected override void OnRenderToolStripBackground(ToolStripRenderEventArgs e)
        {
            using (var brush = new SolidBrush(_theme.MenuBackgroundColor))
            {
                e.Graphics.FillRectangle(brush, e.AffectedBounds);
            }
        }
    }
}
