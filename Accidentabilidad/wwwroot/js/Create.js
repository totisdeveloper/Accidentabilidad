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
    var link = `/Clients/Recaida?inputValue=${encodeURIComponent(inputValue)}`;
    window.location.href = link;
});










