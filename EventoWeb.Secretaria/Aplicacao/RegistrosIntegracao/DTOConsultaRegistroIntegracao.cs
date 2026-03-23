using EventoWeb.Comum.Negocio.Entidades.Financeiro;
using EventoWeb.Comum.Negocio.Servicos;

namespace EventoWeb.Secretaria.Aplicacao.RegistrosIntegracao
{
    public class DTOConsultaRegistroIntegracao
    {
        public EnumTipoPagamento TipoTransacao { get; set; }
        public EnumStatusTransacao Status { get; set; }
        public string? ImagemQRCodePixBase64 { get; set; }
        public string? PixCopiaECola { get; set; }
        public string? LinkPagamento { get; set; }
        public decimal Valor { get; set; }
    }
}
