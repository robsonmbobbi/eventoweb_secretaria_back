using EventoWeb.Secretaria.Negocio.Entidades.Atividades;
using EventoWeb.Secretaria.Negocio.Repositorios;
using EventoWeb.Secretaria.Relatorios.Aplicacao.Interfaces;

namespace EventoWeb.Secretaria.Relatorios.Aplicacao.Servicos;

/// <summary>
/// Aplicação responsável por gerar o relatório de divisão de atividades.
/// </summary>
public class AppDivisaoAtividadeRelatorio
{
    private readonly IAtividades _repositorioAtividades;
    private readonly IRelatorioGerador<Atividade> _relatorioGerador;

    public AppDivisaoAtividadeRelatorio(
        IAtividades repositorioAtividades,
        IRelatorioGerador<Atividade> relatorioGerador)
    {
        _repositorioAtividades = repositorioAtividades ?? throw new ArgumentNullException(nameof(repositorioAtividades));
        _relatorioGerador = relatorioGerador ?? throw new ArgumentNullException(nameof(relatorioGerador));
    }

    /// <summary>
    /// Gera relatório em PDF com as divisões de uma atividade.
    /// </summary>
    /// <param name="idAtividade">ID da atividade.</param>
    /// <param name="detalhar">Se true, inclui ID de inscrição, cidade e UF.</param>
    /// <returns>Array de bytes contendo o PDF.</returns>
    /// <exception cref="ArgumentException">Lançada quando a atividade não é encontrada ou não possui divisões.</exception>
    public async Task<byte[]> GerarRelatorioAsync(int idAtividade, bool detalhar)
    {
        var atividade = _repositorioAtividades.Obter(idAtividade);

        if (atividade == null)
        {
            throw new ArgumentException($"Atividade com ID {idAtividade} não encontrada.");
        }

        if (!atividade.Divisoes.Any())
        {
            throw new ArgumentException($"A atividade com ID {idAtividade} não possui divisões cadastradas.");
        }

        return await _relatorioGerador.GerarPdfAsync(atividade, detalhar);
    }
}
