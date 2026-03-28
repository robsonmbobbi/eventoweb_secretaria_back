namespace EventoWeb.Secretaria.Aplicacao.Atividades;

public class DTOParticipanteDivisao
{
    public int Id { get; set; }
    public required string Nome { get; set; }
    public DateTime DataNascimento { get; set; }
    public string? Cidade { get; set; }
    public string? UF { get; set; }
    public bool EhCoordenador { get; set; }
}
