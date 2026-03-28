using EventoWeb.Secretaria.Negocio.Entidades.Quartos;
using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;

namespace EventoWeb.Secretaria.Persistencia.Mapeamentos
{
    public class QuartoInscritoMapping: ClassMapping<QuartoInscrito>
    {
        public QuartoInscritoMapping()
        {
            this.Table("quartos_inscritos");
            this.Id(x => x.Id, m =>
            {
                m.Access(Accessor.NoSetter);
                m.Column("id");
                m.Generator(Generators.Native, g =>
                {
                    g.Params(new { sequence = "gen_quarto_inscrito" });
                });
            });

            this.Property(x => x.EhCoordenador, m =>
              {
                  m.Access(Accessor.Property);
                  m.NotNullable(true);
                  m.Column("eh_coordenador");
              });

            this.ManyToOne(x => x.Inscricao, m =>
            {
                m.Access(Accessor.NoSetter);
                m.NotNullable(true);
                m.Column("id_inscricao");
            });

            this.ManyToOne(x => x.Quarto, m =>
            {
                m.Access(Accessor.NoSetter);
                m.NotNullable(true);
                m.Column("id_quarto");
            });
        }
    }
}
