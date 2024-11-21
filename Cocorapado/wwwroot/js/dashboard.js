// Crear el gráfico
const ctx = document.getElementById('salesChart').getContext('2d');
let salesChart = new Chart(ctx, {
    type: 'line', // Tipo de gráfico (línea)
    data: {
        labels: [], // Inicialmente vacío
        datasets: [{
            label: 'Ventas',
            data: [], // Inicialmente vacío
            borderColor: 'rgba(75, 192, 192, 1)',
            borderWidth: 2,
            fill: false
        }]
    },
    options: {
        responsive: true,
        scales: {
            y: {
                beginAtZero: true
            }
        }
    }
});

// Función para actualizar el gráfico según el rango de tiempo
function updateChart() {
    const selectedTimeRange = document.getElementById('timeRange').value; // Obtiene el rango de tiempo seleccionado

    // Llamada AJAX para obtener los datos de ventas
    fetch(`/Dashboard/GetSalesData?timeRange=${selectedTimeRange}`)
        .then(response => {
            if (!response.ok) {
                throw new Error('Network response was not ok');
            }
            return response.json();
        })
        .then(data => {
            const labels = data.map(item => item.label); // Etiquetas para el eje X (horas/días)
            const salesValues = data.map(item => item.value); // Valores de ventas

            // Actualizar los datos del gráfico
            salesChart.data.labels = labels; // Asigna las etiquetas
            salesChart.data.datasets[0].data = salesValues; // Asigna los valores de ventas
            salesChart.update(); // Actualiza el gráfico
        })
        .catch(error => {
            console.error('Error al obtener los datos de ventas:', error);
        });
}

// Ejecutar la actualización inicial al cargar la página
document.addEventListener('DOMContentLoaded', function () {
    updateChart();
});

// Escuchar cambios en el select para actualizar el gráfico
document.getElementById('timeRange').addEventListener('change', updateChart);
