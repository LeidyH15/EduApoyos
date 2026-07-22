/*
    AUTOR: Leidy Stephania Hernandez Varon
    Ejercicio 1

    Lista las solicitudes pendientes con más de cinco días
    sin actualización, ordenadas desde la más antigua.
    
    EstadoSolicitud.Pendiente = 1

*/

SELECT
    s.Id,
    s.EstudianteId,
    s.TipoApoyo,
    s.MontoSolicitado,
    s.Descripcion,
    s.Estado,
    s.FechaSolicitud,
    s.FechaActualizacion,
    DATEDIFF(
        DAY,
        s.FechaActualizacion,
        SYSUTCDATETIME()
    ) AS DiasSinActualizacion
FROM dbo.SolicitudesApoyo AS s
WHERE
    s.Estado = 1
    AND s.FechaActualizacion <
        DATEADD(
            DAY,
            -5,
            SYSUTCDATETIME()
        )
ORDER BY
    s.FechaActualizacion ASC;