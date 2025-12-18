function getAntiForgeryToken() {
    return document.querySelector('input[name="__RequestVerificationToken"]')?.value || '';
}

async function postJson(url) {
    const res = await fetch(url, {
        method: 'POST',
        credentials: 'same-origin', // ✅ CLAVE: manda cookie de login
        headers: {
            'Content-Type': 'application/json',
            'RequestVerificationToken': getAntiForgeryToken()
        }
    });

    // Si no está autorizado, Identity suele responder 302/401/403
    if (!res.ok) {
        // Intenta detectar si te mandó a login
        if (res.status === 401 || res.status === 403) {
            window.location.href = '/Identity/Account/Login';
            return null;
        }
        // 302 a veces se ve como ok=false/opaquereirect (depende)
        throw new Error(`HTTP ${res.status}`);
    }

    // Puede fallar si vino HTML en vez de JSON
    const contentType = res.headers.get('content-type') || '';
    if (!contentType.includes('application/json')) {
        // probablemente te devolvió HTML del login
        window.location.href = '/Identity/Account/Login';
        return null;
    }

    return await res.json();
}

function applyVoteToDom(id, data) {
    if (!data?.success) return;

    // Index
    document.getElementById(`likes-${id}`)?.textContent = data.likes;
    document.getElementById(`dislikes-${id}`)?.textContent = data.dislikes;

    // Details
    document.getElementById('likesCount')?.textContent = data.likes;
    document.getElementById('dislikesCount')?.textContent = data.dislikes;
}

async function likeJoke(id) {
    try {
        const data = await postJson(`/JokeModels/Like/${id}`);
        applyVoteToDom(id, data);
    } catch (e) {
        console.error('Like error:', e);
    }
}

async function dislikeJoke(id) {
    try {
        const data = await postJson(`/JokeModels/Dislike/${id}`);
        applyVoteToDom(id, data);
    } catch (e) {
        console.error('Dislike error:', e);
    }
}
