using EventoWeb.Comum.Negocio.Entidades;
using EventoWeb.Secretaria.Negocio.Entidades.Atividades;
using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;

namespace EventoWeb.Secretaria.Persistencia.Mapeamentos
{
    public class DivisaoAtividadeParticipanteMapping : ClassMapping<DivisaoAtividadeParticipante>
    {
        public DivisaoAtividadeParticipanteMapping()
        {
            Table("divisoes_atividades_participantes");
            Lazy(false);

            Id(x => x.Id, m =>
            {
                m.Access(Accessor.NoSetter);
                m.Column("id");
                m.Generator(Generators.Native, g =>
                {
                    g.Params(new { sequence = "gen_divisao_atividade_participante" });
                });
            });

            ManyToOne(x => x.Inscricao, m =>
            {
                m.Access(Accessor.Property);
                m.Column("id_inscricao");
                m.NotNullable(true);
            });

            Property(x => x.EhCoordenador, m =>
            {
                m.Access(Accessor.Property);
                m.Column("eh_coordenador");
                m.NotNullable(true);
            });
        }
    }
}
