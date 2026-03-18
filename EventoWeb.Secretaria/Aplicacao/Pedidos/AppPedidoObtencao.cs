using EventoWeb.Comum.Negocio.Entidades;
using EventoWeb.Comum.Negocio.Entidades.Financeiro;
using EventoWeb.Comum.Negocio.Entidades.IntegracaoFinanceira;
using EventoWeb.Comum.Negocio.Repositorios;
using EventoWeb.Comum.Aplicacao;
using EventoWeb.Secretaria.Aplicacao.Conversores;
using EventoWeb.Secretaria.Aplicacao.DTOs;

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

        public DTOInscricaoPedidoContaIntegracao? ObterInscricaoComDados(int idInscricao)
        {
            DTOInscricaoPedidoContaIntegracao? resultado = null;

            ExecutarSeguramente(() =>
            {
                /*var pedido = m_Pedidos.ObterPorInscricao(idInscricao) ?? 
                    throw new Exception($"Nenhum pedido foi encontrado para essa inscrição. Id Inscrição {idInscricao}");



                var inscricao = m_Inscricoes.Obter(idInscricao);
                if (inscricao == null)
                    return;

                resultado = new DTOInscricaoPedidoContaIntegracao
                {
                    Inscricao = inscricao.ToResumidoDTO()
                };

                // Buscar pedido vinculado à inscrição
                
                if (pedido != null)
                {
                    resultado.Pedido = pedido.ToResumidoDTO();

                    // Buscar conta do pedido
                    var conta = pedido.Conta;
                    if (conta != null)
                    {
                        resultado.Conta = conta.Converter();

                        // Buscar registro de integração da conta
                        var registroIntegracao = BuscarRegistroIntegracaoPorConta(conta);
                        if (registroIntegracao != null)
                        {
                            resultado.RegistroIntegracao = registroIntegracao.Converter();
                        }
                    }
                }*/
            });

            return resultado;
        }

        private Pedido? BuscarPedidoPorInscricao(Inscricao inscricao)
        {
            return m_Pedidos.ObterPorInscricao(inscricao.Id);
        }

        private RegistroIntegracaoFinanceira? BuscarRegistroIntegracaoPorConta(Conta conta)
        {
            return m_RegistrosIntegracao.ObterPorConta(conta.Id);
        }
    }
}
