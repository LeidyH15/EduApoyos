import {
  HttpClient,
  HttpParams
} from '@angular/common/http';

import {
  Injectable,
  inject
} from '@angular/core';

import {
  Observable
} from 'rxjs';

import {
  environment
} from '../../../../environments/environment';

import {
  ResultadoPaginado
} from '../../../core/models/resultado-paginado';

import {
  CambiarEstadoSolicitudRequest,
  CrearSolicitudRequest,
  Solicitud,
  SolicitudFiltro
} from '../models/solicitud.model';

@Injectable({
  providedIn: 'root'
})
export class SolicitudesService {
  private readonly http =
    inject(HttpClient);

  listar(
    filtro: SolicitudFiltro
  ): Observable<
    ResultadoPaginado<Solicitud>
  > {
    let params = new HttpParams()
      .set(
        'pagina',
        filtro.pagina.toString()
      )
      .set(
        'tamanoPagina',
        filtro.tamanoPagina.toString()
      );

    if (
      filtro.estado !== null &&
      filtro.estado !== undefined
    ) {
      params = params.set(
        'estado',
        filtro.estado.toString()
      );
    }

    if (
      filtro.tipoApoyo !== null &&
      filtro.tipoApoyo !== undefined
    ) {
      params = params.set(
        'tipoApoyo',
        filtro.tipoApoyo.toString()
      );
    }

    return this.http.get<
      ResultadoPaginado<Solicitud>
    >(
      `${environment.apiUrl}/solicitudes`,
      { params }
    );
  }

  listarPorEstudiante(
    estudianteId: string,
    pagina: number,
    tamanoPagina: number
  ): Observable<
    ResultadoPaginado<Solicitud>
  > {
    const params = new HttpParams()
      .set(
        'pagina',
        pagina.toString()
      )
      .set(
        'tamanoPagina',
        tamanoPagina.toString()
      );

    return this.http.get<
      ResultadoPaginado<Solicitud>
    >(
      `${environment.apiUrl}/estudiantes/${estudianteId}/solicitudes`,
      { params }
    );
  }

  obtenerPorId(
    id: string
  ): Observable<Solicitud> {
    return this.http.get<Solicitud>(
      `${environment.apiUrl}/solicitudes/${id}`
    );
  }

  crear(
    request: CrearSolicitudRequest
  ): Observable<Solicitud> {
    return this.http.post<Solicitud>(
      `${environment.apiUrl}/solicitudes`,
      request
    );
  }

  cambiarEstado(
    id: string,
    request:
      CambiarEstadoSolicitudRequest
  ): Observable<Solicitud> {
    return this.http.patch<Solicitud>(
      `${environment.apiUrl}/solicitudes/${id}/estado`,
      request
    );
  }

  descargarConstancia(
    id: string
  ): Observable<Blob> {
    return this.http.get(
      `${environment.apiUrl}/solicitudes/${id}/constancia`,
      {
        responseType: 'blob'
      }
    );
  }
}