import {
  ComponentFixture,
  TestBed
} from '@angular/core/testing';
import {
  HttpErrorResponse
} from '@angular/common/http';
import { Router } from '@angular/router';
import {
  MatSnackBar
} from '@angular/material/snack-bar';
import {
  of,
  throwError
} from 'rxjs';

import {
  SesionService
} from '../../../../core/auth/services/sesion.service';
import {
  EstudiantesService
} from '../../../estudiante/services/estudiantes.service';
import {
  TipoApoyo
} from '../../models/solicitud.model';
import {
  SolicitudesService
} from '../../services/solicitudes.service';
import {
  FormularioSolicitud
} from './formulario-solicitud';

describe('FormularioSolicitud', () => {
  let component: FormularioSolicitud;
  let fixture: ComponentFixture<FormularioSolicitud>;
  let snackBar: MatSnackBar;

  const solicitudId =
    '4e1afe7f-ec92-47f2-a7aa-c03aafef8cdf';

  const solicitudesServiceMock = {
    crear: vi.fn()
  };

  const estudiantesServiceMock = {
    listar: vi.fn()
  };

  const sesionServiceMock = {
    obtenerRol: vi
      .fn()
      .mockReturnValue('Estudiante')
  };

  const routerMock = {
    navigate: vi
      .fn()
      .mockResolvedValue(true)
  };

  beforeEach(async () => {
    vi.clearAllMocks();

    sesionServiceMock
      .obtenerRol
      .mockReturnValue('Estudiante');

    estudiantesServiceMock
      .listar
      .mockReturnValue(
        of({
          elementos: [],
          pagina: 1,
          tamanoPagina: 100,
          totalElementos: 0,
          totalPaginas: 0
        })
      );

    await TestBed.configureTestingModule({
      imports: [
        FormularioSolicitud
      ],
      providers: [
        {
          provide: SolicitudesService,
          useValue: solicitudesServiceMock
        },
        {
          provide: EstudiantesService,
          useValue: estudiantesServiceMock
        },
        {
          provide: SesionService,
          useValue: sesionServiceMock
        },
        {
          provide: Router,
          useValue: routerMock
        }
      ]
    }).compileComponents();

    fixture =
      TestBed.createComponent(
        FormularioSolicitud
      );

    component = fixture.componentInstance;

    snackBar =
      fixture.debugElement.injector.get(
        MatSnackBar
      );

    fixture.detectChanges();

    await fixture.whenStable();
  });

  it('debe crear el componente', () => {
    expect(component).toBeTruthy();
  });

  it(
    'debe iniciar con el formulario inválido',
    () => {
      expect(
        component.formulario.invalid
      ).toBe(true);

      expect(
        component.formulario.controls
          .tipoApoyo.hasError('required')
      ).toBe(true);

      expect(
        component.formulario.controls
          .montoSolicitado.hasError('required')
      ).toBe(true);

      expect(
        component.formulario.controls
          .descripcion.hasError('required')
      ).toBe(true);
    }
  );

  it(
    'no debe enviar un formulario inválido',
    () => {
      component.crearSolicitud();

      expect(
        solicitudesServiceMock.crear
      ).not.toHaveBeenCalled();

      expect(
        component.formulario.controls
          .tipoApoyo.touched
      ).toBe(true);

      expect(
        component.formulario.controls
          .montoSolicitado.touched
      ).toBe(true);

      expect(
        component.formulario.controls
          .descripcion.touched
      ).toBe(true);
    }
  );

  it(
    'debe validar el monto solicitado',
    () => {
      const monto =
        component.formulario.controls
          .montoSolicitado;

      monto.setValue(0);

      expect(
        monto.hasError('min')
      ).toBe(true);

      monto.setValue(1000000000);

      expect(
        monto.hasError('max')
      ).toBe(true);

      monto.setValue(1500000);

      expect(monto.valid).toBe(true);
    }
  );

  it(
    'debe validar la longitud de la descripción',
    () => {
      const descripcion =
        component.formulario.controls
          .descripcion;

      descripcion.setValue('Corta');

      expect(
        descripcion.hasError('minlength')
      ).toBe(true);

      descripcion.setValue(
        'Solicitud para cubrir los costos de matrícula.'
      );

      expect(descripcion.valid).toBe(true);
    }
  );

  it(
    'debe crear una solicitud como estudiante',
    () => {
      const abrirNotificacion =
        vi.spyOn(snackBar, 'open')
          .mockImplementation(
            () => undefined as never
          );

      solicitudesServiceMock
        .crear
        .mockReturnValue(
          of({
            id: solicitudId
          })
        );

      component.formulario.patchValue({
        tipoApoyo: TipoApoyo.Beca,
        montoSolicitado: 2500000,
        descripcion:
          'Solicitud para cubrir los costos de matrícula.'
      });

      component.crearSolicitud();

      expect(
        solicitudesServiceMock.crear
      ).toHaveBeenCalledWith({
        tipoApoyo: TipoApoyo.Beca,
        montoSolicitado: 2500000,
        descripcion:
          'Solicitud para cubrir los costos de matrícula.'
      });

      expect(
        abrirNotificacion
      ).toHaveBeenCalledWith(
        'La solicitud fue creada correctamente.',
        'Cerrar',
        {
          duration: 3500,
          panelClass: [
            'snackbar-success'
          ]
        }
      );

      expect(
        routerMock.navigate
      ).toHaveBeenCalledWith([
        '/solicitudes',
        solicitudId
      ]);

      expect(
        component.guardando()
      ).toBe(false);
    }
  );

  it(
    'debe mostrar el detalle enviado por la API',
    () => {
      const abrirNotificacion =
        vi.spyOn(snackBar, 'open')
          .mockImplementation(
            () => undefined as never
          );

      const error =
        new HttpErrorResponse({
          status: 422,
          error: {
            detail:
              'El monto solicitado no cumple las reglas.'
          }
        });

      solicitudesServiceMock
        .crear
        .mockReturnValue(
          throwError(() => error)
        );

      component.formulario.patchValue({
        tipoApoyo: TipoApoyo.Subsidio,
        montoSolicitado: 750000,
        descripcion:
          'Solicitud para cubrir gastos educativos.'
      });

      component.crearSolicitud();

      expect(
        solicitudesServiceMock.crear
      ).toHaveBeenCalled();

      expect(
        abrirNotificacion
      ).toHaveBeenCalledWith(
        'El monto solicitado no cumple las reglas.',
        'Cerrar',
        {
          duration: 5000,
          panelClass: [
            'snackbar-error'
          ]
        }
      );

      expect(
        routerMock.navigate
      ).not.toHaveBeenCalled();

      expect(
        component.guardando()
      ).toBe(false);
    }
  );

  it(
    'debe regresar al portal al cancelar como estudiante',
    () => {
      component.cancelar();

      expect(
        routerMock.navigate
      ).toHaveBeenCalledWith([
        '/estudiante/portal'
      ]);
    }
  );
});

describe(
  'FormularioSolicitud como asesor',
  () => {
    let component: FormularioSolicitud;
    let fixture:
      ComponentFixture<FormularioSolicitud>;
    let snackBar: MatSnackBar;

    const estudianteId =
      'c2e62029-f60d-4ed8-9f2a-bda17632eb26';

    const solicitudId =
      'e421cb4a-c01c-4f90-b2b0-675d22da02a7';

    const estudiante = {
      id: estudianteId,
      usuarioId:
        '3a29c052-d333-4d2c-b485-3312b01bc4d3',
      nombreCompleto:
        'Estudiante de Pruebas',
      email:
        'estudiante.pruebas@eduapoyos.local',
      numeroDocumento:
        '1000000001',
      tipoDocumento: 1,
      programaAcademico:
        'Ingeniería de Sistemas',
      semestre: 5
    };

    const solicitudesServiceMockAsesor = {
      crear: vi.fn()
    };

    const estudiantesServiceMockAsesor = {
      listar: vi.fn()
    };

    const sesionServiceMockAsesor = {
      obtenerRol: vi
        .fn()
        .mockReturnValue('Asesor')
    };

    const routerMockAsesor = {
      navigate: vi
        .fn()
        .mockResolvedValue(true)
    };

    beforeEach(async () => {
      vi.clearAllMocks();

      estudiantesServiceMockAsesor
        .listar
        .mockReturnValue(
          of({
            elementos: [
              estudiante
            ],
            pagina: 1,
            tamanoPagina: 100,
            totalElementos: 1,
            totalPaginas: 1
          })
        );

      await TestBed.configureTestingModule({
        imports: [
          FormularioSolicitud
        ],
        providers: [
          {
            provide: SolicitudesService,
            useValue:
              solicitudesServiceMockAsesor
          },
          {
            provide: EstudiantesService,
            useValue:
              estudiantesServiceMockAsesor
          },
          {
            provide: SesionService,
            useValue:
              sesionServiceMockAsesor
          },
          {
            provide: Router,
            useValue:
              routerMockAsesor
          }
        ]
      }).compileComponents();

      fixture =
        TestBed.createComponent(
          FormularioSolicitud
        );

      component =
        fixture.componentInstance;

      snackBar =
        fixture.debugElement.injector.get(
          MatSnackBar
        );

      fixture.detectChanges();

      await fixture.whenStable();
    });

    it(
      'debe cargar estudiantes al iniciar como asesor',
      () => {
        expect(component.esAsesor).toBe(true);

        expect(
          estudiantesServiceMockAsesor.listar
        ).toHaveBeenCalledWith(
          1,
          100
        );

        expect(
          component.estudiantes()
        ).toEqual([
          estudiante
        ]);

        expect(
          component.cargandoEstudiantes()
        ).toBe(false);

        expect(
          component.mensajeErrorEstudiantes()
        ).toBeNull();
      }
    );

    it(
      'debe incluir el estudiante al crear la solicitud',
      () => {
        vi.spyOn(snackBar, 'open')
          .mockImplementation(
            () => undefined as never
          );

        solicitudesServiceMockAsesor
          .crear
          .mockReturnValue(
            of({
              id: solicitudId
            })
          );

        component.formulario.patchValue({
          estudianteId,
          tipoApoyo: TipoApoyo.Credito,
          montoSolicitado: 1800000,
          descripcion:
            'Solicitud creada por el asesor para el estudiante.'
        });

        component.crearSolicitud();

        expect(
          solicitudesServiceMockAsesor.crear
        ).toHaveBeenCalledWith({
          estudianteId,
          tipoApoyo: TipoApoyo.Credito,
          montoSolicitado: 1800000,
          descripcion:
            'Solicitud creada por el asesor para el estudiante.'
        });

        expect(
          routerMockAsesor.navigate
        ).toHaveBeenCalledWith([
          '/solicitudes',
          solicitudId
        ]);
      }
    );

    it(
      'debe regresar al panel del asesor al cancelar',
      () => {
        component.cancelar();

        expect(
          routerMockAsesor.navigate
        ).toHaveBeenCalledWith([
          '/asesor/solicitudes'
        ]);
      }
    );
  }
);