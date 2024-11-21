using Cocorapado.Models; // Asegúrate de tener esta referencia
using Cocorapado.Service;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity; // Necesario para Identity
using Microsoft.EntityFrameworkCore; // Necesario para el contexto de EF
using System.Data;
using System.Data.SqlClient;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.AddConsole();

// Add services to the container.
builder.Services.AddControllersWithViews();

// Register IDbConnection for Dapper
builder.Services.AddScoped<IDbConnection>(sp =>
    new SqlConnection(builder.Configuration.GetConnectionString("DefaultConnection")));

// Register SucursalService
builder.Services.AddScoped<SucursalService>();

// Register UsuarioService with injected IDbConnection
builder.Services.AddScoped<UsuarioService>();

// Register ServiciosPorProfesionalService
builder.Services.AddScoped<ServiciosPorProfesionalService>();

// Register ProfesionalesPorSucursalService
builder.Services.AddScoped<ProfesionalesPorSucursalService>();

// Register ServicioService
builder.Services.AddScoped<ServicioService>();

// Register TurnoService
builder.Services.AddScoped<TurnoService>();

// Register PerfilService
builder.Services.AddScoped<PerfilService>();

// Configure Entity Framework and Identity
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));


// Configure Identity
builder.Services.AddIdentity<Usuario, IdentityRole>(options =>
{
    // Configuración de opciones de Identity
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = true;
    options.Password.RequiredLength = 6;
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();

// Agregar soporte para sesiones
builder.Services.AddDistributedMemoryCache(); // Necesario para almacenar los datos de la sesión
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Duración de la sesión
    options.Cookie.HttpOnly = true; // Configuración de la cookie
    options.Cookie.IsEssential = true; // Necesario para cumplir con GDPR
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.UseSession(); // Habilitar el uso de sesiones

// Map routes for API controllers
app.MapControllerRoute(
    name: "api",
    pattern: "api/{controller}/{action}/{id?}");

// Map route for Home controller
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "sucursal",
    pattern: "Sucursal/Details/{id}",
    defaults: new { controller = "Sucursales", action = "SucursalDetails" });

// Map specific routes for other areas
app.MapControllerRoute(
    name: "calendario",
    pattern: "calendario/{idSucursal}",
    defaults: new { controller = "Calendario", action = "Index" });

app.MapControllerRoute(
    name: "getEventos",
    pattern: "calendario/getEventos/{idSucursal}/{idProfesional}",
    defaults: new { controller = "Calendario", action = "GetEventos" });

app.MapControllerRoute(
    name: "login",
    pattern: "Account/login",
    defaults: new { controller = "Account", action = "Login" });

app.MapControllerRoute(
    name: "registro",
    pattern: "Account/registro",
    defaults: new { controller = "Account", action = "Registro" });

app.MapControllerRoute(
    name: "admin_dashboard",
    pattern: "Admin/Dashboard",
    defaults: new { controller = "Dashboard", action = "Index" });

// Map route for ABMProfesionales with the 'ABM' prefix
app.MapControllerRoute(
    name: "ABMProfesionales",
    pattern: "ABM/ABMProfesionales/{action=Index}/{id?}",
    defaults: new { controller = "ABMProfesionales", action = "Index" });

// Add more routes for other ABM sections if necessary
app.MapControllerRoute(
    name: "ABMSucursales",
    pattern: "ABM/ABMSucursales/{action=Index}/{id?}",
    defaults: new { controller = "ABMSucursales", action = "Index" });

app.MapControllerRoute(
    name: "ABMServicios",
    pattern: "ABM/ABMServicios/{action=Index}/{id?}",
    defaults: new { controller = "ABMServicios", action = "Index" });

app.MapControllerRoute(
    name: "ABMServiciosPorProfesional",
    pattern: "ABM/ABMServiciosPorProfesional/{action=Index}/{id?}",
    defaults: new { controller = "ABMServiciosPorProfesional", action = "Index" });

app.MapControllerRoute(
    name: "ABMProfesionalesPorSucursal",
    pattern: "ABM/ABMProfesionalesPorSucursal/{action=Index}/{id?}",
    defaults: new { controller = "ABMProfesionalesPorSucursal", action = "Index" });

app.MapControllerRoute(
    name: "MisReservas",
    pattern: "Account/MisReservas",
    defaults: new { controller = "MisReservas", action = "Index" }
);

app.MapControllerRoute(
    name: "MiCuenta",
    pattern: "Account/MiCuenta",
    defaults: new { controller = "MiCuenta", action = "Index" }
);

app.MapControllerRoute(
    name: "RecuperarPassword",
    pattern: "Account/RecuperarPassword",
    defaults: new { controller = "Account", action = "RecuperarContraseña" }
);

app.MapControllerRoute(
    name: "ReservasProfesional",
    pattern: "Profesional/ReservasProfesional",
    defaults: new { controller = "Profesionales", action = "Index" }
);

app.Run();
