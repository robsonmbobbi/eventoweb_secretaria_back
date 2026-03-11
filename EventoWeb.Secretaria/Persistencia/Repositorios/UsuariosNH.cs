using EventoWeb.Comum.Persistencia.Repositorios;
using EventoWeb.Secretaria.Negocio.Entidades.Seguranca;
using EventoWeb.Secretaria.Negocio.Repositorios;
using NHibernate;

namespace EventoWeb.Secretaria.Persistencia.Repositorios
{
    public class UsuariosNH : PersistenciaNH<Usuario>, IUsuarios
    {
        public UsuariosNH(NHibernate.ISession sessao) : base(sessao)
        {
        }

        public IList<Usuario> ListarTodos()
        {
            return Sessao
                .QueryOver<Usuario>()
                .List();
        }

        public Usuario ObterPeloLogin(string login)
        {
            return Sessao.Get<Usuario>(login);
        }
    }
}
