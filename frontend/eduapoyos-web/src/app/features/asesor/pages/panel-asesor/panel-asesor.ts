import {
  ChangeDetectionStrategy,
  Component,
  DestroyRef,
  OnInit,
  computed,
  inject,
  signal
} from '@angular/core';
import { CurrencyPipe, DatePipe } from '@angular/common';
import { FormControl, ReactiveFormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';

import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatChipsModule } from '@angular/material/chips';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatIconModule } from '@angular/material/icon';
import {
  MatPaginatorModule,
  PageEvent
} from '@angular/material/paginator';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatSelectModule } from '@angular/material/select';
import { MatTableModule } from '@angular/material/table';

import {
  EstadoSolicitud,
  Solicitud,
  TipoApoyo
} from '../../../solicitudes/models/solicitud.model';
import {
  SolicitudesService
} from '../../../solicitudes/services/solicitudes.service';

@Component({
  selector: 'app-panel-asesor',
  imports: [
    CurrencyPipe,
    DatePipe,
    ReactiveFormsModule,
    MatButtonModule,
    MatCardModule,
    MatChipsModule,
    MatFormFieldModule,
    MatIconModule,
    MatPaginatorModule,
    MatProgressSpinnerModule,
    MatSelectModule,
    MatTableModule
  ],
  templateUrl: './panel-asesor.html',
  styleUrl: './panel-asesor.scss',
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class PanelAsesor implements OnInit {
  private readonly solicitudesService =
    inject(SolicitudesService);

  private readonly router = inject(Router);
  private readonly destroyRef = inject(DestroyRef);

  readonly EstadoSolicitud = EstadoSolicitud;
  readonly TipoApoyo = TipoApoyo;

  readonly filtroEstado =
    new FormControl<EstadoSolicitud | null>(null);

  readonly filtroTipoApoyo =
    new FormControl<TipoApoyo | null>(null);

  readonly solicitudes = signal<Solicitud[]>([]);
  readonly cargando = signal(false);
  readonly mensajeError = signal<string | null>(null);

  readonly pagina = signal(1);
  readonly tamanoPagina = signal(10);
  readonly totalElementos = signal(0);

  readonly columnas = [
    'estudiante',
    'tipoApoyo',
    'monto',
    'estado',
    'fechaSolicitud',
    'acciones'
  ];

  readonly haySolicitudes = computed(
    () => this.solicitudes().length > 0
  );

  ngOnInit(): void {
    this.cargarSolicitudes();
  }

  buscar(): void {
    this.pagina.set(1);
    this.cargarSolicitudes();
  }

  limpiarFiltros(): void {
    this.filtroEstado.setValue(null);
    this.filtroTipoApoyo.setValue(null);
    this.pagina.set(1);
    this.cargarSolicitudes();
  }

  cambiarPagina(evento: PageEvent): void {
    this.pagina.set(evento.pageIndex + 1);
    this.tamanoPagina.set(evento.pageSize);
    this.cargarSolicitudes();
  }

  verDetalle(id: string): void {
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

  cargarSolicitudes(): void {
    this.cargando.set(true);
    this.mensajeError.set(null);

    this.solicitudesService
      .listar({
        pagina: this.pagina(),
        tamanoPagina: this.tamanoPagina(),
        estado: this.filtroEstado.value,
        tipoApoyo: this.filtroTipoApoyo.value
      })
      .pipe(
        takeUntilDestroyed(this.destroyRef)
      )
      .subscribe({
        next: (resultado) => {
          this.solicitudes.set(resultado.elementos);
          this.totalElementos.set(
            resultado.totalElementos
          );
          this.cargando.set(false);
        },
        error: () => {
          this.solicitudes.set([]);
          this.totalElementos.set(0);
          this.mensajeError.set(
            'No fue posible cargar las solicitudes. Intenta nuevamente.'
          );
          this.cargando.set(false);
        }
      });
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
      [EstadoSolicitud.Pendiente]: 'estado-pendiente',
      [EstadoSolicitud.EnRevision]: 'estado-revision',
      [EstadoSolicitud.Aprobada]: 'estado-aprobada',
      [EstadoSolicitud.Rechazada]: 'estado-rechazada'
    };

    return clases[estado] ?? '';
  }
}