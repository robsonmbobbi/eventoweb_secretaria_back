using EventoWeb.Comum.Aplicacao.Inscricoes;
using EventoWeb.Comum.Negocio.Entidades;

namespace EventoWeb.Secretaria.Aplicacao.Inscricoes;

public class DTOInscricaoListagem
{
    public int? Id { get; set; }
    public string Nome { get; set; }
    public EnumTipoInscricao Tipo { get; set; }
    public EnumSituacaoInscricao Situacao { get; set; }
    public string? Cidade { get; set; }
    public string? UF { get; set; }
    public bool Dormira { get; set; }
    public EnumTipoParticipante? TipoParticipante { get; set; }
}
