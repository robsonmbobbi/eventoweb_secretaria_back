using EventoWeb.Comum.Negocio.Entidades.Financeiro;

namespace EventoWeb.Secretaria.Aplicacao.Pedidos
{
    public static class ConversorDTOConta
    {
        public static DTOConta Converter(this Conta conta)
        {
            return new DTOConta
            {
                Id = conta.Id,
                Valor = conta.Valor.Valor,
                DataVencimento = conta.DataVencimento,
                Liquidado = conta.Liquidado,
                ValorTotalTransacoes = conta.ValorTotalTransacoes,
                ValorTotalDesconto = conta.ValorTotalDesconto,
                ValorTotalJuros = conta.ValorTotalJuros,
                ValorTotalMulta = conta.ValorTotalMulta
            };
        }
    }
}
