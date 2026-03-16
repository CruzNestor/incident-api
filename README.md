# Incident API

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

#### Domain
- Entidades del negocio
- Enums y reglas básicas
#### Application
- Servicios
- Interfaces
- DTOs
- Excepciones
#### Infrastructure
- Implementación de repositorios (ASP.NET Core Identity)
- EF Core
- MongoDB
- Integraciones externas
#### Api
- Controladores
- Configuraciones
- DI

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
Asegúrese de tener las credenciales correctas en `ConnectionStrings`

```bash
"ConnectionStrings": {
  "DefaultConnection": "Server=sqlserver,1433;User Id=sa;Database=IncidentDb;Password=mypassword;TrustServerCertificate=True"
},
```

#### Migraciones
Este proyecto ya tiene las migraciones creadas y se ejecutarán automáticamente al levantar el proyecto

## Usuarios de Prueba
El sistema crea usuarios automáticamente mediante seed. Es necesario iniciar sesión, ya que, las rutas estan protegidas.

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
