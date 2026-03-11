using EventoWeb.Comum.Negocio.Repositorios;
using EventoWeb.Secretaria.Negocio.Repositorios;

namespace EventoWeb.Secretaria.Aplicacao.Seguranca
{
    public class AppUsuarioExclusao : AppUsuarioBase
    {
        public AppUsuarioExclusao(IContexto contexto, IUsuarios usuarios)
            : base(contexto, usuarios) { }

        public required string Login {  get; set; }

        public void Excluir()
        {
            if (string.IsNullOrWhiteSpace(Login))
                throw new Exception("Login precisa ser informado");

            ExecutarSeguramente(() =>
            {
                var usuario = ObterOuExcecao(Login);
                Usuarios.Excluir(usuario);
            });
        }
    }
}
