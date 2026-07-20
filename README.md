# EduApoyos

API para gestionar solicitudes de apoyo económico de estudiantes de una institución de educación superior.

## Estado

Proyecto en construcción incremental. Inicialmente se desarrollará y verificará el backend; el frontend se abordará después.

## Stack acordado

- .NET 8 y ASP.NET Core Web API
- Entity Framework Core 8 (Code First)
- SQL Server
- ASP.NET Core Identity y autenticación JWT
- Swagger / OpenAPI
- xUnit y Moq
- Docker Compose para ejecución local

## Arquitectura prevista

La solución seguirá una arquitectura limpia dividida en `Domain`, `Application`, `Infrastructure` y `Api`, acompañada por proyectos de pruebas.

Los patrones principales serán:

1. **Repository**, para abstraer la persistencia y facilitar pruebas de los casos de uso.
2. **CQRS**, para separar consultas de comandos y mantener responsabilidades claras.

También se aplicará **Strategy** para validar las transiciones permitidas entre estados de una solicitud.

## Alcance del backend

- Registro e inicio de sesión con roles `Asesor` y `Estudiante`.
- Gestión de estudiantes.
- Creación, consulta y actualización de solicitudes.
- Flujo `Pendiente -> EnRevision -> Aprobada | Rechazada`.
- Historial auditable de cambios de estado.
- Filtros y paginación.
- Acceso por rol y propiedad del recurso.
- Respuestas de error con Problem Details.
- Validaciones, Swagger y pruebas automatizadas.
- Migraciones e instrucciones SQL requeridas por el ejercicio.

## Ejecución

Las instrucciones se agregarán cuando exista el primer incremento ejecutable.

