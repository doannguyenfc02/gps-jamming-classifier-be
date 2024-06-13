using gps_jamming_classifier_be.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace gps_jamming_classifier_be.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FileUploadController : ControllerBase
    {
        private readonly SignalProcessingService _signalProcessingService;
        private readonly ILogger<FileUploadController> _logger;

        public FileUploadController(SignalProcessingService signalProcessingService, ILogger<FileUploadController> logger)
        {
            _signalProcessingService = signalProcessingService;
            _logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> UploadFile(IFormFile file, [FromForm] int numImages, [FromForm] double fs, [FromForm] double time)
        {
            if (file == null || file.Length == 0)
            {
                _logger.LogError("No file uploaded.");
                return BadRequest("No file uploaded.");
            }

            try
            {
                var resultDescription = await _signalProcessingService.ProcessFile(file, numImages, fs, time);
                _logger.LogInformation("File processed successfully.");
                return Ok(resultDescription);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing file.");
                return StatusCode(500, "Internal server error");
            }
        }
    }
}