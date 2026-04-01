using iText.IO.Font.Constants;
using iText.Kernel.Font;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas;
using iText.Layout;
using iText.Layout.Element;

namespace EventoWeb.Secretaria.Relatorios.Relatorios.Implementacoes
{
    public class RelatorioEtiquetaCracha
    {
        public Stream Gerar(IList<Inscricao> inscritos)
        {
            using var stream = new MemoryStream();
            using var pdfWriter = new PdfWriter(stream);

            var documentoPDF = new PdfDocument(pdfWriter);
            var page = documentoPDF.AddNewPage(PageSize.LETTER);

            var pdfCanvas = new PdfCanvas(page);

            var fonteTitulo = PdfFontFactory.CreateFont(StandardFonts.HELVETICA_BOLD);
            var fonteCidade = PdfFontFactory.CreateFont(StandardFonts.HELVETICA_OBLIQUE);

            const float LarguraEtiqueta = 101.6f;
            const float AlturaEtiqueta = 25.4f;
            const float MargemSuperior = 0.4f;
            const int TotalLinhasPag = 10;
            var PosicaoYInicial = PageSize.LETTER.GetHeight() - 14.9f.MillimetersToPointsTextSharp();

            var posicaoY = 0f;
            var posicaoX = 0f;

            var qualEtiqueta = 1;
            var linha = 1;

            for (var indice = 1; indice <= inscritos.Count(); indice++)
            {
                var inscricao = inscritos[indice - 1];

                if (qualEtiqueta == 1)
                    posicaoX = 5f.MillimetersToPointsTextSharp();
                else
                    posicaoX = 111f.MillimetersToPointsTextSharp();

                posicaoY = PosicaoYInicial - (AlturaEtiqueta * linha + MargemSuperior).MillimetersToPointsTextSharp();

                var canvas = new Canvas(
                    page,
                    new Rectangle(
                        posicaoX, //x inicial 
                        posicaoY, // y inicial
                        LarguraEtiqueta.MillimetersToPointsTextSharp(), //x final
                        (AlturaEtiqueta - MargemSuperior).MillimetersToPointsTextSharp()
                        )
                    );

                canvas.Add(
                    new Paragraph(
                            (String.IsNullOrWhiteSpace(inscricao.NomeCracha) ? inscricao.Pessoa.Nome.Nome : inscricao.NomeCracha) + "\n")
                        .SetFont(fonteTitulo)
                        .SetFontSize(20)
                        .SetTextAlignment(iText.Layout.Properties.TextAlignment.CENTER)
                     );

                canvas.Add(
                     new Paragraph(
                            inscricao.Pessoa.Cidade + "/" + inscricao.Pessoa.UF)
                        .SetFont(fonteCidade)
                        .SetFontSize(14)
                        .SetTextAlignment(iText.Layout.Properties.TextAlignment.CENTER)
                     );
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
            return new MemoryStream(stream.GetBuffer());
        }
    }
}
