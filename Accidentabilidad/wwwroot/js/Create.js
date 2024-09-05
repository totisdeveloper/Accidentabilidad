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


document.getElementById('Incapacidadinicial').addEventListener('change', calculateDaysDifference);
document.getElementById('Iniciolabores').addEventListener('change', calculateDaysDifference);

function calculateDaysDifference() {
    // Obtener las fechas del formulario
    const startDate = new Date(document.getElementById('Incapacidadinicial').value);
    const endDate = new Date(document.getElementById('Iniciolabores').value);

    // Comprobar que ambas fechas estén seleccionadas
    if (!isNaN(startDate) && !isNaN(endDate)) {
        // Calcular la diferencia en milisegundos
        const timeDifference = endDate - startDate;

        // Convertir la diferencia de milisegundos a días
        const daysDifference = timeDifference / (1000 * 3600 * 24);


        if (daysDifference >= 0 && daysDifference <= 31) {

            // Mostrar el resultado en el campo de texto
            document.getElementById('dias_sub').value = daysDifference;
        }
        else {
            alert('El numero de dias no cumple con el rango permitido permitido')
        }
    }
    else {
        document.getElementById('dias_sub').value = 0;
    }
}



















