namespace EventoWeb.Secretaria.Relatorios.Aplicacao.Interfaces;

/// <summary>
/// Interface para geração de relatórios.
/// </summary>
/// <typeparam name="T">Tipo de entidade para o qual o relatório será gerado.</typeparam>
public interface IGeradorEtiqueta<T>
{
    /// <summary>
    /// Gera um relatório em PDF.
    /// </summary>
    /// <param name="dados">Dados para gerar o relatório.</param>
    /// <returns>Array de bytes contendo o PDF.</returns>
    byte[] GerarPdf(T dados);
}
