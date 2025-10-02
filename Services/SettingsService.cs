using System;
using System.IO;
using System.Text.Json;
using System.Collections.Generic;

namespace InventoryQRManager.Services
{
    public class SettingsService
    {
        private readonly string _settingsFilePath;
        private AppSettings _settings;

        public SettingsService()
        {
            _settingsFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "settings.json");
            _settings = LoadSettings();
        }

        public class AppSettings
        {
            public string CompanyName { get; set; } = "Mi Empresa";
            public string DefaultCategory { get; set; } = "Otros";
            public string DefaultLocation { get; set; } = "Almacén Principal";
            public int LowStockThreshold { get; set; } = 10;
            public bool AutoGenerateQR { get; set; } = true;
            public bool ShowNotifications { get; set; } = true;
            public string Language { get; set; } = "es";
            public string Theme { get; set; } = "Light";
            public bool AutoBackup { get; set; } = true;
            public int BackupIntervalDays { get; set; } = 7;
            public string BackupLocation { get; set; } = "";
            public bool ShowWelcomeScreen { get; set; } = true;
            public int GridPageSize { get; set; } = 50;
            public bool EnableAuditLog { get; set; } = true;
            public List<string> CustomCategories { get; set; } = new();
            public List<string> CustomLocations { get; set; } = new();
        }

        public AppSettings GetSettings()
        {
            return _settings;
        }

        public void SaveSettings(AppSettings settings)
        {
            _settings = settings;
            SaveSettingsToFile();
        }

        public void UpdateSetting<T>(string propertyName, T value)
        {
            var property = typeof(AppSettings).GetProperty(propertyName);
            if (property != null && property.CanWrite)
            {
                property.SetValue(_settings, value);
                SaveSettingsToFile();
            }
        }

        public T GetSetting<T>(string propertyName)
        {
            var property = typeof(AppSettings).GetProperty(propertyName);
            if (property != null)
            {
                var value = property.GetValue(_settings);
                if (value is T typedValue)
                {
                    return typedValue;
                }
            }
            return default(T)!;
        }

        public void ResetToDefaults()
        {
            _settings = new AppSettings();
            SaveSettingsToFile();
        }

        public bool ExportSettings(string filePath)
        {
            try
            {
                var json = JsonSerializer.Serialize(_settings, new JsonSerializerOptions 
                { 
                    WriteIndented = true 
                });
                File.WriteAllText(filePath, json);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool ImportSettings(string filePath)
        {
            try
            {
                if (!File.Exists(filePath))
                {
                    return false;
                }

                var json = File.ReadAllText(filePath);
                var importedSettings = JsonSerializer.Deserialize<AppSettings>(json);
                
                if (importedSettings != null)
                {
                    _settings = importedSettings;
                    SaveSettingsToFile();
                    return true;
                }

                return false;
            }
            catch
            {
                return false;
            }
        }

        public void AddCustomCategory(string category)
        {
            if (!string.IsNullOrWhiteSpace(category) && !_settings.CustomCategories.Contains(category))
            {
                _settings.CustomCategories.Add(category);
                SaveSettingsToFile();
            }
        }

        public void RemoveCustomCategory(string category)
        {
            if (_settings.CustomCategories.Contains(category))
            {
                _settings.CustomCategories.Remove(category);
                SaveSettingsToFile();
            }
        }

        public void AddCustomLocation(string location)
        {
            if (!string.IsNullOrWhiteSpace(location) && !_settings.CustomLocations.Contains(location))
            {
                _settings.CustomLocations.Add(location);
                SaveSettingsToFile();
            }
        }

        public void RemoveCustomLocation(string location)
        {
            if (_settings.CustomLocations.Contains(location))
            {
                _settings.CustomLocations.Remove(location);
                SaveSettingsToFile();
            }
        }

        public List<string> GetAllCategories()
        {
            var categories = new List<string>
            {
                "Electrónicos",
                "Ropa",
                "Hogar",
                "Libros",
                "Otros"
            };
            
            categories.AddRange(_settings.CustomCategories);
            return categories;
        }

        public List<string> GetAllLocations()
        {
            var locations = new List<string>
            {
                "Almacén Principal",
                "Oficina",
                "Tienda"
            };
            
            locations.AddRange(_settings.CustomLocations);
            return locations;
        }

        private AppSettings LoadSettings()
        {
            try
            {
                if (File.Exists(_settingsFilePath))
                {
                    var json = File.ReadAllText(_settingsFilePath);
                    var settings = JsonSerializer.Deserialize<AppSettings>(json);
                    if (settings != null)
                    {
                        return settings;
                    }
                }
            }
            catch
            {
              
            }

            return new AppSettings();
        }

        private void SaveSettingsToFile()
        {
            try
            {
                var json = JsonSerializer.Serialize(_settings, new JsonSerializerOptions 
                { 
                    WriteIndented = true 
                });
                File.WriteAllText(_settingsFilePath, json);
            }
            catch
            {
               
            }
        }
    }
}
