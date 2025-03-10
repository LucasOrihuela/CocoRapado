let turnoSeleccionado = {};
let telefonoSucursal = 0;
let telefonoCliente = 0;
let precioMinTotal = 0;
let precioMaxTotal = 0;
let duracionTurno = 0;

// Función para obtener parámetros de URL
function getParameterByName(name) {
    const urlParams = new URLSearchParams(window.location.search);
    return urlParams.get(name);
}

$(window).on('load', function () {
    var idSucursal = getParameterByName('idSucursal');
    var idProfesional = getParameterByName('idProfesional');
    var idsServicios = getParameterByName('idsServicios');

    idSucursal = parseInt(idSucursal);
    idProfesional = parseInt(idProfesional);

    // Deshabilitar el botón "Reservar" al inicio
    $('#botonReservar').prop('disabled', true).css({ cursor: 'not-allowed', opacity: 0.6 });

    // Llamada a los horarios, turnos y duración de servicios
    $.when(
        $.ajax({
            url: '/Calendario/GetHorarios',
            type: 'GET',
            data: { idSucursal: idSucursal }
        }),
        $.ajax({
            url: '/Calendario/GetEventos',
            type: 'GET',
            data: { idSucursal: idSucursal, idProfesional: idProfesional }
        }),
        $.ajax({
            url: '/Calendario/GetDuracionTurno',
            type: 'GET',
            data: { idsServicios: idsServicios }
        })
    ).done(function (horariosData, eventosData, duracionData) {
        var horarios = horariosData[0];
        var eventos = eventosData[0];
        duracionTurno = parseInt(duracionData[0]); // Duración total en minutos

        var businessHoursConfig = [];
        var minTime = '24:00:00';
        var maxTime = '00:00:00';
        var isClosedAllDays = true;

        // Generar configuración de businessHours y calcular minTime y maxTime dinámicos
        horarios.forEach(function (horario) {
            if (!horario.dia) return;

            var diaSemana = convertDiaSemanaToDow(horario.dia);

            if (horario.horarioApertura !== "00:00" || horario.horarioCierre !== "00:00") {
                isClosedAllDays = false;
                businessHoursConfig.push(
                    { dow: [diaSemana], start: horario.horarioApertura, end: horario.horarioCierreMediodia },
                    { dow: [diaSemana], start: horario.horarioAperturaMediodia, end: horario.horarioCierre }
                );

                minTime = moment.min(moment(horario.horarioApertura, "HH:mm"), moment(minTime, "HH:mm")).format("HH:mm");
                maxTime = moment.max(moment(horario.horarioCierre, "HH:mm"), moment(maxTime, "HH:mm")).format("HH:mm");
            }
        });

        // Configuración del calendario
        $('#calendario').fullCalendar({
            locale: 'es',
            header: {
                left: 'prev,next today',
                center: 'title',
                right: 'agendaWeek,agendaDay'
            },
            defaultView: 'agendaWeek',
            allDaySlot: false,
            slotDuration: '00:15:00',
            selectable: true,
            selectHelper: true,
            businessHours: isClosedAllDays ? false : businessHoursConfig,
            minTime: minTime,
            maxTime: maxTime,
            events: eventos.map(evento => {
                evento.color = 'grey'; // Color de fondo gris
                evento.textColor = 'white'; // Texto blanco
                return evento;
            }),
            selectOverlap: false,
            eventRender: function (event, element) {
                // Mostrar hora de inicio y fin para el evento de turno seleccionado
                if (event.id === 'turnoSeleccionado') {
                    //var startTime = moment(event.start).format('HH:mm');
                    //var endTime = moment(event.end).format('HH:mm');
                    /*                    element.find('.fc-title').empty().html(`${startTime} - ${endTime}`); // Limpiar y agregar la hora*/
                } else {
                    // Eventos normales solo muestran título o tiempo si aplica
                    element.find('.fc-title').text(event.title); // Solo mostrar el título
                }
            },
            select: function (start, end) {
                // Crear el momento en la zona horaria de Buenos Aires
                // var startLocal = moment(start).tz("America/Argentina/Buenos_Aires");

                // Calcular el final del turno seleccionado en función de la duración
                var turnoFin = start.clone().add(duracionTurno, 'minutes');

                // Validar que el turno no se extienda fuera de los horarios de trabajo
                if (!isWithinBusinessHours(start, turnoFin, businessHoursConfig)) {
                    alert("El turno seleccionado debe estar dentro del rango horario disponible.");
                    $('#calendario').fullCalendar('unselect');
                    return;
                }

                // Limpiar cualquier turno previamente seleccionado
                $('#calendario').fullCalendar('removeEvents', function (event) {
                    return event.id === 'turnoSeleccionado';
                });

                $.ajax({
                    url: '/Calendario/GetTurnoDetails',
                    type: 'GET',
                    data: {
                        idSucursal: idSucursal,
                        idProfesional: idProfesional,
                        idsServicios: idsServicios
                    },
                    success: function (data) {
                        if (data && data.profesional) {
                            // Dividir el nombre y apellido solo si `data.profesional` está definido
                            const nombreCompleto = data.profesional.split(' ');
                            turnoSeleccionado = {
                                sucursalNombre: data.sucursal,
                                profesionalNombre: nombreCompleto[0] || '',
                                profesionalApellido: nombreCompleto.slice(1).join(' ') || '',
                                servicios: data.servicios ? data.servicios.map(nombre => ({ nombre: nombre })) : [],
                                fecha: start.format("DD/MM/YYYY"),
                                hora: start.format("HH:mm"),
                                duracion: duracionTurno
                            };
                            precioMinTotal = data.precioMin;
                            precioMaxTotal = data.precioMax;
                            telefonoSucursal = data.telefonoSucursal;
                            telefonoCliente = data.telefonoCliente;
                            actualizarEstadoBotonReservar();
                        } else {
                            console.error("Error: El campo `profesional` no está definido en la respuesta del servidor.");
                            alert("No se pudo obtener los detalles del turno. Por favor, inténtalo de nuevo.");
                        }
                    },
                    error: function (xhr, status, error) {
                        console.error('Error al obtener los detalles del turno:', error);
                        alert('No se pudo obtener los detalles del turno. Por favor, inténtalo de nuevo.');
                    }
                });


                // Crear el evento con la duración total
                var evento = {
                    id: 'turnoSeleccionado',
                    title: 'Su reserva',
                    start: start,
                    end: turnoFin, // Asigna la duración
                    color: '#3a87ad', // Color del evento de selección
                    textColor: 'white', // Texto blanco para el evento seleccionado
                    allDay: false
                };

                // Agregar el evento al calendario y cancelar la selección
                $('#calendario').fullCalendar('renderEvent', evento, true);
                $('#calendario').fullCalendar('unselect'); // Cancela la selección visual para evitar duplicados

                // Habilitar el botón "Reservar"
                actualizarEstadoBotonReservar();
            },
            unselect: function () {
                // No deshabilitar el botón aquí
            },
            validRange: {
                start: moment().format("YYYY-MM-DD")
            }
        });

        // Función para habilitar o deshabilitar el botón "Reservar" basado en `turnoSeleccionado`
        function actualizarEstadoBotonReservar() {
            // Verificar los valores actuales de turnoSeleccionado
            console.log("Turno seleccionado:", turnoSeleccionado);

            // Verificar si tanto la fecha como la hora están definidas
            const habilitado = turnoSeleccionado.fecha && turnoSeleccionado.hora;

            // Habilitar o deshabilitar el botón en función de la condición
            $('#botonReservar').prop('disabled', !habilitado).css({
                cursor: habilitado ? 'pointer' : 'not-allowed',
                opacity: habilitado ? 1 : 0.6,
                backgroundColor: habilitado ? '#ffc107' : '',
                color: habilitado ? 'black' : '',
                border: habilitado ? '1px solid black' : 'none'
            });

        }


        // Evento de clic en el botón "Reservar"
        $('#botonReservar').click(function () {
            mostrarModalConfirmacion();
        });

        function isWithinBusinessHours(start, end, businessHoursConfig) {
            var isWithinHours = false;
            businessHoursConfig.forEach(function (period) {
                if (start.day() === period.dow[0]) {
                    var periodStart = moment(start).set({ hour: period.start.split(':')[0], minute: period.start.split(':')[1] });
                    var periodEnd = moment(start).set({ hour: period.end.split(':')[0], minute: period.end.split(':')[1] });
                    if (start.isSameOrAfter(periodStart) && end.isSameOrBefore(periodEnd)) {
                        isWithinHours = true;
                    }
                }
            });
            return isWithinHours;
        }

        function mostrarModalConfirmacion() {
            if (!turnoSeleccionado.fecha || !turnoSeleccionado.hora) {
                mostrarAlerta('danger', 'Por favor, seleccione un horario en el calendario.');
                return;
            }
            abrirModalConfirmacion(turnoSeleccionado);
        }

        function abrirModalConfirmacion(turno) {
            document.getElementById('sucursalNombre').innerText = turno.sucursalNombre;
            document.getElementById('profesionalNombre').innerText = turno.profesionalNombre;
            document.getElementById('profesionalApellido').innerText = turno.profesionalApellido;

            const listaServicios = document.getElementById('listaServicios');
            listaServicios.innerHTML = '';
            turno.servicios.forEach(servicio => {
                const li = document.createElement('li');
                li.innerText = servicio.nombre;
                listaServicios.appendChild(li);
            });

            document.getElementById('fechaSeleccionada').innerText = turno.fecha;
            document.getElementById('horaSeleccionada').innerText = turno.hora;
            document.getElementById('duracionTurno').innerText = duracionTurno + ' Minutos';

            // Mostrar el modal usando Bootstrap
            $('.modal-backdrop').remove();
            $('#modalConfirmacionTurno').modal('show');
            $('#modalConfirmacionTurno').modal({
                backdrop: 'static',
                keyboard: false
            });
        }

        // Función para convertir día de la semana a número de día (Dow)
        function convertDiaSemanaToDow(dia) {
            switch (dia.toLowerCase()) {
                case 'lunes': return 1;
                case 'martes': return 2;
                case 'miércoles': return 3;
                case 'jueves': return 4;
                case 'viernes': return 5;
                case 'sábado': return 6;
                case 'domingo': return 0;
                default: return null;
            }
        }



        // Evento de confirmación del turno
        document.getElementById('confirmarTurno').addEventListener('click', function () {
            $('#modalConfirmacionTurno').modal('hide');
            let precio = precioMinTotal;
            if (precioMaxTotal > 0) {
                precio = (precioMinTotal + precioMaxTotal) / 2;
            }

            // Enviar mensaje de WhatsApp
            //var mensajeWhatsapp = `Hola! Reservé un turno para el dia ${turnoSeleccionado.fecha} a las ${turnoSeleccionado.hora}, en la sucursal ${turnoSeleccionado.sucursalNombre}. Codigo de turno ${turnoSeleccionado.idTurno}.`;
            //var urlWhatsapp = `https://wa.me/${telefonoSucursal}?text=${encodeURIComponent(mensajeWhatsapp)}`;
            //window.open(urlWhatsapp, '_blank');

            // Guardar turno en la base de datos
            $.ajax({
                url: '/Calendario/GuardarTurno',
                type: 'POST',
                data: {
                    fecha: turnoSeleccionado.fecha,
                    hora: turnoSeleccionado.hora,
                    idSucursal: idSucursal,
                    idProfesional: idProfesional,
                    precio: precio,
                    duracionMin: duracionTurno
                },
                success: function () {
                    $('#confirmacionModal').modal('show');

                    // Enviar el correo al confirmar el turno
                },
                error: function (error) {
                    console.error("Error al almacenar el turno:", error);
                    mostrarAlerta("danger", "Hubo un error al confirmar tu turno.");
                }
            });
        });



    });

    $('#btnConfirmar').on('click', function () {
        window.location.href = 'Home/Index';
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
});
