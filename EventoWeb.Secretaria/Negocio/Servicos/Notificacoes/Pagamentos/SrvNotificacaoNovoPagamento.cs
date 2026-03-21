using EventoWeb.Comum.Negocio.Entidades;
using EventoWeb.Comum.Negocio.Entidades.Financeiro;
using EventoWeb.Comum.Negocio.Entidades.Notificacoes;
using EventoWeb.Comum.Negocio.ObjetosValor;
using EventoWeb.Comum.Negocio.Repositorios;
using EventoWeb.Comum.Negocio.Servicos;
using System.Text.Json;

namespace EventoWeb.Secretaria.Negocio.Servicos.Notificacoes.Pagamentos
{
    public class SrvNotificacaoNovoPagamento(IModelosMensagemNotificacao modelosNotificacao, IMensagens mensagens)
    {
        private readonly IModelosMensagemNotificacao m_ModelosNotificacao = modelosNotificacao;
        private readonly IMensagens m_Mensagens = mensagens;

        public void Notificar(Pessoa pessoa, int idEvento, DadosRetornoIntegracaoExterna dadosRetorno)
        {
            var modelos = m_ModelosNotificacao.ListarPorTipo(idEvento, EnumTipoNotificacao.NovoPagamento);
            foreach (var modelo in modelos)
            {
                var destinatario = "";
                if (modelo.Meio == EnumMeioNotificacao.EMail)
                {
                    destinatario = pessoa.Email.Endereco;
                }
                else
                {
                    destinatario = pessoa.CelularWP.Numero;
                }

                var tipoTransacao = "";
                switch (dadosRetorno.TipoTransacao)
                {
                    case EnumTipoPagamento.CartaoCredito:
                        tipoTransacao = "Cartão Crédito";
                        break;
                    case EnumTipoPagamento.PIX:
                        tipoTransacao = "PIX";
                        break;
                }

                var mensagem = new MensagemNotificacao(
                    modelo,
                    destinatario,
                    JsonSerializer.Serialize(
                        new
                        {
                            NomeEvento = modelo.Evento.Nome,
                            TipoTransacao = tipoTransacao,
                            dadosRetorno.Valor,
                            dadosRetorno.LinkPagamento,
                            dadosRetorno.ImagemQRCodePixBase64,
                            dadosRetorno.PixCopiaECola
                        }
                    )
                );
                m_Mensagens.Incluir(mensagem);
            }
        }
    }
}
