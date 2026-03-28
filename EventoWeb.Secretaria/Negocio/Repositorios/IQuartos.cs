using EventoWeb.Comum.Negocio.Repositorios;
using EventoWeb.Secretaria.Negocio.Entidades.Quartos;

namespace EventoWeb.Secretaria.Negocio.Repositorios
{
    public interface IQuartos: IPersistencia<Quarto>
    {
        IList<Quarto> ListarTodosQuartosPorEvento(int idEvento);
        Quarto ObterQuartoPorIdEventoEQuarto(int idEvento, int idQuarto);
        IList<Quarto> ListarTodosQuartosPorEventoComParticipantes(int idEvento);
        Quarto? BuscarQuartoDoInscrito(int idEvento, int idInscricao);
        Boolean HaOutroQuartoComCapacidadeInfinita(Quarto quarto);
    }
}
