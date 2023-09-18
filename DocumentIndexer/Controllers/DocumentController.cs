using DocumentIndexer.Services;
using Microsoft.AspNetCore.Mvc;

namespace DocumentIndexer.Controllers
{
    [ApiController]
    [Route("api/doc")]
    public class DocumentController : Controller
    {
        private readonly DocumentService _documentService;

        public DocumentController(DocumentService documentService)
        {
            _documentService = documentService;
        }

        [HttpGet]
        [Route("get/{file}")]
        public IActionResult Get([FromRoute] string file)
        {
            try
            {
                return Ok(_documentService.ReadDocument(file));
            }
            catch (ApplicationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        [Route("all")]
        public IActionResult GetAll()
        {
            try
            {
                return Ok(_documentService.ReadAllDocuments());
            }
            catch (ApplicationException ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
