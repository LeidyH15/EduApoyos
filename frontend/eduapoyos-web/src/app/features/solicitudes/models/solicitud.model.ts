export enum TipoApoyo {
  Beca = 1,
  Credito = 2,
  Subsidio = 3
}

export enum EstadoSolicitud {
  Pendiente = 1,
  EnRevision = 2,
  Aprobada = 3,
  Rechazada = 4
}

export interface HistorialEstado {
  id: string;
  estadoAnterior: EstadoSolicitud;
  estadoNuevo: EstadoSolicitud;
  fechaCambio: string;
  usuarioId: string;
  observacion: string | null;
}

export interface Solicitud {
  id: string;
  estudianteId: string;
  usuarioEstudianteId: string;
  nombreEstudiante: string;
  numeroDocumento: string;
  tipoApoyo: TipoApoyo;
  montoSolicitado: number;
  descripcion: string;
  estado: EstadoSolicitud;
  fechaSolicitud: string;
  fechaActualizacion: string;
  asesorId: string | null;
  historial: HistorialEstado[];
}

export interface SolicitudFiltro {
  pagina: number;
  tamanoPagina: number;
  estado?: EstadoSolicitud | null;
  tipoApoyo?: TipoApoyo | null;
}

export interface CambiarEstadoSolicitudRequest {
  nuevoEstado: EstadoSolicitud;
  observacion: string;
}