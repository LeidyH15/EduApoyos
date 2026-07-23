import {
  Injectable,
  signal
} from '@angular/core';

import {
  AutenticacionResponse,
  RolUsuario
} from '../models/autenticacion-response';

@Injectable({
  providedIn: 'root'
})
export class SesionService {
  private readonly storageKey =
    'eduapoyos_session';

  private readonly usuarioSignal =
    signal<AutenticacionResponse | null>(
      this.leerSesion()
    );

  readonly usuarioActual =
    this.usuarioSignal.asReadonly();

  iniciarSesion(
    autenticacion: AutenticacionResponse
  ): void {
    sessionStorage.setItem(
      this.storageKey,
      JSON.stringify(autenticacion)
    );

    this.usuarioSignal.set(
      autenticacion
    );
  }

  cerrarSesion(): void {
    sessionStorage.removeItem(
      this.storageKey
    );

    this.usuarioSignal.set(null);
  }

  obtenerToken(): string | null {
    const usuario =
      this.usuarioSignal();

    if (!usuario) {
      return null;
    }

    if (
      this.tokenEstaExpirado(
        usuario.expiracion
      )
    ) {
      this.cerrarSesion();
      return null;
    }

    return usuario.token;
  }

  obtenerRol(): RolUsuario | null {
    return (
      this.usuarioSignal()?.rol ??
      null
    );
  }

  obtenerEstudianteId(): string | null {
    const usuario =
      this.usuarioSignal();

    if (
      !usuario ||
      usuario.rol !== 'Estudiante'
    ) {
      return null;
    }

    return usuario.estudianteId ?? null;
  }

  estaAutenticado(): boolean {
    return this.obtenerToken() !== null;
  }

  tieneRol(
    rol: RolUsuario
  ): boolean {
    return (
      this.estaAutenticado() &&
      this.obtenerRol() === rol
    );
  }

  private leerSesion():
    AutenticacionResponse | null {
    const sesion =
      sessionStorage.getItem(
        this.storageKey
      );

    if (!sesion) {
      return null;
    }

    try {
      const autenticacion =
        JSON.parse(
          sesion
        ) as AutenticacionResponse;

      if (
        !autenticacion.token ||
        this.tokenEstaExpirado(
          autenticacion.expiracion
        )
      ) {
        sessionStorage.removeItem(
          this.storageKey
        );

        return null;
      }

      return autenticacion;
    } catch {
      sessionStorage.removeItem(
        this.storageKey
      );

      return null;
    }
  }

  private tokenEstaExpirado(
    expiracion: string
  ): boolean {
    const fechaExpiracion =
      new Date(expiracion).getTime();

    return (
      Number.isNaN(fechaExpiracion) ||
      fechaExpiracion <= Date.now()
    );
  }
}