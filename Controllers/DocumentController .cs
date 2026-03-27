using SemantiCore_API.Application.Interfaces;
using SemantiCore_API.Shared.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace SemantiCore_API.Controllers
{
    [ApiController]
    [Route("api/documnet")]
    public class DocumentController: ControllerBase
    {
        private readonly IDocumentService _documentService;

        public DocumentController(IDocumentService service)
        {
            _documentService = service;
        }

        [HttpPost("upload")]
        public async Task<IActionResult> Upload([FromForm] UploadIndexDocumentDto dto)
        {
            var result = await _documentService.UploadAsync(
                dto.CategoryName,
                dto.IndexName,
                dto.File);

            return Ok(new
            {
                result.Id,
                result.OriginalFileName,
                result.FileSize,
                result.UploadedAt
            });
        }

        [HttpGet("{documentId:int}")]
        public async Task<IActionResult> GetDocumentByDocumentID(int documentId)
        {
            var result = await _documentService.GetByIdAsync(documentId);

            if (result == null)
                return NotFound($"Document with ID {documentId} not found");

            return Ok(result);
        }

        [HttpGet("{documentId:int}/view")]
        public async Task<IActionResult> ViewDocument(int documentId)
        {
            var document = await _documentService.GetDocumentForViewAsync(documentId);

            if (document == null)
                return NotFound();

            return File(
                document.FileBytes,
                document.ContentType,
                enableRangeProcessing: true
            );
        }


    }
}
