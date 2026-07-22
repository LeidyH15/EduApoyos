import { ComponentFixture, TestBed } from '@angular/core/testing';

import { PortalEstudiante } from './portal-estudiante';

describe('PortalEstudiante', () => {
  let component: PortalEstudiante;
  let fixture: ComponentFixture<PortalEstudiante>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [PortalEstudiante],
    }).compileComponents();

    fixture = TestBed.createComponent(PortalEstudiante);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
