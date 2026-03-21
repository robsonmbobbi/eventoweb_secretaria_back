using EventoWeb.Comum.Negocio.Entidades;
using EventoWeb.Comum.Negocio.Entidades.Financeiro;
using EventoWeb.Comum.Negocio.Entidades.IntegracaoFinanceira;
using EventoWeb.Comum.Negocio.ObjetosValor;
using EventoWeb.Comum.Negocio.Repositorios;
using EventoWeb.Comum.Negocio.Servicos;
using EventoWeb.Secretaria.Aplicacao.RegistrosIntegracao;
using EventoWeb.Secretaria.Negocio.Servicos.Notificacoes.Pagamentos;

namespace EventoWeb.Secretaria.Negocio.Servicos.RegistroIntegracao
{
    public class SrvCriacaoRegistroIntegracao
    {
        private readonly IPersistencia<Conta> m_Contas;
        private readonly IFormasPagamento m_FormasPagamento;
        private readonly IIntegracaoFinanceiraPorFormasPagamentos m_Integracoes;
        private readonly IDictionary<EnumIntegracaoExterna, IIntegracaoExterna> m_IntegracoesExternas;
        private readonly IRegistrosIntegracoesFinanceiras m_RegistrosIntegracao;
        private readonly SrvNotificacaoNovoPagamento m_SrvNotificacao;
        private readonly IEventos m_Eventos;

        public SrvCriacaoRegistroIntegracao(
            IPersistencia<Conta> contas,
            IFormasPagamento formasPagamento,
            IIntegracaoFinanceiraPorFormasPagamentos integracoes,
            IDictionary<EnumIntegracaoExterna, IIntegracaoExterna> integracoesExternas,
            IRegistrosIntegracoesFinanceiras registrosIntegracao,
            SrvNotificacaoNovoPagamento srvNotificacao,
            IEventos eventos)
        {
            m_Contas = contas ?? throw new ArgumentNullException(nameof(contas));
            m_FormasPagamento = formasPagamento ?? throw new ArgumentNullException(nameof(formasPagamento));
            m_Integracoes = integracoes ?? throw new ArgumentNullException(nameof(integracoes));
            m_IntegracoesExternas = integracoesExternas ?? throw new ArgumentNullException(nameof(integracoesExternas));
            m_RegistrosIntegracao = registrosIntegracao ?? throw new ArgumentNullException(nameof(registrosIntegracao));
            m_SrvNotificacao = srvNotificacao ?? throw new ArgumentNullException(nameof(srvNotificacao));
            m_Eventos = eventos ?? throw new ArgumentNullException(nameof(eventos));
        }

        public DTORegistroIntegracao Criar(int idConta, int idFormaPagamento, int idEvento, decimal valor, int? numeroParcelas)
        {
            var conta = m_Contas.Obter(idConta) ?? throw new Exception($"Conta com ID {idConta} não encontrada.");
            var formaPagamento = m_FormasPagamento.Obter(idFormaPagamento) ?? throw new Exception($"Forma de pagamento com ID {idFormaPagamento} não encontrada.");
            var evento = m_Eventos.Obter(idEvento) ?? throw new Exception($"Evento com ID {idEvento} não encontrado.");

            var integracao = m_Integracoes.ObterPorFormaPagamento(formaPagamento);
            if (integracao == null)
                throw new Exception($"Não existe integração configurada para a forma de pagamento: {formaPagamento.Id}");

            var integradorExterno = m_IntegracoesExternas[integracao.Integrador.IntegracaoExterna];

            // Chamar integração externa para criar cobrança usando o novo método com Conta e valor
            var retornoIntegracao = integradorExterno
                .CriarCobrancaPorConta(integracao, conta, valor, formaPagamento.Tipo, numeroParcelas)
                .Result;

            // Criar registro de integração com o valor informado
            var registroIntegracao = new RegistroIntegracaoFinanceira(
                integracao.Integrador,
                conta,
                new ValorMonetario(valor),
                formaPagamento.Tipo,
                retornoIntegracao.IdTransacao,
                numeroParcelas
            );

            // Persistir registro
            m_RegistrosIntegracao.Incluir(registroIntegracao);

            // Notificar novo pagamento
            m_SrvNotificacao.Notificar(conta.Pessoa, evento.Id, retornoIntegracao);

            // Converter e retornar
            return registroIntegracao.Converter();
        }
    }
}
