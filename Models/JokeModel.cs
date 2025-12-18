using System.ComponentModel.DataAnnotations;

namespace JokesAppByMe.Models
{   /// <summary>
    /// Modelo principal que representa un chiste en la aplicación
    /// Incluye propiedades para contenido, estadísticas y relaciones
    /// </summary>
    public class JokeModel
    {
        // ID único del chiste (Primary Key)
        public int Id { get; set; }
        /// <summary>
        /// Pregunta o setup del chiste
        /// Máximo 500 caracteres
        /// </summary>
        [Required(ErrorMessage = "La pregunta del chiste es requerida")]
        [StringLength(500, ErrorMessage = "La pregunta no puede exceder 500 caracteres")]
        [Display(Name = "Pregunta")]
        public string? PreguntaChiste { get; set; }
        /// <summary>
        /// Respuesta o punchline del chiste
        /// Máximo 1000 caracteres
        /// </summary>
        [Required(ErrorMessage = "La respuesta del chiste es requerida")]
        [StringLength(1000, ErrorMessage = "La respuesta no puede exceder 1000 caracteres")]
        [Display(Name = "Respuesta")]
        public string? RespuestaChiste { get; set; }
        /// <summary>
        /// Categoría del chiste
        /// Valores definidos en JokeCategories
        /// </summary>
        [Required]
        [Display(Name = "Categoría")]
        public string Categoria { get; set; } = "General";
        /// <summary>
        /// Contador de "me gusta" (calculado desde JokeVotes)
        /// </summary>
        [Display(Name = "Me Gusta")]
        public int Likes { get; set; } = 0;
        /// <summary>
        /// Contador de "no me gusta" (calculado desde JokeVotes)
        /// </summary>
        [Display(Name = "No Me Gusta")]
        public int Dislikes { get; set; } = 0;
        /// <summary>
        /// Contador total de visualizaciones
        /// Se incrementa cada vez que alguien ve los detalles
        /// </summary>
        [Display(Name = "Vistas")]
        public int Vistas { get; set; } = 0;
        /// <summary>
        /// Calificación promedio (1-5 estrellas)
        /// Calculado desde JokeRatings
        /// </summary>
        [Range(0, 5)]
        [Display(Name = "Rating Promedio")]
        public double RatingPromedio { get; set; } = 0;
        /// <summary>
        /// Número total de calificaciones recibidas
        /// </summary>
        [Display(Name = "Total de Ratings")]
        public int TotalRatings { get; set; } = 0;
        /// <summary>
        /// Usuario que creó el chiste
        /// Almacena el email o username de Identity
        /// </summary>
        [Display(Name = "Creado Por")]
        public string? CreadoPor { get; set; }
        /// <summary>
        /// Fecha y hora de creación del chiste
        /// </summary>
        [Display(Name = "Fecha de Creación")]
        public DateTime FechaCreacion { get; set; } = DateTime.Now;
        // 
        // RELACIONES (Navigation Properties)
        // 
        /// <summary>
        /// Lista de votos (likes/dislikes) asociados a este chiste
        /// </summary>
        public List<JokeVote> Votes { get; set; } = new();
        /// <summary>
        /// Lista de calificaciones (ratings) asociadas a este chiste
        /// </summary>
        public List<JokeRating> Ratings { get; set; } = new();
        public JokeModel()
        {
        }
    }


    // 
    // CLASE ESTÁTICA: Categorías de Chistes
    // 
    /// <summary>
    /// Define las categorías disponibles para clasificar chistes
    /// </summary>
    public static class JokeCategories
    {
        public const string DadJokes = "Dad Jokes"; // Chistes de papá
        public const string KnockKnock = "Knock-Knock"; // Chistes de toc-toc
        public const string DarkHumor = "Dark Humor"; // Humor negro
        public const string TechJokes = "Tech Jokes"; // Chistes de tecnología/programación
        public const string Random = "Random"; // Aleatorios/misceláneos
        public const string General = "General"; // Generales
        /// <summary>
        /// Retorna una lista con todas las categorías disponibles
        /// Usado para poblar dropdowns en las vistas
        /// </summary>
        /// <returns>Lista de strings con los nombres de categorías</returns>
        public static List<string> GetAll()
        {
            return new List<string>
            {
                General,
                DadJokes,
                KnockKnock,
                DarkHumor,
                TechJokes,
                Random
            };
        }
    }


    }
