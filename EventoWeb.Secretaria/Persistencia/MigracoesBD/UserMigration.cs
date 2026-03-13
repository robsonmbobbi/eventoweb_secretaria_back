using FluentMigrator;

namespace EventoWeb.Secretaria.Persistencia.MigracoesBD
{
    [Migration(202603122126)]
    public class UserMigration : Migration
    {
        public override void Down()
        {
            Delete
                .Table("usuarios");
        }

        public override void Up()
        {
            Create.Table("usuarios")
                .WithColumn("login").AsString(150).PrimaryKey()
                .WithColumn("nome").AsString(100).NotNullable()
                .WithColumn("senha").AsString(32).NotNullable()
                .WithColumn("eh_administrador").AsBoolean().NotNullable();

            Insert.IntoTable("usuarios").Row(
                new
                {
                    login = "admin",
                    nome = "Administrador",
                    senha = "b92dacdf9555522caaf9d1900d1b546d", // md5 = admin_492!@P
                    eh_administrador = 1
                });
        }
    }
}
