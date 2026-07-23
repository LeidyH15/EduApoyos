import {
  CurrencyPipe,
  DatePipe
} from '@angular/common';

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
  takeUntilDestroyed
} from '@angular/core/rxjs-interop';

import {
  Router
} from '@angular/router';

import {
  MatButtonModule
} from '@angular/material/button';

import {
  MatCardModule
} from '@angular/material/card';

import {
  MatChipsModule
} from '@angular/material/chips';

import {
  MatIconModule
} from '@angular/material/icon';

import {
  MatPaginatorModule,
  PageEvent
} from '@angular/material/paginator';

import {
  MatProgressSpinnerModule
} from '@angular/material/progress-spinner';

import {
  MatTableModule
} from '@angular/material/table';

import {
  SesionService
} from '../../../../core/auth/services/sesion.service';

import {
  EstadoSolicitud,
  Solicitud,
  TipoApoyo
} from '../../../solicitudes/models/solicitud.model';

import {
  SolicitudesService
} from '../../../solicitudes/services/solicitudes.service';

@Component({
  selector: 'app-portal-estudiante',
  imports: [
    CurrencyPipe,
    DatePipe,
    MatButtonModule,
    MatCardModule,
    MatChipsModule,
    MatIconModule,
    MatPaginatorModule,
    MatProgressSpinnerModule,
    MatTableModule
  ],
  templateUrl: './portal-estudiante.html',
  styleUrl: './portal-estudiante.scss',
  changeDetection:
    ChangeDetectionStrategy.OnPush
})
export class PortalEstudiante
  implements OnInit {
  private readonly solicitudesService =
    inject(SolicitudesService);

  private readonly sesionService =
    inject(SesionService);

  private readonly router =
    inject(Router);

  private readonly destroyRef =
    inject(DestroyRef);

  readonly EstadoSolicitud =
    EstadoSolicitud;

  readonly TipoApoyo =
    TipoApoyo;

  readonly solicitudes =
    signal<Solicitud[]>([]);

  readonly cargando =
    signal(false);

  readonly mensajeError =
    signal<string | null>(null);

  readonly pagina =
    signal(1);

  readonly tamanoPagina =
    signal(10);

  readonly totalElementos =
    signal(0);

  readonly columnas = [
    'tipoApoyo',
    'monto',
    'estado',
    'fechaSolicitud',
    'fechaActualizacion',
    'acciones'
  ];

  readonly usuarioActual =
    this.sesionService.usuarioActual;

  readonly haySolicitudes =
    computed(
      () =>
        this.solicitudes().length > 0
    );

  readonly solicitudesPendientes =
    computed(
      () =>
        this.solicitudes().filter(
          solicitud =>
            solicitud.estado ===
            EstadoSolicitud.Pendiente
        ).length
    );

  readonly solicitudesEnRevision =
    computed(
      () =>
        this.solicitudes().filter(
          solicitud =>
            solicitud.estado ===
            EstadoSolicitud.EnRevision
        ).length
    );

  readonly solicitudesFinalizadas =
    computed(
      () =>
        this.solicitudes().filter(
          solicitud =>
            solicitud.estado ===
              EstadoSolicitud.Aprobada ||
            solicitud.estado ===
              EstadoSolicitud.Rechazada
        ).length
    );

  ngOnInit(): void {
    this.cargarSolicitudes();
  }

  cargarSolicitudes(): void {
    const estudianteId =
      this.sesionService
        .obtenerEstudianteId();

    if (!estudianteId) {
      this.solicitudes.set([]);
      this.totalElementos.set(0);
      this.mensajeError.set(
        'No fue posible identificar el perfil del estudiante. Cierra la sesión e ingresa nuevamente.'
      );

      return;
    }

    this.cargando.set(true);
    this.mensajeError.set(null);

    this.solicitudesService
      .listarPorEstudiante(
        estudianteId,
        this.pagina(),
        this.tamanoPagina()
      )
      .pipe(
        takeUntilDestroyed(
          this.destroyRef
        )
      )
      .subscribe({
        next: resultado => {
          this.solicitudes.set(
            resultado.elementos
          );

          this.totalElementos.set(
            resultado.totalElementos
          );

          this.cargando.set(false);
        },
        error: () => {
          this.solicitudes.set([]);
          this.totalElementos.set(0);

          this.mensajeError.set(
            'No fue posible cargar tus solicitudes. Intenta nuevamente.'
          );

          this.cargando.set(false);
        }
      });
  }

  cambiarPagina(
    evento: PageEvent
  ): void {
    this.pagina.set(
      evento.pageIndex + 1
    );

    this.tamanoPagina.set(
      evento.pageSize
    );

    this.cargarSolicitudes();
  }

  verDetalle(
    id: string
  ): void {
    void this.router.navigate([
      '/solicitudes',
      id
    ]);
  }

  crearSolicitud(): void {
    void this.router.navigate([
      '/solicitudes/nueva'
    ]);
  }

  obtenerNombreTipoApoyo(
    tipoApoyo: TipoApoyo
  ): string {
    const nombres:
      Record<TipoApoyo, string> = {
        [TipoApoyo.Beca]:
          'Beca',

        [TipoApoyo.Credito]:
          'Crédito',

        [TipoApoyo.Subsidio]:
          'Subsidio'
      };

    return (
      nombres[tipoApoyo] ??
      'Desconocido'
    );
  }

  obtenerIconoTipoApoyo(
    tipoApoyo: TipoApoyo
  ): string {
    const iconos:
      Record<TipoApoyo, string> = {
        [TipoApoyo.Beca]:
          'school',

        [TipoApoyo.Credito]:
          'account_balance',

        [TipoApoyo.Subsidio]:
          'volunteer_activism'
      };

    return (
      iconos[tipoApoyo] ??
      'request_quote'
    );
  }

  obtenerNombreEstado(
    estado: EstadoSolicitud
  ): string {
    const nombres:
      Record<EstadoSolicitud, string> = {
        [EstadoSolicitud.Pendiente]:
          'Pendiente',

        [EstadoSolicitud.EnRevision]:
          'En revisión',

        [EstadoSolicitud.Aprobada]:
          'Aprobada',

        [EstadoSolicitud.Rechazada]:
          'Rechazada'
      };

    return (
      nombres[estado] ??
      'Desconocido'
    );
  }

  obtenerClaseEstado(
    estado: EstadoSolicitud
  ): string {
    const clases:
      Record<EstadoSolicitud, string> = {
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
    const iconos:
      Record<EstadoSolicitud, string> = {
        [EstadoSolicitud.Pendiente]:
          'schedule',

        [EstadoSolicitud.EnRevision]:
          'manage_search',

        [EstadoSolicitud.Aprobada]:
          'check_circle',

        [EstadoSolicitud.Rechazada]:
          'cancel'
      };

    return (
      iconos[estado] ??
      'help'
    );
  }
}