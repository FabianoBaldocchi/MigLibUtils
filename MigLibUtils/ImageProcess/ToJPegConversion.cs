using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics;
using System.IO;
using IV.SW.Database;
using System.Runtime.InteropServices;

using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

namespace MigLibUtils.ImageProcess
{
    public class ToJPegConversion
    {
        internal static List<ConversionObject> ConversionThreads = new List<ConversionObject>();

        public static string ExeConversion = "\"C:\\Program Files (x86)\\2JPEG\\2jpeg.exe\"";

        public static string ExeConversionParameters = "-src \"{INPUT}\" -dst \"{OUTPUT}\" -oper Resize size:\"{WIDTH} {HEIGHT}\" -options pages:\"1\" scansf:no alerts:no silent:yes -jpeg thumbnail:yes";
        public static string TempDir = "C:\\VisualStudio\\Workfolder\\thumbnails";

        public static void GenerateImage(string pBase64File, string pExtension, string pSqlCommand, string pErrorSqlCommand, GenConnection pSqlConnection,
                                         int pWitdh = 150, int pHeight = 180, bool DeleteAfter = true)
        {
            if (!File.Exists(ExeConversion.Replace("\"", "")))
                throw new Exception("Execonversion nao existe:" + ExeConversion);

            var dirbasename = TempDir + "\\" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + Guid.NewGuid().ToString().Substring(0, 5);
            var fdirThumb = dirbasename + "\\out";

            Directory.CreateDirectory(dirbasename);
            Directory.CreateDirectory(fdirThumb);

            var fname = dirbasename + "\\" + "arquivo" + pExtension;

            File.WriteAllBytes(fname, Convert.FromBase64String(pBase64File));

            var dosCommArg = ExeConversionParameters.Replace("{INPUT}", fname)
                          .Replace("{OUTPUT}", fdirThumb)
                          .Replace("{WIDTH}", pWitdh.ToString())
                          .Replace("{HEIGHT}", pHeight.ToString());

            var co = new ConversionObject()
            {

                DosCommand = ExeConversion,
                DosCommandArguments = dosCommArg,
                InputTempDir = dirbasename,
                OutputTempDir = fdirThumb,
                sqlCommand = pSqlCommand,
                errorSqlCommand = pErrorSqlCommand,
                sqlConnection = pSqlConnection,
                DeleteInputAfter = DeleteAfter,

                inputFile = fname,
                Width = pWitdh,
                Height = pHeight
            };

            co.RunningThread = new Thread(new ThreadStart(co.Exec));

            ConversionThreads.Add(co);

            co.RunningThread.Start();

        }


        public static string GenerateImage(string pBase64File, string pExtension, int pWitdh = 150, int pHeight = 180, bool DeleteAfter = true)
        {
            if (!File.Exists(ExeConversion.Replace("\"", "")))
                throw new Exception("Execonversion nao existe:" + ExeConversion);


            var dirbasename = TempDir + "\\" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + Guid.NewGuid().ToString().Substring(0, 5);
            var fdirThumb = dirbasename + "\\out";

            Directory.CreateDirectory(dirbasename);
            Directory.CreateDirectory(fdirThumb);

            var fname = dirbasename + "\\" + "arquivo" + pExtension;

            File.WriteAllBytes(fname, Convert.FromBase64String(pBase64File));

            var dosCommArg = ExeConversionParameters.Replace("{INPUT}", fname)
                               .Replace("{OUTPUT}", fdirThumb)
                               .Replace("{WIDTH}", pWitdh.ToString())
                               .Replace("{HEIGHT}", pHeight.ToString());

            var co = new ConversionObject()
            {

                DosCommand = ExeConversion,
                DosCommandArguments = dosCommArg,
                InputTempDir = dirbasename,
                OutputTempDir = fdirThumb,
                sqlCommand = null,
                errorSqlCommand = null,
                sqlConnection = null,
                DeleteInputAfter = DeleteAfter,

                inputFile = fname,
                Width = pWitdh,
                Height = pHeight
            };

            //chamada assincrona
            co.Exec();

            return co.GeneratedBase64;

        }

        internal class ConversionObject
        {
            internal string inputFile;
            internal int Width;
            internal int Height;

            internal string DosCommand;
            internal string DosCommandArguments;
            internal string InputTempDir;
            internal string OutputTempDir;

            internal bool DeleteInputAfter = true;

            internal Thread RunningThread;

            internal string sqlCommand;
            internal string errorSqlCommand;
            internal GenConnection sqlConnection;

            internal string GeneratedBase64;

            [DllImport("user32.dll")]
            static extern bool SetForegroundWindow(IntPtr hWnd);

            internal void Exec()
            {

                int exitCode;

                try
                {
                    this.GeneratedBase64 = OfficeConvert.Convert(inputFile, Width, Height);

                    if (this.GeneratedBase64 == null)
                    {
                        var p = new ProcessStartInfo();
                        p.FileName = DosCommand;
                        p.Arguments = DosCommandArguments;
                        p.WorkingDirectory = Path.GetDirectoryName(p.FileName.Substring(1, p.FileName.Length - 2));
                        p.WindowStyle = ProcessWindowStyle.Hidden;
                        p.CreateNoWindow = true;

                        using (Process proc = Process.Start(p))
                        {
                            proc.WaitForExit();
                            exitCode = proc.ExitCode;
                        }

                        if (Directory.Exists(OutputTempDir))
                        {
                            foreach (var f in Directory.GetFiles(OutputTempDir, "*.jpg"))
                            {
                                var b = File.ReadAllBytes(f);
                                this.GeneratedBase64 = Convert.ToBase64String(b);

                                if (sqlCommand != null && sqlConnection != null)
                                {
                                    sqlCommand = sqlCommand.Replace("{FILE_CONTENT}", this.GeneratedBase64);
                                    this.sqlConnection.ExecCommand(sqlCommand);
                                }
                                break;
                            }
                        }
                        else
                        {
                            throw new Exception("Nao gerou arquivo esperado.");
                        }
                    }


                }
                catch (Exception ex)
                {
                    throw ex;
                }

                if (DeleteInputAfter && Directory.Exists(InputTempDir))
                    Directory.Delete(InputTempDir, true);

                if (ToJPegConversion.ConversionThreads.Contains(this))
                    ToJPegConversion.ConversionThreads.Remove(this);
            }



        }


        public static Bitmap ResizeImage(Image image, int width, int height)
        {
            var destRect = new Rectangle(0, 0, width, height);
            var destImage = new Bitmap(width, height);

            destImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);

            using (var graphics = Graphics.FromImage(destImage))
            {
                graphics.CompositingMode = CompositingMode.SourceCopy;
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

                using (var wrapMode = new ImageAttributes())
                {
                    wrapMode.SetWrapMode(WrapMode.TileFlipXY);
                    graphics.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, wrapMode);
                }
            }

            return destImage;
        }

    }
}
