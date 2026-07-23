import {
  ChangeDetectionStrategy,
  Component,
  computed,
  inject
} from '@angular/core';
import {
  Router,
  RouterLink,
  RouterLinkActive,
  RouterOutlet
} from '@angular/router';
import {
  MatButtonModule
} from '@angular/material/button';
import {
  MatToolbarModule
} from '@angular/material/toolbar';
import {
  SesionService
} from '../../core/auth/services/sesion.service';

@Component({
  selector: 'app-shell',
  imports: [
    RouterOutlet,
    RouterLink,
    RouterLinkActive,
    MatButtonModule,
    MatToolbarModule
  ],
  templateUrl: './app-shell.html',
  styleUrl: './app-shell.scss',
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class AppShell {
  private readonly sesionService =
    inject(SesionService);

  private readonly router =
    inject(Router);

  protected readonly usuario =
    this.sesionService.usuarioActual;

  protected readonly rutaInicio =
    computed(() =>
      this.usuario()?.rol === 'Asesor'
        ? '/asesor/solicitudes'
        : '/estudiante/portal'
    );

  protected cerrarSesion(): void {
    this.sesionService.cerrarSesion();

    void this.router.navigate(
      ['/login']
    );
  }
}