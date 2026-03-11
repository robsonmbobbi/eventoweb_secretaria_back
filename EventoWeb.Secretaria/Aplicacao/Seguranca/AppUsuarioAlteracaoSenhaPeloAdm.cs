using EventoWeb.Comum.Negocio.Repositorios;
using EventoWeb.Secretaria.Negocio.Repositorios;

namespace EventoWeb.Secretaria.Aplicacao.Seguranca
{
    public class AppUsuarioAlteracaoSenhaPeloAdm : AppUsuarioBase
    {
        public AppUsuarioAlteracaoSenhaPeloAdm(IContexto contexto, IUsuarios usuarios)
            : base(contexto, usuarios) { }

        public required string Login { get; set; }
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
               
                usuario.Senha.AlterarSenha(NovaSenha, NovaSenhaRepeticao);
                Usuarios.Atualizar(usuario);
            });
        }
    }
}
