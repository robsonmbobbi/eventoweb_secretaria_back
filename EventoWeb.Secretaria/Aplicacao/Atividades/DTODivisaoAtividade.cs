namespace EventoWeb.Secretaria.Aplicacao.Atividades;

public class DTODivisaoAtividade
{
    public int Id { get; set; }
    public required string Nome { get; set; }
    public required IList<DTOParticipanteDivisao> Participantes { get; set; }
}
