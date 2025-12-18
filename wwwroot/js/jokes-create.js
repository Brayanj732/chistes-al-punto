/**
 * Script para el formulario de creación de chistes
 * Maneja contadores de caracteres en tiempo real
 */


// INICIALIZACIÓN AL CARGAR LA PÁGINA
/**
 * Configura los contadores de caracteres para los textareas
 * Se ejecuta cuando el DOM está completamente cargado
 */
document.addEventListener('DOMContentLoaded', () => {
    // Obtener referencias a los elementos del DOM
    const preguntaInput = document.getElementById('preguntaInput');
    const respuestaInput = document.getElementById('respuestaInput');
    const preguntaCount = document.getElementById('preguntaCount');
    const respuestaCount = document.getElementById('respuestaCount');

    // Verificar que todos los elementos existan (previene errores en otras páginas)
    if (!preguntaInput || !respuestaInput || !preguntaCount || !respuestaCount) return;
    // FUNCIONES DE ACTUALIZACIÓN DE CONTADORES
    /**
     * Actualiza el contador de caracteres de la pregunta
     */
    const updatePregunta = () => (preguntaCount.textContent = preguntaInput.value.length);
     /**
     * Actualiza el contador de caracteres de la respuesta
     */
    const updateRespuesta = () => (respuestaCount.textContent = respuestaInput.value.length);

    // EVENT LISTENERS

    // Actualizar contador cada vez que el usuario escribe
    preguntaInput.addEventListener('input', updatePregunta);
    respuestaInput.addEventListener('input', updateRespuesta);

    // Inicializar contadores (útil si la vista tiene valores precargados)
    updatePregunta();
    updateRespuesta();
});