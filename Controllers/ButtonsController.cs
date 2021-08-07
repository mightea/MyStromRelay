using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MyStromRelay.Models;

namespace MyStromRelay.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ButtonsController : ControllerBase
    {
        private readonly string _secret;

        private readonly NodeRedReporterModel _nodeRedReporter;

        public ButtonsController(NodeRedReporterModel nodeRedReporter)
        {
            _nodeRedReporter = nodeRedReporter;
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

            await _nodeRedReporter.ReportButtonPress(id, action);
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

            await _nodeRedReporter.ReportBatteryStatus(mac, battery);
            return Ok();
        }
    }
}
