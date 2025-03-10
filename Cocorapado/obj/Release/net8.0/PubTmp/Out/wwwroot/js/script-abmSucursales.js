$(document).ready(function () {

    $('#SucursalImagen').change(function (event) {
        const file = event.target.files[0];
        if (file) {
            const reader = new FileReader();
            reader.onload = function (e) {
                $('#currentImageCreate').attr('src', e.target.result);
            };
            reader.readAsDataURL(file);
        } else {
            // Si no hay archivo, limpiar la vista de la imagen
            $('#currentImageCreate').attr('src', '');
            $('#currentImageCreate').attr('alt', '');
            $('#currentImage').hide();
        }
    });

    // Función para cargar los datos en el formulario de creacion
    window.cargarDatosCreacion = function () {
        try {
            // Limpiar los campos del formulario
            $('#SucursalNombre').val('');
            $('#SucursalDireccion').val('');
            $('#SucursalLocalidad').val('');
            $('#SucursalTelefono').val('');
            $('#currentImageCreate').attr('src', '');
            $('#currentImageCreate').attr('alt', '');
            $('#currentImage').hide();

            // Cargar horarios y servicios para nueva sucursal
            cargarHorariosServiciosCreate(0); // Pasar 0 para cargar datos iniciales

            $('#crearSucursalModal').modal('show'); // Mostrar modal de creación
        } catch (error) {
            console.error("Error al cargar los datos de creación:", error);
        }
    };

    // Función para cargar los datos en el formulario de edición
    window.cargarDatosEdicion = function (id, nombre, direccion, localidad, telefono, imagen) {
        try {
            console.log("Cargando datos de edición para ID:", id);
            $('#editarSucursalId').val(id);
            $('#editSucursalNombre').val(nombre);
            $('#editSucursalDireccion').val(direccion);
            $('#editSucursalLocalidad').val(localidad);
            $('#editSucursalTelefono').val(telefono);

            if (imagen) {
                $('#currentImage').attr('src',imagen);
            }

            // Cargar horarios y servicios
            cargarHorariosServicios(id);

            $('#editSucursalModal').modal('show');
        } catch (error) {
            console.error("Error al cargar los datos de la sucursal:", error);
        }
    };

    // Función para cargar horarios y servicios
    function cargarHorariosServicios(id) {
        // Cargar horarios (sin cambios)
        $.ajax({
            type: 'GET',
            url: '/ABM/ABMSucursales/ObtenerHorarios',
            data: { id: id },
            success: function (response) {
                if (response.success) {
                    let horariosBody = $('#horariosBody');
                    horariosBody.empty(); // Limpiar la tabla antes de llenarla

                    response.data.forEach(function (horario) {
                        horariosBody.append(`
                    <tr>
                        <td>${horario.dia || 'N/A'}</td>
                        <td><input type="time" name="${horario.dia}HorarioApertura" value="${horario.horarioApertura || ''}" class="form-control horario-apertura"></td>
                        <td><input type="time" name="${horario.dia}HorarioCierreMediodia" value="${horario.horarioCierreMediodia || ''}" class="form-control horario-cierre-mediodia"></td>
                        <td><input type="time" name="${horario.dia}HorarioAperturaMediodia" value="${horario.horarioAperturaMediodia || ''}" class="form-control horario-apertura-mediodia"></td>
                        <td><input type="time" name="${horario.dia}HorarioCierre" value="${horario.horarioCierre || ''}" class="form-control horario-cierre"></td>
                    </tr>
                `);
                    });
                } else {
                    mostrarAlerta('danger', response.message); // Mostrar mensaje de error
                }
            },
            error: function (xhr, status, error) {
                mostrarAlerta('danger', 'Ocurrió un error al cargar los horarios.');
            }
        });


        // Cargar servicios
        $.ajax({
            type: 'GET',
            url: '/ABM/ABMSucursales/ObtenerServicios',
            data: { id: id },
            beforeSend: function () {
                console.log(`Solicitando servicios para la sucursal con ID: ${id}`);
            },
            success: function (response) {
                if (response.success) {
                    let serviciosBody = $('#serviciosBody');
                    serviciosBody.empty();

                    // Recorrer la lista de servicios para generar las filas con los checkboxes
                    response.data.forEach(function (servicio) {
                        const checked = servicio.checked ? 'checked' : ''; // Si está marcado, agregar el atributo 'checked'
                        serviciosBody.append(`
                    <tr>
                        <td>${servicio.servicioNombre || 'N/A'}</td>
                        <td>
                            <input type="checkbox" name="serviciosSeleccionados_${servicio.id}" value="${servicio.id}" ${checked}>
                        </td>
                    </tr>
                `);
                    });
                } else {
                    mostrarAlerta('danger', response.message); // Mostrar mensaje de error 
                }
            },
            error: function (xhr, status, error) {
                mostrarAlerta('danger', 'Ocurrió un error al cargar los servicios.');
            }
        });
    }

    function cargarHorariosServiciosCreate(id) {
        // Cargar horarios
        $.ajax({
            type: 'GET',
            url: '/ABM/ABMSucursales/ObtenerHorarios',
            data: { id: id },
            success: function (response) {
                if (response.success) {
                    let horariosBody = $('#horariosBodyCreate');
                    horariosBody.empty(); // Limpiar la tabla antes de llenarla

                    response.data.forEach(function (horario) {
                        horariosBody.append(`
                        <tr>
                            <td>${horario.dia || 'N/A'}</td>
                            <td><input type="time" name="${horario.dia}HorarioApertura" value="${horario.horarioApertura || ''}" class="form-control horario-apertura"></td>
                            <td><input type="time" name="${horario.dia}HorarioCierreMediodia" value="${horario.horarioCierreMediodia || ''}" class="form-control horario-cierre-mediodia"></td>
                            <td><input type="time" name="${horario.dia}HorarioAperturaMediodia" value="${horario.horarioAperturaMediodia || ''}" class="form-control horario-apertura-mediodia"></td>
                            <td><input type="time" name="${horario.dia}HorarioCierre" value="${horario.horarioCierre || ''}" class="form-control horario-cierre"></td>
                        </tr>
                    `);
                    });
                } else {
                    mostrarAlerta('danger', response.message); // Mostrar mensaje de error
                }
            },
            error: function (xhr, status, error) {
                mostrarAlerta('danger', 'Ocurrió un error al cargar los horarios.');
            }
        });

        // Cargar servicios
        $.ajax({
            type: 'GET',
            url: '/ABM/ABMSucursales/ObtenerServicios',
            data: { id: id },
            beforeSend: function () {
                console.log(`Solicitando servicios para la sucursal con ID: ${id}`);
            },
            success: function (response) {
                if (response.success) {
                    let serviciosBody = $('#serviciosBodyCreate');
                    serviciosBody.empty();

                    // Recorrer la lista de servicios para generar las filas con los checkboxes
                    response.data.forEach(function (servicio) {
                        const checked = servicio.checked ? 'checked' : ''; // Si está marcado, agregar el atributo 'checked'
                        serviciosBody.append(`
                        <tr>
                            <td>${servicio.servicioNombre || 'N/A'}</td>
                            <td>
                                <input type="checkbox" name="serviciosSeleccionados_${servicio.id}" value="${servicio.id}" ${checked}>
                            </td>
                        </tr>
                    `);
                    });
                } else {
                    mostrarAlerta('danger', response.message); // Mostrar mensaje de error 
                }
            },
            error: function (xhr, status, error) {
                mostrarAlerta('danger', 'Ocurrió un error al cargar los servicios.');
            }
        });
    }

    // Guardar cambios en la sucursal
    $('#saveChanges').on('click', function (e) {
        e.preventDefault();

        var formData = new FormData($('#editSucursalForm')[0]); // Captura el formulario completo

        var horarios = {}; // Crear un diccionario para los horarios

        // Recorrer los inputs del formulario para construir el objeto de horarios
        $('#horariosBody tr').each(function () {
            var dia = $(this).find('td:first').text().trim(); // Obtener el día de la semana (la clave)

            horarios[dia] = {
                HorarioApertura: $(this).find('input[name="' + dia + 'HorarioApertura"]').val(),
                HorarioCierreMediodia: $(this).find('input[name="' + dia + 'HorarioCierreMediodia"]').val(),
                HorarioAperturaMediodia: $(this).find('input[name="' + dia + 'HorarioAperturaMediodia"]').val(),
                HorarioCierre: $(this).find('input[name="' + dia + 'HorarioCierre"]').val(),
            };
        });

        console.log(JSON.stringify(horarios));
        formData.append('horarios', JSON.stringify(horarios)); // Añadir horarios al formData

        // Capturar los servicios seleccionados
        var serviciosSeleccionados = [];
        $('input[name^="serviciosSeleccionados_"]:checked').each(function () {
            serviciosSeleccionados.push($(this).val());
        });

        console.log('Servicios seleccionados:', serviciosSeleccionados);
        formData.append('servicios', JSON.stringify(serviciosSeleccionados)); // Añadir servicios seleccionados al formData

        $.ajax({
            type: 'POST',
            url: $('#editSucursalForm').attr('action'),
            data: formData,
            contentType: false,
            processData: false,
            success: function (response) {
                if (response.success) {
                    localStorage.setItem('mensajeExito', response.message);
                    window.location.replace('/ABM/ABMSucursales');
                } else {
                    mostrarAlerta('danger', response.message);
                }
                $('#editSucursalModal').modal('hide');
            },
            error: function (xhr, status, error) {
                console.error('Error al enviar el formulario:', error);
                mostrarAlerta('danger', 'Ocurrió un error al procesar la solicitud.');
            }
        });
    });

    $('#saveSucursal').on('click', function (e) {
        e.preventDefault(); // Prevenir el comportamiento por defecto del botón

        var formData = new FormData($('#crearSucursalForm')[0]); // Captura el formulario completo

        var horarios = {}; // Crear un diccionario para los horarios

        // Recorrer los inputs del formulario para construir el objeto de horarios
        $('#horariosBodyCreate tr').each(function () {
            var dia = $(this).find('td:first').text().trim(); // Obtener el día de la semana (la clave)

            horarios[dia] = {
                HorarioApertura: $(this).find('input[name="' + dia + 'HorarioApertura"]').val(),
                HorarioCierreMediodia: $(this).find('input[name="' + dia + 'HorarioCierreMediodia"]').val(),
                HorarioAperturaMediodia: $(this).find('input[name="' + dia + 'HorarioAperturaMediodia"]').val(),
                HorarioCierre: $(this).find('input[name="' + dia + 'HorarioCierre"]').val(),
            };
        });

        console.log(JSON.stringify(horarios));
        formData.append('horarios', JSON.stringify(horarios)); // Añadir horarios al formData

        // Capturar los servicios seleccionados
        var serviciosSeleccionados = [];
        $('#serviciosBodyCreate input[name^="serviciosSeleccionados_"]:checked').each(function () {
            serviciosSeleccionados.push($(this).val());
        });

        console.log('Servicios seleccionados:', serviciosSeleccionados);
        formData.append('servicios', JSON.stringify(serviciosSeleccionados)); // Añadir servicios seleccionados al formData

        // Realizar la solicitud AJAX para crear la nueva sucursal
        $.ajax({
            type: 'POST',
            url: $('#crearSucursalForm').attr('action'), // URL de destino
            data: formData,
            contentType: false, // No procesar los datos como string
            processData: false, // No procesar el formData
            success: function (response) {
                if (response.success) {
                    localStorage.setItem('mensajeExito', response.message);
                    window.location.replace('/ABM/ABMSucursales'); // Redirigir a la página de sucursales
                } else {
                    mostrarAlerta('danger', response.message); // Mostrar error si no es exitoso
                }
                $('#crearSucursalModal').modal('hide'); // Ocultar el modal
            },
            error: function (xhr, status, error) {
                console.error('Error al enviar el formulario:', error);
                mostrarAlerta('danger', 'Ocurrió un error al procesar la solicitud.');
            }
        });
    });


    // Función para eliminar una sucursal
    window.eliminarSucursal = function (id) {
        // Mostrar el modal de confirmación
        $('#confirmacionModal').modal('show');

        // Cuando el usuario hace clic en "Confirmar"
        $('#btnConfirmar').off('click').on('click', function () {
            // Ocultar el modal
            $('#confirmacionModal').modal('hide');

            // Ejecutar la solicitud AJAX para eliminar la sucursal
            $.ajax({
                type: 'POST',
                url: '/ABM/ABMSucursales/EliminarSucursal', // URL del método EliminarSucursal
                data: { id: id },
                success: function (response) {
                    if (response.success) {
                        localStorage.setItem('mensajeExito', response.message);
                        window.location.replace('/ABM/ABMSucursales');
                    } else {
                        mostrarAlerta('danger', 'No se pudo eliminar la sucursal.');
                    }
                },
                error: function (xhr, status, error) {
                    console.error('Error al eliminar la sucursal:', error); // Log para errores
                    mostrarAlerta('danger', 'Ocurrió un error al intentar eliminar la sucursal.');
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
