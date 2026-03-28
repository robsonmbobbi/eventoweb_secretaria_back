using FastReport;
using FastReport.Export.PdfSimple;
using FastReport.Utils;
using System.Collections.ObjectModel;
using System.Drawing;
using EventoWeb.Secretaria.Negocio.Entidades.Atividades;
using EventoWeb.Secretaria.Relatorios.Aplicacao.Interfaces;
using EventoWeb.Secretaria.Relatorios.Relatorios.Modelos;

namespace EventoWeb.Secretaria.Relatorios.Relatorios.Implementacoes;

/// <summary>
/// Implementação do gerador de relatório para divisões de atividades usando FastReport.
/// Geração totalmente programaticamente através da API do FastReport.
/// </summary>
public class DivisaoAtividadeRelatorio : IRelatorioGerador<Atividade>
{
    private const float MARGIN = 10f;
    private const float CM = 37.79f; // pixels por cm

    /// <summary>
    /// Gera um PDF com as divisões de uma atividade.
    /// Cada divisão é exibida em uma página separada.
    /// </summary>
    /// <param name="atividade">Atividade contendo as divisões.</param>
    /// <param name="detalhar">Se true, inclui ID inscrição, cidade e UF.</param>
    /// <returns>Array de bytes contendo o PDF.</returns>
    public async Task<byte[]> GerarPdfAsync(Atividade atividade, bool detalhar)
    {
        return await Task.Run(() => GerarPdf(atividade, detalhar));
    }

    private byte[] GerarPdf(Atividade atividade, bool detalhar)
    {
        try
        {
            // Preparar dados
            var dados = PrepararDados(atividade, detalhar);

            // Criar o relatório
            using var report = new Report();

            // Registrar dados no relatório
            report.RegisterData(dados, "Divisoes");

            // Construir o design do relatório programaticamente
            ConstruirDesignProgramaticamente(report, detalhar);

            // Preparar o relatório para renderização
            if (!report.Prepare())
            {
                throw new InvalidOperationException("Falha ao preparar o relatório.");
            }

            // Exportar para PDF
            using var pdfStream = new MemoryStream();
            var pdfExport = new PDFSimpleExport();
            report.Export(pdfExport, pdfStream);

            return pdfStream.ToArray();
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("Erro ao gerar relatório em PDF.", ex);
        }
    }

    private Collection<ModeloDivisaoAtividade> PrepararDados(Atividade atividade, bool detalhar)
    {
        var divisoes = new Collection<ModeloDivisaoAtividade>();

        foreach (var divisao in atividade.Divisoes)
        {
            var modeloDivisao = new ModeloDivisaoAtividade
            {
                NomeDivisao = divisao.Nome
            };

            // Separar coordenadores de participantes
            foreach (var participante in divisao.Participantes)
            {
                var pessoa = participante.Inscricao.Pessoa;

                var modeloParticipante = new ModeloParticipante
                {
                    IdInscricao = participante.Inscricao.Id,
                    Nome = pessoa.Nome.ToString(),
                    Cidade = detalhar ? pessoa.Cidade : null,
                    UF = detalhar ? pessoa.UF : null,
                    EhCoordenador = participante.EhCoordenador
                };

                if (participante.EhCoordenador)
                {
                    modeloDivisao.Coordenadores.Add(modeloParticipante);
                }
                else
                {
                    modeloDivisao.Participantes.Add(modeloParticipante);
                }
            }

            divisoes.Add(modeloDivisao);
        }

        return divisoes;
    }

    private void ConstruirDesignProgramaticamente(Report report, bool detalhar)
    {
        // Criar página do relatório
        var page = new ReportPage();
        page.CreateUniqueName();
        report.Pages.Add(page);

        // Criar DataBand principal que itera sobre as divisões
        var dataBand = new DataBand();
        dataBand.CreateUniqueName();
        dataBand.DataSource = report.GetDataSource("Divisoes");
        dataBand.Height = CM * 16; // altura aproximada para múltiplas linhas
        dataBand.PageBreak = true; // quebra de página após cada divisão

        // Variável para controlar a posição vertical dentro da banda
        float posY = 0;

        // ===== TÍTULO DA DIVISÃO =====
        var titleDivisao = new TextObject
        {
            Text = "[Divisoes.NomeDivisao]",
            Left = MARGIN,
            Top = posY,
            Width = CM * 17, // A4 é 210mm, menos margens
            Height = CM * 0.8f
        };
        titleDivisao.CreateUniqueName();
        titleDivisao.Font = new Font("Arial", 14, FontStyle.Bold);
        dataBand.Objects.Add(titleDivisao);

        posY += CM * 1;

        // ===== TÍTULO COORDENADORES =====
        var coordHeader = new TextObject
        {
            Text = "Coordenadores:",
            Left = MARGIN,
            Top = posY,
            Width = CM * 17,
            Height = CM * 0.5f
        };
        coordHeader.CreateUniqueName();
        coordHeader.Font = new Font("Arial", 11, FontStyle.Bold);
        dataBand.Objects.Add(coordHeader);

        posY += CM * 0.6f;

        // ===== DATABND COORDENADORES =====
        var coordBand = new DataBand();
        coordBand.CreateUniqueName();
        coordBand.DataSource = report.GetDataSource("Divisoes.Coordenadores");
        coordBand.Height = CM * 0.4f;
        coordBand.Left = MARGIN;
        coordBand.Top = posY;
        coordBand.Width = CM * 17;
        coordBand.PageBreak = false;

        var coordText = new TextObject
        {
            Text = detalhar
                ? "[Divisoes.Coordenadores.Nome] (Id: [Divisoes.Coordenadores.IdInscricao], [Divisoes.Coordenadores.Cidade]/[Divisoes.Coordenadores.UF])"
                : "[Divisoes.Coordenadores.Nome]",
            Left = CM * 0.5f,
            Top = 0,
            Width = CM * 16.5f,
            Height = CM * 0.4f
        };
        coordText.CreateUniqueName();
        coordText.Font = new Font("Arial", 10);
        coordBand.Objects.Add(coordText);

        dataBand.Objects.Add(coordBand);

        posY += CM * 2; // espaço reservado para a databnd (aproximado)

        // ===== ESPAÇO =====
        posY += CM * 0.5f;

        // ===== TÍTULO PARTICIPANTES =====
        var partHeader = new TextObject
        {
            Text = "Participantes:",
            Left = MARGIN,
            Top = posY,
            Width = CM * 17,
            Height = CM * 0.5f
        };
        partHeader.CreateUniqueName();
        partHeader.Font = new Font("Arial", 11, FontStyle.Bold);
        dataBand.Objects.Add(partHeader);

        posY += CM * 0.6f;

        // ===== DATABND PARTICIPANTES =====
        var partBand = new DataBand();
        partBand.CreateUniqueName();
        partBand.DataSource = report.GetDataSource("Divisoes.Participantes");
        partBand.Height = CM * 0.4f;
        partBand.Left = MARGIN;
        partBand.Top = posY;
        partBand.Width = CM * 17;
        partBand.PageBreak = false;

        var partText = new TextObject
        {
            Text = detalhar
                ? "[Divisoes.Participantes.Nome] (Id: [Divisoes.Participantes.IdInscricao], [Divisoes.Participantes.Cidade]/[Divisoes.Participantes.UF])"
                : "[Divisoes.Participantes.Nome]",
            Left = CM * 0.5f,
            Top = 0,
            Width = CM * 16.5f,
            Height = CM * 0.4f
        };
        partText.CreateUniqueName();
        partText.Font = new Font("Arial", 10);
        partBand.Objects.Add(partText);

        dataBand.Objects.Add(partBand);

        // Adicionar a DataBand principal à página
        page.Bands.Add(dataBand);
    }
}
