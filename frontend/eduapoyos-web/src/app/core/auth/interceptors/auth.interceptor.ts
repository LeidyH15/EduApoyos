import {
  inject
} from '@angular/core';
import {
  HttpInterceptorFn
} from '@angular/common/http';
import {
  environment
} from '../../../../environments/environment';
import {
  SesionService
} from '../services/sesion.service';

export const authInterceptor:
  HttpInterceptorFn = (request, next) => {
    const sesionService =
      inject(SesionService);

    const token =
      sesionService.obtenerToken();

    const esPeticionApi =
      request.url.startsWith(
        environment.apiUrl
      );

    if (!token || !esPeticionApi) {
      return next(request);
    }

    const requestAutorizado =
      request.clone({
        setHeaders: {
          Authorization: `Bearer ${token}`
        }
      });

    return next(requestAutorizado);
  };