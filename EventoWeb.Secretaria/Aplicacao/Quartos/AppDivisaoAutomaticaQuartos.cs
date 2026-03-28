using EventoWeb.Comum.Aplicacao;
using EventoWeb.Comum.Negocio.Entidades;
using EventoWeb.Comum.Negocio.Repositorios;
using EventoWeb.Secretaria.Negocio.Repositorios;
using EventoWeb.Secretaria.Negocio.Servicos.Quartos;

namespace EventoWeb.Secretaria.Aplicacao.Quartos;

public class AppDivisaoAutomaticaQuartos : AppBase
{
    private readonly IQuartos _repQuartos;
    private readonly IInscricoes _repInscricoes;
    private readonly IEventos _repEventos;

    public AppDivisaoAutomaticaQuartos(IContexto contexto, IQuartos repQuartos, IInscricoes repInscricoes, IEventos repEventos) 
        : base(contexto)
    {
        _repQuartos = repQuartos ?? throw new ArgumentNullException(nameof(repQuartos));
        _repInscricoes = repInscricoes ?? throw new ArgumentNullException(nameof(repInscricoes));
        _repEventos = repEventos ?? throw new ArgumentNullException(nameof(repEventos));
    }

    public List<DTOQuarto> Dividir(int idEvento)
    {
        var quartosDto = new List<DTOQuarto>();

        ExecutarSeguramente(() =>
        {
            var evento = _repEventos.Obter(idEvento);
            if (evento == null)
                throw new InvalidOperationException($"Evento com ID {idEvento} não encontrado.");

            var divisao = new DivisaoAutomaticaInscricoesPorQuarto(evento, _repInscricoes, _repQuartos);
            var quartos = divisao.Dividir();            

            quartosDto = ConverterQuartosParaDTO(quartos);
        });

        return quartosDto;
    }

    private List<DTOQuarto> ConverterQuartosParaDTO(IList<EventoWeb.Secretaria.Negocio.Entidades.Quartos.Quarto> quartos)
    {
        var resultado = new List<DTOQuarto>();

        foreach (var quarto in quartos)
        {
            var dtoQuarto = new DTOQuarto
            {
                Id = quarto.Id,
                Nome = quarto.Nome,
                Capacidade = quarto.Capacidade,
                EhFamilia = quarto.EhFamilia,
                Sexo = quarto.Sexo,
                Inscritos = quarto.Inscritos
                    .Select(qi => new DTOInscritoQuarto
                    {
                        Id = qi.Inscricao.Id,
                        Nome = qi.Inscricao.Pessoa.Nome.Nome,
                        Cidade = qi.Inscricao.Pessoa.Cidade,
                        UF = qi.Inscricao.Pessoa.UF
                    })
                    .ToList()
            };

            resultado.Add(dtoQuarto);
        }

        return resultado;
    }
}
