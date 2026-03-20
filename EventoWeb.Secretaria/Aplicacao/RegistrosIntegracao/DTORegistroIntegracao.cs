using EventoWeb.Comum.Negocio.Entidades.Financeiro;
using EventoWeb.Comum.Negocio.Entidades.IntegracaoFinanceira;

namespace EventoWeb.Secretaria.Aplicacao.RegistrosIntegracao
{
    public class DTORegistroIntegracao
    {
        public int Id { get; set; }
        public string IdentificacaoNoIntegrador { get; set; } = string.Empty;
        public EnumSituacaoIntegracao Situacao { get; set; }
        public EnumTipoPagamento Tipo { get; set; }
        public decimal Valor { get; set; }
        public DateTime DataRegistro { get; set; }
        public int? NumeroParcelas { get; set; }
        public DateTime? DataConcluidoAbortado { get; set; }
        public List<DTORegistroIntegracaoLog> Logs { get; set; } = new();
    }
}
