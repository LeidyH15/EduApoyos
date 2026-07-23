import {
  inject,
  Injectable
} from '@angular/core';
import {
  HttpClient
} from '@angular/common/http';
import {
  Observable,
  tap
} from 'rxjs';
import {
  environment
} from '../../../../environments/environment';
import {
  LoginRequest
} from '../models/login-request';
import {
  AutenticacionResponse
} from '../models/autenticacion-response';
import {
  SesionService
} from './sesion.service';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private readonly http =
    inject(HttpClient);

  private readonly sesionService =
    inject(SesionService);

  private readonly endpoint =
    `${environment.apiUrl}/auth`;

  iniciarSesion(
    request: LoginRequest
  ): Observable<AutenticacionResponse> {
    return this.http
      .post<AutenticacionResponse>(
        `${this.endpoint}/login`,
        request
      )
      .pipe(
        tap((response) =>
          this.sesionService.iniciarSesion(
            response
          )
        )
      );
  }

  cerrarSesion(): void {
    this.sesionService.cerrarSesion();
  }
}