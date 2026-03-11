using EventoWeb.Comum.Negocio.Repositorios;
using EventoWeb.Secretaria.Negocio.Repositorios;

namespace EventoWeb.Secretaria.Aplicacao.Seguranca
{
    public class AppUsuarioAlteracaoSenhaPeloUsuario : AppUsuarioBase
    {
        public AppUsuarioAlteracaoSenhaPeloUsuario(IContexto contexto, IUsuarios usuarios)
            : base(contexto, usuarios) { }

        public required string Login { get; set; }
        public required string SenhaAtual { get; set; }
        public required string NovaSenha { get; set; }
        public required string NovaSenhaRepeticao { get; set; }

        public void Alterar()
        {
            if (string.IsNullOrWhiteSpace(Login))
                throw new Exception("Login precisa ser informado");

            ExecutarSeguramente(() =>
            {
                var usuario = Usuarios.ObterPeloLogin(Login) ?? 
                    throw new Exception("Nenhum usuário foi encontrado com esse login!");

                if (!usuario.Senha.EhIgual(SenhaAtual))
                    throw new Exception("A senha informada não é igual a que temos cadastrada!");
                
                usuario.Senha.AlterarSenha(NovaSenha, NovaSenhaRepeticao);
                Usuarios.Atualizar(usuario);
            });
        }
    }
}
