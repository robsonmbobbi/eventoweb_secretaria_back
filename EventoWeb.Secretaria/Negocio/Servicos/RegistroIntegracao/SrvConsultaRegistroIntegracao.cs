using EventoWeb.Comum.Negocio.Entidades.IntegracaoFinanceira;
using EventoWeb.Comum.Negocio.Repositorios;
using EventoWeb.Comum.Negocio.Servicos;

namespace EventoWeb.Secretaria.Negocio.Servicos.RegistroIntegracao
{
    public class SrvConsultaRegistroIntegracao
    {
        private readonly IRegistrosIntegracoesFinanceiras m_RegistrosIntegracao;
        private readonly IDictionary<EnumIntegracaoExterna, IIntegracaoExterna> m_IntegracoesExternas;

        public SrvConsultaRegistroIntegracao(
            IRegistrosIntegracoesFinanceiras registrosIntegracao,
            IDictionary<EnumIntegracaoExterna, IIntegracaoExterna> integracoesExternas)
        {
            m_RegistrosIntegracao = registrosIntegracao ?? throw new ArgumentNullException(nameof(registrosIntegracao));
            m_IntegracoesExternas = integracoesExternas ?? throw new ArgumentNullException(nameof(integracoesExternas));
        }

        public DadosRetornoIntegracaoExterna? Consultar(int idRegistroIntegracao)
        {
            var registro = m_RegistrosIntegracao.Obter(idRegistroIntegracao) 
                ?? throw new Exception($"Registro de integração com ID {idRegistroIntegracao} não encontrado.");

            var integradorExterno = m_IntegracoesExternas[registro.Integrador.IntegracaoExterna];

            var retorno = integradorExterno
                .ConsultarCobranca(registro.Integrador, registro.IdentificacaoNoIntegrador)
                .Result;

            return retorno;
        }
    }
}
