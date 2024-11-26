// Crear el gráfico para ventas por sucursal
const ctx = document.getElementById('salesChart').getContext('2d');
let salesChart = new Chart(ctx, {
    type: 'line', // Tipo de gráfico (línea)
    data: {
        labels: [], // Inicialmente vacío
        datasets: [] // Inicialmente vacío, se llenará con las ventas de cada sucursal
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

    // Llamada AJAX para obtener los datos de ventas por todas las sucursales
    fetch(`/Dashboard/GetSalesDataByBranch?timeRange=${selectedTimeRange}`)
        .then(response => {
            if (!response.ok) {
                throw new Error('Network response was not ok');
            }
            return response.json();
        })
        .then(data => {
            const labels = data[0].salesData.map(item => item.label); // Las etiquetas de tiempo (por ejemplo, días, horas)

            // Limpiar los datasets previos
            salesChart.data.datasets = [];

            // Crear un dataset por cada sucursal
            data.forEach(sucursal => {
                const salesValues = sucursal.salesData.map(item => item.value); // Los valores de ventas para esta sucursal

                salesChart.data.datasets.push({
                    label: sucursal.sucursalName, // Nombre de la sucursal
                    data: salesValues, // Datos de ventas de esa sucursal
                    borderColor: getRandomColor(), // Un color aleatorio para la sucursal
                    borderWidth: 2,
                    fill: false
                });
            });

            // Actualizar el gráfico con los nuevos datos
            salesChart.data.labels = labels; // Asigna las etiquetas (fechas, horas, etc.)
            salesChart.update(); // Actualiza el gráfico
        })
        .catch(error => {
            console.error('Error al obtener los datos de ventas:', error);
        });
}

// Función para generar un color aleatorio para cada sucursal
function getRandomColor() {
    const letters = '0123456789ABCDEF';
    let color = '#';
    for (let i = 0; i < 6; i++) {
        color += letters[Math.floor(Math.random() * 16)];
    }
    return color;
}

// Ejecutar la actualización inicial al cargar la página
document.addEventListener('DOMContentLoaded', function () {
    updateChart();
});

// Escuchar cambios en el select de rango de tiempo para actualizar el gráfico
document.getElementById('timeRange').addEventListener('change', updateChart);
