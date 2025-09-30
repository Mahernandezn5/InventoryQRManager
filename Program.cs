using System;
using System.Windows.Forms;
using InventoryQRManager.Views;
using InventoryQRManager.Services;

namespace InventoryQRManager
{
    internal static class Program
    {
        /// <summary>
        /// Punto de entrada principal para la aplicación.
        /// </summary>
        [STAThread]
        static void Main()
        {
            try
            {
                // Configurar manejo global de excepciones
                Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
                Application.ThreadException += Application_ThreadException;
                AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                
                // Verificar si debe mostrar pantalla de bienvenida
                var settingsService = new SettingsService();
                var showWelcome = settingsService.GetSetting<bool>("ShowWelcomeScreen");
                
                if (showWelcome)
                {
                    var welcomeForm = new WelcomeForm();
                    var result = welcomeForm.ShowDialog();
                    
                    if (result == DialogResult.Cancel)
                    {
                        return; // Salir de la aplicación
                    }
                    
                    // Procesar acción específica si se seleccionó una
                    var mainForm = new MainForm();
                    if (welcomeForm.Tag != null)
                    {
                        var action = welcomeForm.Tag.ToString();
                        switch (action)
                        {
                            case "NewItem":
                                mainForm.Show();
                                mainForm.ShowAddItemForm();
                                break;
                            case "Search":
                                mainForm.Show();
                                mainForm.ShowAdvancedSearch();
                                break;
                            case "Reports":
                                mainForm.Show();
                                mainForm.ShowReports();
                                break;
                            case "Settings":
                                mainForm.Show();
                                mainForm.ShowSettings();
                                break;
                            default:
                                Application.Run(mainForm);
                                break;
                        }
                    }
                    else
                    {
                        Application.Run(mainForm);
                    }
                }
                else
                {
                    Application.Run(new MainForm());
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error crítico en la aplicación: {ex.Message}", "Error Crítico", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private static void Application_ThreadException(object sender, System.Threading.ThreadExceptionEventArgs e)
        {
            MessageBox.Show($"Error no manejado: {e.Exception.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            MessageBox.Show($"Error crítico no manejado: {e.ExceptionObject}", "Error Crítico", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}

