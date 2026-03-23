using EventoWeb.Comum.Negocio.Servicos;

namespace EventoWeb.Secretaria.Aplicacao.RegistrosIntegracao
{
    public class ConversorDTOConsultaRegistroIntegracao
    {
        public static DTOConsultaRegistroIntegracao Converter(DadosRetornoIntegracaoExterna dados)
        {
            return new DTOConsultaRegistroIntegracao
            {
                TipoTransacao = dados.TipoTransacao,
                Status = dados.Status,
                ImagemQRCodePixBase64 = dados.ImagemQRCodePixBase64,
                PixCopiaECola = dados.PixCopiaECola,
                LinkPagamento = dados.LinkPagamento,
                Valor = dados.Valor
            };
        }
    }
}
