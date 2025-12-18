using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace JokesAppByMe.Models
{    /// <summary>
     /// Modelo que representa un voto (like/dislike) de un usuario sobre un chiste
     /// Implementa el patrón de "un voto por usuario por chiste"
     /// El usuario puede cambiar su voto, pero no votar múltiples veces
     /// </summary>
    public class JokeVote
    {
        // ID único del voto (Primary Key)
        public int Id { get; set; }
        /// <summary>
        /// ID del chiste al que pertenece este voto (Foreign Key)
        /// </summary>
        [Required]
        public int JokeModelId { get; set; }
        /// <summary>
        /// Propiedad de navegación hacia el chiste
        /// Permite acceder al chiste completo desde el voto
        /// </summary>
        public JokeModel JokeModel { get; set; } = default!;
        /// <summary>
        /// ID del usuario que emitió el voto (Foreign Key)
        /// Referencia a la tabla de Identity
        /// </summary>
        [Required]
        public string UserId { get; set; } = default!;
        /// <summary>
        /// Propiedad de navegación hacia el usuario
        /// Permite acceder a los datos del usuario desde el voto
        /// </summary>
        public IdentityUser User { get; set; } = default!;
        /// <summary>
        /// Tipo de voto: true = Like , false = Dislike 
        /// </summary>        
        public bool IsLike { get; set; }
        /// <summary>
        /// Fecha y hora en que se emitió el voto por primera vez
        /// Usa UTC para evitar problemas de zona horaria
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        /// <summary>
        /// Fecha y hora de la última actualización del voto
        /// Se actualiza cuando el usuario cambia de like a dislike o viceversa
        /// Null si nunca se ha actualizado
        /// </summary>
        public DateTime? UpdatedAt { get; set; }
    }
}
