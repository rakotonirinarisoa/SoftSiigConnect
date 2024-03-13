function setCurrentRoute(link) {
    localStorage.setItem('j3rGjf', link);

    $('nav').find(`a[href="../${link}"].nav-link`).css({ 'color': 'red' });
}

function getCurrentLink() {
    const currentRoute = localStorage.getItem('j3rGjf');

    if (currentRoute) {
        $('nav').find(`a[href="../${currentRoute}"].nav-link`).css({ 'color': 'red' });

        return currentRoute;
    }

    return '';
}

$('nav').find(`a.nav-link`).on('click', (e) => {
    const id = $(e.currentTarget).prop('href');

    alert(id === `${User.origin}//${id}`);
});
