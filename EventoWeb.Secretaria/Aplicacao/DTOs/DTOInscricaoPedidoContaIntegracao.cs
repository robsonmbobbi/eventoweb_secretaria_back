using EventoWeb.Comum.Aplicacao.FormasPagamento;
using EventoWeb.Comum.Aplicacao.Inscricoes;
using EventoWeb.Comum.Negocio.Entidades;
using EventoWeb.Comum.Negocio.Entidades.Financeiro;
using EventoWeb.Comum.Negocio.Entidades.IntegracaoFinanceira;

namespace EventoWeb.Secretaria.Aplicacao.DTOs
{
    public class DTOPedidoCompleto
    {
        public required int Id { get; set; }
        public required decimal Valor { get; set; }
        public required EnumTipoPedido Tipo { get; set; }
        public required DTOFormaPagamento? FormaPagamento { get; set; }
        public required List<DTOInscricao> Inscricoes { get; set; }
        public required DTOPessoa Pagador { get; set; }
        public required DTOConta Conta { get; set; }

    }

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

    public class DTORegistroIntegracaoLog
    {
        public string Mensagem { get; set; } = string.Empty;
        public EnumTipoLog Tipo { get; set; }
        public DateTime Data { get; set; }
        public string? Dados { get; set; }
    }

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

    public class DTOInscricaoPedidoContaIntegracao
    {
        public DTOPedidoCompleto? Pedido { get; set; }
        public DTORegistroIntegracao? RegistroIntegracao { get; set; }
    }
}
