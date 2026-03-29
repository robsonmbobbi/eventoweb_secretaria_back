using System.Data;
using FluentMigrator;

namespace EventoWeb.Secretaria.Persistencia.MigracoesBD
{
    [Migration(202603281713)]
    public class QuartosMigration : Migration
    {
        public override void Down()
        {
            Delete.Table("quartos_inscritos");
            Delete.Table("quartos");
        }

        public override void Up()
        {
            CriarTabelaQuartos();
            CriarTabelaQuartosInscritos();
        }

        private void CriarTabelaQuartos()
        {
            Create.Table("quartos")
                .WithColumn("id").AsInt32().PrimaryKey().Identity()
                .WithColumn("id_evento").AsInt32().NotNullable()
                    .ForeignKey("fk_quartos_1", "eventos", "id").OnUpdate(Rule.Cascade)
                .WithColumn("nome").AsString(100).NotNullable()
                .WithColumn("capacidade").AsInt32().Nullable()
                .WithColumn("eh_familia").AsBoolean().NotNullable()
                .WithColumn("sexo").AsInt16().NotNullable();
        }

        private void CriarTabelaQuartosInscritos()
        {
            Create.Table("quartos_inscritos")
                .WithColumn("id").AsInt32().PrimaryKey().Identity()
                .WithColumn("id_quarto").AsInt32().NotNullable()
                    .ForeignKey("fk_quartos_inscritos_1", "quartos", "id").OnUpdate(Rule.Cascade).OnDelete(Rule.Cascade)
                .WithColumn("id_inscricao").AsInt32().NotNullable()
                    .ForeignKey("fk_quartos_inscritos_2", "inscricoes", "id").OnUpdate(Rule.Cascade)
                .WithColumn("eh_coordenador").AsBoolean().NotNullable();
        }
    }
}
