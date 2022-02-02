using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MyStromRelay.Models;

namespace MyStromRelay.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ButtonsController : ControllerBase
    {
        private readonly string _secret;

        private readonly IReporterModel _reporterModel;

        public ButtonsController(IReporterModel reporterModel)
        {
            _reporterModel = reporterModel;
            _secret = Environment.GetEnvironmentVariable("MYSTROM_RELAY_SECRET");
        }

        public static async Task<string> GetRawBodyStringAsync(HttpRequest request, Encoding encoding = null)
        {
            if (encoding == null)
                encoding = Encoding.UTF8;

            using StreamReader reader = new(request.Body, encoding);
            return await reader.ReadToEndAsync();
        }

        [HttpPost]
        public async Task<IActionResult> ButtonPressed([FromForm] string secret, [FromForm] string id, [FromForm] string action)
        {
            if (!String.IsNullOrWhiteSpace(_secret) && _secret != secret)
            {
                return Unauthorized("Secret does not match");
            }

            await _reporterModel.ReportButtonPress(id, action);
            return Ok();
        }

        [HttpPost]
        [Route("status")]
        public async Task<IActionResult> ReportStatusAsync([FromForm] string secret, [FromForm] string mac, [FromForm] string action, [FromForm] string wheel, [FromForm] int battery)
        {
            if (!String.IsNullOrWhiteSpace(_secret) && _secret != secret)
            {
                return Unauthorized("Secret does not match");
            }

            await _reporterModel.ReportBatteryStatus(mac, battery);
            return Ok();
        }
    }
}
