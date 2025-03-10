function toggleFavorite(event, idProfesional) {
    const star = event.target;

    // Verificar si el usuario está logeado como cliente
    $.ajax({
        url: '/Home/VerificarSesion',  // URL de la acción VerificarSesion
        type: 'POST',
        success: function (response) {
            // Si el usuario está logeado, agregar o eliminar el favorito
            const star = event.target;
            const isFavorited = star.classList.contains('favorited');

            if (isFavorited) {
                eliminarFavorito(idProfesional, star);
            } else {
                agregarFavorito(idProfesional, star);
            }
        },
        error: function (xhr) {
            // Si la respuesta es 401 Unauthorized, mostrar el modal
            if (xhr.status === 401) {
                $('#confirmacionModal').modal('show');
            } else {
                alert("Hubo un problema al procesar la solicitud.");
            }
        }
    });


}

// Función para agregar un profesional a favoritos
function agregarFavorito(idProfesional, star) {
    console.log(idProfesional);
    $.ajax({
        url: `/Home/AgregarFavorito/${idProfesional}`,
        type: 'POST',
        data: { idProfesional: idProfesional },
        success: function (response) {
            if (response.mensaje) {
                star.classList.add('favorited');
            }
        },
        error: function () {
            alert("Error al agregar el favorito.");
        }
    });
}

// Función para eliminar un profesional de favoritos
function eliminarFavorito(idProfesional, star) {
    console.log(idProfesional);
    $.ajax({
        url: `/Home/EliminarFavorito/${idProfesional}`,
        type: 'POST',
        data: { idProfesional: idProfesional },
        success: function (response) {
            if (response.mensaje) {
                star.classList.remove('favorited');
            }
        },
        error: function () {
            alert("Error al eliminar el favorito.");
        }
    });
}

// Redirigir al login si el usuario no está logeado
$('#btnConfirmar').click(function () {
    window.location.href = '/Account/Login'; // Redirige a la página de login
});