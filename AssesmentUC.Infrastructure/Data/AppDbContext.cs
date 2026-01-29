using AssesmentUC.Model.Entity;
using Microsoft.EntityFrameworkCore;

namespace AssesmentUC.Infrastructure.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }
        public DbSet<Encuesta> Encuestas { get; set; }
        public DbSet<RespuestaEncuesta> RespuestasEncuesta { get; set; }
        public DbSet<EncuestaBloque> EncuestaBloques { get; set; }
        public DbSet<EncuestaPregunta> EncuestaPreguntas { get; set; }
        public DbSet<RespuestaPregunta> RespuestaPreguntas { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Encuesta>().HasKey(e => e.EncuestaId);
            modelBuilder.Entity<EncuestaBloque>().HasKey(b => b.BloqueId);
            modelBuilder.Entity<EncuestaPregunta>().HasKey(p => p.EncuestaDetalleId);
            modelBuilder.Entity<RespuestaEncuesta>().HasKey(r => r.RespuestaEncuestaId);
            modelBuilder.Entity<RespuestaPregunta>().HasKey(r => r.RespuestaPreguntaId);

            modelBuilder.Entity<Encuesta>().HasNoKey().ToView(null);
            modelBuilder.Entity<RespuestaEncuesta>().HasNoKey().ToView(null);
        }
    }
}