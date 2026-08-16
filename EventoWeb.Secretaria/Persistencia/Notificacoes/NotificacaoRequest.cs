using EventoWeb.Comum.Negocio.Entidades.Notificacoes;

namespace EventoWeb.Secretaria.Persistencia.Notificacoes
{
    internal sealed class NotificacaoRequest
    {
        public Guid Id { get; set; }
        public EnumMeioNotificacao Meio { get; set; }
        public string Destinatario { get; set; } = "";
        public string? TemplateAssunto { get; set; }
        public string TemplateConteudo { get; set; } = "";
        public string? DadosJson { get; set; }
    }
}
