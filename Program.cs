using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using VeterinariaAPI.Models;

var builder = WebApplication.CreateBuilder(args);

// NumberHandling = Strict: los números son solo números (no "integer | string" en el documento OpenAPI)
builder.Services.AddControllers()
    .AddJsonOptions(o => o.JsonSerializerOptions.NumberHandling = JsonNumberHandling.Strict);
builder.Services.ConfigureHttpJsonOptions(o => o.SerializerOptions.NumberHandling = JsonNumberHandling.Strict);
// OpenAPI 3.0: con 3.1 los parámetros {id} salen como "integer | string" y Swagger UI no los acepta
builder.Services.AddOpenApi(o => o.OpenApiVersion = Microsoft.OpenApi.OpenApiSpecVersion.OpenApi3_0);

// La cadena de conexión NO está en appsettings.json:
//   - Local: dotnet user-secrets  (ConnectionStrings:Veterinaria)
//   - Nube:  variable de entorno / cadena de conexión del proveedor con el nombre "Veterinaria"
var cadena = builder.Configuration.GetConnectionString("Veterinaria")
    ?? throw new InvalidOperationException("Falta la cadena de conexión 'Veterinaria'.");

builder.Services.AddDbContext<VeterinariaContext>(options =>
    options.UseSqlServer(cadena, sql => sql.EnableRetryOnFailure()));

// CORS con orígenes concretos (no AllowAnyOrigin), definidos en appsettings.json -> Cors:Origenes
var origenes = builder.Configuration.GetSection("Cors:Origenes").Get<string[]>() ?? [];
builder.Services.AddCors(options =>
    options.AddPolicy("Front", policy => policy
        .WithOrigins(origenes)
        .AllowAnyHeader()
        .AllowAnyMethod()));

var app = builder.Build();

// Swagger habilitado también en producción para poder probar la API publicada
app.MapOpenApi();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/openapi/v1.json", "Veterinaria API v1");
    options.RoutePrefix = "swagger";
});

// La raíz redirige a Swagger para que el enlace publicado abra directo la documentación
app.MapGet("/", () => Results.Redirect("/swagger")).ExcludeFromDescription();

app.UseHttpsRedirection();
app.UseCors("Front");
app.UseAuthorization();
app.MapControllers();

app.Run();
