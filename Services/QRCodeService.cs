using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using QRCoder;

namespace InventoryQRManager.Services
{
    public class QRCodeService
    {
        public QRCodeService()
        {
           
        }

        public Bitmap GenerateQRCode(string text)
        {
            try
            {
                if (string.IsNullOrEmpty(text))
                {
                    throw new ArgumentException("El texto para generar el código QR no puede estar vacío.");
                }

                using (var qrGenerator = new QRCodeGenerator())
                {
                    var qrCodeData = qrGenerator.CreateQrCode(text, QRCodeGenerator.ECCLevel.Q);
                    using (var qrCode = new QRCode(qrCodeData))
                    {
                        var qrCodeImage = qrCode.GetGraphic(20); 
                        return new Bitmap(qrCodeImage);
                    }
                }
            }
            catch (Exception ex)
            {
               
                System.Diagnostics.Debug.WriteLine($"Error generando QR con QRCoder: {ex}");
                
                
                try
                {
                    var fallbackBitmap = new Bitmap(150, 150);
                    using (var g = Graphics.FromImage(fallbackBitmap))
                    {
                        g.Clear(Color.White);
                        using (var font = new Font("Arial", 8))
                        using (var brush = new SolidBrush(Color.Black))
                        {
                            g.DrawString("QR Error", font, brush, 10, 10);
                            g.DrawString(text.Substring(0, Math.Min(20, text.Length)), font, brush, 10, 30);
                        }
                    }
                    return fallbackBitmap;
                }
                catch (Exception fallbackEx)
                {
                    throw new Exception($"Error generando código QR: {ex.Message}. Fallback: {fallbackEx.Message}");
                }
            }
        }

        public string ReadQRCode(Bitmap image)
        {
            
            return "Funcionalidad de lectura en desarrollo";
        }

        public string ReadQRCodeFromFile(string filePath)
        {
           
            return "Funcionalidad de lectura en desarrollo";
        }

        public void SaveQRCodeToFile(string text, string filePath)
        {
            try
            {
                var qrCode = GenerateQRCode(text);
                qrCode.Save(filePath, ImageFormat.Png);
                qrCode.Dispose();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error guardando código QR: {ex.Message}");
            }
        }

        public string GenerateUniqueQRCode(string sku, string name)
        {
            var timestamp = DateTime.Now.Ticks;
            var uniqueText = $"INV-{sku}-{name.Replace(" ", "_")}-{timestamp}";
            return uniqueText;
        }

        public bool ValidateQRCode(string qrCode)
        {
            
            return !string.IsNullOrEmpty(qrCode) && qrCode.StartsWith("INV-");
        }
    }
}

