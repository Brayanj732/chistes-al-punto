using JokesAppByMe.Data;
using JokesAppByMe.Models;
using Microsoft.AspNetCore.Authorization; 
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace JokesAppByMe.Controllers
{
    /// <summary>
    /// Controlador principal para gestionar operaciones CRUD de chistes
    /// Incluye funcionalidades de votación, calificación y visualización
    /// </summary>
    public class JokeModelsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        /// <summary>
        /// Constructor con inyección de dependencias
        /// </summary>
        /// <param name="context">Contexto de base de datos</param>
        /// <param name="userManager">Administrador de usuarios de Identity</param>
        public JokeModelsController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
        }

        // 
        // GET: JokeModels/Index
        // 
        /// <summary>
        /// Muestra la lista paginada de chistes con opciones de búsqueda, filtrado y ordenamiento
        /// </summary>
        /// <param name="searchString">Texto de búsqueda en pregunta o respuesta</param>
        /// <param name="categoria">Categoría para filtrar</param>
        /// <param name="sortOrder">Criterio de ordenamiento</param>
        /// <param name="page">Número de página actual (por defecto 1)</param>
        /// <returns>Vista con la lista paginada de chistes</returns>
        public async Task<IActionResult> Index(string searchString, string categoria, string sortOrder, int page = 1)
        {
            // Guardar parámetros actuales en ViewData para mantener estado en la paginación
            ViewData["CurrentFilter"] = searchString;
            ViewData["CurrentCategory"] = categoria;
            ViewData["CurrentSort"] = sortOrder;
            // Configurar parámetros de ordenamiento para alternar entre ascendente/descendente
            ViewData["DateSortParm"] = sortOrder == "Date" ? "date_desc" : "Date";
            ViewData["PopularitySortParm"] = sortOrder == "Popular" ? "popular_desc" : "Popular";
            ViewData["RatingSortParm"] = sortOrder == "Rating" ? "rating_desc" : "Rating";

            // Query inicial: obtener todos los chistes
            var jokes = from j in _context.JokeModel select j;

            // Filtro por búsqueda de texto
            if (!string.IsNullOrEmpty(searchString))
            {
                jokes = jokes.Where(j => j.PreguntaChiste.Contains(searchString) ||
                                        j.RespuestaChiste.Contains(searchString));
            }
            // Filtro por categoría
            if (!string.IsNullOrEmpty(categoria) && categoria != "Todas")
            {
                jokes = jokes.Where(j => j.Categoria == categoria);
            }
            // Aplicar ordenamiento según el criterio seleccionado
            switch (sortOrder)
            {
                case "Date":
                    jokes = jokes.OrderBy(j => j.FechaCreacion);
                    break;
                case "date_desc":
                    jokes = jokes.OrderByDescending(j => j.FechaCreacion);
                    break;
                case "Popular":
                    jokes = jokes.OrderByDescending(j => j.Likes - j.Dislikes);
                    break;
                case "popular_desc":
                    jokes = jokes.OrderBy(j => j.Likes - j.Dislikes);
                    break;
                case "Rating":
                    jokes = jokes.OrderByDescending(j => j.RatingPromedio);
                    break;
                case "rating_desc":
                    jokes = jokes.OrderBy(j => j.RatingPromedio);
                    break;
                default:
                    jokes = jokes.OrderByDescending(j => j.FechaCreacion);
                    break;
            }
            // Cargar lista de categorías para el dropdown de filtros
            ViewBag.Categories = JokeCategories.GetAll();

            //  PAGINACIÓN MANUAL 
            int pageSize = 12; // Número de chistes por página
            int totalJokes = await jokes.CountAsync(); // Total de chistes después de filtros
            int totalPages = (int)Math.Ceiling(totalJokes / (double)pageSize); // Total de páginas

            // Validar que el número de página sea válido
            if (page < 1) page = 1;
            if (page > totalPages && totalPages > 0) page = totalPages;
            // Obtener solo los chistes de la página actual
            var paginatedJokes = await jokes
                .Skip((page - 1) * pageSize)// Saltar chistes de páginas anteriores
                .Take(pageSize) // Tomar solo 12 chistes
                .ToListAsync();

            // Pasar información de paginación a la vista
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;
            ViewBag.TotalJokes = totalJokes;
            ViewBag.HasPreviousPage = page > 1;
            ViewBag.HasNextPage = page < totalPages;

            return View(paginatedJokes);
        }
        //
        // GET: JokeModels/Random
        // 
        /// <summary>
        /// Muestra un chiste aleatorio e incrementa su contador de vistas
        /// </summary>
        /// <returns>Vista Details con un chiste aleatorio</returns>
        public async Task<IActionResult> Random()
        {
            var totalJokes = await _context.JokeModel.CountAsync();  
            // Si no hay chistes, redirigir al índice
            if (totalJokes == 0)
            {
                return RedirectToAction(nameof(Index));  // Si no hay chistes, vuelve al Index
            }
            // Generar un número aleatorio y obtener el chiste correspondiente
            var random = new Random();
            var randomIndex = random.Next(0, totalJokes);  //  Número aleatorio entre 0 y total
            var randomJoke = await _context.JokeModel.Skip(randomIndex).FirstOrDefaultAsync();

            if (randomJoke != null)
            {
                randomJoke.Vistas++;  //  Incrementa el contador de vistas
                await _context.SaveChangesAsync();
                return View("Details", randomJoke);  //  Muestra el chiste en la vista Details
            }

            return RedirectToAction(nameof(Index));
        }
        // 
        // POST: JokeModels/Like
        // 
        /// <summary>
        /// Registra un "like" de un usuario autenticado
        /// Previene votos duplicados y permite cambiar de opinión
        /// </summary>
        /// <param name="id">ID del chiste</param>
        /// <returns>JSON con el resultado y contadores actualizados</returns>
        [Authorize] // Solo usuarios autenticados
        [HttpPost]
        public async Task<IActionResult> Like(int id)
        {
            return await UpsertVote(id, isLike: true);
        }

        //
        // POST: JokeModels/Dislike
        //
        /// <summary>
        /// Registra un "dislike" de un usuario autenticado
        /// </summary>
        /// <param name="id">ID del chiste</param>
        /// <returns>JSON con el resultado y contadores actualizados</returns>
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Dislike(int id)
        {
            return await UpsertVote(id, isLike: false);
        }
        // 
        // MÉTODO PRIVADO: UpsertVote
        // 
        /// <summary>
        /// Crea o actualiza el voto de un usuario (like/dislike)
        /// Implementa la lógica de prevención de duplicados
        /// </summary>
        /// <param name="id">ID del chiste</param>
        /// <param name="isLike">true = like, false = dislike</param>
        /// <returns>JSON con resultado y contadores</returns>
        private async Task<IActionResult> UpsertVote(int id, bool isLike)
        {
            // Obtener el ID del usuario actual
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return Unauthorized();
            // Verificar que el chiste exista
            var joke = await _context.JokeModel.FindAsync(id);
            if (joke == null) return NotFound();
            // Buscar si el usuario ya votó este chiste
            var existing = await _context.JokeVotes
                .FirstOrDefaultAsync(v => v.JokeModelId == id && v.UserId == userId);

            if (existing == null)
            {
                // Primera vez que vota: crear nuevo registro
                _context.JokeVotes.Add(new JokeVote
                {
                    JokeModelId = id,
                    UserId = userId,
                    IsLike = isLike
                });
            }
            else
            {
                // Ya votó antes: verificar si está cambiando su voto
                if (existing.IsLike == isLike)
                {
                    // Si vota lo mismo, no hacer nada (evita spam)
                    return Json(new
                    {
                        success = true,
                        likes = joke.Likes,
                        dislikes = joke.Dislikes
                    });
                }
                // Cambiar el voto (de like a dislike o viceversa)
                existing.IsLike = isLike;
                existing.UpdatedAt = DateTime.UtcNow;
            }

            await _context.SaveChangesAsync();

            // Recalcular los contadores desde la base de datos (cache)
            joke.Likes = await _context.JokeVotes.CountAsync(v => v.JokeModelId == id && v.IsLike);
            joke.Dislikes = await _context.JokeVotes.CountAsync(v => v.JokeModelId == id && !v.IsLike);
            await _context.SaveChangesAsync();

            return Json(new { success = true, likes = joke.Likes, dislikes = joke.Dislikes });

        }
        // 
        // POST: JokeModels/Rate
        // 
        /// <summary>
        /// Registra o actualiza la calificación (1-5 estrellas) de un usuario
        /// Recalcula el promedio automáticamente
        /// </summary>
        /// <param name="id">ID del chiste</param>
        /// <param name="rating">Valor de 1 a 5</param>
        /// <returns>JSON con resultado y promedio actualizado</returns>
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Rate(int id, int rating)
        {
            // Validar que el rating esté entre 1 y 5
            if (rating < 1 || rating > 5) return BadRequest();

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return Unauthorized();

            var joke = await _context.JokeModel.FindAsync(id);
            if (joke == null) return NotFound();
            // Buscar si el usuario ya calificó este chiste
            var existing = await _context.JokeRatings
                .FirstOrDefaultAsync(r => r.JokeModelId == id && r.UserId == userId);

            if (existing == null)
            {
                // Primera calificación del usuario
                _context.JokeRatings.Add(new JokeRating
                {
                    JokeModelId = id,
                    UserId = userId,
                    Value = rating
                });
            }
            else
            {
                // Ya calificó antes: actualizar su calificación
                existing.Value = rating;
                existing.UpdatedAt = DateTime.UtcNow;
            }

            await _context.SaveChangesAsync();

            // Recalcular el promedio y total de calificaciones
            var total = await _context.JokeRatings.CountAsync(r => r.JokeModelId == id);
            var avg = total == 0
                ? 0
                : await _context.JokeRatings.Where(r => r.JokeModelId == id).AverageAsync(r => r.Value);

            joke.TotalRatings = total;
            joke.RatingPromedio = Math.Round(avg, 1); // Redondear a 1 decimal

            await _context.SaveChangesAsync();

            return Json(new { success = true, rating = joke.RatingPromedio, totalRatings = joke.TotalRatings });
        }
        // 
        // GET: JokeModels/Details/5
        // 
        /// <summary>
        /// Muestra los detalles completos de un chiste
        /// Incrementa el contador de vistas automáticamente
        /// </summary>
        /// <param name="id">ID del chiste</param>
        /// <returns>Vista con los detalles del chiste</returns>
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var jokeModel = await _context.JokeModel.FirstOrDefaultAsync(m => m.Id == id);
            if (jokeModel == null) return NotFound();

            // Incrementar contador de vistas (cualquier usuario, puede repetir)
            jokeModel.Vistas++;
            await _context.SaveChangesAsync();

            return View(jokeModel);
        }
        // 
        // GET: JokeModels/Create
        // 
        /// <summary>
        /// Muestra el formulario para crear un nuevo chiste
        /// Solo accesible para usuarios autenticados
        /// </summary>
        /// <returns>Vista con formulario de creación</returns>
        [Authorize]
        public IActionResult Create()
        {
            // Cargar categorías para el dropdown
            ViewBag.Categories = new SelectList(JokeCategories.GetAll());
            return View();
        }

        // 
        // POST: JokeModels/Create
        // 
        /// <summary>
        /// Procesa la creación de un nuevo chiste
        /// Asigna automáticamente el creador y la fecha
        /// </summary>
        /// <param name="jokeModel">Modelo con los datos del chiste</param>
        /// <returns>Redirecciona al índice si es exitoso</returns>
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken] // Protección contra CSRF
        public async Task<IActionResult> Create([Bind("Id,PreguntaChiste,RespuestaChiste,Categoria")] JokeModel jokeModel)
        {
            if (ModelState.IsValid)
            {
                // Asignar automáticamente el usuario que lo creó
                jokeModel.CreadoPor = User.Identity?.Name;
                jokeModel.FechaCreacion = DateTime.Now;
                _context.Add(jokeModel);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(jokeModel);
        }

        // 
        // GET: JokeModels/Edit/5
        // 
        /// <summary>
        /// Muestra el formulario para editar un chiste existente
        /// </summary>
        /// <param name="id">ID del chiste a editar</param>
        /// <returns>Vista con formulario de edición</returns>
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var jokeModel = await _context.JokeModel.FindAsync(id);
            if (jokeModel == null)
            {
                return NotFound();
            }
            // Cargar categorías con la actual seleccionada
            ViewBag.Categories = new SelectList(JokeCategories.GetAll(), jokeModel.Categoria);
            return View(jokeModel);
        }
        // 
        // POST: JokeModels/Edit/5
        // 
        /// <summary>
        /// Procesa la edición de un chiste
        /// Solo actualiza pregunta, respuesta y categoría (mantiene estadísticas)
        /// </summary>
        /// <param name="id">ID del chiste</param>
        /// <param name="jokeModel">Modelo con los datos actualizados</param>
        /// <returns>Redirecciona al índice si es exitoso</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,PreguntaChiste,RespuestaChiste,Categoria")] JokeModel jokeModel)
        {
            if (id != jokeModel.Id) return NotFound();

            if (!ModelState.IsValid)
            {
                // Si hay errores, recargar el dropdown de categorías
                ViewBag.Categories = new SelectList(JokeCategories.GetAll(), jokeModel.Categoria);
                return View(jokeModel);
            }

            // Obtener el chiste original de la base de datos
            var original = await _context.JokeModel.FindAsync(id);
            if (original == null) return NotFound();

            // Actualizar SOLO los campos editables (mantiene likes, vistas, etc.)
            original.PreguntaChiste = jokeModel.PreguntaChiste;
            original.RespuestaChiste = jokeModel.RespuestaChiste;
            original.Categoria = jokeModel.Categoria;

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // 
        // GET: JokeModels/Delete/5
        // 
        /// <summary>
        /// Muestra la vista de confirmación para eliminar un chiste
        /// Muestra todos los datos del chiste antes de eliminarlo
        /// </summary>
        /// <param name="id">ID del chiste a eliminar</param>
        /// <returns>Vista de confirmación</returns>
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var jokeModel = await _context.JokeModel
                .FirstOrDefaultAsync(m => m.Id == id);
            if (jokeModel == null)
            {
                return NotFound();
            }

            return View(jokeModel);
        }

        // 
        // POST: JokeModels/Delete/5
        // 
        /// <summary>
        /// Procesa la eliminación permanente de un chiste
        /// Se pierden todos los likes, ratings y vistas asociados
        /// </summary>
        /// <param name="id">ID del chiste</param>
        /// <returns>Redirecciona al índice</returns>        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var jokeModel = await _context.JokeModel.FindAsync(id);
            if (jokeModel != null)
            {
                _context.JokeModel.Remove(jokeModel);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        // 
        // MÉTODO PRIVADO: JokeModelExists
        // 
        /// <summary>
        /// Verifica si un chiste existe en la base de datos
        /// </summary>
        /// <param name="id">ID del chiste</param>
        /// <returns>True si existe, False si no</returns>
        private bool JokeModelExists(int id)
        {
            return _context.JokeModel.Any(e => e.Id == id);
        }
    }
}
