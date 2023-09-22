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

        [HttpGet]
        [Route("search")]
        public async Task<IActionResult> Search([FromQuery] string text, CancellationToken cancellationToken)
        {
            try
            {
                return Ok(await _elasticService.SearchByTextAsync(text, cancellationToken));
            }
            catch (ApplicationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        [Route("get")]
        public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
        {
            try
            {
                var docs = _documentService.ReadAllDocuments();

                return Ok(await _elasticService.CatchDosumentsAsync(docs, cancellationToken));
            }
            catch (ApplicationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        [Route("{file}")]
        public async Task<IActionResult> GetById([FromRoute] string id, CancellationToken cancellationToken)
        {
            try
            {
                return Ok(await _elasticService.GetByIdAsync(id, cancellationToken));
            }
            catch (ApplicationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [Route("{file}")]
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

        [HttpDelete]
        public async Task<IActionResult> DeleteRequired([FromQuery] List<string> fileNames, CancellationToken cancellationToken)
        {
            try
            {
                var docs = _documentService.ReadRequiredDocuments(fileNames);
                await _elasticService.DeleteAsync(docs, cancellationToken);

                return Ok("Удаление указанных документов прошло успешно");
            }
            catch (ApplicationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete]
        [Route("delete")]
        public async Task<IActionResult> Delete(CancellationToken cancellationToken)
        {
            try
            {
                var docs = _documentService.ReadAllDocuments();
                await _elasticService.DeleteAsync(docs, cancellationToken);

                return Ok("Удаление всех документов прошло успешно");
            }
            catch (ApplicationException ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
