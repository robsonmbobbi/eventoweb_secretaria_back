namespace EventoWeb.Secretaria.Relatorios.Aplicacao.Interfaces;

/// <summary>
/// Interface para geração de relatórios.
/// </summary>
/// <typeparam name="T">Tipo de entidade para o qual o relatório será gerado.</typeparam>
public interface IRelatorioGerador<T>
{
    /// <summary>
    /// Gera um relatório em PDF.
    /// </summary>
    /// <param name="dados">Dados para gerar o relatório.</param>
    /// <param name="detalhar">Se true, inclui informações detalhadas (id inscrição, cidade/UF).</param>
    /// <returns>Array de bytes contendo o PDF.</returns>
    Task<byte[]> GerarPdfAsync(T dados, bool detalhar);
}
