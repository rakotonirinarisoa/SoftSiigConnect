function formatCurrency(amount) {
    return new Intl.NumberFormat("fr-FR").format(amount);
}

function formatDate(date) {
    return dayjs(date).format('DD/MM/YYYY');
}
