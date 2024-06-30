using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;
using gps_jamming_classifier_be.Data;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace gps_jamming_classifier_be.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DataManagementController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public DataManagementController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetUserData()
        {
            var userId = User.FindFirstValue(ClaimTypes.Name); // Lấy userId từ token

            var data = await _context.SignalDatas
                .Where(d => d.UserId == userId)
                .Select(d => new {
                    d.Id,
                    d.FileName,
                    d.Timestamp
                })
                .ToListAsync();

            if (data == null || data.Count == 0)
            {
                return NotFound("No data found.");
            }
            return Ok(data);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteData(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.Name); // Lấy userId từ token

            var data = await _context.SignalDatas.FirstOrDefaultAsync(d => d.Id == id && d.UserId == userId);
            if (data == null)
            {
                return NotFound("Data not found or you do not have permission to delete this data.");
            }

            _context.SignalDatas.Remove(data);
            await _context.SaveChangesAsync();
            return Ok();
        }
    }
}
