function disconnect() {
    sessionStorage.setItem("user", null);
    window.location = window.location.origin;
}