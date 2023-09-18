using DocumentIndexer.Services;
using Microsoft.AspNetCore.Mvc;

namespace DocumentIndexer.Controllers
{
    [ApiController]
    [Route("api/es")]
    public class ElasticController : Controller
    {
        private readonly ElasticService _elasticService;
        private readonly DocumentService _documentService;

        public ElasticController(ElasticService elasticService, DocumentService documentService)
        {
            _documentService = documentService;
            _elasticService = elasticService;
        }

        [HttpPost]
        [Route("index/{file}")]
        public async Task<IActionResult> Index([FromRoute] string file, CancellationToken cancellationToken)
        {
            try
            {
                var doc = _documentService.ReadDocument(file);
                await _elasticService.IndexAsync(doc, cancellationToken);

                return Ok("Документ успешно индексирован");
            }
            catch (ApplicationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [Route("bulk")]
        public async Task<IActionResult> Bulk(CancellationToken cancellationToken)
        {
            try
            {
                var docs = _documentService.ReadAllDocuments();
                await _elasticService.IndexManyAsync(docs, cancellationToken);

                return Ok("Документы успешно индексированы");
            }
            catch (ApplicationException ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
