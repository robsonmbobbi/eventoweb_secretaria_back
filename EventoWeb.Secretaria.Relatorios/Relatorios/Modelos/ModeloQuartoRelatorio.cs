namespace EventoWeb.Secretaria.Relatorios.Relatorios.Modelos;

/// <summary>
/// Modelo de dados para o quarto no relatório.
/// </summary>
public class ModeloQuartoRelatorio
{
    public string NomeQuarto { get; set; } = string.Empty;
    public List<ModeloParticipante> Coordenadores { get; set; } = new();
    public List<ModeloParticipante> Participantes { get; set; } = new();
}
