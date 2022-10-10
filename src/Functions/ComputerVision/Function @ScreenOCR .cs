using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;

using Tesseract;

namespace DxMLEngine.Functions
{
    internal class OCR
    {
        internal const string TESSSERACT_DATA = @".\src\Services\Tesseract\TessData\";

        public static (int, int, int, int) FindTextOnScreen(string text)
        {
            Thread.Sleep(1000);

            ////0
            //var windowWidth = Screen.PrimaryScreen.Bounds.Width;
            //var windowHeight = Screen.PrimaryScreen.Bounds.Height;
            var windowWidth = 2736;
            var windowHeight = 1824;

            ////1
            var bitmap = new Bitmap(windowWidth, windowHeight);
            var graphics = Graphics.FromImage(bitmap);
            graphics.CopyFromScreen(0, 0, 0, 0, bitmap.Size);

            ////2
            var folderPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            var filePath = $"{folderPath}\\Screenshot @Temporary .jpg";
            bitmap.Save(filePath, System.Drawing.Imaging.ImageFormat.Jpeg);

            ////3
            var foundedRect = Rect.FromCoords(0, 0, 0, 0);
            using (var engine = new TesseractEngine(TESSSERACT_DATA, "eng", EngineMode.Default))
            {
                using (var image = Pix.LoadFromFile(filePath))
                {
                    using (var page = engine.Process(image))
                    {
                        using (var iterator = page.GetIterator())
                        {
            ////4
                            iterator.Begin();
                            do
                            {
                                if (iterator.TryGetBoundingBox(PageIteratorLevel.Word, out var rect))
                                {
            ////5
                                    var curText = iterator.GetText(PageIteratorLevel.Word);
                                    if (curText.Contains(text))
                                    {
                                        foundedRect = rect;
                                    }
                                }
                            } while (iterator.Next(PageIteratorLevel.TextLine));
                        }
                    }
                }
            }
            ////6
            File.Delete(filePath);

            return (foundedRect.X1, foundedRect.Y1, foundedRect.X2, foundedRect.Y2);
        }
    }
}
