document.addEventListener("DOMContentLoaded", function () {

    var tabla = document.getElementById("myTable");

    for (var i = 1; i < tabla.rows.length; i++) {


        const texto = (tabla.rows[i].cells[8].innerText);

        const palabra = "pendiente";
        const regex = new RegExp(`\\b${palabra}\\b`, 'i');

        if (regex.test(texto)) {
            tabla.rows[i].cells[8].style.backgroundColor = "#fdfd96";
        }
    }
});

function exportTableToExcel(tableID, filename = '') {

    var table = document.getElementById(tableID);
    var wb = XLSX.utils.table_to_book(table, { sheet: "Sheet1" });
    var wbout = XLSX.write(wb, { bookType: 'xlsx', type: 'binary' });

    function s2ab(s) {
        var buf = new ArrayBuffer(s.length);
        var view = new Uint8Array(buf);
        for (var i = 0; i < s.length; i++) view[i] = s.charCodeAt(i) & 0xFF;
        return buf;
    }

    var blob = new Blob([s2ab(wbout)], { type: "application/octet-stream" });
    var link = document.createElement("a");
    link.href = URL.createObjectURL(blob);
    link.download = filename ? filename + '.xlsx' : 'excel_data.xlsx';
    document.body.appendChild(link);
    link.click();
    document.body.removeChild(link);
}


