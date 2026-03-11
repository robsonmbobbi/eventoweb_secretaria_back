using EventoWeb.Comum.Aplicacao;
using EventoWeb.Comum.Negocio.Repositorios;
using EventoWeb.Secretaria.Negocio.Entidades.Seguranca;
using EventoWeb.Secretaria.Negocio.Repositorios;

namespace EventoWeb.Secretaria.Aplicacao.Seguranca;

public abstract class AppUsuarioBase: AppBase
{
    protected IUsuarios Usuarios { get; private set; }

    public AppUsuarioBase(IContexto contexto, IUsuarios usuarios) : 
        base(contexto)
    {
        Usuarios = usuarios ?? throw new ArgumentNullException(nameof(usuarios));
    }

    protected Usuario ObterOuExcecao(string login)
    {
        return Usuarios.ObterPeloLogin(login) ?? throw new Exception($"Usuário não encontrado com o login \"{login}\"");
    }
}