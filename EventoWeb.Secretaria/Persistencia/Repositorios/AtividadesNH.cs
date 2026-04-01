using EventoWeb.Comum.Persistencia.Repositorios;
using EventoWeb.Secretaria.Negocio.Entidades.Atividades;
using EventoWeb.Secretaria.Negocio.Repositorios;
using NHibernate;

namespace EventoWeb.Secretaria.Persistencia.Repositorios;

public class AtividadesNH : PersistenciaNH<Atividade>, IAtividades
{
    public AtividadesNH(ISession sessao) : base(sessao)
    {
    }

    public IList<DivisaoAtividadeParticipante> ObterDivisaoParticipante(int idInscricao)
    {
        return Sessao.QueryOver<DivisaoAtividadeParticipante>()
            .Where(x => x.Inscricao.Id == idInscricao)
            .List();
    }
}
