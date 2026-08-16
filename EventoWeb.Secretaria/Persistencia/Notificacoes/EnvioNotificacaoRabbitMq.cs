using System.Text.Json;
using EventoWeb.Comum.Negocio.Entidades.Notificacoes;
using EventoWeb.Comum.Negocio.Servicos.Notificacoes;
using RabbitMQ.Client;

namespace EventoWeb.Secretaria.Persistencia.Notificacoes
{
    public class EnvioNotificacaoRabbitMq : IEnvioNotificacao, IAsyncDisposable
    {
        private readonly ConfiguracaoRabbitMqNotificacao m_Configuracao;
        private readonly SemaphoreSlim m_SemaforoConexao = new(1, 1);
        private readonly SemaphoreSlim m_SemaforoPublicacao = new(1, 1);

        private IConnection? m_Conexao;
        private IChannel? m_Canal;

        public EnvioNotificacaoRabbitMq(ConfiguracaoRabbitMqNotificacao configuracao)
        {
            m_Configuracao = configuracao;
        }

        public async Task Enviar(MensagemNotificacao mensagem)
        {
            var canal = await ObterCanalAsync();

            var request = new NotificacaoRequest
            {
                Id = Guid.NewGuid(),
                Meio = mensagem.Modelo.Meio,
                Destinatario = mensagem.Destinatario.Valor,
                TemplateAssunto = mensagem.Modelo.Assunto?.Valor,
                TemplateConteudo = mensagem.Modelo.Mensagem.Valor,
                DadosJson = mensagem.VariaveisJson?.Valor
            };

            var corpo = JsonSerializer.SerializeToUtf8Bytes(request);
            var propriedades = new BasicProperties
            {
                CorrelationId = mensagem.Id.ToString(),
                ReplyTo = m_Configuracao.ResponseQueueName
            };

            await m_SemaforoPublicacao.WaitAsync();
            try
            {
                await canal.BasicPublishAsync(
                    exchange: "",
                    routingKey: m_Configuracao.RequestQueueName,
                    mandatory: false,
                    basicProperties: propriedades,
                    body: corpo);
            }
            finally
            {
                m_SemaforoPublicacao.Release();
            }
        }

        private async Task<IChannel> ObterCanalAsync()
        {
            if (m_Canal is { IsOpen: true })
            {
                return m_Canal;
            }

            await m_SemaforoConexao.WaitAsync();
            try
            {
                if (m_Canal is { IsOpen: true })
                {
                    return m_Canal;
                }

                if (m_Conexao is not { IsOpen: true })
                {
                    var factory = new ConnectionFactory
                    {
                        HostName = m_Configuracao.HostName,
                        Port = m_Configuracao.Port,
                        UserName = m_Configuracao.UserName,
                        Password = m_Configuracao.Password,
                        VirtualHost = m_Configuracao.VirtualHost,
                        AutomaticRecoveryEnabled = true
                    };
                    m_Conexao = await factory.CreateConnectionAsync();
                }

                m_Canal = await m_Conexao.CreateChannelAsync();

                // Declaração idempotente: o listener (NotificacaoRespostaListenerService) também a
                // declara de forma independente, sem depender de ordem entre os dois serviços.
                await m_Canal.QueueDeclareAsync(
                    queue: m_Configuracao.ResponseQueueName,
                    durable: true,
                    exclusive: false,
                    autoDelete: false);

                return m_Canal;
            }
            finally
            {
                m_SemaforoConexao.Release();
            }
        }

        public async ValueTask DisposeAsync()
        {
            if (m_Canal is not null)
            {
                await m_Canal.CloseAsync();
                m_Canal.Dispose();
            }

            if (m_Conexao is not null)
            {
                await m_Conexao.CloseAsync();
                m_Conexao.Dispose();
            }

            m_SemaforoConexao.Dispose();
            m_SemaforoPublicacao.Dispose();
        }
    }
}
