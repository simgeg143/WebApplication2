using Microsoft.AspNetCore.Mvc;

namespace WebApplication2.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UploadsController : ControllerBase
    {
        private readonly IWebHostEnvironment webHostEnvironment;

        public UploadsController(IWebHostEnvironment env)
        {
            webHostEnvironment = env;
        }

        [HttpPost]
        public async Task<IActionResult> Upload(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("No file uploaded.");
            var uploadsFolder = Path.Combine(webHostEnvironment.WebRootPath, "uploads");
            if(!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }
            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
            var filePath = Path.Combine(uploadsFolder, fileName);

            using(var stream = new FileStream(filePath,FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }
            var fileUrl = $"/uploads/{fileName}";
            return Ok(new{fileUrl});
        }
    }
}
