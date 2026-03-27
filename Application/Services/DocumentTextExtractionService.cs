using DocumentFormat.OpenXml.Packaging;
using SemantiCore_API.Application.Interfaces;
using UglyToad.PdfPig;

namespace SemantiCore_API.Application.Services
{
    public class DocumentTextExtractionService: IDocumentTextExtractionService
    {
        public async Task<string> ExtractTextAsync(string filePath, string contentType)
        {
            if (contentType.Contains("pdf"))
                return ExtractPdfText(filePath);

            if (contentType.Contains("word"))
                return ExtractDocxText(filePath);

            if (contentType.Contains("text"))
                return await File.ReadAllTextAsync(filePath);

            throw new NotSupportedException("Unsupported document type");
        }

        private string ExtractPdfText(string path)
        {
            using var pdf = PdfDocument.Open(path);
            return string.Join(
                Environment.NewLine,
                pdf.GetPages().Select(p => p.Text)
            );
        }

        private string ExtractDocxText(string path)
        {
            using var doc = WordprocessingDocument.Open(path, false);
            return doc.MainDocumentPart?.Document.Body?.InnerText ?? "";
        }
    }
}
