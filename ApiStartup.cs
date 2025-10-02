using InventoryQRManager.Services;
using InventoryQRManager.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace InventoryQRManager
{
    public class ApiStartup
    {
        public static void ConfigureServices(IServiceCollection services)
        {
            services.AddScoped<DatabaseContext>();
            services.AddScoped<AuthService>();
            services.AddScoped<InventoryService>();
            services.AddScoped<ReportService>();
            services.AddScoped<QRCodeService>();
            services.AddScoped<HistoryService>();
            services.AddScoped<BackupService>();
            services.AddScoped<SettingsService>();
            services.AddScoped<ThemeService>();

            services.AddCors(options =>
            {
                options.AddPolicy("AllowMobileApp", policy =>
                {
                    policy.AllowAnyOrigin()
                          .AllowAnyMethod()
                          .AllowAnyHeader();
                });
            });

            services.AddControllers()
                .AddNewtonsoftJson(options =>
                {
                    options.SerializerSettings.DateFormatHandling = Newtonsoft.Json.DateFormatHandling.IsoDateFormat;
                    options.SerializerSettings.DateTimeZoneHandling = Newtonsoft.Json.DateTimeZoneHandling.Utc;
                });

            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
                {
                    Title = "Inventory QR Manager API",
                    Version = "v1",
                    Description = "API REST para el sistema de gestión de inventario con códigos QR"
                });
            });

            services.AddLogging(builder =>
            {
                builder.AddConsole();
                builder.AddDebug();
            });
        }

        public static void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Inventory QR Manager API v1");
                c.RoutePrefix = "api-docs"; // Acceder en /api-docs
            });

            app.UseRouting();
            app.UseCors("AllowMobileApp");
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
