using EventoWeb.Comum.Aplicacao;
using EventoWeb.Comum.Negocio.Repositorios;
using EventoWeb.Secretaria.Negocio.Servicos.RegistroIntegracao;

namespace EventoWeb.Secretaria.Aplicacao.RegistrosIntegracao
{
    public class AppRegistroIntegracaoConsulta : AppBase
    {
        private readonly SrvConsultaRegistroIntegracao m_Servico;

        public AppRegistroIntegracaoConsulta(
            IContexto contexto,
            SrvConsultaRegistroIntegracao servico) : base(contexto)
        {
            m_Servico = servico ?? throw new ArgumentNullException(nameof(servico));
        }

        public DTOConsultaRegistroIntegracao? ConsultarDados(int idRegistroIntegracao)
        {
            DTOConsultaRegistroIntegracao? resultado = null;

            ExecutarSeguramente(() =>
            {
                var dados = m_Servico.Consultar(idRegistroIntegracao);
                
                if (dados != null)
                {
                    resultado = ConversorDTOConsultaRegistroIntegracao.Converter(dados);
                }
            });

            return resultado;
        }
    }
}
