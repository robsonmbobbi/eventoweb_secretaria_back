using EventoWeb.Nucleo.Negocio.Repositorios;
using iText.IO.Font.Constants;
using iText.Kernel.Font;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas;
using iText.Layout;
using iText.Layout.Element;
using iText.StyledXmlParser.Jsoup.Nodes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace EventoWeb.Secretaria.Relatorios.Relatorios.Implementacoes
{
    public class RelatorioEtiquetaCaderno
    {
        public Stream Gerar(IList<CrachaInscrito> inscritos)
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

                /*pdfCanvas.Rectangle(new Rectangle(
                        posicaoX, //x inicial 
                        posicaoY, // y inicial
                        LarguraEtiqueta.MillimetersToPointsTextSharp(), //x final
                        (AlturaEtiqueta - MargemSuperior).MillimetersToPointsTextSharp())
                    );
                pdfCanvas.Stroke();*/

                var canvas = new Canvas(
                    page,
                    new Rectangle(
                        posicaoX, //x inicial 
                        posicaoY, // y inicial
                        LarguraEtiqueta.MillimetersToPointsTextSharp(), //x final
                        (AlturaEtiqueta - MargemSuperior).MillimetersToPointsTextSharp())
                    );

                var paragrafoNome = new Paragraph(
                    (String.IsNullOrWhiteSpace(inscricao.NomeConhecido) ? inscricao.Nome : inscricao.NomeConhecido))
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

                if (!String.IsNullOrEmpty(inscricao.SalaEstudo))
                {
                    paragrafoInformacoes
                        .Add(new Text("Sala: ").SimulateBold());
                    paragrafoInformacoes
                        .Add(new Text(inscricao.SalaEstudo + " ").SimulateItalic());
                }

                if (!String.IsNullOrEmpty(inscricao.Afrac))
                {
                    paragrafoInformacoes
                        .Add(new Text("Oficina: ").SimulateBold());
                    paragrafoInformacoes
                        .Add(new Text(inscricao.Afrac + " ").SimulateItalic());
                }

                if (!String.IsNullOrEmpty(inscricao.Departamento))
                {
                    paragrafoInformacoes
                        .Add(new Text("Departamento: ").SimulateBold());
                    paragrafoInformacoes
                        .Add(new Text(inscricao.Departamento + " ").SimulateItalic());
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

            /*var documento = new Document(new iTextSharp.text.Rectangle(PageSize.LETTER),
                Utilities.MillimetersToPoints(4f),
                Utilities.MillimetersToPoints(4f),
                Utilities.MillimetersToPoints(11.4f),
                Utilities.MillimetersToPoints(0f));
            var escritorPDF = PdfWriter.GetInstance(documento, stream);

            documento.Open();

            PdfContentByte cb = escritorPDF.DirectContent;

            var fonteTitulo = new Font(Font.FontFamily.HELVETICA, 11, (int)System.Drawing.FontStyle.Bold);
            var fonteNormal = new Font(Font.FontFamily.HELVETICA, 10, (int)System.Drawing.FontStyle.Italic);
            var fonteNormalBold = new Font(Font.FontFamily.HELVETICA, 10, (int)System.Drawing.FontStyle.Bold);

            const float LarguraEtiqueta = 101.6f;
            const float AlturaEtiqueta = 25.4f;
            const float MargemSuperior = 0.4f;
            const int TotalLinhasPag = 10;
            var PosicaoYInicial = PageSize.LETTER.Height - Utilities.MillimetersToPoints(14.6f);

            var posicaoY = 0f;
            var posicaoX = 0f;

            var linha = 1;
            var qualEtiqueta = 1;

            for (var indice = 1; indice <= inscritos.Count(); indice++)
            {
                var inscricao = inscritos[indice - 1];

                if (qualEtiqueta == 1)
                    posicaoX = Utilities.MillimetersToPoints(6.1f);
                else
                    posicaoX = Utilities.MillimetersToPoints(112.8f);

                posicaoY = PosicaoYInicial - Utilities.MillimetersToPoints(AlturaEtiqueta * linha - 1 + MargemSuperior);

                var coluna = new ColumnText(cb);
                coluna.Alignment = Element.ALIGN_CENTER;
                coluna.SetSimpleColumn(
                    posicaoX, //x inicial 
                    posicaoY, // y inicial
                    posicaoX + Utilities.MillimetersToPoints(LarguraEtiqueta), //x final
                    posicaoY + Utilities.MillimetersToPoints(AlturaEtiqueta - MargemSuperior));

                var paragrafoNome = new Paragraph(
                    (String.IsNullOrWhiteSpace(inscricao.NomeConhecido) ? inscricao.Nome : inscricao.NomeConhecido),
                    fonteTitulo);

                paragrafoNome.Alignment = Element.ALIGN_LEFT;
                var paragrafoInformacoes = new Paragraph();
                paragrafoInformacoes.Alignment = Element.ALIGN_LEFT;

                if (!String.IsNullOrEmpty(inscricao.Quarto))
                {
                    paragrafoInformacoes.Add(new Chunk("Quarto: ", fonteNormalBold));
                    paragrafoInformacoes.Add(new Chunk(inscricao.Quarto + " ", fonteNormal));
                }

                if (!String.IsNullOrEmpty(inscricao.SalaEstudo))
                {
                    paragrafoInformacoes.Add(new Chunk("Sala: ", fonteNormalBold));
                    paragrafoInformacoes.Add(new Chunk(inscricao.SalaEstudo + " ", fonteNormal));
                }

                if (!String.IsNullOrEmpty(inscricao.Afrac))
                {
                    paragrafoInformacoes.Add(new Chunk("Oficina: ", fonteNormalBold));
                    paragrafoInformacoes.Add(new Chunk(inscricao.Afrac + " ", fonteNormal));
                }

                if (!String.IsNullOrEmpty(inscricao.Departamento))
                {
                    paragrafoInformacoes.Add(new Chunk("Departamento: ", fonteNormalBold));
                    paragrafoInformacoes.Add(new Chunk(inscricao.Departamento, fonteNormal));
                }

                coluna.AddElement(paragrafoNome);
                coluna.AddElement(paragrafoInformacoes);

                coluna.Go();

                if (qualEtiqueta == 2)
                {
                    qualEtiqueta = 1;
                    linha = linha + 1;

                    if (linha > TotalLinhasPag)
                    {
                        linha = 1;
                        documento.NewPage();
                    }
                }
                else
                    qualEtiqueta = qualEtiqueta + 1;
            }*/

            documentoPDF.Close();
            return new MemoryStream(stream.GetBuffer());
        }
    }
}
