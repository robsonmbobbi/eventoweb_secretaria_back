using EventoWeb.Comum.Negocio.Entidades;
using EventoWeb.Comum.Aplicacao.Inscricoes;
using EventoWeb.Comum.Aplicacao.FormasPagamento;
using EventoWeb.Secretaria.Aplicacao.Inscricoes;

namespace EventoWeb.Secretaria.Aplicacao.Pedidos
{
    public static class ConversorDTOPedido
    {
        public static DTOPedido Converter(this Pedido pedido)
        {
            return new DTOPedido
            {
                Id = pedido.Id,
                Valor = pedido.Valor.Valor,
                Tipo = pedido.Tipo,
                FormaPagamento = pedido.FormaPagamento?.Converter(),
                Inscricoes = pedido.Inscricoes?.Select(i => i.ConverterParaListagem()).ToList() ?? [],
                Pagador = pedido.Pagador.Converter(),
                Conta = pedido.Conta.Converter()
            };
        }
    }
}
