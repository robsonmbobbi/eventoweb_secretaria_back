using EventoWeb.Comum.Negocio.Entidades.Financeiro;
using EventoWeb.Comum.Negocio.Repositorios;
using EventoWeb.Secretaria.Negocio.Entidades.Atividades;

namespace EventoWeb.Secretaria.Negocio.Repositorios;

public interface IAtividades : IPersistencia<Atividade>
{
    IList<DivisaoAtividadeParticipante> ObterDivisaoParticipante(int idInscricao);
}

