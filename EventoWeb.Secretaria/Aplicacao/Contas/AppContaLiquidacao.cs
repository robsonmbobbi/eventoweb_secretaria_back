using EventoWeb.Comum.Aplicacao;
using EventoWeb.Comum.Negocio.Repositorios;
using EventoWeb.Secretaria.Negocio.Servicos.Contas;

namespace EventoWeb.Secretaria.Aplicacao.Contas;

public class AppContaLiquidacao : AppBase
{
    private readonly SrvLiquidacaoConta m_Servico;

    public AppContaLiquidacao(
        IContexto contexto,
        SrvLiquidacaoConta servico) : base(contexto)
    {
        m_Servico = servico ?? throw new ArgumentNullException(nameof(servico));
    }

    public void Liquidar(DTOLiquidacaoConta dto)
    {
        ExecutarSeguramente(() =>
        {
            m_Servico.Liquidar(
                dto.IdConta,
                dto.IdContaBancaria,
                dto.Valor
            );
        });
    }
}
