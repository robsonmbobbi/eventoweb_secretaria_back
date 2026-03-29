using EventoWeb.Comum.Negocio.Entidades;
using EventoWeb.Comum.Negocio.ObjetosValor;
using EventoWeb.Secretaria.Negocio.Entidades.Atividades;
using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;

namespace EventoWeb.Secretaria.Persistencia.Mapeamentos
{
    public class DivisaoAtividadeMapping : ClassMapping<DivisaoAtividade>
    {
        public DivisaoAtividadeMapping()
        {
            Table("divisoes_atividades");
            Lazy(false);

            Id(x => x.Id, m =>
            {
                m.Access(Accessor.NoSetter);
                m.Column("id");
                m.Generator(Generators.Native, g =>
                {
                    g.Params(new { sequence = "gen_divisao_atividade" });
                });
            });

            ManyToOne(x => x.Atividade, m =>
            {
                m.Access(Accessor.NoSetter);
                m.Column("id_atividade");
                m.NotNullable(true);
            });

            Property(x => x.Nome, m =>
            {
                m.Access(Accessor.Property);
                m.Column("nome");
                m.Length(200);
                m.NotNullable(true);
            });

            Component(x => x.FaixaEtaria, c =>
            {
                c.Access(Accessor.NoSetter);
                c.Property(y => y.IdadeMin, m =>
                {
                    m.Access(Accessor.NoSetter);
                    m.Column("faixa_etaria_min");
                    m.NotNullable(false);
                });
                c.Property(y => y.IdadeMax, m =>
                {
                    m.Access(Accessor.NoSetter);
                    m.Column("faixa_etaria_max");
                    m.NotNullable(false);
                });
            });

            Property(x => x.DeveSerParNumeroTotalParticipantes, m =>
            {
                m.Access(Accessor.Property);
                m.Column("deve_ser_par_numero_total_participantes");
                m.NotNullable(true);
            });

            Property(x => x.NumeroTotalParticipantes, m =>
            {
                m.Access(Accessor.Property);
                m.Column("numero_total_participantes");
                m.NotNullable(false);
            });

            Bag(x => x.Participantes, m =>
            {
                m.Access(Accessor.NoSetter);
                m.Cascade(Cascade.All | Cascade.DeleteOrphans);
                m.Key(k =>
                {
                    k.Column("id_divisao_atividade");
                    k.ForeignKey("fk_divisoes_atividades_1");
                });
            },
            r => r.OneToMany(ot => ot.Class(typeof(DivisaoAtividadeParticipante))));
        }
    }
}
