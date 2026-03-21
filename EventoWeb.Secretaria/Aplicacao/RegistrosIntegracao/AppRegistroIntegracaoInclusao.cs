using EventoWeb.Comum.Aplicacao;
using EventoWeb.Comum.Negocio.Repositorios;
using EventoWeb.Secretaria.Negocio.Servicos.RegistroIntegracao;

namespace EventoWeb.Secretaria.Aplicacao.RegistrosIntegracao
{
    public class AppRegistroIntegracaoInclusao : AppBase
    {
        private readonly SrvCriacaoRegistroIntegracao m_Servico;

        public AppRegistroIntegracaoInclusao(
            IContexto contexto,
            SrvCriacaoRegistroIntegracao servico) : base(contexto)
        {
            m_Servico = servico ?? throw new ArgumentNullException(nameof(servico));
        }

        public DTORegistroIntegracao Incluir(DTORegistroIntegracaoInclusao dto)
        {
            DTORegistroIntegracao? resultado = null;

            ExecutarSeguramente(() =>
            {
                resultado = m_Servico.Criar(
                    dto.IdConta,
                    dto.IdFormaPagamento,
                    dto.IdEvento,
                    dto.Valor,
                    dto.NumeroParcelas
                );
            });

            return resultado!;
        }
    }
}
