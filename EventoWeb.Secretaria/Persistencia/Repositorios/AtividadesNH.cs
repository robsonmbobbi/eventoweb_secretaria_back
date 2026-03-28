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
}
