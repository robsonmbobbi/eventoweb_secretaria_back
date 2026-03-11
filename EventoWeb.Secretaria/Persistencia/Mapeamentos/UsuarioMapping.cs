using EventoWeb.Secretaria.Negocio.Entidades.Seguranca;
using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;

namespace EventoWeb.Secretaria.Persistencia.Mapeamentos
{
    public class UsuarioMapping : ClassMapping<Usuario>
    {
        public UsuarioMapping()
        {
            this.Table("usuarios");
            this.Id(x => x.Login, m =>
            {
                m.Access(Accessor.Property);
                m.Column("LOGIN");
                m.Length(150);
                m.Generator(Generators.Assigned);
            });

            Property(x => x.Nome, m =>
            {
                m.Access(Accessor.NoSetter);
                m.Column("NOME");
                m.NotNullable(true);
                m.Length(100);
            });

            Component(x => x.Senha, c =>
            {
                c.Access(Accessor.NoSetter);
                c.Property("m_Senha", m =>
                {
                    m.Access(Accessor.Field);
                    m.Column("SENHA");
                    m.NotNullable(true);
                    m.Length(32);
                });
            });

            Property(x => x.EhAdministrador, m =>
            {
                m.Access(Accessor.Property);
                m.Column("EH_ADMINISTRADOR");
                m.NotNullable(true);
            });
        }
    }
}
