/*
    AUTOR: Leidy Stephania Hernandez Varon
    
    Ejercicio 2

    Cuenta las solicitudes agrupadas por estado y tipo
    de apoyo creadas durante el último mes.

    Estados:
        1 = Pendiente
        2 = EnRevision
        3 = Aprobada
        4 = Rechazada

    Tipos de apoyo:
        1 = Beca
        2 = Credito
        3 = Subsidio
*/

DECLARE @FechaInicio DATETIME2 =
    DATEADD(
        MONTH,
        -1,
        SYSUTCDATETIME()
    );

SELECT
    s.Estado,
    CASE s.Estado
        WHEN 1 THEN 'Pendiente'
        WHEN 2 THEN 'En revisión'
        WHEN 3 THEN 'Aprobada'
        WHEN 4 THEN 'Rechazada'
        ELSE 'Desconocido'
    END AS NombreEstado,

    s.TipoApoyo,
    CASE s.TipoApoyo
        WHEN 1 THEN 'Beca'
        WHEN 2 THEN 'Crédito'
        WHEN 3 THEN 'Subsidio'
        ELSE 'Desconocido'
    END AS NombreTipoApoyo,

    COUNT(*) AS TotalSolicitudes
FROM dbo.SolicitudesApoyo AS s
WHERE
    s.FechaSolicitud >= @FechaInicio
    AND s.FechaSolicitud <= SYSUTCDATETIME()
GROUP BY
    s.Estado,
    s.TipoApoyo
ORDER BY
    s.Estado,
    s.TipoApoyo;