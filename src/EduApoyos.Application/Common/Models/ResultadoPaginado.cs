namespace EduApoyos.Application.Common.Models;

public class ResultadoPaginado<T>
{
    public ResultadoPaginado(
        IReadOnlyCollection<T> elementos,
        int pagina,
        int tamanoPagina,
        int totalElementos)
    {
        Elementos = elementos;
        Pagina = pagina;
        TamanoPagina = tamanoPagina;
        TotalElementos = totalElementos;
    }

    public IReadOnlyCollection<T> Elementos { get; }

    public int Pagina { get; }

    public int TamanoPagina { get; }

    public int TotalElementos { get; }

    public int TotalPaginas =>
        TotalElementos == 0
            ? 0
            : (int)Math.Ceiling(
                TotalElementos / (double)TamanoPagina);
}