using FastReport;
using FastReport.Export.PdfSimple;
using FastReport.Utils;
using System.Collections.ObjectModel;
using System.Drawing;
using EventoWeb.Secretaria.Negocio.Entidades.Quartos;
using EventoWeb.Secretaria.Relatorios.Aplicacao.Interfaces;
using EventoWeb.Secretaria.Relatorios.Relatorios.Modelos;

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
            report.RegisterData(dados, "Quartos");

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
        dataBand.PageBreak = true; // quebra de página após cada quarto

        // Variável para controlar a posição vertical dentro da banda
        float posY = 0;

        // ===== TÍTULO DO QUARTO =====
        var titleQuarto = new TextObject
        {
            Text = "[Quartos.NomeQuarto]",
            Left = MARGIN,
            Top = posY,
            Width = CM * 17, // A4 é 210mm, menos margens
            Height = CM * 0.8f
        };
        titleQuarto.CreateUniqueName();
        titleQuarto.Font = new Font("Arial", 14, FontStyle.Bold);
        dataBand.Objects.Add(titleQuarto);

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
        coordBand.DataSource = report.GetDataSource("Quartos.Coordenadores");
        coordBand.Height = CM * 0.4f;
        coordBand.Left = MARGIN;
        coordBand.Top = posY;
        coordBand.Width = CM * 17;
        coordBand.PageBreak = false;

        var coordText = new TextObject
        {
            Text = detalhar
                ? "[Quartos.Coordenadores.Nome] (Id: [Quartos.Coordenadores.IdInscricao], [Quartos.Coordenadores.Cidade]/[Quartos.Coordenadores.UF])"
                : "[Quartos.Coordenadores.Nome]",
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
        partBand.DataSource = report.GetDataSource("Quartos.Participantes");
        partBand.Height = CM * 0.4f;
        partBand.Left = MARGIN;
        partBand.Top = posY;
        partBand.Width = CM * 17;
        partBand.PageBreak = false;

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
        partText.Font = new Font("Arial", 10);
        partBand.Objects.Add(partText);

        dataBand.Objects.Add(partBand);

        // Adicionar DataBand à página
        page.Bands.Add(dataBand);
    }
}
