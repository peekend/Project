using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using nnnn;

namespace Diplom2.Controllers
{
    //протетсировать
    public class AINetworkController : Controller
    {
        private byte[] ConvertToByteArray(IFormFile file)
        {
            var memoryStream = new MemoryStream();
            file.CopyToAsync(memoryStream);
            return memoryStream.ToArray();
        }
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public IActionResult ScanAdmin()
        {
            return View("~/Views/AINetwork/ScanAdmin.cshtml");
        }
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public IActionResult ScanAdmin(IFormFile image)
        {
            if (image == null)
                throw new ArgumentNullException(nameof(image));
            byte[] imageData = ConvertToByteArray(image);
            var nn = new NeuronNetwork();
            var result = nn.NeuronPredict(imageData);
            result = result.Replace("_", " ");
            ViewBag.ScanResult = result;
            return View("ScanResult");
        }
        [Authorize]
        [HttpGet]
        public IActionResult Scan()
        {
            return View("~/Views/AINetwork/Scan.cshtml");
        }
        [Authorize]
        [HttpPost]
        public IActionResult Scan(IFormFile image)
        {
            if (image == null)
                throw new ArgumentNullException(nameof(image));
            byte[] imageData = ConvertToByteArray(image);
            var nn = new NeuronNetwork();
            var result = nn.NeuronPredict(imageData);
            ViewBag.ScanResult = result;
            return Content("ScanResult");
        }
        
    }
}
