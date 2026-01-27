using AssesmentUC.Model.Entity;
using Microsoft.EntityFrameworkCore;

namespace AssesmentUC.Infrastructure.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        // Tablas principales de SQL Server
        public DbSet<Encuesta> Encuestas { get; set; }
        public DbSet<RespuestaEncuesta> RespuestasEncuesta { get; set; }
        public DbSet<EncuestaBloque> EncuestaBloques { get; set; }
        public DbSet<EncuestaPregunta> EncuestaPreguntas { get; set; }
        public DbSet<RespuestaPregunta> RespuestaPreguntas { get; set; }

        // Configuraciones adicionales si necesitas relaciones
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configuraciones para consultas de solo lectura
            modelBuilder.Entity<Encuesta>()
                .HasNoKey()
                .ToView(null);

            modelBuilder.Entity<RespuestaEncuesta>()
                .HasNoKey()
                .ToView(null);

            // Si necesitas configurar relaciones, agregarlas aquí
        }
    }
}