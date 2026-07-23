import {
  ComponentFixture,
  TestBed
} from '@angular/core/testing';
import {
  provideRouter
} from '@angular/router';
import {
  AppShell
} from './app-shell';

describe('AppShell', () => {
  let component: AppShell;
  let fixture: ComponentFixture<AppShell>;

  beforeEach(async () => {
    sessionStorage.clear();

    await TestBed.configureTestingModule({
      imports: [
        AppShell
      ],
      providers: [
        provideRouter([])
      ]
    }).compileComponents();

    fixture =
      TestBed.createComponent(AppShell);

    component =
      fixture.componentInstance;

    fixture.detectChanges();
  });

  afterEach(() => {
    sessionStorage.clear();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should render the application brand', () => {
    const compiled =
      fixture.nativeElement as HTMLElement;

    expect(
      compiled.textContent
    ).toContain('EduApoyos');
  });
});