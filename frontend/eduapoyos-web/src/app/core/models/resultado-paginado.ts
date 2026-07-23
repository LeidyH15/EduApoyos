export interface ResultadoPaginado<T> {
  elementos: T[];
  pagina: number;
  tamanoPagina: number;
  totalElementos: number;
  totalPaginas: number;
}