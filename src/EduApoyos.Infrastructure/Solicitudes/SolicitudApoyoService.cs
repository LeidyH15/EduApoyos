using EduApoyos.Application.Solicitudes.Constancias;
using EduApoyos.Application.Abstractions.Persistence;
using EduApoyos.Application.Common.Exceptions;
using EduApoyos.Application.Common.Models;
using EduApoyos.Application.Solicitudes;
using EduApoyos.Domain.Entities;
using EduApoyos.Domain.Enums;
using EduApoyos.Domain.Exceptions;
using EduApoyos.Domain.Strategies;
using EduApoyos.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace EduApoyos.Infrastructure.Solicitudes;

public class SolicitudApoyoService : ISolicitudApoyoService
{
    private readonly ISolicitudApoyoRepository
        _solicitudRepository;

    private readonly IEstudianteRepository
        _estudianteRepository;

    private readonly IUnidadTrabajo _unidadTrabajo;

    private readonly ApplicationDbContext _context;

    private readonly IReadOnlyCollection<IEstrategiaEstadoSolicitud>
        _estrategias;

    private readonly IConstanciaSolicitudFactory
    _constanciaFactory;

    public SolicitudApoyoService(
    ISolicitudApoyoRepository solicitudRepository,
    IEstudianteRepository estudianteRepository,
    IUnidadTrabajo unidadTrabajo,
    ApplicationDbContext context,
    IEnumerable<IEstrategiaEstadoSolicitud> estrategias,
    IConstanciaSolicitudFactory constanciaFactory)
    {
        _solicitudRepository = solicitudRepository;
        _estudianteRepository = estudianteRepository;
        _unidadTrabajo = unidadTrabajo;
        _context = context;
        _estrategias = estrategias.ToList();
        _constanciaFactory = constanciaFactory;
    }

    public async Task<ResultadoPaginado<SolicitudResponse>>
        ListarAsync(
            SolicitudFiltroRequest filtro,
            CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(filtro);

        ValidarPaginacion(
            filtro.Pagina,
            filtro.TamanoPagina);

        ValidarRangoFechas(
            filtro.FechaDesde,
            filtro.FechaHasta);

        var resultado =
            await _solicitudRepository.ListarAsync(
                filtro.Estado,
                filtro.TipoApoyo,
                filtro.FechaDesde,
                filtro.FechaHasta,
                filtro.Pagina,
                filtro.TamanoPagina,
                cancellationToken);

        return await CrearResultadoPaginadoAsync(
            resultado,
            estudianteConocido: null,
            cancellationToken);
    }

    public async Task<SolicitudResponse> ObtenerPorIdAsync(
        Guid solicitudId,
        Guid usuarioActualId,
        bool esAsesor,
        CancellationToken cancellationToken = default)
    {
        ValidarIdentificador(
            solicitudId,
            nameof(solicitudId));

        ValidarIdentificador(
            usuarioActualId,
            nameof(usuarioActualId));

        var solicitud =
            await ObtenerSolicitudAsync(
                solicitudId,
                incluirHistorial: true,
                cancellationToken);

        ValidarAcceso(
            solicitud,
            usuarioActualId,
            esAsesor);

        return await CrearRespuestaAsync(
            solicitud,
            solicitud.Estudiante,
            cancellationToken);
    }

    public async Task<ConstanciaSolicitudArchivo>
    GenerarConstanciaAsync(
        Guid solicitudId,
        Guid usuarioActualId,
        bool esAsesor,
        CancellationToken cancellationToken = default)
    {
        var solicitud =
            await ObtenerPorIdAsync(
                solicitudId,
                usuarioActualId,
                esAsesor,
                cancellationToken);

        return _constanciaFactory.Crear(
            solicitud);
    }

    public async Task<SolicitudResponse> CrearAsync(
        Guid usuarioActualId,
        bool esAsesor,
        CrearSolicitudRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        ValidarIdentificador(
            usuarioActualId,
            nameof(usuarioActualId));

        Estudiante estudiante;

        if (esAsesor)
        {
            if (!request.EstudianteId.HasValue ||
                request.EstudianteId.Value == Guid.Empty)
            {
                throw new ArgumentException(
                    "El asesor debe indicar el estudiante de la solicitud.",
                    nameof(request.EstudianteId));
            }

            estudiante =
                await ObtenerEstudianteAsync(
                    request.EstudianteId.Value,
                    cancellationToken);
        }
        else
        {
            estudiante =
                await _estudianteRepository
                    .ObtenerPorUsuarioIdAsync(
                        usuarioActualId,
                        cancellationToken)
                ?? throw new KeyNotFoundException(
                    "No se encontró el perfil del estudiante autenticado.");
        }

        var solicitud = new SolicitudApoyo(
            estudiante.Id,
            request.TipoApoyo,
            request.MontoSolicitado,
            request.Descripcion);

        await _solicitudRepository.AgregarAsync(
            solicitud,
            cancellationToken);

        await _unidadTrabajo.GuardarCambiosAsync(
            cancellationToken);

        return await CrearRespuestaAsync(
            solicitud,
            estudiante,
            cancellationToken);
    }

    public async Task<SolicitudResponse> ActualizarAsync(
        Guid solicitudId,
        Guid usuarioActualId,
        bool esAsesor,
        ActualizarSolicitudRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        ValidarIdentificador(
            solicitudId,
            nameof(solicitudId));

        ValidarIdentificador(
            usuarioActualId,
            nameof(usuarioActualId));

        var solicitud =
            await ObtenerSolicitudAsync(
                solicitudId,
                incluirHistorial: false,
                cancellationToken);

        ValidarAcceso(
            solicitud,
            usuarioActualId,
            esAsesor);

        if (solicitud.Estado != EstadoSolicitud.Pendiente)
        {
            throw new ReglaNegocioException(
                "Solo se puede actualizar una solicitud pendiente.");
        }

        solicitud.ActualizarInformacion(
            request.TipoApoyo,
            request.MontoSolicitado,
            request.Descripcion);

        _solicitudRepository.Actualizar(solicitud);

        await _unidadTrabajo.GuardarCambiosAsync(
            cancellationToken);

        return await CrearRespuestaAsync(
            solicitud,
            solicitud.Estudiante,
            cancellationToken);
    }

    public async Task<SolicitudResponse> CambiarEstadoAsync(
        Guid solicitudId,
        Guid asesorId,
        CambiarEstadoSolicitudRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        ValidarIdentificador(
            solicitudId,
            nameof(solicitudId));

        ValidarIdentificador(
            asesorId,
            nameof(asesorId));

        var solicitud =
            await ObtenerSolicitudAsync(
                solicitudId,
                incluirHistorial: true,
                cancellationToken);

        var estrategia = _estrategias.FirstOrDefault(
            item => item.EstadoOrigen == solicitud.Estado);

        if (estrategia is null)
        {
            throw new ReglaNegocioException(
                $"No existen transiciones disponibles desde el estado {solicitud.Estado}.");
        }

        var historialExistenteIds =
            solicitud.HistorialEstados
                .Select(historial => historial.Id)
                .ToHashSet();

        estrategia.CambiarEstado(
            solicitud,
            request.NuevoEstado,
            asesorId,
            request.Observacion);

        var nuevoHistorial =
            solicitud.HistorialEstados.Single(
                historial =>
                    !historialExistenteIds.Contains(
                        historial.Id));

        _context.Entry(nuevoHistorial).State =
            EntityState.Added;

        await _unidadTrabajo.GuardarCambiosAsync(
            cancellationToken);

        return await CrearRespuestaAsync(
            solicitud,
            solicitud.Estudiante,
            cancellationToken);
    }

    public async Task<ResultadoPaginado<SolicitudResponse>>
        ListarPorEstudianteAsync(
            Guid estudianteId,
            Guid usuarioActualId,
            bool esAsesor,
            int pagina,
            int tamanoPagina,
            CancellationToken cancellationToken = default)
    {
        ValidarIdentificador(
            estudianteId,
            nameof(estudianteId));

        ValidarIdentificador(
            usuarioActualId,
            nameof(usuarioActualId));

        ValidarPaginacion(
            pagina,
            tamanoPagina);

        var estudiante =
            await ObtenerEstudianteAsync(
                estudianteId,
                cancellationToken);

        if (!esAsesor &&
            estudiante.UsuarioId != usuarioActualId)
        {
            throw new AccesoDenegadoException(
                "No tiene permiso para consultar las solicitudes de otro estudiante.");
        }

        var resultado =
            await _solicitudRepository
                .ListarPorEstudianteAsync(
                    estudianteId,
                    pagina,
                    tamanoPagina,
                    cancellationToken);

        return await CrearResultadoPaginadoAsync(
            resultado,
            estudiante,
            cancellationToken);
    }

    private async Task<SolicitudApoyo> ObtenerSolicitudAsync(
        Guid solicitudId,
        bool incluirHistorial,
        CancellationToken cancellationToken)
    {
        var solicitud =
            await _solicitudRepository.ObtenerPorIdAsync(
                solicitudId,
                incluirHistorial,
                cancellationToken);

        return solicitud ??
            throw new KeyNotFoundException(
                "No se encontró la solicitud de apoyo.");
    }

    private async Task<Estudiante> ObtenerEstudianteAsync(
        Guid estudianteId,
        CancellationToken cancellationToken)
    {
        var estudiante =
            await _estudianteRepository.ObtenerPorIdAsync(
                estudianteId,
                cancellationToken);

        return estudiante ??
            throw new KeyNotFoundException(
                "No se encontró el estudiante indicado.");
    }

    private static void ValidarAcceso(
        SolicitudApoyo solicitud,
        Guid usuarioActualId,
        bool esAsesor)
    {
        if (esAsesor)
        {
            return;
        }

        if (solicitud.Estudiante.UsuarioId != usuarioActualId)
        {
            throw new AccesoDenegadoException(
                "No tiene permiso para acceder a esta solicitud.");
        }
    }

    private async Task<ResultadoPaginado<SolicitudResponse>>
        CrearResultadoPaginadoAsync(
            ResultadoPaginado<SolicitudApoyo> resultado,
            Estudiante? estudianteConocido,
            CancellationToken cancellationToken)
    {
        var usuariosIds = estudianteConocido is not null
            ? new List<Guid>
            {
                estudianteConocido.UsuarioId
            }
            : resultado.Elementos
                .Select(solicitud =>
                    solicitud.Estudiante.UsuarioId)
                .Distinct()
                .ToList();

        var usuarios = await _context.Users
            .AsNoTracking()
            .Where(usuario =>
                usuariosIds.Contains(usuario.Id))
            .ToDictionaryAsync(
                usuario => usuario.Id,
                usuario => usuario.NombreCompleto,
                cancellationToken);

        var respuestas = resultado.Elementos
            .Select(solicitud =>
            {
                var estudiante =
                    estudianteConocido ??
                    solicitud.Estudiante;

                usuarios.TryGetValue(
                    estudiante.UsuarioId,
                    out var nombreEstudiante);

                return CrearRespuesta(
                    solicitud,
                    estudiante,
                    nombreEstudiante ?? string.Empty);
            })
            .ToList();

        return new ResultadoPaginado<SolicitudResponse>(
            respuestas,
            resultado.Pagina,
            resultado.TamanoPagina,
            resultado.TotalElementos);
    }

    private async Task<SolicitudResponse> CrearRespuestaAsync(
        SolicitudApoyo solicitud,
        Estudiante estudiante,
        CancellationToken cancellationToken)
    {
        var nombreEstudiante =
            await _context.Users
                .AsNoTracking()
                .Where(usuario =>
                    usuario.Id == estudiante.UsuarioId)
                .Select(usuario =>
                    usuario.NombreCompleto)
                .FirstOrDefaultAsync(cancellationToken)
            ?? string.Empty;

        return CrearRespuesta(
            solicitud,
            estudiante,
            nombreEstudiante);
    }

    private static SolicitudResponse CrearRespuesta(
        SolicitudApoyo solicitud,
        Estudiante estudiante,
        string nombreEstudiante)
    {
        return new SolicitudResponse
        {
            Id = solicitud.Id,
            EstudianteId = solicitud.EstudianteId,
            UsuarioEstudianteId =
                estudiante.UsuarioId,
            NombreEstudiante =
                nombreEstudiante,
            NumeroDocumento =
                estudiante.NumeroDocumento,
            TipoApoyo =
                solicitud.TipoApoyo,
            MontoSolicitado =
                solicitud.MontoSolicitado,
            Descripcion =
                solicitud.Descripcion,
            Estado =
                solicitud.Estado,
            FechaSolicitud =
                solicitud.FechaSolicitud,
            FechaActualizacion =
                solicitud.FechaActualizacion,
            AsesorId =
                solicitud.AsesorId,

            Historial = solicitud.HistorialEstados
                .OrderByDescending(historial =>
                    historial.FechaCambio)
                .Select(historial =>
                    new HistorialEstadoResponse
                    {
                        Id = historial.Id,
                        EstadoAnterior =
                            historial.EstadoAnterior,
                        EstadoNuevo =
                            historial.EstadoNuevo,
                        FechaCambio =
                            historial.FechaCambio,
                        UsuarioId =
                            historial.UsuarioId,
                        Observacion =
                            historial.Observacion
                    })
                .ToList()
        };
    }

    private static void ValidarPaginacion(
        int pagina,
        int tamanoPagina)
    {
        if (pagina < 1)
        {
            throw new ArgumentException(
                "La página debe ser mayor o igual a 1.",
                nameof(pagina));
        }

        if (tamanoPagina is < 1 or > 100)
        {
            throw new ArgumentException(
                "El tamaño de página debe estar entre 1 y 100.",
                nameof(tamanoPagina));
        }
    }

    private static void ValidarRangoFechas(
        DateTime? fechaDesde,
        DateTime? fechaHasta)
    {
        if (fechaDesde.HasValue &&
            fechaHasta.HasValue &&
            fechaDesde.Value > fechaHasta.Value)
        {
            throw new ArgumentException(
                "La fecha inicial no puede ser posterior a la fecha final.");
        }
    }

    private static void ValidarIdentificador(
        Guid identificador,
        string nombreParametro)
    {
        if (identificador == Guid.Empty)
        {
            throw new ArgumentException(
                "El identificador no es válido.",
                nombreParametro);
        }
    }
}