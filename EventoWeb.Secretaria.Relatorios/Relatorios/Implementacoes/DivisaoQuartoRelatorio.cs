using EventoWeb.Secretaria.Negocio.Entidades.Quartos;
using EventoWeb.Secretaria.Relatorios.Aplicacao.Interfaces;
using EventoWeb.Secretaria.Relatorios.Relatorios.Modelos;
using FastReport;
using FastReport.Data;
using FastReport.Export.PdfSimple;
using FastReport.Utils;
using System.Collections.ObjectModel;
using System.Drawing;

namespace EventoWeb.Secretaria.Relatorios.Relatorios.Implementacoes;

/// <summary>
/// Implementação do gerador de relatório para quartos usando FastReport.
/// Geração totalmente programaticamente através da API do FastReport.
/// </summary>
public class DivisaoQuartoRelatorio : IRelatorioGerador<IList<Quarto>>
{
    private const float MARGIN = 10f;
    private const float CM = 37.79f; // pixels por cm

    /// <summary>
    /// Gera um PDF com a listagem de participantes por quarto.
    /// Cada quarto é exibido em uma página separada.
    /// </summary>
    /// <param name="quartos">Lista de quartos contendo os inscritos.</param>
    /// <param name="detalhar">Se true, inclui ID inscrição, cidade e UF.</param>
    /// <returns>Array de bytes contendo o PDF.</returns>
    public async Task<byte[]> GerarPdfAsync(IList<Quarto> quartos, bool detalhar)
    {
        return await Task.Run(() => GerarPdf(quartos, detalhar));
    }

    private byte[] GerarPdf(IList<Quarto> quartos, bool detalhar)
    {
        try
        {
            // Preparar dados
            var dados = PrepararDados(quartos, detalhar);

            // Criar o relatório
            using var report = new Report();

            // Registrar dados no relatório
            report.Dictionary.RegisterBusinessObject(dados, "Quartos", 10, true);

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

    private Collection<ModeloQuartoRelatorio> PrepararDados(IList<Quarto> quartos, bool detalhar)
    {
        var modelosQuartos = new Collection<ModeloQuartoRelatorio>();

        foreach (var quarto in quartos)
        {
            var modeloQuarto = new ModeloQuartoRelatorio
            {
                NomeQuarto = quarto.Nome
            };

            // Separar coordenadores de participantes através da propriedade EhCoordenador
            foreach (var quartoInscrito in quarto.Inscritos)
            {
                var pessoa = quartoInscrito.Inscricao.Pessoa;

                var modeloParticipante = new ModeloParticipante
                {
                    IdInscricao = quartoInscrito.Inscricao.Id,
                    Nome = pessoa.Nome.ToString(),
                    Cidade = detalhar ? pessoa.Cidade : null,
                    UF = detalhar ? pessoa.UF : null,
                    EhCoordenador = quartoInscrito.EhCoordenador
                };

                if (quartoInscrito.EhCoordenador)
                {
                    modeloQuarto.Coordenadores.Add(modeloParticipante);
                }
                else
                {
                    modeloQuarto.Participantes.Add(modeloParticipante);
                }
            }

            modelosQuartos.Add(modeloQuarto);
        }

        return modelosQuartos;
    }

    private void ConstruirDesignProgramaticamente(Report report, bool detalhar)
    {
        // Criar página do relatório
        var page = new ReportPage();
        page.CreateUniqueName();
        report.Pages.Add(page);

        // Criar DataBand principal que itera sobre os quartos
        var dataBand = new DataBand();
        dataBand.CreateUniqueName();
        dataBand.DataSource = report.GetDataSource("Quartos");
        dataBand.Height = CM * 16; // altura aproximada para múltiplas linhas
        dataBand.StartNewPage = true; // quebra de página após cada quarto
        dataBand.Parent = page;

        // ===== TÍTULO DA DIVISÃO =====
        var titleDivisao = new TextObject
        {
            Text = "[Quartos.NomeQuarto]",
            Left = MARGIN,
            Top = 0f,
            Width = CM * 17, // A4 é 210mm, menos margens
            Height = CM * 0.8f
        };
        titleDivisao.CreateUniqueName();
        titleDivisao.Font = new Font("Arial", 16, FontStyle.Bold);
        dataBand.Objects.Add(titleDivisao);

        // ===== DATABND COORDENADORES =====
        var coordBand = new DataBand();
        coordBand.CreateUniqueName();
        coordBand.DataSource = report.GetDataSource("Quartos.Coordenadores");
        coordBand.Height = CM * 0.7f;
        coordBand.Left = MARGIN;
        coordBand.Width = CM * 17;
        coordBand.PageBreak = false;
        coordBand.Parent = dataBand;

        var coordHeaderBand = new DataHeaderBand();
        coordHeaderBand.Height = CM * 1;
        coordHeaderBand.CreateUniqueName();
        coordHeaderBand.Parent = coordBand;

        // ===== TÍTULO COORDENADORES =====
        var coordHeader = new TextObject
        {
            Text = "Coordenadores:",
            Left = MARGIN,
            Top = CM * 0.25f,
            Width = CM * 17,
            Height = CM * 0.5f
        };
        coordHeader.CreateUniqueName();
        coordHeader.Font = new Font("Arial", 13, FontStyle.Bold);
        coordHeader.Parent = coordHeaderBand;

        var coordText = new TextObject
        {
            Text = detalhar
               ? "[Divisoes.Coordenadores.Nome] (Id: [Quartos.Coordenadores.IdInscricao], [Quartos.Coordenadores.Cidade]/[Quartos.Coordenadores.UF])"
               : "[Divisoes.Coordenadores.Nome]",
            Left = CM * 0.5f,
            Top = 0,
            Width = CM * 16.5f,
            Height = CM * 0.4f
        };
        coordText.CreateUniqueName();
        coordText.Font = new Font("Arial", 12);
        coordText.Parent = coordBand;


        var partBand = new DataBand();
        partBand.CreateUniqueName();
        partBand.DataSource = report.GetDataSource("Quartos.Participantes");
        partBand.Height = CM * 0.7f;
        partBand.Left = MARGIN;
        partBand.Width = CM * 17;
        partBand.PageBreak = false;
        partBand.Parent = dataBand;

        var partHeaderBand = new DataHeaderBand();
        partHeaderBand.Height = CM * 1;
        partHeaderBand.CreateUniqueName();
        partHeaderBand.Parent = partBand;

        var partHeader = new TextObject
        {
            Text = "Participantes:",
            Left = MARGIN,
            Top = CM * 0.25f,
            Width = CM * 17,
            Height = CM * 0.5f,
        };
        partHeader.CreateUniqueName();
        partHeader.Font = new Font("Arial", 13, FontStyle.Bold);
        partHeader.Parent = partHeaderBand;

        var partText = new TextObject
        {
            Text = detalhar
                ? "[Quartos.Participantes.Nome] (Id: [Quartos.Participantes.IdInscricao], [Quartos.Participantes.Cidade]/[Quartos.Participantes.UF])"
                : "[Quartos.Participantes.Nome]",
            Left = CM * 0.5f,
            Top = 0,
            Width = CM * 16.5f,
            Height = CM * 0.4f
        };
        partText.CreateUniqueName();
        partText.Font = new Font("Arial", 12);
        partText.Parent = partBand;

        ///

        var partFooterBand = new DataFooterBand();
        partFooterBand.Height = CM * 1;
        partFooterBand.CreateUniqueName();
        partFooterBand.Parent = partBand;

        var partFooter = new TextObject
        {
            Text = "Total Participantes: [TotalRegistrosParticipantes]",
            Left = MARGIN,
            Top = CM * 0.25f,
            Width = CM * 17,
            Height = CM * 0.5f,
        };
        partFooter.CreateUniqueName();
        partFooter.Font = new Font("Arial", 13, FontStyle.Bold);
        partFooter.Parent = partFooterBand;

        var reportTotal = new Total();
        reportTotal.Name = "TotalRegistrosParticipantes";
        reportTotal.TotalType = TotalType.Count;
        reportTotal.Expression = "[Quartos.Participantes]";
        reportTotal.Evaluator = partBand;
        reportTotal.PrintOn = partHeaderBand;

        report.Dictionary.Totals.Add(reportTotal);
    }
}
