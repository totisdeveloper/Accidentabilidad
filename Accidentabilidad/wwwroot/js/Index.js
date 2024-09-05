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



