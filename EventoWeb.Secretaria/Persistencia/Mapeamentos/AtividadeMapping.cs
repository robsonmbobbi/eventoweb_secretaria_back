using EventoWeb.Comum.Negocio.Entidades;
using EventoWeb.Comum.Negocio.ObjetosValor;
using EventoWeb.Comum.Persistencia.Mapeamentos;
using EventoWeb.Secretaria.Negocio.Entidades.Atividades;
using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;

namespace EventoWeb.Secretaria.Persistencia.Mapeamentos
{
    public class AtividadeMapping : ClassMapping<Atividade>
    {
        public AtividadeMapping()
        {
            Table("atividades");
            Lazy(false);

            Id(x => x.Id, m =>
            {
                m.Access(Accessor.NoSetter);
                m.Column("id");
                m.Generator(Generators.Native, g =>
                {
                    g.Params(new { sequence = "gen_atividade" });
                });
            });

            ManyToOne(x => x.Evento, m =>
            {
                m.Access(Accessor.Property);
                m.Column("id_evento");
                m.NotNullable(true);
            });

            Property(x => x.TipoInscricao, m =>
            {
                m.Access(Accessor.Property);
                m.Column("tipo_insc");
                m.NotNullable(true);
                m.Type<EnumGeneric<EnumTipoInscricao>>();
            });

            Component(x => x.Nome, c =>
            {
                c.Access(Accessor.Property);
                c.Property(y => y.Nome, m =>
                {
                    m.Access(Accessor.Property);
                    m.Column("nome");
                    m.Length(200);
                    m.NotNullable(true);
                });
            });

            Bag(x => x.TipoParticipantes, m =>
            {
                m.Access(Accessor.NoSetter);
                m.Cascade(Cascade.All | Cascade.DeleteOrphans);
                m.Key(k =>
                {
                    k.Column("id_atividade");
                    k.ForeignKey("fk_atividades_1");
                });
            },
            r => r.OneToMany(ot => ot.Class(typeof(AtividadeTipoParticipante))));

            Bag(x => x.Divisoes, m =>
            {
                m.Access(Accessor.NoSetter);
                m.Cascade(Cascade.All | Cascade.DeleteOrphans);
                m.Key(k =>
                {
                    k.Column("id_atividade");
                    k.ForeignKey("fk_atividades_2");
                });
            },
            r => r.OneToMany(ot => ot.Class(typeof(DivisaoAtividade))));
        }
    }
}
