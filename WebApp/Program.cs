using WebApp.Models;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Serilog.Events;

namespace WebApp
{
    internal static class Program
    {
        public static void Main(string[] args)
        {
            const string devSettingsLogger = "appsettings.Development.json";
            const string releaseSettingsLogger = "appsettings.json";
            string? pathLogger;
            
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();
            builder.Services.AddSwaggerGen();
            builder.Services.AddSerilog();
            
            builder.Services.AddEntityFrameworkNpgsql()
                .AddDbContext<ApplicationContext>(opt =>
                    opt.UseNpgsql(
                        builder.Configuration.GetConnectionString("DbConnection")
                    )
                );

            var app = builder.Build();
            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            else
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }
            
            IConfigurationRoot configDevLogger = new ConfigurationBuilder()
                .AddJsonFile(app.Environment.IsDevelopment() ? devSettingsLogger : releaseSettingsLogger, optional: false)
                .Build();
                
            pathLogger = configDevLogger.GetValue<string>("Logging:LogFilePath");
            
            if (pathLogger == null)
            {
                throw new KeyNotFoundException();
            }
            
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
                .Enrich.FromLogContext()
                .WriteTo.File(pathLogger)
                .CreateLogger();
            
             using (var scope = app.Services.CreateScope())
             {
                 var services = scope.ServiceProvider;

                 var context = services.GetRequiredService<ApplicationContext>();
                 if (context.Database.GetPendingMigrations().Any())
                 {
                     context.Database.Migrate();
                 }
             }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}"
            );

            app.Run();
        }
    }
}