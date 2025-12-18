using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using JokesAppByMe.Models;

namespace JokesAppByMe.Data
{    /// <summary>
     /// Contexto de base de datos principal de la aplicación
     /// Hereda de IdentityDbContext para incluir tablas de autenticación
     /// </summary
    public class ApplicationDbContext : IdentityDbContext
    {
        /// <summary>
        /// Constructor que recibe las opciones de configuración del contexto
        /// </summary>
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        /// <summary>
        /// Configuración del modelo de datos
        /// Define índices únicos y relaciones
        /// </summary>
        protected override void OnModelCreating(ModelBuilder builder)
        {
            // Llamar al método base para configurar las tablas de Identity
            base.OnModelCreating(builder);
            // 
            // ÍNDICE ÚNICO: Un voto por usuario por chiste
            // 
            // Previene que un usuario vote múltiples veces el mismo chiste
            // El índice compuesto (JokeModelId, UserId) debe ser único
            builder.Entity<JokeVote>()
                .HasIndex(v => new { v.JokeModelId, v.UserId })
                .IsUnique();
            // 
            // ÍNDICE ÚNICO: Una calificación por usuario por chiste
            // 
            // Previene que un usuario califique múltiples veces el mismo chiste
            // El índice compuesto (JokeModelId, UserId) debe ser único
            builder.Entity<JokeRating>()
                .HasIndex(r => new { r.JokeModelId, r.UserId })
                .IsUnique();
        }
        // 
        // DBSETS: Conjuntos de entidades (tablas)
        // 
        /// <summary>
        /// Tabla de chistes
        /// </summary>
        public DbSet<JokesAppByMe.Models.JokeModel> JokeModel { get; set; } = default!;
        /// <summary>
        /// Tabla de votos (likes/dislikes)
        /// </summary>
        public DbSet<JokeVote> JokeVotes { get; set; } = default!;
        /// <summary>
        /// Tabla de calificaciones (ratings de 1-5 estrellas)
        /// </summary>
        public DbSet<JokeRating> JokeRatings { get; set; } = default!;

    }
}
