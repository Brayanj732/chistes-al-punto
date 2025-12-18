/**Script para manejar likes y dislikes en la página Index Usa AJAX para actualizar contadores sin recargar la página 
 * Incluye manejo de autenticación y tokens CSRF*/

// FUNCIÓN: Obtener token antiforgery (CSRF)
/** Obtiene el token antiforgery del DOM
 * Este token previene ataques Cross-Site Request Forgery (CSRF)
 * ASP.NET Core lo genera automáticamente con @Html.AntiForgeryToken()
 * @returns {string} Token antiforgery o cadena vacía si no existe */
function getAntiForgeryToken() {
    return document.querySelector('input[name="__RequestVerificationToken"]')?.value || '';
}
// FUNCIÓN: Hacer POST con JSON
/**Realiza una petición POST con JSON al servidor
 * Incluye manejo de errores de autenticación y redirección a login
 * @param {string} url - URL del endpoint a llamar
 * @returns {Promise<Object|null>} Respuesta JSON o null si falla*/
// FUNCIÓN: Mostrar mensaje y redirigir al login
/**Muestra un mensaje personalizado y redirige al login después de 3 segundos
 * @param {string} message - Mensaje a mostrar*/
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
        window.location.href = '/Identity/Account/Login?returnUrl=' +
            encodeURIComponent(window.location.pathname);
    }, 3000);
}

async function postJson(url) {
    const token = getAntiForgeryToken();
    // Realizar la petición fetch
    const res = await fetch(url, {
        method: 'POST',
        credentials: 'same-origin', // Incluir cookies de sesión
        headers: {
            'Content-Type': 'application/json',
            'RequestVerificationToken': token // Token CSRF
        }
    });
    // Manejar errores de autorización (usuario no autenticado)
    if (!res.ok) {
        if (res.status === 401 || res.status === 403) {
            showLoginMessage('Debes iniciar sesión para realizar esta acción');
            // Redirigir a login con returnUrl para volver después
            return null;
        }
        throw new Error(`HTTP ${res.status}`);
    }
    // Verificar que la respuesta sea JSON (no HTML de redirect)
    const contentType = res.headers.get('content-type') || '';
    if (!contentType.includes('application/json')) {
        showLoginMessage('Debes iniciar sesión para realizar esta acción');
        return null;
    }

    return await res.json();
}
// FUNCIÓN: Dar Like a un chiste
/**Registra un "like" en un chiste
 * Actualiza los contadores en el DOM sin recargar la página
 * @param {number} id - ID del chiste*/
async function likeJoke(id) {
    try {
        const data = await postJson(`/JokeModels/Like/${id}`);
        if (data) {
            // Actualizar contadores en la tarjeta del Index
            document.getElementById(`likes-${id}`).textContent = data.likes;
            document.getElementById(`dislikes-${id}`).textContent = data.dislikes;
            // Actualizar contadores en Details (si existe)
            const likesCount = document.getElementById('likesCount');
            const dislikesCount = document.getElementById('dislikesCount');
            if (likesCount) likesCount.textContent = data.likes;
            if (dislikesCount) dislikesCount.textContent = data.dislikes;
            async function likeJoke(id) {
                try {
                    const data = await postJson(`/JokeModels/Like/${id}`);
                    if (data) {
                        // Actualizar contadores en la tarjeta del Index
                        const likesElement = document.getElementById(`likes-${id}`);
                        const dislikesElement = document.getElementById(`dislikes-${id}`);

                        if (likesElement) likesElement.textContent = data.likes;
                        if (dislikesElement) dislikesElement.textContent = data.dislikes;

                        // Actualizar contadores en Details (si existe)
                        const likesCount = document.getElementById('likesCount');
                        const dislikesCount = document.getElementById('dislikesCount');
                        if (likesCount) likesCount.textContent = data.likes;
                        if (dislikesCount) dislikesCount.textContent = data.dislikes;
                        showNotification('¡Like agregado!', 'success');
                    }
                } catch (e) {
                    console.error('Like error:', e);
                    showNotification('Error al dar like. Por favor, intenta de nuevo.', 'error');
                }
            }
        }
    } catch (e) {
        console.error('Like error:', e);
        alert('Error al dar like. Por favor, intenta de nuevo.');
    }
}
// FUNCIÓN: Dar Dislike a un chiste
/**Registra un "dislike" en un chiste
 * Actualiza los contadores en el DOM sin recargar la página
 * @param {number} id - ID del chiste*/
async function dislikeJoke(id) {
    try {
        const data = await postJson(`/JokeModels/Dislike/${id}`);
        if (data) {
            // Actualizar contadores en la tarjeta del Index
            document.getElementById(`likes-${id}`).textContent = data.likes;
            document.getElementById(`dislikes-${id}`).textContent = data.dislikes;

            // Actualizar contadores en Details (si existe)
            const likesCount = document.getElementById('likesCount');
            const dislikesCount = document.getElementById('dislikesCount');
            if (likesCount) likesCount.textContent = data.likes;
            if (dislikesCount) dislikesCount.textContent = data.dislikes;
            async function dislikeJoke(id) {
                try {
                    const data = await postJson(`/JokeModels/Dislike/${id}`);
                    if (data) {
                        // Actualizar contadores en la tarjeta del Index
                        const likesElement = document.getElementById(`likes-${id}`);
                        const dislikesElement = document.getElementById(`dislikes-${id}`);

                        if (likesElement) likesElement.textContent = data.likes;
                        if (dislikesElement) dislikesElement.textContent = data.dislikes;

                        // Actualizar contadores en Details (si existe)
                        const likesCount = document.getElementById('likesCount');
                        const dislikesCount = document.getElementById('dislikesCount');
                        if (likesCount) likesCount.textContent = data.likes;
                        if (dislikesCount) dislikesCount.textContent = data.dislikes;
                        showNotification('Dislike agregado', 'success');
                    }
                } catch (e) {
                    console.error('Dislike error:', e);
                    
                    showNotification('Error al dar dislike. Por favor, intenta de nuevo.', 'error');
                }
            }
        }
    } catch (e) {
        console.error('Dislike error:', e);
        alert('Error al dar dislike. Por favor, intenta de nuevo.');
    }
}
// FUNCIÓN: Mostrar notificación toast
/**Muestra una notificación temporal en la esquina superior derecha
 * @param {string} message - Mensaje a mostrar
 * @param {string} type - Tipo de notificación ('success' o 'error')*/
function showNotification(message, type) {
    const notification = document.createElement('div');
    notification.textContent = message;
    notification.className = `toast-notification toast-${type}`;

    document.body.appendChild(notification);

    // Remover después de 2.5 segundos
    setTimeout(() => notification.remove(), 2500);
}