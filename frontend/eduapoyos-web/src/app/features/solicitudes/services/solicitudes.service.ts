import {
  HttpClient,
  HttpParams
} from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';

import {
  environment
} from '../../../../environments/environment';
import {
  ResultadoPaginado
} from '../../../core/models/resultado-paginado';
import {
  CambiarEstadoSolicitudRequest,
  Solicitud,
  SolicitudFiltro
} from '../models/solicitud.model';

@Injectable({
  providedIn: 'root'
})
export class SolicitudesService {
  private readonly http = inject(HttpClient);

  listar(
    filtro: SolicitudFiltro
  ): Observable<ResultadoPaginado<Solicitud>> {
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

  obtenerPorId(
    id: string
  ): Observable<Solicitud> {
    return this.http.get<Solicitud>(
      `${environment.apiUrl}/solicitudes/${id}`
    );
  }

  cambiarEstado(
    id: string,
    request: CambiarEstadoSolicitudRequest
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