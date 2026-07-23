import { ComponentFixture, TestBed } from '@angular/core/testing';
import { provideRouter } from '@angular/router';
import { of } from 'rxjs';

import { PanelAsesor } from './panel-asesor';
import {
  EstadoSolicitud,
  TipoApoyo
} from '../../../solicitudes/models/solicitud.model';
import {
  SolicitudesService
} from '../../../solicitudes/services/solicitudes.service';

describe('PanelAsesor', () => {
  let component: PanelAsesor;
  let fixture: ComponentFixture<PanelAsesor>;

  const resultadoPaginado = {
    elementos: [
      {
        id: 'cfab917f-6344-4cea-a57f-f1e56dd6c223',
        estudianteId:
          '1ba51f9b-0d5a-446c-8f92-b8f1d9dfb736',
        usuarioEstudianteId:
          '231a7892-45fb-453b-a4cb-c5963d4f978',
        nombreEstudiante:
          'Laura Marcela Gómez Rodríguez',
        numeroDocumento: '1020304050',
        tipoApoyo: TipoApoyo.Beca,
        montoSolicitado: 2800000,
        descripcion:
          'Solicitud para cubrir los costos de matrícula.',
        estado: EstadoSolicitud.Pendiente,
        fechaSolicitud: '2026-07-21T22:29:03.835Z',
        fechaActualizacion:
          '2026-07-21T22:29:03.835Z',
        asesorId: null,
        historial: []
      }
    ],
    pagina: 1,
    tamanoPagina: 10,
    totalElementos: 1,
    totalPaginas: 1
  };

  const solicitudesServiceMock = {
    listar: () => of(resultadoPaginado)
  };

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [PanelAsesor],
      providers: [
        provideRouter([]),
        {
          provide: SolicitudesService,
          useValue: solicitudesServiceMock
        }
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(PanelAsesor);
    component = fixture.componentInstance;

    fixture.detectChanges();
    await fixture.whenStable();
    fixture.detectChanges();
  });

  it('debe crear el panel del asesor', () => {
    expect(component).toBeTruthy();
  });

  it('debe cargar las solicitudes al iniciar', () => {
    expect(component.solicitudes().length).toBe(1);
    expect(component.totalElementos()).toBe(1);
    expect(component.cargando()).toBe(false);
    expect(component.mensajeError()).toBeNull();
  });

  it('debe mostrar la solicitud en la tabla', () => {
    const elemento =
      fixture.nativeElement as HTMLElement;

    expect(elemento.textContent).toContain(
      'Laura Marcela Gómez Rodríguez'
    );

    expect(elemento.textContent).toContain(
      'Beca'
    );

    expect(elemento.textContent).toContain(
      'Pendiente'
    );
  });

  it('debe limpiar los filtros y volver a la primera página', () => {
    component.filtroEstado.setValue(
      EstadoSolicitud.Aprobada
    );

    component.filtroTipoApoyo.setValue(
      TipoApoyo.Subsidio
    );

    component.pagina.set(3);

    component.limpiarFiltros();

    expect(component.filtroEstado.value).toBeNull();
    expect(component.filtroTipoApoyo.value).toBeNull();
    expect(component.pagina()).toBe(1);
  });

  it('debe obtener los nombres de los estados', () => {
    expect(
      component.obtenerNombreEstado(
        EstadoSolicitud.Pendiente
      )
    ).toBe('Pendiente');

    expect(
      component.obtenerNombreEstado(
        EstadoSolicitud.EnRevision
      )
    ).toBe('En revisión');

    expect(
      component.obtenerNombreEstado(
        EstadoSolicitud.Aprobada
      )
    ).toBe('Aprobada');

    expect(
      component.obtenerNombreEstado(
        EstadoSolicitud.Rechazada
      )
    ).toBe('Rechazada');
  });
});