using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Spire.Pdf;
using Spire.Pdf.HtmlConverter;
using System.Drawing;
using System.IO;
using System.Threading;



namespace MigLibUtils.FileProcess
{
    public class Pdf
    {

        /// <summary>
        /// Converte url para pdf (limitado apenas para uma página em formato A4)        
        /// </summary>
        /// <param name="stringUrl">http://e.migalhas.com.br/payment/2020/03/25/SANTANDER_0236f0078_save.html</param>
        /// <param name="outputFile">byteArray de um pdf</param>
        /// <returns>string base 64 de um pdf</returns>
        public static string ConvertUrlToPdf(string stringUrl, out Byte[] outputFile)
        {
            PdfDocument pdfDoc = null;
            PdfPageSettings pdfPageSetting = null;
            PdfHtmlLayoutFormat pdfHtmlLayoutFormat = null;
            FileStream fsOutput = null;
            byte[] pdfByteArray = null;
            string stringFilePath = null;
            Thread threadLoadHtml = null;
            try
            {
                pdfDoc = new PdfDocument();
                pdfDoc.ConvertOptions.SetPdfToHtmlOptions(true, true, 1);

                pdfPageSetting = new PdfPageSettings();
                pdfPageSetting.Size = PdfPageSize.A4;
                pdfPageSetting.Orientation = PdfPageOrientation.Portrait;
                pdfPageSetting.Margins = new Spire.Pdf.Graphics.PdfMargins(10);

                pdfHtmlLayoutFormat = new PdfHtmlLayoutFormat();
                pdfHtmlLayoutFormat.IsWaiting = false; //Não espera a leitura completa da url, se colocar true demora 30s para ler a url. :(
                pdfHtmlLayoutFormat.FitToPage = Clip.Width;
                pdfHtmlLayoutFormat.Layout = Spire.Pdf.Graphics.PdfLayoutType.OnePage;

                //Faz a leitura em outra thread conforme documentação do componente Spire.Pdf
                threadLoadHtml = new Thread(() =>
                { pdfDoc.LoadFromHTML(stringUrl, false, false, false, pdfPageSetting, pdfHtmlLayoutFormat); });
                threadLoadHtml.SetApartmentState(ApartmentState.STA);
                threadLoadHtml.Start();
                threadLoadHtml.Join();

                stringFilePath = AppDomain.CurrentDomain.BaseDirectory + Guid.NewGuid().ToString() + ".pdf";

                fsOutput = new FileStream(stringFilePath, FileMode.CreateNew, FileAccess.ReadWrite);

                pdfDoc.SaveToStream(fsOutput, FileFormat.PDF);

                fsOutput.Close(); //Existe uma operação de io neste momento

                pdfByteArray = File.ReadAllBytes(stringFilePath);  //Existe uma operação de io neste momento

                File.Delete(stringFilePath);  //Existe uma operação de io neste momento

                pdfDoc.Close();

                outputFile = pdfByteArray;

                return Convert.ToBase64String(pdfByteArray);
            }
            catch
            {
                throw; //em caso de erro, joga o erro pra cima
            }
            finally
            {
                pdfDoc = null;
                pdfPageSetting = null;
                pdfHtmlLayoutFormat = null;
                fsOutput = null;
                pdfByteArray = null;
                stringFilePath = null;
                threadLoadHtml = null;
            }
        }
    }
}
