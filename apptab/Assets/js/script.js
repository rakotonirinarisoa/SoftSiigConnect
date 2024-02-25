let Origin;

function disconnect() {
    Origin = User.origin;
    sessionStorage.setItem("user", null);
    window.location = Origin;
}
