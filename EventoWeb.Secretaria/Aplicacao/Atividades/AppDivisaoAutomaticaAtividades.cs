using EventoWeb.Comum.Aplicacao;
using EventoWeb.Comum.Negocio.Entidades;
using EventoWeb.Comum.Negocio.Repositorios;
using EventoWeb.Secretaria.Negocio.Repositorios;
using EventoWeb.Secretaria.Negocio.Servicos.Atividades;

namespace EventoWeb.Secretaria.Aplicacao.Atividades;

/// <summary>
/// Serviço de aplicação responsável pela divisão automática de atividades.
/// </summary>
public class AppDivisaoAutomaticaAtividades : AppBase
{
    private readonly IAtividades _repAtividades;
    private readonly IInscricoes _repInscricoes;
    private readonly IEventos _repEventos;

    /// <summary>
    /// Inicializa uma nova instância de <see cref="AppDivisaoAutomaticaAtividades"/>.
    /// </summary>
    /// <param name="contexto">Contexto da aplicação</param>
    /// <param name="repAtividades">Repositório de atividades</param>
    /// <param name="repInscricoes">Repositório de inscrições</param>
    /// <param name="repEventos">Repositório de eventos</param>
    public AppDivisaoAutomaticaAtividades(IContexto contexto, IAtividades repAtividades, IInscricoes repInscricoes, IEventos repEventos)
        : base(contexto)
    {
        _repAtividades = repAtividades ?? throw new ArgumentNullException(nameof(repAtividades));
        _repInscricoes = repInscricoes ?? throw new ArgumentNullException(nameof(repInscricoes));
        _repEventos = repEventos ?? throw new ArgumentNullException(nameof(repEventos));
    }

    /// <summary>
    /// Realiza a divisão automática de participantes em uma atividade.
    /// </summary>
    /// <param name="idEvento">ID do evento</param>
    /// <param name="idAtividade">ID da atividade a ser dividida</param>
    /// <returns>Lista de divisões com os participantes atribuídos</returns>
    public List<DTODivisaoAtividade> Dividir(int idEvento, int idAtividade)
    {
        var divisoesDto = new List<DTODivisaoAtividade>();

        ExecutarSeguramente(() =>
        {
            var evento = _repEventos.Obter(idEvento);
            if (evento == null)
                throw new InvalidOperationException($"Evento com ID {idEvento} não encontrado.");

            var atividade = _repAtividades.Obter(idAtividade);
            if (atividade == null)
                throw new InvalidOperationException($"Atividade com ID {idAtividade} não encontrada.");

            var divisao = new DivisaoAutomaticaAtividades(evento, _repInscricoes, _repAtividades);
            divisao.Dividir(atividade);

            divisoesDto = ConverterDivisoesParaDTO(atividade.Divisoes);
        });

        return divisoesDto;
    }

    private List<DTODivisaoAtividade> ConverterDivisoesParaDTO(IEnumerable<EventoWeb.Secretaria.Negocio.Entidades.Atividades.DivisaoAtividade> divisoes)
    {
        var resultado = new List<DTODivisaoAtividade>();

        foreach (var divisao in divisoes)
        {
            var dtoDivisao = new DTODivisaoAtividade
            {
                Id = divisao.Id,
                Nome = divisao.Nome,
                Participantes = divisao.Participantes
                    .Select(dp => new DTOParticipanteDivisao
                    {
                        Id = dp.Inscricao.Id,
                        Nome = dp.Inscricao.Pessoa.Nome.Nome,
                        DataNascimento = dp.Inscricao.Pessoa.DataNascimento!.Data,
                        Cidade = dp.Inscricao.Pessoa.Cidade,
                        UF = dp.Inscricao.Pessoa.UF,
                        EhCoordenador = dp.EhCoordenador
                    })
                    .ToList()
            };

            resultado.Add(dtoDivisao);
        }

        return resultado;
    }
}
