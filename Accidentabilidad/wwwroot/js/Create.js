function filtrarLista() {
    var input, filter, ul, li, a, i, txtValue;
    input = document.getElementById('InputBuscaNomina');
    filter = input.value.toUpperCase();
    select = document.getElementById("lista");
    options = select.getElementsByTagName('option');

    for (i = 0; i < options.length; i++) {
        txtValue = options[i].textContent || options[i].innerText;
        if (txtValue.toUpperCase().indexOf(filter) > -1) {
            options[i].style.display = "";
        } else {
            options[i].style.display = "none";
        }
    }
}

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
    var link = `/Clients/Recaida?inputValue=${encodeURIComponent(inputValue)}`;
    window.location.href = link;
});










