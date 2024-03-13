function setCurrentRoute(link) {
    localStorage.setItem('j3rGjf', link);

    $('nav').find(`a[href="${link}"].nav-link`).find('p').css({ 'color': 'red' });
}

function getCurrentLink() {
    const currentRoute = localStorage.getItem('j3rGjf');

    console.log($('nav').find(`a[href="${currentRoute}"].nav-link`));

    if (currentRoute) {
        $('nav').find(`a[href="${currentRoute}"].nav-link`).find('p').css({ 'color': 'red' });

        return currentRoute;
    }

    return '';
}

$(document).ready(() => {
    console.log('d');

    getCurrentLink();
});

$('nav').find(`a.nav-link`).on('click', (e) => {
    const id = $(e.currentTarget).prop('href');

    setCurrentRoute(id);
});