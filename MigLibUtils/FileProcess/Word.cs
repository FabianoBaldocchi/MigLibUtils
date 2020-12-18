using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MigLibUtils.FileProcess
{
    public class Word
    {
        public static string ToHTML(string fBase64, string nomeoriginal)
        {
            using (var doc = new Spire.Doc.Document())
            {
                using (Stream stream = new MemoryStream(Convert.FromBase64String(fBase64)))
                {
                    doc.LoadFromStream(stream, Path.GetExtension(nomeoriginal).ToUpper().Contains("DOCX") ? Spire.Doc.FileFormat.Docx : Spire.Doc.FileFormat.Doc);
                    doc.HtmlExportOptions.ImageEmbedded = true;
                    doc.HtmlExportOptions.IsExportDocumentStyles = false;
                    doc.HtmlExportOptions.CssStyleSheetType = Spire.Doc.CssStyleSheetType.Internal;
                    doc.HtmlExportOptions.HasHeadersFooters = false;
                    doc.HtmlExportOptions.IsTextInputFormFieldAsText = true;

                    var outstr = new MemoryStream();
                    doc.SaveToStream(outstr, Spire.Doc.FileFormat.Html);
                    var rethtml = Encoding.GetEncoding("utf-8").GetString(outstr.ToArray()); //basicamente funciona com HTML

                    return rethtml;
                }

            }
        }
    }
}
