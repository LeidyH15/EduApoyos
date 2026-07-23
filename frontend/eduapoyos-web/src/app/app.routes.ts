import {
  Routes
} from '@angular/router';
import {
  authGuard
} from './core/auth/guards/auth.guard';
import {
  roleGuard
} from './core/auth/guards/role.guard';

export const routes: Routes = [
  {
    path: '',
    pathMatch: 'full',
    redirectTo: 'login'
  },
  {
    path: 'login',
    title: 'Iniciar sesión | EduApoyos',
    loadComponent: () =>
      import(
        './features/auth/pages/login/login'
      ).then(
        (component) => component.Login
      )
  },
  {
    path: '',
    canActivate: [
      authGuard
    ],
    loadComponent: () =>
      import(
        './layout/app-shell/app-shell'
      ).then(
        (component) => component.AppShell
      ),
    children: [
      {
        path: 'asesor/solicitudes',
        title:
          'Panel del asesor | EduApoyos',
        canActivate: [
          roleGuard
        ],
        data: {
          roles: [
            'Asesor'
          ]
        },
        loadComponent: () =>
          import(
            './features/asesor/pages/panel-asesor/panel-asesor'
          ).then(
            (component) =>
              component.PanelAsesor
          )
      },
      {
        path: 'estudiante/portal',
        title:
          'Portal del estudiante | EduApoyos',
        canActivate: [
          roleGuard
        ],
        data: {
          roles: [
            'Estudiante'
          ]
        },
        loadComponent: () =>
          import(
            './features/estudiante/pages/portal-estudiante/portal-estudiante'
          ).then(
            (component) =>
              component.PortalEstudiante
          )
      },
      {
        path: 'solicitudes/nueva',
        title:
          'Nueva solicitud | EduApoyos',
        loadComponent: () =>
          import(
            './features/solicitudes/pages/formulario-solicitud/formulario-solicitud'
          ).then(
            (component) =>
              component.FormularioSolicitud
          )
      },
      {
        path: 'solicitudes/:id',
        title:
          'Detalle de solicitud | EduApoyos',
        loadComponent: () =>
          import(
            './features/solicitudes/pages/detalle-solicitud/detalle-solicitud'
          ).then(
            (component) =>
              component.DetalleSolicitud
          )
      }
    ]
  },
  {
    path: '**',
    title:
      'Página no encontrada | EduApoyos',
    loadComponent: () =>
      import(
        './shared/pages/not-found/not-found'
      ).then(
        (component) => component.NotFound
      )
  }
];