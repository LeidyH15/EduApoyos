/*
    AUTOR: Leidy Stephania Hernandez Varon
    
    Ejercicio 3

    Índice no agrupado para optimizar consultas que filtran
    solicitudes por estado y fecha de actualización.

    Es especialmente útil para la consulta que busca
    solicitudes pendientes con más de cinco días sin actualizar.

    Estado se ubica primero porque se utiliza como condición
    de igualdad.

    FechaActualizacion se ubica después porque se utiliza
    como condición de rango y criterio de ordenamiento.
*/

IF NOT EXISTS
(
    SELECT 1
    FROM sys.indexes
    WHERE
        name =
            'IX_SolicitudesApoyo_Estado_FechaActualizacion'
        AND object_id =
            OBJECT_ID('dbo.SolicitudesApoyo')
)
BEGIN
    CREATE NONCLUSTERED INDEX
        IX_SolicitudesApoyo_Estado_FechaActualizacion
    ON dbo.SolicitudesApoyo
    (
        Estado ASC,
        FechaActualizacion ASC
    )
    INCLUDE
    (
        EstudianteId,
        TipoApoyo,
        MontoSolicitado,
        FechaSolicitud
    );

    PRINT 'Índice creado correctamente.';
END
ELSE
BEGIN
    PRINT 'El índice ya existe.';
END;
GO

SELECT
    i.name AS NombreIndice,
    i.type_desc AS TipoIndice,
    i.is_unique AS EsUnico,
    i.is_disabled AS EstaDeshabilitado
FROM sys.indexes AS i
WHERE
    i.object_id =
        OBJECT_ID('dbo.SolicitudesApoyo')
    AND i.name =
        'IX_SolicitudesApoyo_Estado_FechaActualizacion';