using EventoWeb.Comum.Negocio.Entidades;
using EventoWeb.Comum.Negocio.Entidades.Financeiro;
using EventoWeb.Comum.Negocio.Entidades.IntegracaoFinanceira;
using EventoWeb.Comum.Negocio.Repositorios;
using EventoWeb.Comum.Aplicacao;
using EventoWeb.Secretaria.Aplicacao.Conversores;
using EventoWeb.Secretaria.Aplicacao.RegistrosIntegracao;

namespace EventoWeb.Secretaria.Aplicacao.Pedidos
{
    public class AppPedidoObtencao : AppBase
    {
        private readonly IInscricoes m_Inscricoes;
        private readonly IPedidos m_Pedidos;
        private readonly IRegistrosIntegracoesFinanceiras m_RegistrosIntegracao;

        public AppPedidoObtencao(
            IContexto contexto,
            IInscricoes inscricoes,
            IPedidos pedidos,
            IRegistrosIntegracoesFinanceiras registrosIntegracao) : base(contexto)
        {
            m_Inscricoes = inscricoes ?? throw new ArgumentNullException(nameof(inscricoes));
            m_Pedidos = pedidos ?? throw new ArgumentNullException(nameof(pedidos));
            m_RegistrosIntegracao = registrosIntegracao ?? throw new ArgumentNullException(nameof(registrosIntegracao));
        }

        public DTOPedido? ObterPorInscricao(int idInscricao)
        {
            DTOPedido? resultado = null;

            ExecutarSeguramente(() =>
            {
                var pedido = m_Pedidos.ObterPorInscricao(idInscricao);
                if (pedido != null)
                {
                    resultado = pedido.Converter();
                }
            });

            return resultado;
        }
    }
}
