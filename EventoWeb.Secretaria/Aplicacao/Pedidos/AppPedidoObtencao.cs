using EventoWeb.Comum.Aplicacao;
using EventoWeb.Comum.Negocio.Repositorios;

namespace EventoWeb.Secretaria.Aplicacao.Pedidos
{
    public class AppPedidoObtencao : AppBase
    {
        private readonly IPedidos m_Pedidos;

        public AppPedidoObtencao(
            IContexto contexto,
            IPedidos pedidos) : base(contexto)
        {
            m_Pedidos = pedidos ?? throw new ArgumentNullException(nameof(pedidos));
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
