/** Script para manejar interacciones en la página Details Incluye likes, dislikes y calificación por estrellas*/
// FUNCIÓN: Obtener token antiforgery
/** Obtiene el token CSRF del DOM
 * @returns {string} Token antiforgery
 */
function getAntiForgeryToken() {
    return document.querySelector('input[name="__RequestVerificationToken"]')?.value || '';
}
// FUNCIÓN: Mostrar mensaje y redirigir al login
/** Muestra un mensaje personalizado y redirige al login después de 3 segundos
 * @param {string} message - Mensaje a mostrar */
function showLoginMessage(message) {
    // Crear overlay
    const overlay = document.createElement('div');
    overlay.className = 'auth-modal-overlay';

    // Crear modal
    const modal = document.createElement('div');
    modal.className = 'auth-modal';
    modal.innerHTML = `
        <h3>Autenticación requerida</h3>
        <p>${message}</p>
        <button class="auth-modal-btn" id="loginBtn">
            Ir a iniciar sesión
        </button>
    `;

    overlay.appendChild(modal);
    document.body.appendChild(overlay);

    // Redirigir al hacer click
    document.getElementById('loginBtn').addEventListener('click', () => {
        window.location.href = '/Identity/Account/Login?returnUrl=' +
            encodeURIComponent(window.location.pathname);
    });
    // Redirigir automáticamente después de 3 segundos
    setTimeout(() => {
        window.location.href = '/Identity/Account/Login?returnUrl=' + encodeURIComponent(window.location.pathname);
    }, 3000);
}
// FUNCIÓN: Dar Like
/** Registra un like en el chiste actual @param {number} id - ID del chiste */
async function likeJoke(id) {
    try {
        const token = getAntiForgeryToken();
        const res = await fetch(`/JokeModels/Like/${id}`, {
            method: 'POST',
            credentials: 'same-origin',
            headers: {
                'Content-Type': 'application/json',
                'RequestVerificationToken': token
            }
        });
        // Manejo de errores de autenticación
        if (!res.ok) {
            if (res.status === 401 || res.status === 403) {
                showLoginMessage('Debes iniciar sesión para dar like'); 
                return;  
            }
            throw new Error(`HTTP ${res.status}`);
        }
        const contentType = res.headers.get('content-type') || '';
        if (!contentType.includes('application/json')) {
            showLoginMessage('Debes iniciar sesión para dar like');
            return;
        }

        const data = await res.json();
        if (data.success) {
            // Actualizar contadores en el DOM
            document.getElementById('likesCount').textContent = data.likes;
            document.getElementById('dislikesCount').textContent = data.dislikes;
            showNotification('¡Like agregado!', 'success');
        }
    } catch (err) {
        console.error('Like error:', err);
        showNotification('Error al dar like');
    }
}
// FUNCIÓN: Dar Dislike
/** Registra un dislike en el chiste actual
 * @param {number} id - ID del chiste*/
async function dislikeJoke(id) {
    try {
        const token = getAntiForgeryToken();
        const res = await fetch(`/JokeModels/Dislike/${id}`, {
            method: 'POST',
            credentials: 'same-origin',
            headers: {
                'Content-Type': 'application/json',
                'RequestVerificationToken': token
            }
        });

        if (!res.ok) {
            if (res.status === 401 || res.status === 403) {
                showLoginMessage('Debes iniciar sesión para dar dislike'); 
                return;
            }
            throw new Error(`HTTP ${res.status}`);
        }
        const contentType = res.headers.get('content-type') || '';
        if (!contentType.includes('application/json')) {
            showLoginMessage('Debes iniciar sesión para dar dislike');
            return;
        }
        const data = await res.json();
        if (data.success) {
            document.getElementById('likesCount').textContent = data.likes;
            document.getElementById('dislikesCount').textContent = data.dislikes;
            showNotification('Dislike agregado', 'success');
        }
    } catch (err) {
        console.error('Dislike error:', err);
        showNotification('Error al dar dislike');
    }
}
// FUNCIÓN: Calificar con estrellas
/** Registra una calificación de 1-5 estrellas
 * @param {number} id - ID del chiste
 * @param {number} rating - Valor de 1 a 5 */
async function rateJoke(id, rating) {
    try {
        const token = getAntiForgeryToken();
        const res = await fetch(`/JokeModels/Rate/${id}?rating=${rating}`, {
            method: 'POST',
            credentials: 'same-origin',
            headers: {
                'Content-Type': 'application/json',
                'RequestVerificationToken': token
            }
        });

        if (!res.ok) {
            if (res.status === 401 || res.status === 403) {
                showLoginMessage('Debes iniciar sesión para calificar este chiste');
                return;
            }
            throw new Error(`HTTP ${res.status}`);
        }
        const contentType = res.headers.get('content-type') || '';
        if (!contentType.includes('application/json')) {
            showLoginMessage('Debes iniciar sesión para calificar este chiste');
            return;
        }
        const data = await res.json();
        if (data.success) {
            // Actualizar el rating promedio en el DOM
            document.getElementById('ratingValue').textContent = data.rating.toFixed(1);
            // Actualizar el total de votos
            const totalLabel = document.querySelector('.rating-section .label');
            if (totalLabel) {
                totalLabel.textContent = `Rating (${data.totalRatings} votos)`;
            }
            // Resaltar las estrellas seleccionadas
            highlightStars(rating);
            showNotification(`Valoraste con ${rating} estrellas`, 'success');
        }
    } catch (err) {
        console.error('Rating error:', err);
        showNotification('Error al calificar');
    }
}
// FUNCIÓN: Resaltar estrellas
/**Marca visualmente las estrellas según la calificación
 * @param {number} rating - Número de estrellas a resaltar (1-5)*/
function highlightStars(rating) {
    document.querySelectorAll('.star').forEach((star, index) => {
        star.classList.toggle('active', index < rating);
    });
}
// FUNCIÓN: Mostrar notificación toast
/**Muestra una notificación temporal en la esquina superior derecha
 * @param {string} message - Mensaje a mostrar
 * @param {string} type - Tipo de notificación ('success' o 'error') */
function showNotification(message, type) {
    const notification = document.createElement('div');
    notification.textContent = message;
    notification.className = `toast-notification toast-${type}`;

    document.body.appendChild(notification);

    // Remover después de 2.5 segundos
    setTimeout(() => notification.remove(), 2500);
}
// INICIALIZACIÓN AL CARGAR LA PÁGINA
/**Inicializa el sistema de calificación al cargar la página
 * Resalta las estrellas según el rating promedio actual*/
document.addEventListener('DOMContentLoaded', () => {
    const starContainer = document.getElementById('starRating');
    if (!starContainer) return;
    // Obtener el rating actual y redondearlo
    const currentRating = Math.round(
        parseFloat(document.getElementById('ratingValue')?.textContent || 0)
    );
    // Resaltar las estrellas correspondientes
    highlightStars(currentRating);
});