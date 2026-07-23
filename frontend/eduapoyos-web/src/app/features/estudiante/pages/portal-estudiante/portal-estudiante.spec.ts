import { signal } from '@angular/core';
import {
  ComponentFixture,
  TestBed
} from '@angular/core/testing';
import { Router } from '@angular/router';
import {
  of,
  throwError
} from 'rxjs';

import {
  AutenticacionResponse
} from '../../../../core/auth/models/autenticacion-response';
import {
  SesionService
} from '../../../../core/auth/services/sesion.service';
import {
  ResultadoPaginado
} from '../../../../core/models/resultado-paginado';
import {
  EstadoSolicitud,
  Solicitud,
  TipoApoyo
} from '../../../solicitudes/models/solicitud.model';
import {
  SolicitudesService
} from '../../../solicitudes/services/solicitudes.service';
import { PortalEstudiante } from './portal-estudiante';

describe('PortalEstudiante', () => {
  let component: PortalEstudiante;
  let fixture: ComponentFixture<PortalEstudiante>;

  const estudianteId =
    'b5fc2d07-d89d-4b0e-a19e-7d1127bc8e5d';

  const usuarioId =
    'b7e31e0a-d351-4872-8627-c4e1a668915b';

  const usuario: AutenticacionResponse = {
    usuarioId,
    estudianteId,
    nombreCompleto:
      'Laura Marcela Gómez Rodríguez',
    email: 'estudiante@eduapoyos.local',
    rol: 'Estudiante',
    token: 'token-pruebas',
    expiracion: '2099-12-31T23:59:59Z'
  };

  const solicitudes: Solicitud[] = [
    {
      id:
        '6a96bb23-405f-4ee9-840c-5db9b3b49b25',
      estudianteId,
      usuarioEstudianteId: usuarioId,
      nombreEstudiante:
        'Laura Marcela Gómez Rodríguez',
      numeroDocumento: '1020304050',
      tipoApoyo: TipoApoyo.Beca,
      montoSolicitado: 2800000,
      descripcion: 'Solicitud de beca.',
      estado: EstadoSolicitud.Pendiente,
      fechaSolicitud:
        '2026-07-21T22:29:00Z',
      fechaActualizacion:
        '2026-07-21T22:29:00Z',
      asesorId: null,
      historial: []
    },
    {
      id:
        '03fe17ea-cb66-4cca-a156-41c7435e9bea',
      estudianteId,
      usuarioEstudianteId: usuarioId,
      nombreEstudiante:
        'Laura Marcela Gómez Rodríguez',
      numeroDocumento: '1020304050',
      tipoApoyo: TipoApoyo.Credito,
      montoSolicitado: 2000000,
      descripcion: 'Solicitud de crédito.',
      estado: EstadoSolicitud.EnRevision,
      fechaSolicitud:
        '2026-07-22T10:00:00Z',
      fechaActualizacion:
        '2026-07-22T12:00:00Z',
      asesorId:
        '0232eef2-d85e-45df-a87c-e08e51647d4a',
      historial: []
    },
    {
      id:
        '95e46c14-83d6-44aa-aa9b-14538611020a',
      estudianteId,
      usuarioEstudianteId: usuarioId,
      nombreEstudiante:
        'Laura Marcela Gómez Rodríguez',
      numeroDocumento: '1020304050',
      tipoApoyo: TipoApoyo.Subsidio,
      montoSolicitado: 750000,
      descripcion: 'Solicitud de subsidio.',
      estado: EstadoSolicitud.Aprobada,
      fechaSolicitud:
        '2026-07-20T08:00:00Z',
      fechaActualizacion:
        '2026-07-22T14:00:00Z',
      asesorId:
        '0232eef2-d85e-45df-a87c-e08e51647d4a',
      historial: []
    }
  ];

  const resultado:
    ResultadoPaginado<Solicitud> = {
      elementos: solicitudes,
      pagina: 1,
      tamanoPagina: 10,
      totalElementos: 3,
      totalPaginas: 1
    };

  const usuarioSignal =
    signal<AutenticacionResponse | null>(
      usuario
    );

  const sesionServiceMock = {
    usuarioActual:
      usuarioSignal.asReadonly(),

    obtenerEstudianteId: vi
      .fn()
      .mockReturnValue(estudianteId)
  };

  const solicitudesServiceMock = {
    listarPorEstudiante: vi
      .fn()
      .mockReturnValue(of(resultado))
  };

  const routerMock = {
    navigate: vi
      .fn()
      .mockResolvedValue(true)
  };

  beforeEach(async () => {
    vi.clearAllMocks();

    sesionServiceMock
      .obtenerEstudianteId
      .mockReturnValue(estudianteId);

    solicitudesServiceMock
      .listarPorEstudiante
      .mockReturnValue(of(resultado));

    await TestBed.configureTestingModule({
      imports: [PortalEstudiante],
      providers: [
        {
          provide: SesionService,
          useValue: sesionServiceMock
        },
        {
          provide: SolicitudesService,
          useValue: solicitudesServiceMock
        },
        {
          provide: Router,
          useValue: routerMock
        }
      ]
    }).compileComponents();

    fixture =
      TestBed.createComponent(
        PortalEstudiante
      );

    component = fixture.componentInstance;

    fixture.detectChanges();

    await fixture.whenStable();
  });

  it('debe crear el componente', () => {
    expect(component).toBeTruthy();
  });

  it(
    'debe cargar las solicitudes del estudiante autenticado',
    () => {
      expect(
        sesionServiceMock
          .obtenerEstudianteId
      ).toHaveBeenCalled();

      expect(
        solicitudesServiceMock
          .listarPorEstudiante
      ).toHaveBeenCalledWith(
        estudianteId,
        1,
        10
      );

      expect(
        component.solicitudes()
      ).toEqual(solicitudes);

      expect(
        component.totalElementos()
      ).toBe(3);

      expect(
        component.cargando()
      ).toBe(false);

      expect(
        component.mensajeError()
      ).toBeNull();
    }
  );

  it(
    'debe calcular las solicitudes por estado',
    () => {
      expect(
        component.solicitudesPendientes()
      ).toBe(1);

      expect(
        component.solicitudesEnRevision()
      ).toBe(1);

      expect(
        component.solicitudesFinalizadas()
      ).toBe(1);

      expect(
        component.haySolicitudes()
      ).toBe(true);
    }
  );

  it(
    'debe navegar al detalle de una solicitud',
    () => {
      const solicitudId =
        solicitudes[0].id;

      component.verDetalle(solicitudId);

      expect(
        routerMock.navigate
      ).toHaveBeenCalledWith([
        '/solicitudes',
        solicitudId
      ]);
    }
  );

  it(
    'debe navegar al formulario de nueva solicitud',
    () => {
      component.crearSolicitud();

      expect(
        routerMock.navigate
      ).toHaveBeenCalledWith([
        '/solicitudes/nueva'
      ]);
    }
  );

  it(
    'debe actualizar la página y recargar las solicitudes',
    () => {
      component.cambiarPagina({
        pageIndex: 1,
        previousPageIndex: 0,
        pageSize: 5,
        length: 15
      });

      expect(
        component.pagina()
      ).toBe(2);

      expect(
        component.tamanoPagina()
      ).toBe(5);

      expect(
        solicitudesServiceMock
          .listarPorEstudiante
      ).toHaveBeenLastCalledWith(
        estudianteId,
        2,
        5
      );
    }
  );

  it(
    'debe mostrar un error cuando falla la consulta',
    () => {
      solicitudesServiceMock
        .listarPorEstudiante
        .mockReturnValue(
          throwError(
            () =>
              new Error(
                'Error de prueba'
              )
          )
        );

      component.cargarSolicitudes();

      expect(
        component.solicitudes()
      ).toEqual([]);

      expect(
        component.totalElementos()
      ).toBe(0);

      expect(
        component.cargando()
      ).toBe(false);

      expect(
        component.mensajeError()
      ).not.toBeNull();
    }
  );

  it(
    'debe impedir la consulta sin identificador de estudiante',
    () => {
      sesionServiceMock
        .obtenerEstudianteId
        .mockReturnValue(null);

      solicitudesServiceMock
        .listarPorEstudiante
        .mockClear();

      component.cargarSolicitudes();

      expect(
        solicitudesServiceMock
          .listarPorEstudiante
      ).not.toHaveBeenCalled();

      expect(
        component.solicitudes()
      ).toEqual([]);

      expect(
        component.totalElementos()
      ).toBe(0);

      expect(
        component.cargando()
      ).toBe(false);

      expect(
        component.mensajeError()
      ).not.toBeNull();
    }
  );
});