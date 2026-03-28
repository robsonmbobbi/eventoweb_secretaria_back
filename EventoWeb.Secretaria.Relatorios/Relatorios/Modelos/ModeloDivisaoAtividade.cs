namespace EventoWeb.Secretaria.Relatorios.Relatorios.Modelos;

/// <summary>
/// Modelo de dados para o participante no relatório de divisão.
/// </summary>
public class ModeloParticipante
{
    public long IdInscricao { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string? Cidade { get; set; }
    public string? UF { get; set; }
    public bool EhCoordenador { get; set; }
}

/// <summary>
/// Modelo de dados para a divisão no relatório.
/// </summary>
public class ModeloDivisaoAtividade
{
    public string NomeDivisao { get; set; } = string.Empty;
    public List<ModeloParticipante> Coordenadores { get; set; } = new();
    public List<ModeloParticipante> Participantes { get; set; } = new();
}
