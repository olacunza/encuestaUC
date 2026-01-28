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

            // Configurar claves primarias para entidades
            modelBuilder.Entity<Encuesta>().HasKey(e => e.EncuestaId);
            modelBuilder.Entity<EncuestaBloque>().HasKey(b => b.BloqueId);
            modelBuilder.Entity<EncuestaPregunta>().HasKey(p => p.EncuestaDetalleId);
            modelBuilder.Entity<RespuestaEncuesta>().HasKey(r => r.RespuestaId);
            modelBuilder.Entity<RespuestaPregunta>().HasKey(r => r.RespuestaPreguntaId);

            // Para consultas de solo lectura (cuando se usan SPs que devuelven tipos diferentes)
            // Estas configuraciones son para evitar que EF intente mapearlas a tablas
            modelBuilder.Entity<Encuesta>().HasNoKey().ToView(null);
            modelBuilder.Entity<RespuestaEncuesta>().HasNoKey().ToView(null);
        }
    }
}