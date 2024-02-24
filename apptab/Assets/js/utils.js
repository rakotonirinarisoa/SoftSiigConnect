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

    const tableHTML = tableSelect.outerHTML.replace(/ /g, '%20');

    alert("OK");

    // Specify file name
    filename = filename ? filename + '.xls' : 'excel_data.xls;';

    if (confirm("Voulez-vous le télécharger ?")) {
        if (navigator.msSaveOrOpenBlob) {
            const blob = new Blob(['\ufeff', tableHTML], {
                type: dataType
            });

            navigator.msSaveOrOpenBlob(blob, filename);
        } else {
            // Create download link element
            downloadLink = document.createElement("a");

            document.body.appendChild(downloadLink);

            // Create a link to the file
            downloadLink.href = 'data:' + dataType + ', ' + tableHTML;

            // Setting the file name
            downloadLink.download = filename;

            //triggering the function
            downloadLink.click();
        }
    }
}
