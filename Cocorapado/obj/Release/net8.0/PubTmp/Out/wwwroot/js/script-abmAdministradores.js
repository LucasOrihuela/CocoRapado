$(document).ready(function () {

    // Enviar formulario de creación de Administrador
    $('#crearAdministradorForm').on('submit', function (e) {
        e.preventDefault(); // Evita el envío del formulario normal

        $.ajax({
            type: 'POST',
            url: $(this).attr('action'),
            data: $(this).serialize(), // Serializa los datos del formulario
            success: function (response) {
                if (response.success) {
                    // Guardar mensaje en localStorage
                    localStorage.setItem('mensajeExito', 'El Administrador se ha agregado correctamente.');
                    window.location.replace('/ABM/ABMAdministradores'); // Redirigir a la lista de Administradores
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

    // Filtrar tabla en tiempo real
    $('#searchInput').on('keyup', function () {
        var searchValue = $(this).val().toLowerCase();

        $('#AdministradoresTableBody tr').filter(function () {
            var nombre = $(this).find('td:nth-child(1)').text().toLowerCase();
            var apellido = $(this).find('td:nth-child(2)').text().toLowerCase();
            var servicio = $(this).find('td:nth-child(3)').text().toLowerCase();

            $(this).toggle(nombre.indexOf(searchValue) > -1 || apellido.indexOf(searchValue) > -1 || servicio.indexOf(searchValue) > -1);
        });
    });

    // Evento click para el botón de editar
    $(document).on('click', '.btn-warning', function (e) {
        e.preventDefault();
        const id = $(this).attr('href').split('/').pop(); // Obtiene el id del URL

        $.ajax({
            type: 'GET',
            url: `/ABM/ABMAdministradores/ObtenerAdministrador/${id}`, // URL para obtener detalles del Administrador
            success: function (response) {
                if (response) {
                    // Rellenar el formulario de edición con los datos recibidos
                    $('#editId').val(id);
                    $('#editNombre').val(response.nombre);
                    $('#editApellido').val(response.apellido);
                    $('#editCorreo').val(response.correo);
                    $('#editTelefono').val(response.telefono);
                    // Mostrar el modal de edición
                    $('#editModal').modal('show');
                } else {
                    mostrarAlerta('danger', 'No se pudo cargar la información del Administrador.');
                }
            },
            error: function () {
                mostrarAlerta('danger', 'Ocurrió un error al obtener los datos del Administrador.');
            }
        });
    });

    // Guardar cambios en el Administrador
    $('#saveChanges').on('click', function (e) {
        e.preventDefault();
        $.ajax({
            type: 'POST',
            url: $('#editAdministradorForm').attr('action'),
            data: $('#editAdministradorForm').serialize(), // Serializa los datos del formulario
            success: function (response) {
                if (response.success) {
                    // Guardar mensaje en localStorage
                    localStorage.setItem('mensajeExito', 'El Administrador se ha editado correctamente.');
                    window.location.replace('/ABM/ABMAdministradores');
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

    // Función para eliminar un Administrador
    window.eliminarAdministrador = function (id) {
        // Mostrar el modal de confirmación
        $('#confirmacionModal').modal('show');

        // Cuando el usuario hace clic en "Confirmar"
        $('#btnConfirmar').off('click').on('click', function () {
            // Ocultar el modal
            $('#confirmacionModal').modal('hide');

            // Ejecutar la solicitud AJAX para eliminar el Administrador
            $.ajax({
                type: 'POST',
                url: '/ABM/ABMAdministradores/Delete', // URL de eliminación
                data: { id: id },
                success: function (response) {
                    if (response.success) {
                        // Guardar mensaje en localStorage
                        localStorage.setItem('mensajeExito', response.message);
                        window.location.replace('/ABM/ABMAdministradores'); // Redirigir a la lista de Administradores
                    } else {
                        mostrarAlerta('danger', response.message); // Mostrar mensaje de error
                    }
                },
                error: function () {
                    mostrarAlerta('danger', 'Ocurrió un error al eliminar el Administrador.');
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
