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
  Estudiante
} from '../models/estudiante.model';

@Injectable({
  providedIn: 'root'
})
export class EstudiantesService {
  private readonly http =
    inject(HttpClient);

  listar(
    pagina = 1,
    tamanoPagina = 100
  ): Observable<ResultadoPaginado<Estudiante>> {
    const params =
      new HttpParams()
        .set(
          'pagina',
          pagina.toString()
        )
        .set(
          'tamanoPagina',
          tamanoPagina.toString()
        );

    return this.http.get<
      ResultadoPaginado<Estudiante>
    >(
      `${environment.apiUrl}/estudiantes`,
      {
        params
      }
    );
  }
}