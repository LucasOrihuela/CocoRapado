$(document).ready(function () {
    let idSucursalesGlobal;
    let idProfesionalGlobal;

    // Enviar formulario de creación de relación sucursal-profesional
    $('#crearRelacionForm').on('submit', function (e) {
        e.preventDefault(); // Evita el envío del formulario normal

        $.ajax({
            type: 'POST',
            url: $(this).attr('action'),
            data: $(this).serialize(), // Serializa los datos del formulario            
            success: function (response) {
                if (response.success) {
                    // Guardar mensaje en localStorage
                    localStorage.setItem('mensajeExito', 'La relación sucursal-profesional se ha agregado correctamente.');
                    window.location.replace('/ABM/ABMProfesionalesPorSucursal'); // Redirigir a la lista de relaciones
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

        setTimeout(function () {
            $('.alert').alert('close'); // Cierra la alerta automáticamente
        }, 4000);
    }

    // Verificar si hay un mensaje de éxito en localStorage y mostrarlo
    var mensajeExito = localStorage.getItem('mensajeExito');
    if (mensajeExito) {
        mostrarAlerta('success', mensajeExito);
        localStorage.removeItem('mensajeExito');
    }

    // Nueva función para cargar profesionales y sucursales
    async function cargarProfesionalesYSucursales() {
        try {
            const response = await fetch('/ABMProfesionalesPorSucursal/ObtenerProfesionalesYSucursales');
            const data = await response.json();

            if (data.profesionales && data.sucursales) {
                $('#selectProfesional').empty();
                $('#selectSucursal').empty();

                data.profesionales.forEach(function (profesional) {
                    if (profesional.id && profesional.nombre && profesional.apellido) {
                        $('#selectProfesional').append(
                            `<option value="${profesional.id}">${profesional.nombre} ${profesional.apellido}</option>`
                        );
                    }
                });

                // Aquí puedes cargar sucursales inicialmente si es necesario
                data.sucursales.forEach(function (sucursal) {
                    if (sucursal.id && sucursal.Nombre) {
                        $('#selectSucursal').append(
                            `<option value="${sucursal.id}">${sucursal.Nombre}</option>`
                        );
                    }
                });

                // Cargar las sucursales sin asignar para el primer profesional por defecto
                const primerProfesional = data.profesionales[0]?.id; // Obtener el primer profesional
                if (primerProfesional) {
                    $('#selectProfesional').val(primerProfesional); // Establecer el primer profesional como seleccionado
                    actualizarSucursalesSinAsignar(primerProfesional); // Cargar sucursales sin asignar para el primer profesional
                }
            } else {
                console.log('No se encontraron profesionales o sucursales.');
            }
        } catch (error) {
            console.error('Error al obtener los profesionales y sucursales:', error);
        }
    }

    // Al abrir el modal de creación, cargar los profesionales y sucursales
    $('#crearRelacionModal').on('show.bs.modal', function () {
        // Cargar profesionales y sucursales
        cargarProfesionalesYSucursales();
    });

    // Función para eliminar relación usando modal de confirmación
    window.eliminarRelacion = function (idsucursal, idProfesional) {
        // Guardamos los IDs en variables globales para utilizarlos luego
        idSucursalGlobal = idsucursal;
        idProfesionalGlobal = idProfesional;

        // Mostrar el modal de confirmación
        $('#confirmacionModal').modal('show');
    };

    // Acción cuando el usuario confirma la eliminación
    $('#btnConfirmar').on('click', function () {
        $.ajax({
            type: "POST",
            url: "/ABM/ABMProfesionalesPorSucursal/EliminarRelacion", // Asegúrate de que la URL sea correcta
            data: {
                id_sucursal: idSucursalGlobal,
                id_profesional: idProfesionalGlobal
            },
            success: function (response) {
                if (response.success) {
                    localStorage.setItem('mensajeExito', response.message);
                    window.location.replace('/ABM/ABMProfesionalesPorSucursal');
                } else {
                    mostrarAlerta('danger', response.message);
                }
            },
            error: function (xhr, status, error) {
                console.error("Error al eliminar la relación:", error);
                mostrarAlerta('danger', "Ocurrió un error al intentar eliminar la relación.");
            }
        });

        // Cerrar el modal
        $('#confirmacionModal').modal('hide');
    });

    // Evento de cambio para el select de profesionales
    $('#selectProfesional').on('change', function () {
        const idProfesionalSeleccionado = $(this).val();
        actualizarSucursalesSinAsignar(idProfesionalSeleccionado);
    });

    // Función para actualizar el select de sucursales
    function actualizarSucursalesSinAsignar(idProfesional) {
        $.ajax({
            type: "POST",
            url: "/ABMProfesionalesPorSucursal/ObtenerSucursalesSinAsignar", // Asegúrate de que esta URL esté correcta
            data: { id_profesional: idProfesional },
            success: function (response) {
                $('#selectSucursal').empty(); // Limpiar el select de sucursales
                if (response.success && response.sucursales) {
                    response.sucursales.forEach(function (sucursal) {
                        $('#selectSucursal').append(
                            `<option value="${sucursal.id}">${sucursal.nombre}</option>`
                        );
                    });
                } else {
                    mostrarAlerta('danger', 'No se encontraron sucursales para este profesional.');
                }
            },
            error: function (xhr, status, error) {
                console.error('Error al obtener sucursales:', error);
                mostrarAlerta('danger', 'Ocurrió un error al obtener los sucursales.');
            }
        });
    }
});
