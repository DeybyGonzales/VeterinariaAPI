# Veterinaria API

Web API REST (ASP.NET Core .NET 10 + EF Core, base de datos primero) para gestionar los **propietarios** de una veterinaria y sus **mascotas** (relación 1:N).

## Endpoints (11)
| Método | Ruta | Descripción |
|---|---|---|
| GET | `/api/propietarios?buscar=` | Lista propietarios (filtra por nombre o DNI) |
| GET | `/api/propietarios/{id}` | Propietario por id |
| GET | `/api/propietarios/{id}/mascotas` | Mascotas de un propietario (ruta anidada) |
| POST | `/api/propietarios` | Crea propietario |
| PUT | `/api/propietarios/{id}` | Actualiza propietario |
| DELETE | `/api/propietarios/{id}` | Elimina propietario |
| GET | `/api/mascotas?especie=&propietarioId=&buscar=&edadMax=` | Lista mascotas con filtros |
| GET | `/api/mascotas/{id}` | Mascota por id (incluye propietario) |
| POST | `/api/mascotas` | Crea mascota |
| PUT | `/api/mascotas/{id}` | Actualiza mascota |
| DELETE | `/api/mascotas/{id}` | Elimina mascota |

## Reglas de negocio (409 Conflict)
1. El DNI de un propietario no puede repetirse.
2. No se puede eliminar un propietario que tiene mascotas registradas.
3. Un propietario no puede tener dos mascotas con el mismo nombre.

## Ejecutar en local
```bash
cp .env.example .env                 # pon tu clave de SQL Server
docker compose up -d                 # SQL Server + script database/VeterinariaDB.sql
dotnet user-secrets set "ConnectionStrings:Veterinaria" "Server=localhost,1433;Database=VeterinariaDB;User Id=sa;Password=<tu-clave>;TrustServerCertificate=True;"
dotnet run
```
Swagger: http://localhost:5284/swagger

## Modelo (Scaffold)
```bash
dotnet ef dbcontext scaffold "Name=ConnectionStrings:Veterinaria" Microsoft.EntityFrameworkCore.SqlServer -o Models --data-annotations --no-onconfiguring --context VeterinariaContext
```

> La cadena de conexión no está en el repositorio: en local se usa `dotnet user-secrets` y en la nube una cadena de conexión/variable de entorno llamada `Veterinaria`.

## Despliegue en la nube
- **API:** MonsterASP.NET (plan Free, .NET 10, IIS) → https://veterinaria-api.runasp.net/swagger
- **Base de datos:** MonsterASP.NET MSSQL (plan Free, SQL Server 2025, 1 GB)
- La cadena de conexión se configura en el panel como variable de entorno `ConnectionStrings__Veterinaria`; no está en el repositorio.
- El script `database/VeterinariaDB.sql` se ejecutó directamente sobre la base de la nube.
