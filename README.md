# Incident API

- Frontend → https://github.com/CruzNestor/incident-web.git
- Infrastructure → https://github.com/CruzNestor/incident-infra.git

API REST para gestión de incidencias.

## Tecnologías

- ASP.NET Core
- Entity Framework Core
- ASP.NET Core Identity
- Swagger
- Json Web Token
- MongoDB
- SQL Server

## Arquitectura

El proyecto sigue Clean Architecture:

```bash
 ├── Incident.Api
 │    ├── Controllers
 │    ├── Configurations
 │    └── Program.cs
 │
 ├── Incident.Application
 │    ├── DTOs
 │    ├── Interfaces
 │    ├── Services
 │    └── Exceptions
 │
 ├── Incident.Domain
 │    ├── Entities
 │    ├── Enums
 │    └── Repositories
 │
 └── Incident.Infrastructure
      ├── Persistence
      │    ├── Relational
      │    │    ├── Repositories
      │    │    └── ApplicationDbContext.cs
      │    │
      │    └── Mongo
      │         ├── Documents
      │         └── Repositories
      │
      ├── ExternalServices
      │    └── ServiceCatalog
      │
      └── Migrations
```

## Decisiones de arquitectura (y tradeoffs)
#### Clean Architecture
Se eligió Clean Architecture para desacoplar el dominio de los detalles de infraestructura.

##### Ventajas
- Separación clara de responsabilidades
- Código más testeable
- Fácil reemplazar infraestructura
##### Tradeoff
- Mayor cantidad de capas y archivos.

#### SQL Server + MongoDB

##### SQL Server
- Incidentes
- Usuarios
- Roles
##### MongoDB
- Historial de eventos de cada incidente
##### Ventajas
- SQL para datos relacionales
- Mongo para eventos flexibles
##### Tradeoff
- Dos bases de datos implican mayor complejidad operativa

## Configuración
La conexión a la base de datos se configura mediante `ConnectionStrings` en `appsettings.json`.

Ejemplo:
```bash
"ConnectionStrings": {
  "DefaultConnection": "Server=sqlserver,1433;User Id=sa;Database=IncidentDb;Password=mypassword;TrustServerCertificate=True"
},
```
En el entorno Docker el host `sqlserver` corresponde al contenedor de SQL Server definido en `docker-compose`.

#### Migraciones
Este proyecto ya tiene las migraciones creadas y se ejecutarán automáticamente al levantar el proyecto

## Usuarios de Prueba
El sistema crea usuarios automáticamente mediante seed. Es necesario iniciar sesión, ya que, las rutas están protegidas.

| Usuario           | Password     | 
| :--------         | :-------     | 
| `admin@email.com` | `Admin123!`  | 
| `user@email.com`  | `User123!`   |

## Tests

El proyecto incluye pruebas unitarias en `Incident.Api.Tests`.

Ejecutar:

```bash
dotnet test
```
