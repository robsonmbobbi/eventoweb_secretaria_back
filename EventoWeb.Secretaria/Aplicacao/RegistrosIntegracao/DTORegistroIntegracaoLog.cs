using EventoWeb.Comum.Negocio.Entidades.IntegracaoFinanceira;

namespace EventoWeb.Secretaria.Aplicacao.RegistrosIntegracao
{
    public class DTORegistroIntegracaoLog
    {
        public string Mensagem { get; set; } = string.Empty;
        public EnumTipoLog Tipo { get; set; }
        public DateTime Data { get; set; }
        public string? Dados { get; set; }
    }
}
