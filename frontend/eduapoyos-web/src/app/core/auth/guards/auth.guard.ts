import {
  inject
} from '@angular/core';
import {
  CanActivateFn,
  Router
} from '@angular/router';
import {
  SesionService
} from '../services/sesion.service';

export const authGuard:
  CanActivateFn = (
    _route,
    state
  ) => {
    const sesionService =
      inject(SesionService);

    const router =
      inject(Router);

    if (sesionService.estaAutenticado()) {
      return true;
    }

    return router.createUrlTree(
      ['/login'],
      {
        queryParams: {
          returnUrl: state.url
        }
      }
    );
  };