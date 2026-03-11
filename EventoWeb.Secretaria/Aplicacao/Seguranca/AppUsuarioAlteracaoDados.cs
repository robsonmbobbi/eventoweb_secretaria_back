using EventoWeb.Comum.Negocio.Repositorios;
using EventoWeb.Secretaria.Negocio.Repositorios;

namespace EventoWeb.Secretaria.Aplicacao.Seguranca
{
    public class AppUsuarioAlteracaoDados : AppUsuarioBase
    {
        public AppUsuarioAlteracaoDados(IContexto contexto, IUsuarios usuarios)
            : base(contexto, usuarios) { }

        public DTOUsuario? DadosUsuario { get; set; } = null;

        public void Alterar()
        {
            if (DadosUsuario == null)
                throw new Exception("Dados do usuário não foram informados!");

            if (string.IsNullOrWhiteSpace(DadosUsuario.Login))
                throw new Exception("Login precisa ser informado");

            ExecutarSeguramente(() =>
            {
                var usuario = ObterOuExcecao(DadosUsuario.Login);

                usuario.Nome = DadosUsuario.Nome;
                usuario.EhAdministrador = DadosUsuario.EhAdministrador;

                Usuarios.Atualizar(usuario);
            });
        }
    }
}
