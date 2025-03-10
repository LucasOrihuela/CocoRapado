$(document).ready(function () {
    var turnoId; // Variable para almacenar el ID del turno a eliminar

    // Mostrar el modal de confirmación y almacenar el ID del turno a eliminar
    $('button[data-toggle="modal"]').on('click', function () {
        turnoId = $(this).data('id');
    });

    // Confirmar la acción en el modal
    $('#btnConfirmar').on('click', function () {
        // Llamar a la acción de eliminación de turno
        $.ajax({
            url: '/MisReservas/CancelarTurno', // Ruta de la acción que elimina el turno
            type: 'POST',
            data: { turnoId: turnoId },
            success: function (response) {
                if (response.exito) {                    
                    location.reload();
                } else {
                    // Mostrar alerta de error
                    alert('No se pudo cancelar el turno. Intenta nuevamente.');
                }
            },
            error: function () {
                // Mostrar alerta de error si la solicitud falla
                $('#confirmacionModal').modal('hide');
                alert('Hubo un error al intentar cancelar el turno.');
            },
        });
    });
});
