using EventoWeb.Secretaria.Negocio.Entidades.Quartos;
using EventoWeb.Secretaria.Negocio.Repositorios;
using EventoWeb.Secretaria.Relatorios.Aplicacao.Interfaces;

namespace EventoWeb.Secretaria.Relatorios.Aplicacao.Servicos;

/// <summary>
/// Aplicação responsável por gerar o relatório de participantes agrupados por quarto.
/// </summary>
public class AppRelatorioQuartos
{
    private readonly IQuartos _repositorioQuartos;
    private readonly IRelatorioGerador<IList<Quarto>> _relatorioGerador;

    public AppRelatorioQuartos(
        IQuartos repositorioQuartos,
        IRelatorioGerador<IList<Quarto>> relatorioGerador)
    {
        _repositorioQuartos = repositorioQuartos ?? throw new ArgumentNullException(nameof(repositorioQuartos));
        _relatorioGerador = relatorioGerador ?? throw new ArgumentNullException(nameof(relatorioGerador));
    }

    /// <summary>
    /// Gera relatório em PDF com a listagem de participantes agrupados por quarto.
    /// </summary>
    /// <param name="idEvento">ID do evento.</param>
    /// <param name="detalhar">Se true, inclui ID de inscrição, cidade e UF.</param>
    /// <returns>Array de bytes contendo o PDF.</returns>
    /// <exception cref="ArgumentException">Lançada quando não há quartos para o evento.</exception>
    public async Task<byte[]> GerarRelatorioAsync(int idEvento, bool detalhar)
    {
        var quartos = _repositorioQuartos.ListarTodosQuartosPorEvento(idEvento);

        if (quartos == null || !quartos.Any())
        {
            throw new ArgumentException($"Nenhum quarto encontrado para o evento {idEvento}.");
        }

        return await _relatorioGerador.GerarPdfAsync(quartos, detalhar);
    }
}
