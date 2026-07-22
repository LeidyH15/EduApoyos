import { ComponentFixture, TestBed } from '@angular/core/testing';

import { FormularioSolicitud } from './formulario-solicitud';

describe('FormularioSolicitud', () => {
  let component: FormularioSolicitud;
  let fixture: ComponentFixture<FormularioSolicitud>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [FormularioSolicitud],
    }).compileComponents();

    fixture = TestBed.createComponent(FormularioSolicitud);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
