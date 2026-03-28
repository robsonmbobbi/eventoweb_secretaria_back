using System.Data;
using FluentMigrator;

namespace EventoWeb.Secretaria.Persistencia.MigracoesBD
{
    [Migration(202603281712)]
    public class AtividadesMigration : Migration
    {
        public override void Down()
        {
            Delete.Table("divisoes_atividades_participantes");
            Delete.Table("divisoes_atividades");
            Delete.Table("atividade_tipo_participantes");
            Delete.Table("atividades");
        }

        public override void Up()
        {
            CriarTabelaAtividades();
            CriarTabelaAtividadeTipoParticipantes();
            CriarTabelaDivisoesAtividades();
            CriarTabelaDivisoesAtividadesParticipantes();
        }

        private void CriarTabelaAtividades()
        {
            Create.Table("atividades")
                .WithColumn("id").AsInt32().PrimaryKey().Identity()
                .WithColumn("id_evento").AsInt32().NotNullable()
                    .ForeignKey("fk_eventos_1", "eventos", "id").OnUpdate(Rule.Cascade)
                .WithColumn("tipo_insc").AsInt16().NotNullable()
                .WithColumn("nome").AsString(200).NotNullable();
        }

        private void CriarTabelaAtividadeTipoParticipantes()
        {
            Create.Table("atividade_tipo_participantes")
                .WithColumn("id").AsInt32().PrimaryKey().Identity()
                .WithColumn("id_atividade").AsInt32().NotNullable()
                    .ForeignKey("fk_atividades_1", "atividades", "id").OnUpdate(Rule.Cascade).OnDelete(Rule.Cascade)
                .WithColumn("tipo_participante").AsInt16().NotNullable();
        }

        private void CriarTabelaDivisoesAtividades()
        {
            Create.Table("divisoes_atividades")
                .WithColumn("id").AsInt32().PrimaryKey().Identity()
                .WithColumn("id_atividade").AsInt32().NotNullable()
                    .ForeignKey("fk_atividades_2", "atividades", "id").OnUpdate(Rule.Cascade).OnDelete(Rule.Cascade)
                .WithColumn("nome").AsString(200).NotNullable()
                .WithColumn("faixa_etaria_min").AsInt32().Nullable()
                .WithColumn("faixa_etaria_max").AsInt32().Nullable()
                .WithColumn("deve_ser_par_numero_total_participantes").AsBoolean().NotNullable()
                .WithColumn("numero_total_participantes").AsInt32().Nullable();
        }

        private void CriarTabelaDivisoesAtividadesParticipantes()
        {
            Create.Table("divisoes_atividades_participantes")
                .WithColumn("id").AsInt32().PrimaryKey().Identity()
                .WithColumn("id_divisao_atividade").AsInt32().NotNullable()
                    .ForeignKey("fk_divisoes_atividades_1", "divisoes_atividades", "id").OnUpdate(Rule.Cascade).OnDelete(Rule.Cascade)
                .WithColumn("id_inscricao").AsInt32().NotNullable()
                    .ForeignKey("fk_inscricoes_1", "inscricoes", "id").OnUpdate(Rule.Cascade)
                .WithColumn("eh_coordenador").AsBoolean().NotNullable();
        }
    }
}
