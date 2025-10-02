using System;
using System.Windows.Forms;
using InventoryQRManager.Views;
using InventoryQRManager.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace InventoryQRManager
{
    internal static class Program
    {
        private static IHost? _apiHost;
        private static readonly int ApiPort = 5000;

       
        [STAThread]
        static void Main()
        {
            try
            {
                Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
                Application.ThreadException += Application_ThreadException;
                AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                
                StartApiServer();
                
                var settingsService = new SettingsService();
                var showWelcome = settingsService.GetSetting<bool>("ShowWelcomeScreen");
                
                if (showWelcome)
                {
                    var welcomeForm = new WelcomeForm();
                    var result = welcomeForm.ShowDialog();
                    
                    if (result == DialogResult.Cancel)
                    {
                        StopApiServer();
                        return; 
                    }
                    
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
            finally
            {
                StopApiServer();
            }
        }

        private static void StartApiServer()
        {
            try
            {
                var builder = Host.CreateDefaultBuilder()
                    .ConfigureWebHostDefaults(webBuilder =>
                    {
                        webBuilder.UseUrls($"http://localhost:{ApiPort}")
                                 .ConfigureServices(ApiStartup.ConfigureServices)
                                 .Configure(app => ApiStartup.Configure(app, app.ApplicationServices.GetRequiredService<IWebHostEnvironment>()));
                    });

                _apiHost = builder.Build();
                _apiHost.StartAsync();

                System.Diagnostics.Debug.WriteLine($"API REST iniciada en: http://localhost:{ApiPort}");
                System.Diagnostics.Debug.WriteLine($"Documentación Swagger: http://localhost:{ApiPort}/api-docs");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error iniciando API: {ex.Message}");
            }
        }

        private static void StopApiServer()
        {
            try
            {
                _apiHost?.StopAsync();
                _apiHost?.Dispose();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error deteniendo API: {ex.Message}");
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

