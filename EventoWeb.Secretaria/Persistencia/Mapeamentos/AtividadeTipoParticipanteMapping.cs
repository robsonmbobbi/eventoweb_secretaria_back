using EventoWeb.Comum.Negocio.Entidades;
using EventoWeb.Comum.Persistencia.Mapeamentos;
using EventoWeb.Secretaria.Negocio.Entidades.Atividades;
using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;

namespace EventoWeb.Secretaria.Persistencia.Mapeamentos
{
    public class AtividadeTipoParticipanteMapping : ClassMapping<AtividadeTipoParticipante>
    {
        public AtividadeTipoParticipanteMapping()
        {
            Table("atividade_tipo_participantes");
            Lazy(false);

            Id(x => x.Id, m =>
            {
                m.Access(Accessor.NoSetter);
                m.Column("id");
                m.Generator(Generators.Native, g =>
                {
                    g.Params(new { sequence = "gen_atividade_tipo_participante" });
                });
            });

            ManyToOne(x => x.Atividade, m =>
            {
                m.Access(Accessor.Property);
                m.Column("id_atividade");
                m.NotNullable(true);
            });

            Property(x => x.TipoParticipante, m =>
            {
                m.Access(Accessor.Property);
                m.Column("tipo_participante");
                m.NotNullable(true);
                m.Type<EnumGeneric<EnumTipoParticipante>>();
            });
        }
    }
}
