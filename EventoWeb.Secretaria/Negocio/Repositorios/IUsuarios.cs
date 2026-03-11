using EventoWeb.Comum.Negocio.Repositorios;
using EventoWeb.Secretaria.Negocio.Entidades.Seguranca;

namespace EventoWeb.Secretaria.Negocio.Repositorios
{
    public interface IUsuarios : IPersistencia<Usuario>
    {
        public abstract Usuario ObterPeloLogin(String login);

        public abstract IList<Usuario> ListarTodos();
    }
}
