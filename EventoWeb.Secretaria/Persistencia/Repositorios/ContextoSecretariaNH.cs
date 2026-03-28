using EventoWeb.Comum.Negocio.Entidades.Financeiro;
using EventoWeb.Comum.Negocio.Repositorios;
using EventoWeb.Comum.Persistencia.Repositorios;
using EventoWeb.Secretaria.Negocio.Repositorios;
using EventoWeb.Secretaria.Negocio.Entidades.Atividades;
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
    public IContasBancarias ContasBancarias => new ContasBancariasNH(m_Sessao);
    public IAtividades Atividades => new AtividadesNH(m_Sessao);
    public IQuartos Quartos => new QuartosNH(m_Sessao);
}