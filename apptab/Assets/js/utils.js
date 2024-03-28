function formatCurrency(amount) {
    return new Intl.NumberFormat("fr", {
        style: "decimal",
        minimumFractionDigits: 2,
        currencySign: "accounting",
    }).format(amount);
}

function formatDate(date) {
    if (!dayjs(date).isValid()) {
        return '';
    }

    return dayjs(date).format('DD/MM/YYYY');
}

function formatDateRFR(date) {
    if (!dayjs(date).isValid()) {
        return '';
    }

    return dayjs(date).format('YYYY-MM-DD');
}

function tableToExcel(tableId, name, callback = undefined) {
    let table;
    const uri = 'data:application/vnd.ms-excel;base64,'
        ,
        template = '<html xmlns:o="urn:schemas-microsoft-com:office:office" xmlns:x="urn:schemas-microsoft-com:office:excel" xmlns="http://www.w3.org/TR/REC-html40"><meta http-equiv="content-type" content="application/vnd.ms-excel; charset=UTF-8"><head><!--[if gte mso 9]><xml><x:ExcelWorkbook><x:ExcelWorksheets><x:ExcelWorksheet><x:Name>{worksheet}</x:Name><x:WorksheetOptions><x:DisplayGridlines/></x:WorksheetOptions></x:ExcelWorksheet></x:ExcelWorksheets></x:ExcelWorkbook></xml><![endif]--></head><body><table>{table}</table></body></html>'
        , base64 = function (s) {
            return window.btoa(unescape(encodeURIComponent(s)))
        }
        , format = function (s, c) {
            return s.replace(/{(\w+)}/g, function (m, p) {
                return c[p];
            })
        }

    if (!tableId.nodeType) {
        table = document.getElementById(tableId);
    }
    //const er = table.innerHTML.replace(/^0/g, "'0")
    const ctx = { worksheet: name || 'Worksheet', table: table.innerHTML };

    if (callback) {
        callback();
    }

    const a = document.createElement('a');

    a.href = uri + base64(format(template, ctx));

    a.download = name + '.xls';

    a.click();
}

function isNullOrUndefined(input) {
    return input === null || input === undefined;
} 

function exportTableToPdf(tableId, filename, columnsIndexesToHide = []) {
    $('body').append(`
        <div id="tmp" style="display: none;" ></div >
    `);

    const tmpDiv = $('body').find('#tmp');

    const id = Date.now();

    tmpDiv.html($(`#${tableId}`).parent().attr('id', 'bar').html());

    tmpDiv.find('table').attr('id', id);

    tmpDiv.find('.dt-search').remove();
    tmpDiv.find('.btn-group').remove();
    tmpDiv.find('.dt-info').remove();
    tmpDiv.find('ul.pagination').remove();
    tmpDiv.find('tfoot').remove();
    tmpDiv.find('.dt-column-order').remove();
    tmpDiv.find('table').css({
        'width': 'auto',
        'fontSize': '9px',
        'border': '0px',
        'overflow-x': 'hidden'
    });

    const htmlContent = tmpDiv.html();

    tmpDiv.remove();

    const formData = new FormData();

    formData.append("suser.LOGIN", User.LOGIN);
    formData.append("suser.PWD", User.PWD);

    formData.append('id', String(id));
    formData.append('element', htmlContent);
    formData.append('columnsIndexes', JSON.stringify(columnsIndexesToHide));
    formData.append('filename', filename);

    $.ajax({
        type: 'POST',
        url: Origin + '/Pdf/ExportToPdf',
        data: formData,
        cache: false,
        contentType: false,
        processData: false,
        beforeSend: function () {
            loader.removeClass('display-none');
        },
        complete: function () {
            loader.addClass('display-none');
        },
        success: function (result) {
            var res = JSON.parse(result);

            if (res.type === 'error') {
                alert(res.msg);

                return;
            }

            window.location.href = Origin + 'Pdf/Index';
        },
        error: function () {
            alert('Problème de connexion!');
        }
    });
}
