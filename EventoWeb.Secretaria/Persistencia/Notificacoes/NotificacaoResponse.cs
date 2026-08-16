namespace EventoWeb.Secretaria.Persistencia.Notificacoes
{
    internal sealed class NotificacaoResponse
    {
        public Guid Id { get; set; }
        public bool Sucesso { get; set; }
        public string? MensagemErro { get; set; }
    }
}
