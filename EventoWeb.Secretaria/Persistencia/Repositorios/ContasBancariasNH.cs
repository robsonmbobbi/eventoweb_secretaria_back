using EventoWeb.Comum.Negocio.Entidades.Financeiro;
using EventoWeb.Comum.Persistencia.Repositorios;
using EventoWeb.Secretaria.Negocio.Repositorios;
using NHibernate;

namespace EventoWeb.Secretaria.Persistencia.Repositorios;

public class ContasBancariasNH : PersistenciaNH<ContaBancaria>, IContasBancarias
{
    public ContasBancariasNH(ISession sessao) : base(sessao)
    {
    }

    public IList<ContaBancaria> ListarTodos()
    {
        return Sessao
            .QueryOver<ContaBancaria>()
            .List();
    }
}

