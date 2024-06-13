using gps_jamming_classifier_be.Data;
using gps_jamming_classifier_be.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace gps_jamming_classifier_be.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SpectrogramsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public SpectrogramsController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("{signalDataId}")]
        public async Task<IActionResult> GetSpectrogramsBySignalDataId(int signalDataId)
        {
            var spectrograms = await _context.Spectrograms
                .Where(s => s.SignalDataId == signalDataId)
                .Select(s => new {
                    s.Id,
                    s.ImageName,
                    DataBase64 = s.DataBase64 != null ? s.DataBase64 : null,// Trả về giá trị DataBase64 nếu không null
                    s.Class,
                    s.SignalDataId
                })
                .ToListAsync();


            if (spectrograms == null || spectrograms.Count == 0)
            {
                return NotFound("No spectrograms found.");
            }
            return Ok(spectrograms);
        }
    }
}
