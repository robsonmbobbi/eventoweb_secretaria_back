using EventoWeb.Comum.Aplicacao.FormasPagamento;
using EventoWeb.Comum.Aplicacao.Inscricoes;
using EventoWeb.Comum.Negocio.Entidades;
using EventoWeb.Comum.Negocio.Entidades.Financeiro;
using EventoWeb.Secretaria.Aplicacao.Inscricoes;

namespace EventoWeb.Secretaria.Aplicacao.Pedidos
{
    public class DTOPedido
    {
        public required int Id { get; set; }
        public required decimal Valor { get; set; }
        public required EnumTipoPedido Tipo { get; set; }
        public required DTOFormaPagamento? FormaPagamento { get; set; }
        public required List<DTOInscricaoListagem> Inscricoes { get; set; }
        public required DTOPessoa Pagador { get; set; }
        public required DTOConta Conta { get; set; }
    }
}
