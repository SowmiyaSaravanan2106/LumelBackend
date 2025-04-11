using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using AppStore.Model;

namespace AppStore
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllers();
            builder.Services.AddAuthorization();

            // Database configuration - SQLite
            string? connectionString = builder.Configuration.GetConnectionString("connectionString") ?? "Data Source=sqlfile.db";
            builder.Services.AddSqlite<AppDBContext>(connectionString);

            // Swagger configuration
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "API",
                    Description = "Lumel",
                    Version = "v1"
                });
            });

            var app = builder.Build();

            // Configure middleware
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "API V1");
                });
            }

            app.UseHttpsRedirection();
            app.UseAuthorization();

            app.MapControllers();


            app.Run();
        }
    }
}