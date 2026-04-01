using EventoWeb.Comum.Negocio.Entidades;
using EventoWeb.Comum.Negocio.Repositorios;
using EventoWeb.Secretaria.Negocio.Entidades.Atividades;
using EventoWeb.Secretaria.Negocio.Repositorios;
using EventoWeb.Secretaria.Relatorios.Aplicacao.Interfaces;
using NHibernate;

namespace EventoWeb.Secretaria.Relatorios.Aplicacao.Servicos;

/// <summary>
/// Aplicação responsável por gerar etiquetas de caderninho para participantes e infantis de 10 a 12 anos.
/// </summary>
public class AppEtiquetasCaderninho
{
    private readonly IInscricoes _inscricoes;
    private readonly IQuartos _quartos;
    private readonly IAtividades _atividades;
    private readonly IGeradorEtiqueta<IEnumerable<DadosCadernoInscrito>> _geradorEtiqueta;

    public AppEtiquetasCaderninho(
        IInscricoes inscricoes,
        IQuartos quartos,
        IAtividades atividades,
        IGeradorEtiqueta<IEnumerable<DadosCadernoInscrito>> geradorEtiqueta)
    {
        _inscricoes = inscricoes ?? throw new ArgumentNullException(nameof(inscricoes));
        _quartos = quartos ?? throw new ArgumentNullException(nameof(quartos));
        _atividades = atividades ?? throw new ArgumentNullException(nameof(atividades));
        _geradorEtiqueta = geradorEtiqueta ?? throw new ArgumentNullException(nameof(geradorEtiqueta));
    }

    /// <summary>
    /// Gera etiquetas de caderninho para participantes aceitos e infantis com idade entre 10 e 12 anos.
    /// </summary>
    /// <param name="idEvento">ID do evento.</param>
    /// <returns>Array de bytes contendo o PDF com as etiquetas.</returns>
    /// <exception cref="ArgumentException">Lançada quando não há inscrições que atendam aos critérios.</exception>
    public byte[] GerarEtiquetasCaderninho(int idEvento)
    {
        // 1. Obter inscrições aceitas
        var inscritos = _inscricoes.ListarPorSituacao(idEvento, EnumSituacaoInscricao.Aceita);

        // 2. Filtrar: InscricaoParticipante + InscricaoInfantil(10-12)
        var filtrados = inscritos.Where(i =>
        {
            if (i is InscricaoParticipante)
                return true;

            if (i is InscricaoInfantil infantil)
            {
                int idade = infantil.Pessoa.DataNascimento!
                    .CalcularIdadeEmAnos(infantil.Evento.PeriodoRealizacaoEvento.DataInicial);
                return idade >= 10 && idade <= 12;
            }

            return false;
        }).ToList();

        if (!filtrados.Any())
        {
            throw new ArgumentException($"Nenhuma inscrição que atenda aos critérios encontrada para o evento {idEvento}.");
        }

        // 3. Converter para DadosCadernoInscrito
        var dados = filtrados.Select(insc =>
        {
            var quarto = insc.DormeEvento
                ? _quartos.BuscarQuartoDoInscrito(idEvento, insc.Id)
                : null;

            var atividades = _atividades
                .ObterDivisaoParticipante(insc.Id)
                .Select(p => new DadosAtividadeInscrito
                {
                    NomeAtividade = p.Divisao.Atividade.Nome.Nome,
                    NomeDivisao = p.Divisao.Nome
                })
                .ToList();

            return new DadosCadernoInscrito
            {
                Id = insc.Id,
                Nome = insc.Pessoa.Nome.Nome,
                Cidade = insc.Pessoa.Cidade ?? string.Empty,
                UF = insc.Pessoa.UF ?? string.Empty,
                Quarto = quarto?.Nome,
                Atividades = atividades
            };
        }).ToList();

        // 4. Gerar PDF
        return _geradorEtiqueta.GerarPdf(dados);
    }
}
