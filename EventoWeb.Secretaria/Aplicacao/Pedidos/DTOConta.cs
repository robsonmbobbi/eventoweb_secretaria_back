using EventoWeb.Comum.Negocio.Entidades.Financeiro;

namespace EventoWeb.Secretaria.Aplicacao.Pedidos
{
    public class DTOConta
    {
        public int Id { get; set; }
        public decimal Valor { get; set; }
        public DateTime DataVencimento { get; set; }
        public bool Liquidado { get; set; }
        public decimal ValorTotalTransacoes { get; set; }
        public decimal ValorTotalDesconto { get; set; }
        public decimal ValorTotalJuros { get; set; }
        public decimal ValorTotalMulta { get; set; }
        public string? Descricao { get; set; }
        public DateTime DataCriado { get; set; }
        public EnumTipoTransacao Tipo { get; set; }
    }
}
