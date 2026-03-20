using EventoWeb.Comum.Aplicacao.Inscricoes;
using EventoWeb.Comum.Negocio.Entidades;

namespace EventoWeb.Secretaria.Aplicacao.Inscricoes;

public static class ConversaoInscricaoListagem
{
    public static DTOInscricaoListagem ConverterParaListagem(this Inscricao inscricao)
    {
        var tipoParticipante = (inscricao is InscricaoParticipante participante) ? participante.Tipo : null;

        return new DTOInscricaoListagem
        {
            Id = inscricao.Id,
            Nome = inscricao.Pessoa.Nome.Nome,
            Tipo = inscricao is InscricaoParticipante ? EnumTipoInscricao.Adulto : EnumTipoInscricao.Infantil,
            Situacao = inscricao.Situacao,
            Cidade = inscricao.Pessoa.Cidade,
            UF = inscricao.Pessoa.UF,
            Dormira = inscricao.DormeEvento,
            TipoParticipante = tipoParticipante
        };
    }
}
