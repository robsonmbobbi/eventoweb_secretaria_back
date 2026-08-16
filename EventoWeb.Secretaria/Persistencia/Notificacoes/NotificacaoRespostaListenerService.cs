using System.Text.Json;
using EventoWeb.Comum.Negocio.ObjetosValor;
using EventoWeb.Comum.Negocio.Repositorios;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace EventoWeb.Secretaria.Persistencia.Notificacoes
{
    public class NotificacaoRespostaListenerService : BackgroundService
    {
        private static readonly TimeSpan MinRetryDelay = TimeSpan.FromSeconds(2);
        private static readonly TimeSpan MaxRetryDelay = TimeSpan.FromSeconds(30);

        private readonly ConfiguracaoRabbitMqNotificacao m_Configuracao;
        private readonly IServiceScopeFactory m_ScopeFactory;
        private readonly ILogger<NotificacaoRespostaListenerService> m_Logger;

        private IConnection? m_Conexao;
        private IChannel? m_Canal;

        public NotificacaoRespostaListenerService(
            ConfiguracaoRabbitMqNotificacao configuracao,
            IServiceScopeFactory scopeFactory,
            ILogger<NotificacaoRespostaListenerService> logger)
        {
            m_Configuracao = configuracao;
            m_ScopeFactory = scopeFactory;
            m_Logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var retryDelay = MinRetryDelay;

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await RunOnceAsync(stoppingToken);
                    retryDelay = MinRetryDelay;
                }
                catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
                {
                    break;
                }
                catch (Exception ex)
                {
                    m_Logger.LogWarning(ex, "Conexão com RabbitMQ perdida (listener de retorno). Tentando novamente em {Delay}s.", retryDelay.TotalSeconds);

                    try
                    {
                        await Task.Delay(retryDelay, stoppingToken);
                    }
                    catch (OperationCanceledException)
                    {
                        break;
                    }

                    retryDelay = TimeSpan.FromSeconds(Math.Min(retryDelay.TotalSeconds * 2, MaxRetryDelay.TotalSeconds));
                }
            }
        }

        private async Task RunOnceAsync(CancellationToken stoppingToken)
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

            m_Conexao = await factory.CreateConnectionAsync(stoppingToken);
            m_Canal = await m_Conexao.CreateChannelAsync(cancellationToken: stoppingToken);

            await m_Canal.BasicQosAsync(prefetchSize: 0, prefetchCount: 1, global: false, cancellationToken: stoppingToken);

            var conexaoPerdida = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
            m_Conexao.ConnectionShutdownAsync += (_, _) =>
            {
                conexaoPerdida.TrySetResult();
                return Task.CompletedTask;
            };

            var consumer = new AsyncEventingBasicConsumer(m_Canal);
            consumer.ReceivedAsync += ProcessarRespostaAsync;

            await m_Canal.BasicConsumeAsync(
                queue: m_Configuracao.ResponseQueueName,
                autoAck: false,
                consumer: consumer,
                cancellationToken: stoppingToken);

            m_Logger.LogInformation("Consumindo fila de retorno '{Queue}' em {Host}:{Port}",
                m_Configuracao.ResponseQueueName, m_Configuracao.HostName, m_Configuracao.Port);

            using var registration = stoppingToken.Register(() => conexaoPerdida.TrySetResult());
            await conexaoPerdida.Task;

            stoppingToken.ThrowIfCancellationRequested();
        }

        private async Task ProcessarRespostaAsync(object sender, BasicDeliverEventArgs ea)
        {
            var canal = m_Canal ?? throw new InvalidOperationException("Canal RabbitMQ não inicializado.");

            if (!int.TryParse(ea.BasicProperties.CorrelationId, out var idMensagem))
            {
                m_Logger.LogError("Resposta de notificação com CorrelationId inválido/ausente: '{CorrelationId}'. Descartando.",
                    ea.BasicProperties.CorrelationId);
                await canal.BasicAckAsync(ea.DeliveryTag, multiple: false);
                return;
            }

            NotificacaoResponse? response;
            try
            {
                response = JsonSerializer.Deserialize<NotificacaoResponse>(ea.Body.Span);
            }
            catch (JsonException ex)
            {
                m_Logger.LogError(ex, "Falha ao desserializar resposta de notificação Id={Id}. Descartando.", idMensagem);
                await canal.BasicAckAsync(ea.DeliveryTag, multiple: false);
                return;
            }

            if (response is null)
            {
                m_Logger.LogError("Resposta de notificação vazia para Id={Id}. Descartando.", idMensagem);
                await canal.BasicAckAsync(ea.DeliveryTag, multiple: false);
                return;
            }

            using var escopo = m_ScopeFactory.CreateScope();
            var contexto = escopo.ServiceProvider.GetRequiredService<IContexto>();
            var mensagens = escopo.ServiceProvider.GetRequiredService<IMensagens>();

            try
            {
                contexto.IniciarTransacao();

                var mensagem = mensagens.Obter(idMensagem);
                if (mensagem is null)
                {
                    m_Logger.LogWarning("Resposta de notificação para Id={Id}, mas a mensagem não existe mais. Descartando.", idMensagem);
                    contexto.CancelarTransacao();
                    await canal.BasicAckAsync(ea.DeliveryTag, multiple: false);
                    return;
                }

                if (response.Sucesso)
                {
                    mensagem.RegistrarEnvio();
                }
                else
                {
                    mensagem.RegistrarErroEnvio(new StringClob(response.MensagemErro ?? "Falha ao enviar notificação."));
                }

                mensagens.Atualizar(mensagem);
                contexto.SalvarTransacao();

                await canal.BasicAckAsync(ea.DeliveryTag, multiple: false);
            }
            catch (Exception ex)
            {
                m_Logger.LogError(ex, "Falha transitória ao processar resposta de notificação Id={Id}. Reenfileirando.", idMensagem);
                contexto.CancelarTransacao();
                await canal.BasicNackAsync(ea.DeliveryTag, multiple: false, requeue: true);
            }
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            await base.StopAsync(cancellationToken);

            if (m_Canal is not null)
            {
                await m_Canal.CloseAsync(cancellationToken);
                m_Canal.Dispose();
            }

            if (m_Conexao is not null)
            {
                await m_Conexao.CloseAsync(cancellationToken);
                m_Conexao.Dispose();
            }
        }
    }
}
