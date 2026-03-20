using EventoWeb.Comum.Aplicacao;
using EventoWeb.Comum.Negocio.Repositorios;

namespace EventoWeb.Secretaria.Aplicacao.RegistrosIntegracao
{
    public class AppRegistroIntegracaoObtencao : AppBase
    {
        private readonly IRegistrosIntegracoesFinanceiras m_RegistrosIntegracoesFinanceiras;

        public AppRegistroIntegracaoObtencao(
            IContexto contexto,
            IRegistrosIntegracoesFinanceiras registrosIntegracoesFinanceiras) : base(contexto)
        {
            m_RegistrosIntegracoesFinanceiras = registrosIntegracoesFinanceiras ?? throw new ArgumentNullException(nameof(registrosIntegracoesFinanceiras));
        }

        public IList<DTORegistroIntegracao> ObterPorConta(int idConta)
        {
            var resultado = new List<DTORegistroIntegracao>();

            ExecutarSeguramente(() =>
            {
                var registros = m_RegistrosIntegracoesFinanceiras.ListarPorConta(idConta);
                resultado = registros.Select(r => r.Converter()).ToList();
            });

            return resultado;
        }
    }
}
