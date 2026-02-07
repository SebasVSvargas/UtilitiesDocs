using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using PdfSharp.Pdf;
using PdfSharp.Pdf.IO;

namespace UtilitiesDocs.Services.Pdf
{
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
    }
}
