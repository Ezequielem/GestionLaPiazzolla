using GestionLaPiazzolla.Models;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Microsoft.AspNetCore.Hosting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace GestionLaPiazzolla.Reports
{
    public class ReporteRecibo
    {
        private Document _documento;
        private Font _fontStyle;
        private int _maximoColumnas;
        private PdfPTable _pdfPTable;
        private PdfPCell _pdfCell;
        private MemoryStream _ms;
        private Pago _pago;
        private IWebHostEnvironment _env;

        public ReporteRecibo(IWebHostEnvironment hostEnvironment)
        {
            _env = hostEnvironment;
            _documento = new Document();
            _ms = new MemoryStream();
            _pago = new Pago();
            _pdfPTable = new PdfPTable(3);
            _maximoColumnas = 3;
        }

        public byte[] imprimirRecibo(Pago pago)
        {

            _documento.SetPageSize(PageSize.A5.Rotate());
            _documento.SetMargins(28f, 28f, 20f, 20f);
            _fontStyle = FontFactory.GetFont("Tahoma", 8f, 1);
            PdfWriter docWriter = PdfWriter.GetInstance(_documento, _ms);

            _documento.Open();

            _documento.AddTitle("Recibo");
            _documento.AddAuthor("La Piazzolla");
            _documento.AddKeywords("Hola Mundo");           

            string path = Path.Combine(_env.WebRootPath, "Images", "logoPiazzolla.jpg");
            Image img = Image.GetInstance(path);
            img.ScaleAbsoluteWidth(164.88f);
            img.ScaleAbsoluteHeight(37.16f);                        
            img.Alignment = Element.ALIGN_LEFT;
            //_documento.Add(img);

            _pdfPTable.WidthPercentage = 100;
            _pdfCell = new PdfPCell(img);
            _pdfCell.Colspan = 1;
            _pdfCell.Border = 1;
            _pdfPTable.AddCell(_pdfCell);
            _pdfPTable.CompleteRow();

            _documento.Add(_pdfPTable);
            _documento.Add(new Paragraph(pago.Monto.ToString()));

            _documento.Close();
            docWriter.Close();
            return _ms.ToArray();
        }

        public byte[] Recibo(Pago pago)
        {
            _pago = pago;


            _documento.SetPageSize(PageSize.A5);
            _documento.SetMargins(5f, 5f, 20f, 5f);

            _pdfPTable.WidthPercentage = 100;
            _pdfPTable.HorizontalAlignment = Element.ALIGN_LEFT;

            _fontStyle = FontFactory.GetFont("Tahoma", 8f, 1);
            PdfWriter docWriter = PdfWriter.GetInstance(_documento, _ms);

            _documento.Open();

            float[] sizes = new float[_maximoColumnas];
            for (var i = 0; i < _maximoColumnas; i++)
            {
                if (i==0)
                {
                    sizes[i] = 20;
                }
                else
                {
                    sizes[i] = 100;
                }
            }

            _pdfPTable.SetWidths(sizes);

            this.cabeceraReporte();
            this.FilaVacia(2);
            this.cuerpoReporte();

            _pdfPTable.HeaderRows = 2;
            _documento.Add(_pdfPTable);
            _documento.Close();

            return _ms.ToArray();
        }

        private void cabeceraReporte()
        {
            _pdfCell = new PdfPCell(this.addLogo());
            _pdfCell.Colspan = 1;
            _pdfCell.Border = 0;
            _pdfPTable.AddCell(_pdfCell);

            _pdfCell = new PdfPCell(this.setPageTitle());
            _pdfCell.Colspan = _maximoColumnas - 1;
            _pdfCell.Border = 0;
            _pdfPTable.AddCell(_pdfCell);

            _pdfPTable.CompleteRow();
        }

        private PdfPTable addLogo()
        {
            int maximoColumna = 1;
            PdfPTable pdfPTable = new PdfPTable(maximoColumna);

        
            
            string path = Path.Combine(_env.WebRootPath, "Images", "logoPiazzolla.jpg");

            Image img = Image.GetInstance(path);

            _pdfCell = new PdfPCell(img);
            _pdfCell.Colspan = maximoColumna;
            _pdfCell.HorizontalAlignment = Element.ALIGN_LEFT;
            _pdfCell.Border = 0;
            _pdfCell.ExtraParagraphSpace = 0;
            pdfPTable.AddCell(_pdfCell);
            pdfPTable.CompleteRow();
            return pdfPTable;
        }

        private PdfPTable setPageTitle()
        {
            int maximoColumna = 3;
            PdfPTable pdfPTable = new PdfPTable(maximoColumna);

            _fontStyle = FontFactory.GetFont("Tahoma", 18f, 1);
            _pdfCell = new PdfPCell(new Phrase("Imformacion de pagos", _fontStyle));
            _pdfCell.Colspan = maximoColumna;
            _pdfCell.Border = 0;
            _pdfCell.ExtraParagraphSpace = 0;
            pdfPTable.AddCell(_pdfCell);
            pdfPTable.CompleteRow();

            _fontStyle = FontFactory.GetFont("Tahoma", 14f, 1);
            _pdfCell = new PdfPCell(new Phrase("Nombre Alumno", _fontStyle));
            _pdfCell.Colspan = maximoColumna;
            _pdfCell.Border = 0;
            _pdfCell.ExtraParagraphSpace = 0;
            pdfPTable.AddCell(_pdfCell);
            pdfPTable.CompleteRow();

            return pdfPTable;
        }

        private void FilaVacia(int count)
        {
            for (int i = 0; i <= count; i++)
            {                
                _pdfCell = new PdfPCell(new Phrase("", _fontStyle));
                _pdfCell.Colspan = _maximoColumnas;
                _pdfCell.Border = 0;
                _pdfCell.ExtraParagraphSpace = 10;
                _pdfPTable.AddCell(_pdfCell);
                _pdfPTable.CompleteRow();
            }
        }

        private void cuerpoReporte()
        {
            var fontStyleBold = FontFactory.GetFont("Tahoma", 9f, 1);
            _fontStyle = FontFactory.GetFont("Tahoma", 9f, 0);

            _pdfCell = new PdfPCell(new Phrase("Id", fontStyleBold));
            _pdfCell.HorizontalAlignment = Element.ALIGN_CENTER;
            _pdfCell.VerticalAlignment = Element.ALIGN_MIDDLE;
            _pdfCell.BackgroundColor = BaseColor.Gray;
            _pdfPTable.AddCell(_pdfCell);

            _pdfCell = new PdfPCell(new Phrase("Name", fontStyleBold));
            _pdfCell.HorizontalAlignment = Element.ALIGN_CENTER;
            _pdfCell.VerticalAlignment = Element.ALIGN_MIDDLE;
            _pdfCell.BackgroundColor = BaseColor.Gray;
            _pdfPTable.AddCell(_pdfCell);

            _pdfCell = new PdfPCell(new Phrase("Adress", fontStyleBold));
            _pdfCell.HorizontalAlignment = Element.ALIGN_CENTER;
            _pdfCell.VerticalAlignment = Element.ALIGN_MIDDLE;
            _pdfCell.BackgroundColor = BaseColor.Gray;
            _pdfPTable.AddCell(_pdfCell);

            _pdfPTable.CompleteRow();
        }
    }
}
