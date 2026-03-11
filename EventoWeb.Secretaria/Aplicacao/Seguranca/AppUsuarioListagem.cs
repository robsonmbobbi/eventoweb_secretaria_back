using EventoWeb.Comum.Negocio.Repositorios;
using EventoWeb.Secretaria.Negocio.Repositorios;

namespace EventoWeb.Secretaria.Aplicacao.Seguranca
{
    public class AppUsuarioListagem : AppUsuarioBase
    {
        public AppUsuarioListagem(IContexto contexto, IUsuarios usuarios)
            : base(contexto, usuarios) { }

        public IList<DTOUsuario> ListarTodos()
        {
            var lista = new List<DTOUsuario>();

            ExecutarSeguramente(() =>
            {               
                lista = Usuarios
                    .ListarTodos()
                    .Select(x=> x.Converter()!)
                    .ToList();
            });

            return lista;
        }
    }
}
