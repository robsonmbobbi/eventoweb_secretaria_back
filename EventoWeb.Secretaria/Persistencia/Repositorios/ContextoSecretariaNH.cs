using EventoWeb.Comum.Persistencia.Repositorios;
using EventoWeb.Secretaria.Negocio.Repositorios;
using NHibernate;

namespace EventoWeb.Secretaria.Persistencia.Repositorios;

public class ContextoSecretariaNH : ContextoNH
{
    private readonly ISession m_Sessao;

    public ContextoSecretariaNH(ISession sessao) : base(sessao)
    {
        m_Sessao = sessao;
    }

    public IUsuarios Usuarios => new UsuariosNH(m_Sessao);
}