using EventoWeb.Comum.Negocio.Entidades;
using EventoWeb.Secretaria.Relatorios.Aplicacao.Interfaces;
using iText.IO.Font.Constants;
using iText.Kernel.Font;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas;
using iText.Layout;
using iText.Layout.Element;

namespace EventoWeb.Secretaria.Relatorios.Relatorios.Implementacoes
{
    public class RelatorioEtiquetaCaderno : IGeradorEtiqueta<IList<DadosCadernoInscrito>>
    {
        public byte[] GerarPdf(IList<DadosCadernoInscrito> inscritos)
        {
            using var stream = new MemoryStream();
            using var pdfWriter = new PdfWriter(stream);

            var documentoPDF = new PdfDocument(pdfWriter);
            var page = documentoPDF.AddNewPage(PageSize.LETTER);

            var pdfCanvas = new PdfCanvas(page);

            var tamanhoFonteNormal = 10;
            var tamanhoFonteTitulo = 11;
            var fonteBold = PdfFontFactory.CreateFont(StandardFonts.HELVETICA_BOLD);
            var fonteNormal = PdfFontFactory.CreateFont(StandardFonts.HELVETICA_OBLIQUE);             

            const float LarguraEtiqueta = 101.6f;
            const float AlturaEtiqueta = 25.4f;
            const float MargemSuperior = 0.4f;
            const int TotalLinhasPag = 10;
            var PosicaoYInicial = PageSize.LETTER.GetHeight() - 14.6f.MillimetersToPointsTextSharp();

            var posicaoY = 0f;
            var posicaoX = 0f;

            var linha = 1;
            var qualEtiqueta = 1;

            for (var indice = 1; indice <= inscritos.Count(); indice++)
            {
                var inscricao = inscritos[indice - 1];

                if (qualEtiqueta == 1)
                    posicaoX = 6.1f.MillimetersToPointsTextSharp();
                else
                    posicaoX = 112.8f.MillimetersToPointsTextSharp();

                posicaoY = PosicaoYInicial - (AlturaEtiqueta * linha + MargemSuperior).MillimetersToPointsTextSharp();

                var canvas = new Canvas(
                    page,
                    new Rectangle(
                        posicaoX, //x inicial 
                        posicaoY, // y inicial
                        LarguraEtiqueta.MillimetersToPointsTextSharp(), //x final
                        (AlturaEtiqueta - MargemSuperior).MillimetersToPointsTextSharp())
                    );

                var paragrafoNome = new Paragraph(inscricao.Nome)
                    .SetFont(fonteBold)
                    .SetFontSize(tamanhoFonteTitulo)
                    .SetTextAlignment(iText.Layout.Properties.TextAlignment.LEFT)
                    .SetMarginLeft(2)
                    .SetMarginRight(2);

                var paragrafoInformacoes = new Paragraph();
                paragrafoInformacoes
                    .SetFont(fonteNormal)
                    .SetFontSize(tamanhoFonteNormal)
                    .SetTextAlignment(iText.Layout.Properties.TextAlignment.LEFT)
                    .SetMarginLeft(2)
                    .SetMarginRight(2);

                if (!String.IsNullOrEmpty(inscricao.Quarto))
                {
                    paragrafoInformacoes
                        .Add(new Text("Quarto: ").SetFont(fonteNormal).SimulateBold());
                    paragrafoInformacoes
                        .Add(new Text(inscricao.Quarto + " ").SimulateItalic());
                }

                foreach (var atividade in inscricao.Atividades)
                {
                    paragrafoInformacoes
                        .Add(new Text($"{atividade.NomeAtividade}: ").SimulateBold());
                    paragrafoInformacoes
                        .Add(new Text($"{atividade.NomeDivisao} ").SimulateItalic());
                }

                canvas.Add(paragrafoNome);
                canvas.Add(paragrafoInformacoes);

                canvas.Close();

                if (qualEtiqueta == 2)
                {
                    qualEtiqueta = 1;
                    linha++;

                    if (linha > TotalLinhasPag)
                    {
                        linha = 1;
                        page = documentoPDF.AddNewPage(PageSize.LETTER);

                        pdfCanvas = new PdfCanvas(page);
                    }
                }
                else
                    qualEtiqueta++;
            }

            documentoPDF.Close();

            using var memStream = new MemoryStream(stream.GetBuffer());
            return memStream.ToArray();
        }
    }
}
