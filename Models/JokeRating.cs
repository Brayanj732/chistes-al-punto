using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace JokesAppByMe.Models
{
    /// <summary>
    /// Modelo que representa una calificación (1-5 estrellas) de un usuario sobre un chiste
    /// Implementa el patrón de "una calificación por usuario por chiste"
    /// El usuario puede actualizar su calificación
    /// </summary>
    public class JokeRating
    {
        // ID único de la calificación (Primary Key)
        public int Id { get; set; }
        /// <summary>
        /// ID del chiste al que pertenece esta calificación (Foreign Key)
        /// </summary>
        [Required]
        public int JokeModelId { get; set; }
        /// <summary>
        /// Propiedad de navegación hacia el chiste
        /// </summary>
        public JokeModel JokeModel { get; set; } = default!;
        /// <summary>
        /// ID del usuario que emitió la calificación (Foreign Key)
        /// </summary>
        [Required]
        public string UserId { get; set; } = default!;
        /// <summary>
        /// Propiedad de navegación hacia el usuario
        /// </summary>
        public IdentityUser User { get; set; } = default!;
        /// <summary>
        /// Valor de la calificación: 1 (peor) a 5 (mejor) estrellas
        /// Validado con el atributo Range
        /// </summary>
        [Range(1, 5)]
        public int Value { get; set; }
        /// <summary>
        /// Fecha y hora en que se emitió la calificación por primera vez
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        /// <summary>
        /// Fecha y hora de la última actualización de la calificación
        /// Null si nunca se ha actualizado
        /// </summary>
        public DateTime? UpdatedAt { get; set; }
    }
}
