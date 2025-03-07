function setCurrentRoute(link) {
    sessionStorage.setItem('j3rGjf', link);

    $('nav').find(`a[href="${link}"].nav-link`).find('p').css({ 'color': 'red' });
}

function getCurrentLink() {
    const currentRoute = sessionStorage.getItem('j3rGjf');

    $(`ul[data-menu-accordion-child]`).removeClass('cancel-max-height-0');

    if (currentRoute !== undefined && currentRoute !== null &&  currentRoute !== '') {
        const elmt = $('nav').find(`a[href="${currentRoute}"].nav-link`);

        $(`[data-parent-id]`).removeClass('w3-show');

        const dataChildId = elmt.attr(`data-child-id`);

        $(`[data-parent-id="${dataChildId}"]`).addClass('w3-show');

        const dataMenuAccordionChild = elmt.parent().parent();

        if (dataMenuAccordionChild.is('ul[data-menu-accordion-child]')) {
            dataMenuAccordionChild.addClass(`cancel-max-height-0`);
        }

        //elmt.find('p').css({ 'color': 'red' });
        elmt.find('p').addClass('image-clignote');

        return currentRoute;
    }

    return '';
}

$(document).ready(() => {
    getCurrentLink();
});

$('nav').find(`a.nav-link`).on('click', (e) => {
    $('nav').find(`a.nav-link`).find('p').css({ 'color': 'title' });

    const id = $(e.currentTarget).prop('href');

    const str = id.split(User.origin);

    setCurrentRoute(`..${str[1]}`);
});

$('aside a[href="../Home/TdbAccueil"]').on('click', () => {
    const link = getCurrentLink();

    $('nav').find(`a[href="${link}"].nav-link`).find('p').css({ 'color': 'black' });

    sessionStorage.setItem('j3rGjf', '');
});

$('[data-menu-accordion]').on('click', (e) => {
    const dataMenuAccordionChild = $(e.currentTarget).find(`ul[data-menu-accordion-child]`);

    if (dataMenuAccordionChild.hasClass(`cancel-max-height-0`)) {
        $(`ul[data-menu-accordion-child]`).removeClass('cancel-max-height-0');

        dataMenuAccordionChild.removeClass(`cancel-max-height-0`);
    } else {
        $(`ul[data-menu-accordion-child]`).removeClass('cancel-max-height-0');

        dataMenuAccordionChild.addClass(`cancel-max-height-0`);
    }
});
