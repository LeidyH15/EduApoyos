import {
  ChangeDetectionStrategy,
  Component,
  inject,
  signal
} from '@angular/core';
import {
  FormControl,
  FormGroup,
  ReactiveFormsModule,
  Validators
} from '@angular/forms';
import {
  HttpErrorResponse
} from '@angular/common/http';
import {
  ActivatedRoute,
  Router
} from '@angular/router';
import {
  finalize
} from 'rxjs';
import {
  MatButtonModule
} from '@angular/material/button';
import {
  MatCardModule
} from '@angular/material/card';
import {
  MatFormFieldModule
} from '@angular/material/form-field';
import {
  MatInputModule
} from '@angular/material/input';
import {
  MatProgressSpinnerModule
} from '@angular/material/progress-spinner';
import {
  MatSnackBar,
  MatSnackBarModule
} from '@angular/material/snack-bar';
import {
  AuthService
} from '../../../../core/auth/services/auth.service';

@Component({
  selector: 'app-login',
  imports: [
    ReactiveFormsModule,
    MatButtonModule,
    MatCardModule,
    MatFormFieldModule,
    MatInputModule,
    MatProgressSpinnerModule,
    MatSnackBarModule
  ],
  templateUrl: './login.html',
  styleUrl: './login.scss',
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class Login {
  private readonly authService =
    inject(AuthService);

  private readonly router =
    inject(Router);

  private readonly route =
    inject(ActivatedRoute);

  private readonly snackBar =
    inject(MatSnackBar);

  protected readonly cargando =
    signal(false);

  protected readonly ocultarPassword =
    signal(true);

  protected readonly formulario =
    new FormGroup({
      email: new FormControl(
        '',
        {
          nonNullable: true,
          validators: [
            Validators.required,
            Validators.email
          ]
        }
      ),
      password: new FormControl(
        '',
        {
          nonNullable: true,
          validators: [
            Validators.required,
            Validators.minLength(8)
          ]
        }
      )
    });

  protected alternarPassword(): void {
    this.ocultarPassword.update(
      (oculto) => !oculto
    );
  }

  protected iniciarSesion(): void {
    if (
      this.formulario.invalid ||
      this.cargando()
    ) {
      this.formulario.markAllAsTouched();
      return;
    }

    this.cargando.set(true);

    this.authService
      .iniciarSesion(
        this.formulario.getRawValue()
      )
      .pipe(
        finalize(() =>
          this.cargando.set(false)
        )
      )
      .subscribe({
        next: (response) => {
          this.snackBar.open(
            `Bienvenida, ${response.nombreCompleto}`,
            'Cerrar',
            {
              duration: 3500
            }
          );

          const returnUrl =
            this.route.snapshot
              .queryParamMap
              .get('returnUrl');

          const rutaPorRol =
            response.rol === 'Asesor'
              ? '/asesor/solicitudes'
              : '/estudiante/portal';

          const destino =
            returnUrl?.startsWith('/')
              ? returnUrl
              : rutaPorRol;

          void this.router.navigateByUrl(
            destino
          );
        },
        error: (error: HttpErrorResponse) => {
          const mensaje =
            this.obtenerMensajeError(error);

          this.snackBar.open(
            mensaje,
            'Cerrar',
            {
              duration: 5000,
              panelClass: [
                'snackbar-error'
              ]
            }
          );
        }
      });
  }

  private obtenerMensajeError(
    error: HttpErrorResponse
  ): string {
    if (error.status === 0) {
      return 'No fue posible conectar con la API.';
    }

    if (
      typeof error.error?.detail ===
      'string'
    ) {
      return error.error.detail;
    }

    if (error.status === 401) {
      return 'El correo o la contraseña son incorrectos.';
    }

    return 'Ocurrió un error al iniciar sesión.';
  }
}