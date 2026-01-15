using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ChuanLeMaServer.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class FileController : ControllerBase
    {
        private readonly ILogger<FileController> _logger;
        public FileController(ILogger<FileController> logger)
        {
            _logger = logger;
        }
        [HttpGet("FileList")]
        public IActionResult FileList()
        {
            _logger.LogInformation("FileList endpoint called.");
            return Ok(new string[] { "file1.txt", "file2.txt", "image.png" });
        }
    }
}
