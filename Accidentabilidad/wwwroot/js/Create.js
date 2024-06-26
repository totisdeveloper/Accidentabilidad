$(document).ready(function () {
    $('#exampleSelect').select2();
})

function toggleDiv() {
    var div = document.getElementById("myDiv");
    var checkbox = document.getElementById("toggleSwitch");
    if (checkbox.checked) {
        div.style.display = "block";
    } else {
        div.style.display = "none";
    }
}

document.getElementById('btnBuscarFolio').addEventListener('click', function () {

    var inputValue = document.getElementById('BuscaFolio').value;

    if (inputValue != '') {
        if (inputValue.length == 18) {
            var link = `/Clients/Recaida?inputValue=${encodeURIComponent(inputValue)}`;
            window.location.href = link;
        }
        else {
            alert("Número de folio invalido")
        }
    }
    else {
        alert("Ingrese número de folio")
    }
});










