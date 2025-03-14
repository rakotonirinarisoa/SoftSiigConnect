let inactivityTimeout;

// Réinitialise le timer d'inactivité à chaque interaction
function resetInactivityTimer() {
    clearTimeout(inactivityTimeout);
    inactivityTimeout = setTimeout(() => {
        // Si l'utilisateur est inactif pendant un certain temps, tu peux lancer la redirection
        window.location.href = window.location.origin;   // Redirige vers la page de login après inactivité
    },1800000); // 5 minutes d'inactivité avant de rediriger
}

// Ajoute des écouteurs pour détecter l'activité
window.addEventListener('click', resetInactivityTimer);
window.addEventListener('keydown', resetInactivityTimer);

// Initialise le timer d'inactivité dès que la page est chargée
resetInactivityTimer();