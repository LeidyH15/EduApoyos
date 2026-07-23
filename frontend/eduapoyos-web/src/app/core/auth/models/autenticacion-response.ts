export type RolUsuario =
  | 'Asesor'
  | 'Estudiante';

export interface AutenticacionResponse {
  usuarioId: string;
  nombreCompleto: string;
  email: string;
  rol: RolUsuario;
  token: string;
  expiracion: string;
}