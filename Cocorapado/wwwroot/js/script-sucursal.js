$(document).ready(function () {
    console.log("Id de sucursal:", idSucursal);

    cargarServicios(idSucursal);
    cargarProfesionales(idSucursal);

    $(document).on('change', '.service-checkbox', function () {
        cargarProfesionales(idSucursal);
    });

    function cargarServicios(idSucursal) {
        $.getJSON(`/api/Sucursales/GetServiciosBySucursal/${idSucursal}`, function (servicios) {
            let html = '';
            servicios.forEach(servicio => {
                // Verificamos si el precio máximo es 0
                let precio = servicio.precioMax > 0 ? `$ ${servicio.precioMin} - $ ${servicio.precioMax}` : `$ ${servicio.precioMin}`;

                html += `
            <div class="form-check d-flex justify-content-between align-items-center border p-2 rounded mb-2">
                <input class="form-check-input service-checkbox" type="checkbox" id="servicio${servicio.id}" value="${servicio.id}">
                <label class="form-check-label mb-0 flex-grow-1" for="servicio${servicio.id}">
                    ${servicio.servicioNombre}
                </label>
                <span>${precio}</span>
            </div>
            `;
            });
            $('#servicesList').html(html);
        });
    }


    function cargarProfesionales(idSucursal) {
        const idsServicios = $('.service-checkbox:checked').map(function () {
            return $(this).val();
        }).get();

        const data = {
            idSucursal: idSucursal,
            idsServicios: idsServicios.length > 0 ? idsServicios : null
        };

        $.ajax({
            url: '/api/Sucursales/GetProfesionalesBySucursalAndServicios',
            type: 'POST',
            contentType: 'application/json',
            data: JSON.stringify(data),
            success: function (response) {
                $('#professionalsList').empty(); // Limpia la lista de profesionales

                if (response && response.length > 0) {
                    let html = '';
                    response.forEach(profesional => {
                        html += `
                        <div class="professional-item" data-services="${profesional.servicios}">
                            <button class="btn btn-dark btn-block mb-2 professional-name" onclick="selectProfesional('${profesional.id}')">${profesional.nombre} ${profesional.apellido}</button>
                        </div>
                    `;
                    });
                    $('#professionalsList').html(html);
                } else {
                    // Si no hay profesionales, muestra un mensaje de alerta
                    mostrarAlerta('info', "No hay profesionales disponibles para los servicios seleccionados.");
                }
            },
            error: function (error) {
                console.error("Error al cargar profesionales:", error);
                mostrarAlerta('danger', "Error en la solicitud al cargar profesionales.");
            }
        });
    }

    var mensajeExito = localStorage.getItem('mensajeExito');
    if (mensajeExito) {
        mostrarAlerta('success', mensajeExito);
        localStorage.removeItem('mensajeExito');
    }

    $(document).ready(function () {
        $('#suscribirmeBtn').click(function () {
            const sucursalId = $(this).data('sucursal-id');

            // Llamada AJAX para suscribir al usuario
            $.ajax({
                url: '/api/Sucursales/SuscribirClienteAbono',
                type: 'POST',
                contentType: 'application/json',
                data: JSON.stringify({ idSucursal: sucursalId }),
                success: function (response) {
                    if (response.success) {
                        alert('¡Te has suscrito exitosamente al abono mensual!');
                    } else {
                        alert(response.message || 'Hubo un problema al suscribirte. Por favor, inténtalo nuevamente.');
                    }
                },
                error: function (xhr) {
                    if (xhr.status === 401) {
                        // Mostrar el modal si el usuario no está autenticado
                        $('#confirmacionModal').modal('show');
                    } else {
                        alert('Error al procesar la solicitud. Por favor, inténtalo nuevamente.');
                    }
                }
            });
        });

        // Redirección al login desde el modal
        $('#btnConfirmar').click(function () {
            window.location.href = '/Account/Login';
        });
    });


});


function mostrarAlerta(tipo, mensaje) {
    var alerta = `
            <div class="alert alert-${tipo} alert-dismissible fade show" role="alert">
                ${mensaje}
                <button type="button" class="close" data-dismiss="alert" aria-label="Close">
                    <span aria-hidden="true">&times;</span>
                </button>
            </div>
        `;
    $('#alertContainer').append(alerta);

    setTimeout(function () {
        $('.alert').alert('close');
    }, 4000);
}

// Definir selectProfesional en el ámbito global fuera de $(document).ready
function selectProfesional(idProfesional) {
    const idsServicios = $('.service-checkbox:checked').map(function () {
        return $(this).val();
    }).get();

    if (idsServicios.length === 0) {
        mostrarAlerta('danger', "Debe seleccionar al menos un servicio.");
        return;
    }

    const url = `/Calendario?idSucursal=${idSucursal}&idProfesional=${idProfesional}&idsServicios=${encodeURIComponent(idsServicios.join(','))}`;
    window.location.href = url;
}