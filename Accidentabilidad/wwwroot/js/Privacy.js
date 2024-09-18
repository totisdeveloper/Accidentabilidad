$(document).ready(function () {

    // Recuperar la posición del scroll desde el localStorage
    const savedScrollPosition = localStorage.getItem('scrollPosition');
    if (savedScrollPosition) {
        $('#tableWrapper').scrollTop(savedScrollPosition);
    }

    // Guardar la posición del scroll antes de que se recargue la página
    $('#tableWrapper').on('scroll', function () {
        localStorage.setItem('scrollPosition', $(this).scrollTop());
    });


    // Filtros de texto para cualquier columna
    $('.filter-input').on('keyup', function () {
        var input = $(this);  // El input de filtro actual
        var columnIndex = input.closest('th').index();  // Obtener el índice de la columna

        var filterValue = input.val().toLowerCase();  // El valor que se busca

        $('#myTable tbody tr').each(function () {
            var row = $(this);  // Cada fila de la tabla
            var cell = row.find('td').eq(columnIndex);  // La celda correspondiente al índice de la columna

            if (cell.text().toLowerCase().indexOf(filterValue) > -1) {
                row.show();  // Mostrar la fila si coincide con el filtro
            } else {
                row.hide();  // Ocultar la fila si no coincide
            }
        });
    });

    // Filtro por rango de fechas
    $('.date-filter').on('change', function () {
        var startDate = $('#filterStartDate').val();
        var endDate = $('#filterEndDate').val();
        var columnIndex = $('#filterStartDate').closest('th').index(); // Índice de la columna de fechas

        $('#myTable tbody tr').each(function () {
            var row = $(this);
            var dateValue = row.find('td').eq(columnIndex).text();
            var rowDate = parseDate(dateValue); // Convertir la fecha de la fila en objeto Date

            // Convertir las fechas seleccionadas en objetos Date
            var start = startDate ? new Date(startDate) : null;
            var end = endDate ? new Date(endDate) : null;

            // Comparar la fecha de la fila con el rango de fechas
            if ((!start || rowDate >= start) && (!end || rowDate <= end)) {
                row.show();  // Mostrar si está dentro del rango
            } else {
                row.hide();  // Ocultar si no está dentro del rango
            }
        });
    });

    // Función para convertir fecha en formato "dd/MM/yyyy" a un objeto Date
    function parseDate(input) {
        var parts = input.split('/');
        return new Date(parts[2], parts[1] - 1, parts[0]); // Año, Mes (0-indexed), Día
    }

    $('#clearFiltersBtn').on('click', function () {
        // Limpiar los filtros de texto
        $('.filter-input').val('');

        // Limpiar los filtros de fecha
        $('#filterStartDate').val('');
        $('#filterEndDate').val('');

        // Limpiar el filtro de mes
        $('#filterMonth').val('');

        // Mostrar todas las filas de la tabla
        $('#myTable tr').show();
    });
});