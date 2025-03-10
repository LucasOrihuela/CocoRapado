$(document).ready(function () {
    // Enviar formulario de creación de servicio
    $('#crearServicioForm').on('submit', function (e) {
        e.preventDefault(); // Evita el envío del formulario normal

        $.ajax({
            type: 'POST',
            url: $(this).attr('action'),
            data: $(this).serialize(), // Serializa los datos del formulario
            success: function (response) {
                if (response.success) {
                    // Guardar mensaje en localStorage
                    localStorage.setItem('mensajeExito', 'El servicio se ha agregado correctamente.');
                    window.location.replace('/ABM/ABMServicios'); // Redirigir a la lista de servicios
                } else {
                    mostrarAlerta('danger', response.message); // Mostrar mensaje de error si la respuesta no es exitosa
                }
            },
            error: function (xhr, status, error) {
                console.error('Error al enviar el formulario:', error); // Log para errores
                mostrarAlerta('danger', 'Ocurrió un error al procesar la solicitud.');
            }
        });
    });

    // Filtrar la tabla en tiempo real
    $('#searchInput').on('keyup', function () {
        var searchValue = $(this).val().toLowerCase();

        $('#ServiciosTableBody tr').filter(function () {
            var nombre = $(this).find('td:nth-child(1)').text().toLowerCase();
            var apellido = $(this).find('td:nth-child(2)').text().toLowerCase();
            var servicio = $(this).find('td:nth-child(3)').text().toLowerCase();

            $(this).toggle(nombre.indexOf(searchValue) > -1 || apellido.indexOf(searchValue) > -1 || servicio.indexOf(searchValue) > -1);
        });
    });

    // Función para cargar los datos en el formulario de edición
    window.cargarDatosEdicion = function (id, nombre, descripcion, duracion, precioMin, precioMax) {
        $('#editId').val(id);
        $('#editServicioNombre').val(nombre);
        $('#editServicioDescripcion').val(descripcion);
        $('#editDuracionMinutos').val(duracion);
        $('#editPrecioMin').val(precioMin);
        $('#editPrecioMax').val(precioMax);
        $('#editModal').modal('show');
    };

    // Guardar cambios en el servicio
    $('#saveChanges').on('click', function (e) {
        e.preventDefault();

        var formData = $('#editServicioForm').serialize();
        console.log('Datos del formulario:', formData);

        $.ajax({
            type: 'POST',
            url: $('#editServicioForm').attr('action'),
            data: $('#editServicioForm').serialize(), // Serializa los datos del formulario
            success: function (response) {
                if (response.success) {
                    // Guardar mensaje en localStorage
                    localStorage.setItem('mensajeExito', 'El servicio se ha editado correctamente.');
                    window.location.replace('/ABM/ABMServicios');
                } else {
                    mostrarAlerta('danger', response.message); // Mostrar mensaje de error
                }
                $('#editModal').modal('hide');
            },
            error: function (xhr, status, error) {
                console.error('Error al enviar el formulario:', error); // Imprimir error en consola
                mostrarAlerta('danger', 'Ocurrió un error al procesar la solicitud.');
            }
        });
    });

    // Función para eliminar un servicio
    window.eliminarServicio = function (id) {
        // Mostrar el modal de confirmación
        $('#confirmacionModal').modal('show');

        // Cuando el usuario hace clic en "Confirmar"
        $('#btnConfirmar').off('click').on('click', function () {
            // Ocultar el modal
            $('#confirmacionModal').modal('hide');

            // Ejecutar la solicitud AJAX para eliminar el servicio
            $.ajax({
                type: 'POST',
                url: '/ABM/ABMServicios/Delete', // URL de eliminación
                data: { id: id },
                success: function (response) {
                    if (response.success) {
                        localStorage.setItem('mensajeExito', response.message);
                        window.location.replace('/ABM/ABMServicios');
                    } else {
                        mostrarAlerta('danger', response.message); // Mostrar mensaje de error
                    }
                },
                error: function () {
                    mostrarAlerta('danger', 'Ocurrió un error al eliminar el servicio.');
                }
            });
        });
    };


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

        // Desaparición automática después de 5 segundos
        setTimeout(function () {
            $('.alert').alert('close'); // Cierra la alerta automáticamente
        }, 4000);
    }

    // Verificar si hay un mensaje de éxito en localStorage y mostrarlo
    var mensajeExito = localStorage.getItem('mensajeExito');
    if (mensajeExito) {
        mostrarAlerta('success', mensajeExito); // Mostrar alerta de éxito
        localStorage.removeItem('mensajeExito'); // Limpiar el mensaje de localStorage
    }
});
