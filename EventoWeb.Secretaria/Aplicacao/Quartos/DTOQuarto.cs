using EventoWeb.Comum.Negocio.Entidades;
using EventoWeb.Secretaria.Negocio.Entidades.Quartos;

namespace EventoWeb.Secretaria.Aplicacao.Quartos;

public class DTOInscritoQuarto
{
    public int Id { get; set; }
    public string Nome { get; set; }
    public string? Cidade { get; set; }
    public string? UF { get; set; }
}

public class DTOQuarto
{
    public int Id { get; set; }
    public required string Nome { get; set; }
    public int? Capacidade { get; set; }
    public bool EhFamilia { get; set; }
    public EnumSexoQuarto Sexo { get; set; }
    public required IList<DTOInscritoQuarto> Inscritos { get; set; }
}
