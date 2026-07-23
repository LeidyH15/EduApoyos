import {
  ComponentFixture,
  TestBed
} from '@angular/core/testing';
import {
  ActivatedRoute,
  Router
} from '@angular/router';
import {
  MatSnackBar
} from '@angular/material/snack-bar';
import { of } from 'rxjs';

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
import {
  DetalleSolicitud
} from './detalle-solicitud';

describe('DetalleSolicitud', () => {
  let component: DetalleSolicitud;
  let fixture: ComponentFixture<DetalleSolicitud>;

  const solicitudId =
    '584189b4-c6d3-4d95-bee1-4d03dd24cb9d';

  const solicitud: Solicitud = {
    id: solicitudId,
    estudianteId:
      'ab51f9b8-0d5a-446c-8f92-b8f1d9dfb736',
    usuarioEstudianteId:
      '231a7892-45fb-453b-a4cb-c5963d4f978',
    nombreEstudiante:
      'Laura Marcela Gómez Rodríguez',
    numeroDocumento: '1020304050',
    tipoApoyo: TipoApoyo.Beca,
    montoSolicitado: 2800000,
    descripcion:
      'Solicitud para cubrir matrícula y materiales.',
    estado: EstadoSolicitud.Aprobada,
    fechaSolicitud:
      '2026-07-21T22:29:03.835Z',
    fechaActualizacion:
      '2026-07-21T22:53:09.814Z',
    asesorId:
      'e79818ea-e418-4370-b794-b8f22a10d8d6',
    historial: [
      {
        id:
          '9658d126-c6d1-443c-83fa-809147628e20',
        estadoAnterior:
          EstadoSolicitud.EnRevision,
        estadoNuevo:
          EstadoSolicitud.Aprobada,
        fechaCambio:
          '2026-07-21T22:53:09.814Z',
        usuarioId:
          'e79818ea-e418-4370-b794-b8f22a10d8d6',
        observacion:
          'La solicitud cumple con los requisitos.'
      }
    ]
  };

  const solicitudesServiceMock = {
    obtenerPorId: () => of(solicitud),

    cambiarEstado: () => of(solicitud),

    descargarConstancia: () =>
      of(
        new Blob(
          ['Constancia de solicitud aprobada'],
          {
            type: 'text/plain'
          }
        )
      )
  };

  const sesionServiceMock = {
    obtenerRol: () => 'Asesor'
  };

  const routerMock = {
    navigate: () => Promise.resolve(true)
  };

  const activatedRouteMock = {
    snapshot: {
      paramMap: {
        get: (nombre: string) =>
          nombre === 'id'
            ? solicitudId
            : null
      }
    }
  };

  const snackBarMock = {
    open: () => undefined
  };

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [
        DetalleSolicitud
      ],
      providers: [
        {
          provide: SolicitudesService,
          useValue: solicitudesServiceMock
        },
        {
          provide: SesionService,
          useValue: sesionServiceMock
        },
        {
          provide: Router,
          useValue: routerMock
        },
        {
          provide: ActivatedRoute,
          useValue: activatedRouteMock
        },
        {
          provide: MatSnackBar,
          useValue: snackBarMock
        }
      ]
    }).compileComponents();

    fixture =
      TestBed.createComponent(
        DetalleSolicitud
      );

    component =
      fixture.componentInstance;

    fixture.detectChanges();
    await fixture.whenStable();
    fixture.detectChanges();
  });

  it('debe crear el detalle de la solicitud', () => {
    expect(component).toBeTruthy();
  });

  it('debe consultar la solicitud al iniciar', () => {
    expect(component.solicitud()).toEqual(
      solicitud
    );

    expect(component.cargando()).toBe(false);
    expect(component.mensajeError()).toBeNull();
  });

  it('debe mostrar la información de la solicitud', () => {
    const elemento =
      fixture.nativeElement as HTMLElement;

    expect(elemento.textContent).toContain(
      'Laura Marcela Gómez Rodríguez'
    );

    expect(elemento.textContent).toContain(
      'Aprobada'
    );

    expect(elemento.textContent).toContain(
      'Solicitud para cubrir matrícula y materiales.'
    );
  });

  it('debe permitir descargar constancia cuando está aprobada', () => {
    expect(
      component.puedeDescargarConstancia()
    ).toBe(true);
  });

  it('no debe permitir nuevos cambios en una solicitud aprobada', () => {
    expect(
      component.opcionesEstado()
    ).toEqual([]);

    expect(
      component.puedeCambiarEstado()
    ).toBe(false);
  });

  it('debe reconocer los nombres de tipo de apoyo', () => {
    expect(
      component.obtenerNombreTipoApoyo(
        TipoApoyo.Beca
      )
    ).toBe('Beca');

    expect(
      component.obtenerNombreTipoApoyo(
        TipoApoyo.Credito
      )
    ).toBe('Crédito');

    expect(
      component.obtenerNombreTipoApoyo(
        TipoApoyo.Subsidio
      )
    ).toBe('Subsidio');
  });
});