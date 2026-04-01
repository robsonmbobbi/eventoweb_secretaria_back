using EventoWeb.Comum.Negocio.Entidades;
using EventoWeb.Comum.Negocio.Repositorios;
using EventoWeb.Secretaria.Relatorios.Aplicacao.Interfaces;

namespace EventoWeb.Secretaria.Relatorios.Aplicacao.Servicos;

/// <summary>
/// Aplicação responsável por gerar etiquetas de crachá para participantes aceitos.
/// </summary>
public class AppEtiquetasCracha
{
    private readonly IInscricoes _inscricoes;
    private readonly IGeradorEtiqueta<IEnumerable<Inscricao>> _geradorEtiqueta;

    public AppEtiquetasCracha(
        IInscricoes inscricoes,
        IGeradorEtiqueta<IEnumerable<Inscricao>> geradorEtiqueta)
    {
        _inscricoes = inscricoes ?? throw new ArgumentNullException(nameof(inscricoes));
        _geradorEtiqueta = geradorEtiqueta ?? throw new ArgumentNullException(nameof(geradorEtiqueta));
    }

    /// <summary>
    /// Gera etiquetas de crachá para todos os participantes aceitos no evento.
    /// </summary>
    /// <param name="idEvento">ID do evento.</param>
    /// <returns>Array de bytes contendo o PDF com as etiquetas.</returns>
    /// <exception cref="ArgumentException">Lançada quando não há inscrições aceitas do tipo participante.</exception>
    public byte[] GerarEtiquetasCracha(int idEvento)
    {
        var inscritos = _inscricoes.ListarPorSituacao(idEvento, EnumSituacaoInscricao.Aceita);

        var participantes = inscritos.OfType<InscricaoParticipante>().ToList();

        if (!participantes.Any())
        {
            throw new ArgumentException($"Nenhuma inscrição de participante aceita encontrada para o evento {idEvento}.");
        }

        return _geradorEtiqueta.GerarPdf(participantes);
    }
}
