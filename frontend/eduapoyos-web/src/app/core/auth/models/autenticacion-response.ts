export type RolUsuario =
  | 'Asesor'
  | 'Estudiante';

export interface AutenticacionResponse {
  usuarioId: string;
  estudianteId: string | null;
  nombreCompleto: string;
  email: string;
  rol: RolUsuario;
  token: string;
  expiracion: string;
}