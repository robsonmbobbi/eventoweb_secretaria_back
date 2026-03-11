using EventoWeb.Comum.Negocio.Repositorios;
using EventoWeb.Secretaria.Negocio.Repositorios;

namespace EventoWeb.Secretaria.Aplicacao.Seguranca
{
    public class AppUsuarioObter : AppUsuarioBase
    {
        public AppUsuarioObter(IContexto contexto, IUsuarios usuarios)
            : base(contexto, usuarios) { }

        public required string Login { get; set; }

        public DTOUsuario? Obter()
        {
            DTOUsuario? dto = null;

            ExecutarSeguramente(() =>
            {

                dto = Usuarios
                    .ObterPeloLogin(Login)?
                    .Converter();
            });

            return dto;
        }
    }
}
