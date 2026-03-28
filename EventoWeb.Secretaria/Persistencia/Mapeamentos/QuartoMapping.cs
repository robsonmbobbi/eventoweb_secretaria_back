using EventoWeb.Comum.Negocio.Entidades;
using EventoWeb.Comum.Persistencia.Mapeamentos;
using EventoWeb.Secretaria.Negocio.Entidades.Quartos;
using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;

namespace EventoWeb.Secretaria.Persistencia.Mapeamentos
{
    public class QuartoMapping: ClassMapping<Quarto>
    {
        public QuartoMapping()
        {
            this.Table("quartos");
            this.Id(x => x.Id, m =>
            {
                m.Access(Accessor.NoSetter);
                m.Column("id");
                m.Generator(Generators.Native, g =>
                {
                    g.Params(new { sequence = "gen_quarto" });
                });
            });

            this.Property(x => x.Capacidade, m =>
              {
                  m.Access(Accessor.NoSetter);
                  m.NotNullable(false);
                  m.Column("capacidade");
              });

            this.Property(x => x.EhFamilia, m =>
            {
                m.Access(Accessor.Property);
                m.NotNullable(true);
                m.Column("eh_familia");
            });

            this.ManyToOne(x => x.Evento, m =>
            {
                m.Column("id_evento");
                m.NotNullable(false);
                m.Access(Accessor.NoSetter);
                m.Class(typeof(Evento));
            });

            this.Bag(x => x.Inscritos, m =>
            {
                m.Cascade(Cascade.All | Cascade.DeleteOrphans);
                m.Access(Accessor.NoSetter);
                m.Inverse(true);
                m.Lazy(CollectionLazy.Lazy);
                m.Key(k => k.Column("id_quarto"));
            }, c => c.OneToMany(a => a.Class(typeof(QuartoInscrito))));

            this.Property(x => x.Nome, m =>
            {
                m.Access(Accessor.NoSetter);
                m.NotNullable(true);
                m.Column("nome");
                m.Length(100);
            });

            this.Property(x => x.Sexo, m =>
            {
                m.Access(Accessor.Property);
                m.NotNullable(true);
                m.Column("sexo");
                m.Type<EnumGeneric<EnumSexoQuarto>>();
            });
        }
    }
}
