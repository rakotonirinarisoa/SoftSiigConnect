function formatCurrency(amount) {
    return new Intl.NumberFormat("fr-FR").format(amount);
}

function formatDate(date) {
    if (!dayjs(date).isValid()) {
        return '';
    }

    return dayjs(date).format('DD/MM/YYYY');
}

function exportTableToExcel(tableID, filename = 'RAS') {
    let downloadLink;

    const dataType = 'application/vnd.ms-excel';

    const tableSelect = document.getElementById(tableID);

    // Create a new Blob with the correct encoding
    const blob = new Blob(['\ufeff', tableSelect.outerHTML], { type: dataType });

    // Specify file name
    filename = filename ? filename + '.xls' : 'excel_data.xls;';

    if (confirm("Voulez-vous le télécharger ?")) {
        if (navigator.msSaveOrOpenBlob) {
            // For Internet Explorer
            navigator.msSaveOrOpenBlob(blob, filename);
        } else {
            // For other browsers
            // Create download link element
            downloadLink = document.createElement("a");

            // Create a link to the blob
            downloadLink.href = URL.createObjectURL(blob);

            // Setting the file name
            downloadLink.download = filename;

            // Append the link to the body
            document.body.appendChild(downloadLink);

            // Trigger the click event
            downloadLink.click();

            // Remove the link from the body
            document.body.removeChild(downloadLink);
        }
    }
}
