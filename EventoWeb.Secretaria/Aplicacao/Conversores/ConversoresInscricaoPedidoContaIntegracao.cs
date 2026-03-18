using EventoWeb.Comum.Negocio.Entidades;
using EventoWeb.Comum.Negocio.Entidades.Financeiro;
using EventoWeb.Comum.Negocio.Entidades.IntegracaoFinanceira;
using EventoWeb.Comum.Aplicacao.Inscricoes;
using EventoWeb.Comum.Aplicacao.FormasPagamento;
using EventoWeb.Secretaria.Aplicacao.DTOs;

namespace EventoWeb.Secretaria.Aplicacao.Conversores
{
    public static class ConversoresInscricaoPedidoContaIntegracao
    {
        public static DTOPedidoCompleto ConverterPedidoCompleto(this Pedido pedido)
        {
            return new DTOPedidoCompleto
            {
                Id = pedido.Id,
                Valor = pedido.Valor.Valor,
                Tipo = pedido.Tipo,
                FormaPagamento = pedido.FormaPagamento?.Converter(),
                Inscricoes = pedido.Inscricoes?.Select(i => i.Converter()).ToList() ?? [],
                Pagador = pedido.Pagador.Converter(),
                Conta = pedido.Conta.Converter()
            };
        }

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

        public static DTORegistroIntegracaoLog Converter(this RegistroIntegracaoLog log)
        {
            return new DTORegistroIntegracaoLog
            {
                Mensagem = log.Mensagem,
                Tipo = log.Tipo,
                Data = log.Data,
                Dados = log.Dados
            };
        }

        public static DTORegistroIntegracao Converter(this RegistroIntegracaoFinanceira registro)
        {
            return new DTORegistroIntegracao
            {
                Id = registro.Id,
                IdentificacaoNoIntegrador = registro.IdentificacaoNoIntegrador,
                Situacao = registro.Situacao,
                Tipo = registro.Tipo,
                Valor = registro.Valor.Valor,
                DataRegistro = registro.DataRegistro,
                NumeroParcelas = registro.NumeroParcelas,
                DataConcluidoAbortado = registro.DataConcluidoAbortado,
                Logs = registro.Logs?.Select(l => l.Converter()).ToList() ?? new List<DTORegistroIntegracaoLog>()
            };
        }
    }
}
