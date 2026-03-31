using EventoWeb.Comum.Persistencia.Repositorios;
using EventoWeb.Secretaria.Negocio.Entidades.Quartos;
using EventoWeb.Secretaria.Negocio.Repositorios;
using NHibernate;
using NHibernate.Transform;

namespace EventoWeb.Secretaria.Persistencia.Repositorios
{
    public class QuartosNH : PersistenciaNH<Quarto>, IQuartos
    {
        public QuartosNH(NHibernate.ISession sessao) : base(sessao)
        {
        }

        public IList<Quarto> ListarTodosQuartosPorEvento(int idEvento)
        {
            return Sessao.QueryOver<Quarto>()
                .Where(quarto => quarto.Evento.Id == idEvento)
                .List();
        }

        public Quarto ObterQuartoPorIdEventoEQuarto(int idEvento, int idQuarto)
        {
            return Sessao.QueryOver<Quarto>()
                .Where(quarto => quarto.Evento.Id == idEvento && quarto.Id == idQuarto)
                .SingleOrDefault();
        }

        public Boolean HaOutroQuartoComCapacidadeInfinita(Quarto quarto)
        {
            return Sessao.QueryOver<Quarto>()
                .Where(x => x.Id != quarto.Id && x.Sexo == quarto.Sexo && x.EhFamilia == quarto.EhFamilia &&
                    x.Capacidade == null)
                .RowCount() > 0;
        }

        public IList<Quarto> ListarTodosQuartosPorEventoComParticipantes(int idEvento)
        {
            return Sessao.QueryOver<Quarto>()
                .Where(quarto => quarto.Evento.Id == idEvento)
                .Left.JoinQueryOver(x => x.Inscritos)
                .TransformUsing(Transformers.DistinctRootEntity)
                .List();
        }

        public Quarto? BuscarQuartoDoInscrito(int idEvento, int idInscricao)
        {
            var quartoInscrito = Sessao.QueryOver<QuartoInscrito>()
                .Where(x => x.Inscricao.Id == idInscricao)
                .JoinQueryOver(x => x.Quarto)
                .Where(x => x.Evento.Id == idEvento)
                .SingleOrDefault();

            if (quartoInscrito == null)
                return null;
            else
                return quartoInscrito.Quarto;
        }
    }
}
