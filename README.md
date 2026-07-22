<div align="center">

## EduApoyos

### API para la gestión de solicitudes de apoyo económico estudiantil

![.NET](https://img.shields.io/badge/.NET-8.0-512BD4?style=for-the-badge&logo=dotnet&logoColor=white)
![ASP.NET Core](https://img.shields.io/badge/ASP.NET_Core-Web_API-6C3483?style=for-the-badge&logo=dotnet&logoColor=white)
![SQL Server](https://img.shields.io/badge/SQL_Server-2022-CC2927?style=for-the-badge&logo=microsoftsqlserver&logoColor=white)
![Docker](https://img.shields.io/badge/Docker-Compose-2496ED?style=for-the-badge&logo=docker&logoColor=white)
![Tests](https://img.shields.io/badge/Tests-24_aprobadas-2EA44F?style=for-the-badge)
![Coverage](https://img.shields.io/badge/Cobertura-%3E_80%25-2EA44F?style=for-the-badge)

<br>

**Backend desarrollado con arquitectura limpia, autenticación JWT, trazabilidad de estados, pruebas automatizadas y documentación OpenAPI.**

</div>

---

## Información general

**Fecha de elaboración:** julio de 2026

**Repositorio:** [github.com/LeidyH15/EduApoyos](https://github.com/LeidyH15/EduApoyos)

Una institución de educación superior requiere centralizar la gestión de solicitudes de apoyo económico, como becas, créditos y subsidios. El proceso se realizaba mediante hojas de cálculo y correos electrónicos, lo que ocasionaba reprocesos, pérdida de información y falta de trazabilidad.

**EduApoyos** permite a los asesores registrar y administrar estudiantes y solicitudes, mientras que cada estudiante puede consultar el estado de sus solicitudes desde un portal de autogestión y descargar una constancia.

> [!NOTE]
> Este repositorio contiene actualmente el backend. El frontend se desarrollará como un incremento posterior.

---

## Funcionalidades

- Registro e inicio de sesión con roles `Asesor` y `Estudiante`.
- Autenticación mediante JWT Bearer con expiración configurable.
- CRUD de estudiantes con información personal y académica.
- Creación, consulta y actualización de solicitudes de apoyo.
- Tipos de apoyo: `Beca`, `Crédito` y `Subsidio`.
- Flujo controlado de estados:

```text
Pendiente → En revisión → Aprobada
                       └→ Rechazada
```

- Historial auditable de cambios de estado.
- Listados con filtros por estado, tipo de apoyo y fechas.
- Paginación en estudiantes y solicitudes.
- Portal para consultar las solicitudes propias del estudiante.
- Descarga segura de constancias en formato de texto.
- Restricción de acceso por rol y propiedad del recurso.
- Errores estandarizados mediante `ProblemDetails`.
- Validaciones mediante DataAnnotations y reglas de dominio.
- Documentación interactiva mediante Swagger/OpenAPI.

---

## Arquitectura

La solución utiliza una arquitectura limpia dividida por responsabilidades:

```text
EduApoyos
├── src
│   ├── EduApoyos.Domain
│   ├── EduApoyos.Application
│   ├── EduApoyos.Infrastructure
│   └── EduApoyos.Api
├── tests
│   ├── EduApoyos.UnitTests
│   └── EduApoyos.IntegrationTests
├── database
│   └── scripts
├── docker-compose.yml
├── coverlet.runsettings
└── EduApoyos.sln
```

| Capa | Responsabilidad |
|---|---|
| `Domain` | Entidades, enumeraciones, excepciones y reglas del negocio. No depende de infraestructura. |
| `Application` | Contratos, DTOs, modelos de respuesta, abstracciones y casos de uso. |
| `Infrastructure` | EF Core, SQL Server, Identity, JWT, repositorios, servicios y generadores de archivos. |
| `Api` | Controladores, autenticación, middleware de errores, Swagger y configuración HTTP. |
| `UnitTests` | Pruebas aisladas de entidades y reglas de dominio. |
| `IntegrationTests` | Pruebas de flujos HTTP, autenticación, autorización, persistencia y descargas. |

La dirección de dependencias mantiene el dominio independiente de frameworks y detalles externos.

---

## Patrones de diseño

### 1. Repository

Los contratos `IEstudianteRepository` e `ISolicitudApoyoRepository` abstraen el acceso a datos. Las implementaciones con Entity Framework Core permanecen en Infrastructure.

**Motivo de elección:**

- Evita que las reglas de negocio dependan directamente de EF Core.
- Centraliza consultas y operaciones de persistencia.
- Facilita las pruebas y la sustitución de la tecnología de almacenamiento.
- Mantiene separadas las responsabilidades de negocio y acceso a datos.

### 2. Strategy

Las estrategias de estado encapsulan las transiciones permitidas de una solicitud:

- `EstrategiaSolicitudPendiente`
- `EstrategiaSolicitudEnRevision`

**Motivo de elección:**

- Cada estado aplica sus propias reglas de transición.
- Evita una cadena extensa de condicionales en el servicio.
- Permite agregar nuevos estados sin modificar toda la lógica existente.
- Garantiza que cada cambio genere su registro en el historial.

### 3. Factory

`IConstanciaSolicitudFactory` define la creación de constancias y `ConstanciaSolicitudTextoFactory` genera el archivo descargable.

**Motivo de elección:**

- Separa la construcción del documento de los controladores y servicios.
- Permite sustituir el formato de texto por PDF sin cambiar las reglas de negocio.
- Centraliza el nombre, contenido y tipo MIME del archivo.

> [!TIP]
> También se emplea una unidad de trabajo mediante `IUnidadTrabajo` para coordinar la confirmación de cambios en la base de datos.

---

## Tecnologías utilizadas

| Componente | Tecnología |
|---|---|
| Backend | .NET 8 y ASP.NET Core Web API |
| ORM | Entity Framework Core 8 - Code First |
| Base de datos | SQL Server 2022 |
| Seguridad | ASP.NET Core Identity y JWT Bearer |
| Documentación | Swagger / OpenAPI con Swashbuckle |
| Validación | DataAnnotations y reglas de dominio |
| Pruebas | xUnit, Moq y WebApplicationFactory |
| Cobertura | Coverlet y ReportGenerator |
| Contenedores | Docker y Docker Compose |

---

## Ejecución local

### Prerrequisitos

- [.NET SDK 8](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Docker Desktop](https://www.docker.com/products/docker-desktop/)
- Git
- Visual Studio o un IDE compatible con .NET 8

Compruebe las instalaciones:

```powershell
dotnet --version
docker --version
docker compose version
git --version
```

### 1. Clonar el repositorio

```powershell
git clone https://github.com/LeidyH15/EduApoyos.git
cd EduApoyos
```

### 2. Configurar SQL Server para Docker

Cree un archivo `.env` en la raíz. Este archivo está excluido de Git:

```env
SQL_SA_PASSWORD=<SU_PASSWORD_SEGURA_DE_SQL_SERVER>
```

La contraseña debe cumplir las reglas de complejidad de SQL Server: mayúsculas, minúsculas, números y caracteres especiales.

Inicie el contenedor:

```powershell
docker compose up -d
docker compose ps
```

Para consultar los registros:

```powershell
docker compose logs --tail 50 sqlserver
```

Para detenerlo sin eliminar los datos:

```powershell
docker compose stop
```

### 3. Configurar secretos de desarrollo

La cadena de conexión, la clave JWT y la contraseña inicial no se guardan en `appsettings.json` ni en Git.

```powershell
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Server=localhost,1433;Database=EduApoyosDb;User Id=sa;Password=<SU_PASSWORD_SEGURA_DE_SQL_SERVER>;TrustServerCertificate=True;" --project src/EduApoyos.Api/EduApoyos.Api.csproj

dotnet user-secrets set "Jwt:Key" "<CLAVE_JWT_SEGURA_DE_AL_MENOS_32_BYTES>" --project src/EduApoyos.Api/EduApoyos.Api.csproj

dotnet user-secrets set "SeedAsesor:Password" "<PASSWORD_SEGURA_DEL_ASESOR>" --project src/EduApoyos.Api/EduApoyos.Api.csproj
```

Compruebe la existencia de las claves configuradas:

```powershell
dotnet user-secrets list --project src/EduApoyos.Api/EduApoyos.Api.csproj
```

> [!WARNING]
> No publique el contenido de User Secrets, el archivo `.env`, contraseñas ni tokens JWT.

### 4. Restaurar y compilar

```powershell
dotnet restore EduApoyos.sln
dotnet build EduApoyos.sln
```

### 5. Crear o actualizar la base de datos

```powershell
dotnet ef database update --project src/EduApoyos.Infrastructure/EduApoyos.Infrastructure.csproj --startup-project src/EduApoyos.Api/EduApoyos.Api.csproj
```

Si la herramienta `dotnet-ef` no está instalada:

```powershell
dotnet tool install --global dotnet-ef --version 8.0.24
```

### 6. Ejecutar la API

Desde Visual Studio, configure `EduApoyos.Api` como proyecto de inicio y ejecute el perfil `https`.

Desde PowerShell:

```powershell
dotnet run --project src/EduApoyos.Api/EduApoyos.Api.csproj --launch-profile https
```

Swagger estará disponible en:

```text
https://localhost:7120/swagger
```

---

## Autenticación y roles

Al iniciar la aplicación se crean los roles `Asesor` y `Estudiante`, junto con el asesor inicial configurado mediante:

```text
SeedAsesor:NombreCompleto
SeedAsesor:Email
SeedAsesor:Password
```

Valores públicos predeterminados:

```text
Nombre: Asesor Inicial
Correo: asesor@eduapoyos.local
```

La contraseña debe configurarse con User Secrets.

Para utilizar endpoints protegidos desde Swagger:

1. Ejecute `POST /api/auth/login`.
2. Copie el valor de `token`.
3. Seleccione **Authorize**.
4. Ingrese solamente el token, sin escribir la palabra `Bearer`.
5. Ejecute el endpoint protegido.

---

## Endpoints principales

### Autenticación

| Método | Ruta | Descripción | Acceso |
|---|---|---|---|
| `POST` | `/api/auth/register` | Registra un estudiante y devuelve un JWT. | Público |
| `POST` | `/api/auth/login` | Autentica al usuario y devuelve un JWT. | Público |

### Estudiantes

| Método | Ruta | Descripción | Acceso |
|---|---|---|---|
| `GET` | `/api/estudiantes` | Lista estudiantes con paginación. | Asesor |
| `GET` | `/api/estudiantes/{id}` | Consulta el detalle de un estudiante. | Asesor |
| `POST` | `/api/estudiantes` | Crea el estudiante y su usuario de acceso. | Asesor |
| `PUT` | `/api/estudiantes/{id}` | Actualiza los datos del estudiante. | Asesor |
| `DELETE` | `/api/estudiantes/{id}` | Elimina un estudiante sin solicitudes. | Asesor |

### Solicitudes

| Método | Ruta | Descripción | Acceso |
|---|---|---|---|
| `GET` | `/api/solicitudes` | Lista y filtra solicitudes con paginación. | Asesor |
| `GET` | `/api/solicitudes/{id}` | Obtiene la solicitud y su historial. | Asesor / propietario |
| `POST` | `/api/solicitudes` | Crea una solicitud de apoyo. | Asesor / Estudiante |
| `PUT` | `/api/solicitudes/{id}` | Actualiza una solicitud pendiente. | Asesor / propietario |
| `PATCH` | `/api/solicitudes/{id}/estado` | Cambia el estado aplicando Strategy. | Asesor |
| `GET` | `/api/solicitudes/{id}/constancia` | Descarga la constancia en texto. | Asesor / propietario |
| `GET` | `/api/estudiantes/{id}/solicitudes` | Consulta las solicitudes del portal. | Asesor / propietario |

Los endpoints de listado admiten paginación. El listado de solicitudes también permite filtros por estado, tipo de apoyo y rango de fechas.

---

## Manejo de errores y validaciones

La API utiliza un manejador global de excepciones que genera respuestas `application/problem+json` compatibles con Problem Details.

| Estado HTTP | Uso principal |
|---|---|
| `400 Bad Request` | Datos o parámetros inválidos. |
| `401 Unauthorized` | Token ausente, inválido o credenciales incorrectas. |
| `403 Forbidden` | El usuario no tiene permisos sobre el recurso. |
| `404 Not Found` | El recurso solicitado no existe. |
| `409 Conflict` | Correo o documento duplicado. |
| `422 Unprocessable Entity` | Regla de negocio no satisfecha. |
| `500 Internal Server Error` | Error inesperado sin exposición de detalles internos. |

Cada respuesta controlada contiene `status`, `title`, `detail`, `instance` y `traceId`.

---

## Base de datos y ejercicios SQL

El modelo se genera con migraciones Code First e incluye las entidades principales:

- Usuarios e información de Identity.
- Estudiantes.
- Solicitudes de apoyo.
- Historial de estados.

Los scripts requeridos se encuentran en:

```text
database/scripts
```

Incluyen:

1. Solicitudes pendientes con más de cinco días sin actualización, ordenadas por antigüedad.
2. Total de solicitudes agrupadas por estado y tipo de apoyo durante el último mes.
3. Creación y justificación de un índice no agrupado sobre solicitudes.

### Índice no agrupado

El índice sobre estado y fecha de actualización optimiza las consultas que identifican solicitudes pendientes antiguas y reduce la necesidad de recorrer completamente la tabla.

Su diseño también beneficia filtros operativos frecuentes del asesor, manteniendo como columnas incluidas los datos necesarios para resolver el listado sin accesos adicionales innecesarios.

---

## Pruebas y cobertura

La solución contiene pruebas unitarias y de integración para:

- Reglas de creación y actualización de solicitudes.
- Transiciones de estado mediante Strategy.
- Autenticación correcta e incorrecta.
- Autorización mediante roles y JWT.
- CRUD completo de estudiantes.
- Flujo completo de solicitudes.
- Respuestas ProblemDetails.
- Descarga de constancias.
- Restricción de acceso a solicitudes ajenas.

Ejecute todas las pruebas:

```powershell
dotnet test EduApoyos.sln
```

Estado actual:

```text
24 pruebas aprobadas
0 pruebas fallidas
Cobertura de líneas superior al 80%
```

### Generar el reporte de cobertura

```powershell
Remove-Item TestResults -Recurse -Force -ErrorAction SilentlyContinue
Remove-Item CoverageReport -Recurse -Force -ErrorAction SilentlyContinue

dotnet test EduApoyos.sln --settings coverlet.runsettings --results-directory TestResults

reportgenerator "-reports:TestResults/**/coverage.cobertura.xml" "-targetdir:CoverageReport" "-reporttypes:Html;TextSummary"

Get-Content CoverageReport/Summary.txt
```

Las migraciones generadas por EF Core se excluyen del cálculo porque no contienen lógica escrita manualmente.

---

## Consideraciones de seguridad

- Las contraseñas se almacenan mediante ASP.NET Core Identity, no como texto plano.
- La API valida emisor, audiencia, firma y expiración del JWT.
- La clave JWT debe contener como mínimo 32 bytes.
- El acceso se controla mediante roles y propiedad del recurso.
- Las credenciales locales se mantienen fuera del repositorio.
- Los errores internos no exponen trazas ni información sensible al cliente.
- Swagger se habilita exclusivamente en el ambiente de desarrollo.

---

## Decisiones y mejoras futuras

- La constancia se genera actualmente como texto para cumplir el requisito sin introducir dependencias innecesarias. El Factory permite incorporar PDF posteriormente.
- El frontend se implementará como una SPA que consumirá esta API.
- Se puede incorporar renovación de tokens, recuperación de contraseña y verificación de correo.
- Para producción se recomienda utilizar un gestor de secretos, HTTPS administrado, observabilidad centralizada y CI/CD.
- Las mediciones de rendimiento deben ejecutarse en un ambiente equivalente al despliegue objetivo.

---

## Autora

Desarrollado con dedicación por:

### **LEIDY STEPHANIA HERNÁNDEZ VARÓN**

- **Correo:** [stefania.hernandez.09@hotmail.com](mailto:stefania.hernandez.09@hotmail.com)
- **GitHub:** [LeidyH15](https://github.com/LeidyH15)

<div align="center">

---

**EduApoyos · Arquitectura limpia · .NET 8 · SQL Server**

</div>