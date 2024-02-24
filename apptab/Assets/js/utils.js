function formatCurrency(amount) {
    return new Intl.NumberFormat("fr-FR").format(amount);
}

function formatDate(date) {
    if (!dayjs(date).isValid()) {
        return '';
    }

    return dayjs(date).format('DD/MM/YYYY');
}
