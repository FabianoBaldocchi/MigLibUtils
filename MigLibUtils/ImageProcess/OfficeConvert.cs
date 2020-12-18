using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Spire.Doc;
using Spire.Presentation;
using Spire.Xls;

namespace MigLibUtils.ImageProcess
{
    class OfficeConvert
    {
        public static string Convert(string pFileName, int pWidth, int pHeight)
        {
            var pExtension = Path.GetExtension(pFileName).ToLower();


            //se for office, converte antes para PDF
            if (pExtension.Contains(".xls"))
            {
                Workbook workbook = new Workbook();
                workbook.LoadFromFile(pFileName);
                var sheet = workbook.Worksheets[0];

                var im = sheet.ToImage(1, 1, sheet.LastRow, sheet.LastColumn);

                using (var ms = new MemoryStream())
                {
                    var bmp = ToJPegConversion.ResizeImage(im, pWidth, pHeight);
                    bmp.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
                    return System.Convert.ToBase64String(ms.ToArray());
                }

            } 
            else if (pExtension.Contains(".doc"))
            {
                Document doc = new Document();
                doc.LoadFromFile(pFileName);
                
                var im = doc.SaveToImages(1, Spire.Doc.Documents.ImageType.Bitmap);

                using (var ms = new MemoryStream())
                {
                    var bmp = ToJPegConversion.ResizeImage(im, pWidth, pHeight);
                    bmp.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
                    return System.Convert.ToBase64String(ms.ToArray());
                }

            }
            else if (pExtension.Contains(".ppt"))
            {
                Presentation ppt = new Presentation();
                ppt.LoadFromFile(pFileName);

                var im = ppt.Slides[0].SaveAsImage();

                using (var ms = new MemoryStream())
                {
                    var bmp = ToJPegConversion.ResizeImage(im, pWidth, pHeight);
                    bmp.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
                    return System.Convert.ToBase64String(ms.ToArray());
                }
            }

            return null;
        }

    }
}
