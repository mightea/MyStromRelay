using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace MyStromRelay.Models
{
    public class HomeassistantReporterModel : IReporterModel
    {
        internal static string HTTP_CLIENT_NAME = "home-assistant";

        private readonly ILogger<HomeassistantReporterModel> _logger;
        private readonly IHttpClientFactory _clientFactory;

        public HomeassistantReporterModel(ILogger<HomeassistantReporterModel> logger, IHttpClientFactory clientFactory)
        {
            _logger = logger;
            _clientFactory = clientFactory;
        }

        public async Task ReportButtonPress(string buttonId, string action)
        {
            var client = _clientFactory.CreateClient(HTTP_CLIENT_NAME);
            _logger.LogTrace($"Reporting button press {action}={buttonId} to home-assistant");

            var httpResponse = await client.GetAsync($"api/mystrom?{action}={buttonId}");
            httpResponse.EnsureSuccessStatusCode();
        }

        public Task ReportBatteryStatus(string buttonId, int level)
        {
            _logger.LogInformation($"Reporting battery status {level} to home-assistant is not supported");
            return Task.CompletedTask;
        }
    }
}
