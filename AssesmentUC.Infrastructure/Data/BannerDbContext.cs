using Microsoft.EntityFrameworkCore;

namespace AssesmentUC.Infrastructure.Data
{
    public class BannerDbContext : DbContext
    {
        public BannerDbContext(DbContextOptions<BannerDbContext> options) : base(options)
        {
        }

        // No definimos DbSets ya que solo usamos stored procedures
        // con conexión directa a Oracle
    }
}