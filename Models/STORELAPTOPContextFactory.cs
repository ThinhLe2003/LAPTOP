using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace LAPTOP.Models
{
    public class STORELAPTOPContextFactory : IDesignTimeDbContextFactory<STORELAPTOPContext>
    {
        public STORELAPTOPContext CreateDbContext(string[] args)
        {
            // Đọc appsettings.json khi chạy EF CLI
            IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

            var connectionString = configuration.GetConnectionString("DefaultConnection");

            var optionsBuilder = new DbContextOptionsBuilder<STORELAPTOPContext>();
            optionsBuilder.UseSqlServer(connectionString);

            return new STORELAPTOPContext(optionsBuilder.Options);
        }
    }
}
