using Spire.Xls;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MigLibUtils.FileProcess
{
    public class Excel
    {
        public static string[] GetSheets(string fBase64)
        {
            using (Workbook workbook = new Workbook())
            {
                using (Stream stream = new MemoryStream(Convert.FromBase64String(fBase64)))
                {
                    workbook.LoadFromStream(stream);
                }

                return workbook.Worksheets.Select(w => w.Name).ToArray();
            }

        }


        public static DataTable SelectSheet(string fBase64, int sheetid, int iniRow, int iniCol, int maxRows, int maxCols, bool exportColumnNames)
        {
            using (Workbook workbook = new Workbook())
            {
                using (Stream stream = new MemoryStream(Convert.FromBase64String(fBase64)))
                {
                    workbook.LoadFromStream(stream);
                }

                var sheet = workbook.Worksheets[sheetid];

                int selrow = Math.Min(maxRows, sheet.AllocatedRange.LastRow);
                int selcol = Math.Min(maxCols, sheet.AllocatedRange.LastColumn);

                return sheet.ExportDataTable(iniRow, iniCol, selrow, selcol, exportColumnNames);
            }

        }

    }
}
