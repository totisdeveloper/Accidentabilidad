$(document).ready(function () {
    // Obtener la tabla y las filas
    var table = $('#myTable');

    // Función para filtrar la tabla por fecha
    function filterByDate() {
        var startDate = $('#startDateFilter').val();
        var endDate = $('#endDateFilter').val();


        // Convertir fechas a objetos Date solo si no están vacías
        var start = startDate ? new Date(startDate) : null;
        var end = endDate ? new Date(endDate) : null;

        table.find('tbody tr').each(function () {

            var row = $(this);
            var dateText = row.find('.fecha-modificacion').text().substring(0, 10);
            var date = new Date(dateText); // Asumiendo que el formato de dateText es compatible

            var showRow = true;

            if (start) {
                showRow = showRow && date >= start;
            }

            if (end) {
                showRow = showRow && date <= end;
            }

            if (showRow) {
                row.show();
            } else {
                row.hide();
            }
        });
    }

    // Filtrar por texto en columnas
    $('.column-filter').on('keyup', function () {
        var index = $(this).parent().index();
        var value = $(this).val().toLowerCase();

        table.find('tbody tr').each(function () {
            var row = $(this);
            var cellText = row.find('td').eq(index).text().toLowerCase();

            if (cellText.indexOf(value) !== -1) {
                row.show();
            } else {
                row.hide();
            }
        });
    });

    // Filtrar por fecha
    $('#startDateFilter, #endDateFilter').on('change', function () {
        filterByDate();
    });

    // Limpiar todos los filtros
    $('#clearFiltersButton').on('click', function () {
        $('.column-filter').val('');
        $('#startDateFilter').val('');
        $('#endDateFilter').val('');
        table.find('tbody tr').show();
    });
});
