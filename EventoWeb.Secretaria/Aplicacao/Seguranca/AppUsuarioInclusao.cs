using EventoWeb.Comum.Negocio.Repositorios;
using EventoWeb.Secretaria.Negocio.Entidades.Seguranca;
using EventoWeb.Secretaria.Negocio.Repositorios;

namespace EventoWeb.Secretaria.Aplicacao.Seguranca
{
    public class AppUsuarioInclusao : AppUsuarioBase
    {
        public AppUsuarioInclusao(IContexto contexto, IUsuarios usuarios)
            : base(contexto, usuarios) { }

        public required DTOUsuarioInclusao DadosUsuario {  get; set; }

        public void Incluir()
        {
            if (string.IsNullOrWhiteSpace(DadosUsuario.Login))
                throw new Exception("Login precisa ser informado");

            ExecutarSeguramente(() =>
            {
                if (Usuarios.ObterPeloLogin(DadosUsuario.Login) != null)
                    throw new Exception("Já existe um usuário com este login!");

                var usuario = new Usuario(
                    DadosUsuario.Login,
                    DadosUsuario.Nome,
                    new SenhaUsuario(DadosUsuario.Senha, DadosUsuario.RepeticaoSenha));
                usuario.EhAdministrador = DadosUsuario.EhAdministrador;

                Usuarios.Incluir(usuario);
            });
        }
    }
}
