using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using PdfSharp.Pdf;
using PdfSharp.Pdf.IO;
using PdfSharp.Drawing;
using System.Linq;

namespace UtilitiesDocs.Services.Pdf
{
    public enum RotationScope
    {
        All,
        Odd,
        Even,
        Specific
    }

    public class PdfService
    {
        public void MergePdfs(IEnumerable<string> inputPaths, string outputPath)
        {
            using (PdfDocument outputDocument = new PdfDocument())
            {
                foreach (string file in inputPaths)
                {
                    // Abrir el documento (Importante: PdfDocumentOpenMode.Import para solo lectura eficiente)
                    using (PdfDocument inputDocument = PdfReader.Open(file, PdfDocumentOpenMode.Import))
                    {
                        int count = inputDocument.PageCount;
                        for (int idx = 0; idx < count; idx++)
                        {
                            PdfPage page = inputDocument.Pages[idx];
                            outputDocument.AddPage(page);
                        }
                    }
                }
                outputDocument.Save(outputPath);
            }
        }

        public bool RemovePassword(string inputPath, string outputPath, string password)
        {
            try
            {
                // Estrategia: Abrir el protegido y copiar páginas a uno nuevo sin seguridad
                using (PdfDocument inputDocument = PdfReader.Open(inputPath, password, PdfDocumentOpenMode.Import))
                using (PdfDocument outputDocument = new PdfDocument())
                {
                    int count = inputDocument.PageCount;
                    for (int idx = 0; idx < count; idx++)
                    {
                        outputDocument.AddPage(inputDocument.Pages[idx]);
                    }
                    
                    outputDocument.Save(outputPath);
                    return true;
                }
            }
            catch (Exception)
            {
                // Fallo por contraseña incorrecta u otro error
                return false;
            }
        }

        public void ImagesToPdf(IEnumerable<string> imagePaths, string outputPath)
        {
            using (PdfDocument outputDocument = new PdfDocument())
            {
                foreach (string file in imagePaths)
                {
                    PdfPage page = outputDocument.AddPage();
                    // Load the image
                    using (XImage img = XImage.FromFile(file))
                    {
                        // Set the page size to match the image
                        page.Width = img.PointWidth;
                        page.Height = img.PointHeight;

                        using (XGraphics gfx = XGraphics.FromPdfPage(page))
                        {
                            gfx.DrawImage(img, 0, 0);
                        }
                    }
                }
                outputDocument.Save(outputPath);
            }
        }

        public void RotatePages(string inputPath, string outputPath, int rotationDegrees, RotationScope scope, string specificPages = null)
        {
            using (PdfDocument inputDocument = PdfReader.Open(inputPath, PdfDocumentOpenMode.Import))
            using (PdfDocument outputDocument = new PdfDocument())
            {
                var indicesToRotate = GetIndicesToRotate(inputDocument.PageCount, scope, specificPages);

                for (int i = 0; i < inputDocument.PageCount; i++)
                {
                    PdfPage page = inputDocument.Pages[i];
                    PdfPage newPage = outputDocument.AddPage(page);

                    if (indicesToRotate.Contains(i))
                    {
                        // Rotate is an integer property (0, 90, 180, 270)
                        // Verify current rotation and add the new one
                        int currentRotation = newPage.Rotate;
                        int newRotation = (currentRotation + rotationDegrees) % 360;
                        if (newRotation < 0) newRotation += 360;
                        newPage.Rotate = newRotation;
                    }
                }
                outputDocument.Save(outputPath);
            }
        }

        private HashSet<int> GetIndicesToRotate(int pageCount, RotationScope scope, string specificPages)
        {
            var indices = new HashSet<int>();
            switch (scope)
            {
                case RotationScope.All:
                    for (int i = 0; i < pageCount; i++) indices.Add(i);
                    break;
                case RotationScope.Odd: // Pages 1, 3, 5... (Indices 0, 2, 4...)
                    for (int i = 0; i < pageCount; i += 2) indices.Add(i);
                    break;
                case RotationScope.Even: // Pages 2, 4, 6... (Indices 1, 3, 5...)
                    for (int i = 1; i < pageCount; i += 2) indices.Add(i);
                    break;
                case RotationScope.Specific:
                    if (!string.IsNullOrWhiteSpace(specificPages))
                    {
                        ParseRange(specificPages, pageCount, indices);
                    }
                    break;
            }
            return indices;
        }

        private void ParseRange(string range, int maxPages, HashSet<int> indices)
        {
            // Simple parser for formats like "1,3,5-7"
            var parts = range.Split(',');
            foreach (var part in parts)
            {
                var trimmed = part.Trim();
                if (trimmed.Contains('-'))
                {
                    var bounds = trimmed.Split('-');
                    if (bounds.Length == 2 && 
                        int.TryParse(bounds[0], out int start) && 
                        int.TryParse(bounds[1], out int end))
                    {
                        start = Math.Max(1, start);
                        end = Math.Min(maxPages, end);
                        for (int k = start; k <= end; k++) indices.Add(k - 1);
                    }
                }
                else
                {
                    if (int.TryParse(trimmed, out int pageNum))
                    {
                        if (pageNum >= 1 && pageNum <= maxPages)
                            indices.Add(pageNum - 1);
                    }
                }
            }
        }
    }
}
