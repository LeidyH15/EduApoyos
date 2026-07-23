import {
  inject
} from '@angular/core';
import {
  CanActivateFn,
  Router
} from '@angular/router';
import {
  RolUsuario
} from '../models/autenticacion-response';
import {
  SesionService
} from '../services/sesion.service';

export const roleGuard:
  CanActivateFn = (route) => {
    const sesionService =
      inject(SesionService);

    const router =
      inject(Router);

    if (!sesionService.estaAutenticado()) {
      return router.createUrlTree(
        ['/login']
      );
    }

    const rolesPermitidos =
      route.data['roles'] as
        RolUsuario[] | undefined;

    const rolActual =
      sesionService.obtenerRol();

    if (
      rolActual &&
      rolesPermitidos?.includes(rolActual)
    ) {
      return true;
    }

    const rutaPermitida =
      rolActual === 'Asesor'
        ? '/asesor/solicitudes'
        : '/estudiante/portal';

    return router.createUrlTree(
      [rutaPermitida]
    );
  };