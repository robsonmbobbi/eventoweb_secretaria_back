using EventoWeb.Comum.Negocio.Repositorios;
using EventoWeb.Secretaria.Negocio.Repositorios;

namespace EventoWeb.Secretaria.Aplicacao.Seguranca
{
    public class AppUsuarioAutenticacao : AppUsuarioBase
    {
        public AppUsuarioAutenticacao(IContexto contexto, IUsuarios usuarios)
            : base(contexto, usuarios) { }

        public required string Login { get; set; }
        public required string Senha { get; set; }

        public DTOUsuario? Autenticar()
        {
            if (string.IsNullOrWhiteSpace(Login))
                throw new Exception("Login precisa ser informado");

            DTOUsuario usuario = null;
            ExecutarSeguramente(() =>
            {
                var usr = Usuarios.ObterPeloLogin(Login);
                if (usr != null && usr.Senha.EhIgual(Senha))
                    usuario = usr.Converter()!;

            });

            return usuario;
        }
    }
}
