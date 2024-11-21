$(document).ready(function () {
    let turnoId; // Variable global para almacenar el turnoId

    // Al hacer clic en el botón "Ausente"
    $('.mis-reservas-tabla').on('click', '.btn-ausente', function () {
        turnoId = $(this).data('turno-id'); // Capturar el ID del turno
        $('#confirmacionModal').modal('show'); // Mostrar el modal
    });

    // Confirmar la acción en el modal
    $('#btnConfirmar').on('click', function () {
        if (!turnoId) {
            mostrarAlerta('danger', 'Error: Turno no encontrado.');
            return;
        }

        // Ocultar el modal
        $('#confirmacionModal').modal('hide');

        // Realizar la solicitud AJAX
        $.ajax({
            url: '/Profesionales/ClienteAusente',
            type: 'POST',
            contentType: 'application/x-www-form-urlencoded', // Formato correcto para POST simple
            data: { idTurno: turnoId }, // Enviar ID del turno
            success: function (response) {
                if (response.success) {
                    mostrarAlerta('success', 'El cliente ha sido marcado como ausente.');
                    location.reload(); // Recargar la página para actualizar la tabla
                } else {
                    mostrarAlerta('danger', response.message);
                }
            },
            error: function () {
                mostrarAlerta('danger', 'Error en la solicitud. Inténtelo nuevamente.');
            }
        });
    });

    // Función para mostrar alertas
    function mostrarAlerta(tipo, mensaje) {
        var alerta = `
            <div class="alert alert-${tipo} alert-dismissible fade show" role="alert">
                ${mensaje}
                <button type="button" class="close" data-dismiss="alert" aria-label="Close">
                    <span aria-hidden="true">&times;</span>
                </button>
            </div>
        `;
        $('#alertContainer').append(alerta); // Agregar la alerta al contenedor

        // Desaparición automática después de 4 segundos
        setTimeout(function () {
            $('.alert').alert('close'); // Cierra la alerta automáticamente
        }, 4000);
    }

    // Verificar mensajes almacenados en localStorage (si aplica)
    const mensajeExito = localStorage.getItem('mensajeExito');
    if (mensajeExito) {
        mostrarAlerta('success', mensajeExito);
        localStorage.removeItem('mensajeExito');
    }
});
