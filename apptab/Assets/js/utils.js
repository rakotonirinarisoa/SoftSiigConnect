function formatCurrency(amount) {
    let nombre = amount.toLocaleString("fr-FR",{
                style: 'decimal',
                minimumFractionDigits: 2,
                currencySign: "accounting",
    });

    nombre += '';
    var sep = ' ';
    var reg = /(\d+)(\d{3})/;
    while (reg.test(nombre)) {
        nombre = nombre.replace(reg, '$1' + sep + '$2');
    }

    return nombre.toString().replace('.',',');
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

function exportTableToPdf(tableId, filename, header, footer, columnsIndexesToHide = []) {
    $('body').append(`
        <div id="tmp" style="display: none;" ></div >
    `);

    const tmpDiv = $('body').find('#tmp');

    const id = Date.now();

    tmpDiv.html($(`#${tableId}`).parent().attr('id', 'bar').html());

    tmpDiv.find('table').attr('id', id);

    const table = new DataTable(tmpDiv.find(`#${id}`), {
        dom: 'Bfrtip',
        searching: false,
        paging: false,
        ordering: false,
        initComplete: function () {
            $(`thead td`).removeClass('dt-orderable-asc').removeClass('dt-orderable-desc');
        }
    });

    for (let i = 0; i < columnsIndexesToHide.length; i += 1) {
        table.column(columnsIndexesToHide[i]).visible(false);
    }

    tmpDiv.find('.dt-search').remove();
    tmpDiv.find('.btn-group').remove();
    tmpDiv.find('.dt-info').remove();
    tmpDiv.find('ul.pagination').remove();
    tmpDiv.find('tfoot').remove();
    tmpDiv.find('.dt-column-order').remove();
    tmpDiv.find('.dt-buttons').remove();
    tmpDiv.find('table').css({
        'width': 'auto',
        'fontSize': '9px',
        'border': '0px',
        'overflow-x': 'hidden'
    });
    tmpDiv.find('.dt-info').remove();
    tmpDiv.find('caption').remove();

    tmpDiv.find('tr').each(function (_) {
        $(this).removeClass('select-text demoRayure');
    });

    tmpDiv.find('colgroup').each(function (i) {
        if (i !== 0) {
            $(this).remove();
        }
    });

    const formData = new FormData();

    formData.append("suser.LOGIN", User.LOGIN);
    formData.append("suser.PWD", User.PWD);

    formData.append('id', String(id));
    formData.append('element', tmpDiv.html());
    formData.append('filename', filename);
    formData.append('header', header);
    formData.append('footer', footer);

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

            $('body').find('#tmp').remove();

            window.location.href = Origin + 'Pdf/Index';
        },
        error: function () {
            alert('Probl�me de connexion!');
        }
    });
}

function display404() {
    $('body').html(`
        <h1 style="font-size: 128px; position: absolute; top: 50%; left: 50%; transform: translate(-50%, -50%);">404</h1>
    `);
}

function sanitizeHTML(input) {
    return input.replaceAll('&lt;', '<').replaceAll('&gt;', '>').replaceAll('&quot;', '"');
}
