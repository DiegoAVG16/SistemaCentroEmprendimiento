using CentroEmpData.Configuracion;
using CentroEmpData.Contrato;
using CentroEmpData.Implementacion;
using Microsoft.AspNetCore.Authentication.Cookies;


 // ?? Esto registra IHttpClientFactory


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddHttpClient();

builder.Services.Configure<ConnectionStrings>(builder.Configuration.GetSection("ConnectionStrings"));
builder.Services.AddScoped<ICitaRepositorio, CitaRepositorio>();
builder.Services.AddScoped<IAsesorRepositorio, AsesorRepositorio>();
builder.Services.AddScoped<IEspecialidadRepositorio, EspecialidadRepositorio>();
builder.Services.AddScoped<IRolUsuarioRepositorio, RolUsuarioRepositorio>();
builder.Services.AddScoped<IUsuarioRepositorio, UsuarioRepositorio>();

// Configuración de autenticación con cookies
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(option =>
    {
        option.LoginPath = "/Acceso/Login";  // Ruta de login
        option.ExpireTimeSpan = TimeSpan.FromMinutes(20);
        option.AccessDeniedPath = "/Acceso/Denegado";  // Ruta de acceso denegado
    });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}

app.UseStaticFiles();

app.UseRouting();

// Configuración de autorización
app.UseAuthentication(); // Asegúrate de que UseAuthentication está antes de UseAuthorization
app.UseAuthorization();  // Solo una vez, aquí es donde se autoriza el acceso

// Configuración de rutas
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Acceso}/{action=Login}/{id?}");  // Rutas por defecto, asegúrate que tu controlador Asesor tenga la ruta correcta

app.Run();
