import {
  ChangeDetectionStrategy,
  Component,
  DestroyRef,
  OnInit,
  inject,
  signal
} from '@angular/core';
import {
  HttpErrorResponse
} from '@angular/common/http';
import {
  FormControl,
  FormGroup,
  ReactiveFormsModule,
  Validators
} from '@angular/forms';
import {
  Router
} from '@angular/router';
import {
  takeUntilDestroyed
} from '@angular/core/rxjs-interop';
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
  MatIconModule
} from '@angular/material/icon';
import {
  MatInputModule
} from '@angular/material/input';
import {
  MatProgressSpinnerModule
} from '@angular/material/progress-spinner';
import {
  MatSelectModule
} from '@angular/material/select';
import {
  MatSnackBar,
  MatSnackBarModule
} from '@angular/material/snack-bar';

import {
  SesionService
} from '../../../../core/auth/services/sesion.service';
import {
  Estudiante
} from '../../../estudiante/models/estudiante.model';
import {
  EstudiantesService
} from '../../../estudiante/services/estudiantes.service';
import {
  CrearSolicitudRequest,
  TipoApoyo
} from '../../models/solicitud.model';
import {
  SolicitudesService
} from '../../services/solicitudes.service';

@Component({
  selector: 'app-formulario-solicitud',
  imports: [
    ReactiveFormsModule,
    MatButtonModule,
    MatCardModule,
    MatFormFieldModule,
    MatIconModule,
    MatInputModule,
    MatProgressSpinnerModule,
    MatSelectModule,
    MatSnackBarModule
  ],
  templateUrl: './formulario-solicitud.html',
  styleUrl: './formulario-solicitud.scss',
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class FormularioSolicitud implements OnInit {
  private readonly solicitudesService =
    inject(SolicitudesService);

  private readonly estudiantesService =
    inject(EstudiantesService);

  private readonly sesionService =
    inject(SesionService);

  private readonly router =
    inject(Router);

  private readonly snackBar =
    inject(MatSnackBar);

  private readonly destroyRef =
    inject(DestroyRef);

  readonly TipoApoyo = TipoApoyo;

  readonly esAsesor =
    this.sesionService.obtenerRol() === 'Asesor';

  readonly estudiantes =
    signal<Estudiante[]>([]);

  readonly cargandoEstudiantes =
    signal(false);

  readonly guardando =
    signal(false);

  readonly mensajeErrorEstudiantes =
    signal<string | null>(null);

  readonly formulario = new FormGroup({
    estudianteId:
      new FormControl<string | null>(
        null,
        this.esAsesor
          ? [
              Validators.required
            ]
          : []
      ),

    tipoApoyo:
      new FormControl<TipoApoyo | null>(
        null,
        {
          validators: [
            Validators.required
          ]
        }
      ),

    montoSolicitado:
      new FormControl<number | null>(
        null,
        {
          validators: [
            Validators.required,
            Validators.min(1),
            Validators.max(999999999)
          ]
        }
      ),

    descripcion:
      new FormControl(
        '',
        {
          nonNullable: true,
          validators: [
            Validators.required,
            Validators.minLength(10),
            Validators.maxLength(1000)
          ]
        }
      )
  });

  ngOnInit(): void {
    if (this.esAsesor) {
      this.cargarEstudiantes();
    }
  }

  cargarEstudiantes(): void {
    this.cargandoEstudiantes.set(true);
    this.mensajeErrorEstudiantes.set(null);

    this.estudiantesService
      .listar(1, 100)
      .pipe(
        takeUntilDestroyed(this.destroyRef),
        finalize(() => {
          this.cargandoEstudiantes.set(false);
        })
      )
      .subscribe({
        next: (resultado) => {
          this.estudiantes.set(
            resultado.elementos
          );
        },
        error: (error: HttpErrorResponse) => {
          this.estudiantes.set([]);

          this.mensajeErrorEstudiantes.set(
            this.obtenerMensajeError(
              error,
              'No fue posible cargar los estudiantes.'
            )
          );
        }
      });
  }

  crearSolicitud(): void {
    if (
      this.formulario.invalid ||
      this.guardando()
    ) {
      this.formulario.markAllAsTouched();
      return;
    }

    const tipoApoyo =
      this.formulario.controls
        .tipoApoyo.value;

    const montoSolicitado =
      this.formulario.controls
        .montoSolicitado.value;

    if (
      tipoApoyo === null ||
      montoSolicitado === null
    ) {
      return;
    }

    const request: CrearSolicitudRequest = {
      tipoApoyo,
      montoSolicitado,
      descripcion:
        this.formulario.controls
          .descripcion.value.trim()
    };

    if (this.esAsesor) {
      const estudianteId =
        this.formulario.controls
          .estudianteId.value;

      if (!estudianteId) {
        this.formulario.controls
          .estudianteId.markAsTouched();

        return;
      }

      request.estudianteId =
        estudianteId;
    }

    this.guardando.set(true);

    this.solicitudesService
      .crear(request)
      .pipe(
        takeUntilDestroyed(this.destroyRef),
        finalize(() => {
          this.guardando.set(false);
        })
      )
      .subscribe({
        next: (solicitud) => {
          this.snackBar.open(
            'La solicitud fue creada correctamente.',
            'Cerrar',
            {
              duration: 3500,
              panelClass: [
                'snackbar-success'
              ]
            }
          );

          void this.router.navigate([
            '/solicitudes',
            solicitud.id
          ]);
        },
        error: (error: HttpErrorResponse) => {
          this.snackBar.open(
            this.obtenerMensajeError(
              error,
              'No fue posible crear la solicitud.'
            ),
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

  cancelar(): void {
    if (this.esAsesor) {
      void this.router.navigate([
        '/asesor/solicitudes'
      ]);

      return;
    }

    void this.router.navigate([
      '/estudiante/portal'
    ]);
  }

  obtenerNombreTipoApoyo(
    tipoApoyo: TipoApoyo
  ): string {
    const nombres: Record<TipoApoyo, string> = {
      [TipoApoyo.Beca]:
        'Beca',

      [TipoApoyo.Credito]:
        'Crédito educativo',

      [TipoApoyo.Subsidio]:
        'Subsidio'
    };

    return nombres[tipoApoyo];
  }

  obtenerDescripcionTipoApoyo(
    tipoApoyo: TipoApoyo
  ): string {
    const descripciones: Record<
      TipoApoyo,
      string
    > = {
      [TipoApoyo.Beca]:
        'Apoyo para cubrir matrícula y costos académicos.',

      [TipoApoyo.Credito]:
        'Financiación para continuar tus estudios.',

      [TipoApoyo.Subsidio]:
        'Auxilio económico para gastos educativos.'
    };

    return descripciones[tipoApoyo];
  }

  private obtenerMensajeError(
    error: HttpErrorResponse,
    mensajePredeterminado: string
  ): string {
    if (
      typeof error.error?.detail === 'string' &&
      error.error.detail.trim()
    ) {
      return error.error.detail;
    }

    if (
      error.error?.errors &&
      typeof error.error.errors === 'object'
    ) {
      const mensajes =
        Object.values(
          error.error.errors
        ).flat();

      if (
        mensajes.length > 0 &&
        typeof mensajes[0] === 'string'
      ) {
        return mensajes[0];
      }
    }

    if (error.status === 0) {
      return 'No fue posible conectar con la API.';
    }

    return mensajePredeterminado;
  }
}