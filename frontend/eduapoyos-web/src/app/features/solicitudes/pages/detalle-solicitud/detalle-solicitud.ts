import {
  ChangeDetectionStrategy,
  Component,
  DestroyRef,
  OnInit,
  computed,
  inject,
  signal
} from '@angular/core';
import {
  CurrencyPipe,
  DatePipe
} from '@angular/common';
import {
  FormControl,
  FormGroup,
  ReactiveFormsModule,
  Validators
} from '@angular/forms';
import {
  ActivatedRoute,
  Router
} from '@angular/router';
import {
  HttpErrorResponse
} from '@angular/common/http';
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
  MatDividerModule
} from '@angular/material/divider';
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
  EstadoSolicitud,
  Solicitud,
  TipoApoyo
} from '../../models/solicitud.model';
import {
  SolicitudesService
} from '../../services/solicitudes.service';

interface OpcionEstado {
  valor: EstadoSolicitud;
  nombre: string;
}

@Component({
  selector: 'app-detalle-solicitud',
  imports: [
    CurrencyPipe,
    DatePipe,
    ReactiveFormsModule,
    MatButtonModule,
    MatCardModule,
    MatDividerModule,
    MatFormFieldModule,
    MatIconModule,
    MatInputModule,
    MatProgressSpinnerModule,
    MatSelectModule,
    MatSnackBarModule
  ],
  templateUrl: './detalle-solicitud.html',
  styleUrl: './detalle-solicitud.scss',
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class DetalleSolicitud implements OnInit {
  private readonly solicitudesService =
    inject(SolicitudesService);

  private readonly sesionService =
    inject(SesionService);

  private readonly route =
    inject(ActivatedRoute);

  private readonly router =
    inject(Router);

  private readonly snackBar =
    inject(MatSnackBar);

  private readonly destroyRef =
    inject(DestroyRef);

  readonly EstadoSolicitud = EstadoSolicitud;
  readonly TipoApoyo = TipoApoyo;

  readonly solicitud =
    signal<Solicitud | null>(null);

  readonly cargando =
    signal(true);

  readonly guardandoEstado =
    signal(false);

  readonly descargandoConstancia =
    signal(false);

  readonly mensajeError =
    signal<string | null>(null);

  readonly formularioEstado = new FormGroup({
    nuevoEstado:
      new FormControl<EstadoSolicitud | null>(
        null,
        {
          validators: [
            Validators.required
          ]
        }
      ),

    observacion:
      new FormControl(
        '',
        {
          nonNullable: true,
          validators: [
            Validators.required,
            Validators.minLength(5),
            Validators.maxLength(500)
          ]
        }
      )
  });

  readonly esAsesor = computed(
    () => this.sesionService.obtenerRol() === 'Asesor'
  );

  readonly opcionesEstado = computed<OpcionEstado[]>(
    () => {
      const estadoActual =
        this.solicitud()?.estado;

      if (
        estadoActual === EstadoSolicitud.Pendiente
      ) {
        return [
          {
            valor: EstadoSolicitud.EnRevision,
            nombre: 'En revisión'
          }
        ];
      }

      if (
        estadoActual === EstadoSolicitud.EnRevision
      ) {
        return [
          {
            valor: EstadoSolicitud.Aprobada,
            nombre: 'Aprobar solicitud'
          },
          {
            valor: EstadoSolicitud.Rechazada,
            nombre: 'Rechazar solicitud'
          }
        ];
      }

      return [];
    }
  );

  readonly puedeCambiarEstado = computed(
    () =>
      this.esAsesor() &&
      this.opcionesEstado().length > 0
  );

  readonly puedeDescargarConstancia = computed(
    () =>
      this.solicitud()?.estado ===
      EstadoSolicitud.Aprobada
  );

  ngOnInit(): void {
    const id =
      this.route.snapshot.paramMap.get('id');

    if (!id) {
      this.mensajeError.set(
        'No se proporcionó el identificador de la solicitud.'
      );

      this.cargando.set(false);
      return;
    }

    this.cargarSolicitud(id);
  }

  cargarSolicitud(id?: string): void {
    const solicitudId =
      id ??
      this.route.snapshot.paramMap.get('id');

    if (!solicitudId) {
      return;
    }

    this.cargando.set(true);
    this.mensajeError.set(null);

    this.solicitudesService
      .obtenerPorId(solicitudId)
      .pipe(
        takeUntilDestroyed(this.destroyRef),
        finalize(() => {
          this.cargando.set(false);
        })
      )
      .subscribe({
        next: (solicitud) => {
          this.solicitud.set(solicitud);
          this.formularioEstado.reset({
            nuevoEstado: null,
            observacion: ''
          });
        },
        error: (error: HttpErrorResponse) => {
          this.solicitud.set(null);

          this.mensajeError.set(
            this.obtenerMensajeError(
              error,
              'No fue posible consultar la solicitud.'
            )
          );
        }
      });
  }

  cambiarEstado(): void {
    const solicitudActual =
      this.solicitud();

    if (
      !solicitudActual ||
      this.formularioEstado.invalid ||
      this.guardandoEstado()
    ) {
      this.formularioEstado.markAllAsTouched();
      return;
    }

    const nuevoEstado =
      this.formularioEstado.controls
        .nuevoEstado.value;

    if (nuevoEstado === null) {
      return;
    }

    const observacion =
      this.formularioEstado.controls
        .observacion.value.trim();

    this.guardandoEstado.set(true);

    this.solicitudesService
      .cambiarEstado(
        solicitudActual.id,
        {
          nuevoEstado,
          observacion
        }
      )
      .pipe(
        takeUntilDestroyed(this.destroyRef),
        finalize(() => {
          this.guardandoEstado.set(false);
        })
      )
      .subscribe({
        next: (solicitudActualizada) => {
          this.solicitud.set(
            solicitudActualizada
          );

          this.formularioEstado.reset({
            nuevoEstado: null,
            observacion: ''
          });

          this.snackBar.open(
            'El estado de la solicitud fue actualizado.',
            'Cerrar',
            {
              duration: 4000,
              panelClass: [
                'snackbar-success'
              ]
            }
          );
        },
        error: (error: HttpErrorResponse) => {
          this.snackBar.open(
            this.obtenerMensajeError(
              error,
              'No fue posible cambiar el estado.'
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

  descargarConstancia(): void {
    const solicitudActual =
      this.solicitud();

    if (
      !solicitudActual ||
      this.descargandoConstancia()
    ) {
      return;
    }

    this.descargandoConstancia.set(true);

    this.solicitudesService
      .descargarConstancia(
        solicitudActual.id
      )
      .pipe(
        takeUntilDestroyed(this.destroyRef),
        finalize(() => {
          this.descargandoConstancia.set(false);
        })
      )
      .subscribe({
        next: (archivo) => {
          const url =
            URL.createObjectURL(archivo);

          const enlace =
            document.createElement('a');

          enlace.href = url;
          enlace.download =
            `constancia-${solicitudActual.id}.txt`;

          enlace.click();

          URL.revokeObjectURL(url);

          this.snackBar.open(
            'La constancia fue descargada correctamente.',
            'Cerrar',
            {
              duration: 3500,
              panelClass: [
                'snackbar-success'
              ]
            }
          );
        },
        error: (error: HttpErrorResponse) => {
          this.snackBar.open(
            this.obtenerMensajeError(
              error,
              'No fue posible descargar la constancia.'
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

  volver(): void {
    const rol =
      this.sesionService.obtenerRol();

    if (rol === 'Asesor') {
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
      [TipoApoyo.Beca]: 'Beca',
      [TipoApoyo.Credito]: 'Crédito',
      [TipoApoyo.Subsidio]: 'Subsidio'
    };

    return nombres[tipoApoyo] ?? 'Desconocido';
  }

  obtenerNombreEstado(
    estado: EstadoSolicitud
  ): string {
    const nombres: Record<EstadoSolicitud, string> = {
      [EstadoSolicitud.Pendiente]: 'Pendiente',
      [EstadoSolicitud.EnRevision]: 'En revisión',
      [EstadoSolicitud.Aprobada]: 'Aprobada',
      [EstadoSolicitud.Rechazada]: 'Rechazada'
    };

    return nombres[estado] ?? 'Desconocido';
  }

  obtenerClaseEstado(
    estado: EstadoSolicitud
  ): string {
    const clases: Record<EstadoSolicitud, string> = {
      [EstadoSolicitud.Pendiente]:
        'estado-pendiente',

      [EstadoSolicitud.EnRevision]:
        'estado-revision',

      [EstadoSolicitud.Aprobada]:
        'estado-aprobada',

      [EstadoSolicitud.Rechazada]:
        'estado-rechazada'
    };

    return clases[estado] ?? '';
  }

  obtenerIconoEstado(
    estado: EstadoSolicitud
  ): string {
    const iconos: Record<EstadoSolicitud, string> = {
      [EstadoSolicitud.Pendiente]:
        'schedule',

      [EstadoSolicitud.EnRevision]:
        'manage_search',

      [EstadoSolicitud.Aprobada]:
        'check_circle',

      [EstadoSolicitud.Rechazada]:
        'cancel'
    };

    return iconos[estado] ?? 'help';
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

    if (error.status === 0) {
      return 'No fue posible conectar con la API.';
    }

    return mensajePredeterminado;
  }
}