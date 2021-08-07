using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace MyStromRelay.Models
{
    public class NodeRedReporterModel
    {
        internal static string HTTP_CLIENT_NAME_NODE_RED = "node-red";

        private readonly ILogger<NodeRedReporterModel> _logger;
        private readonly IHttpClientFactory _clientFactory;

        public NodeRedReporterModel(ILogger<NodeRedReporterModel> logger, IHttpClientFactory clientFactory)
        {
            _logger = logger;
            _clientFactory = clientFactory;
        }

        internal async Task ReportButtonPress(string buttonId, string action)
        {
            var client = _clientFactory.CreateClient(HTTP_CLIENT_NAME_NODE_RED);
            var content = new StringContent(JsonSerializer.Serialize(new
            {
                id = buttonId,
                action
            }), null, "application/json");

            _logger.LogTrace($"Reporting button press {buttonId}:{action} to node-red");

            var httpResponse = await client.PostAsync("buttons/trigger", content);
            httpResponse.EnsureSuccessStatusCode();
        }

        internal async Task ReportBatteryStatus(string buttonId, int level)
        {
            var client = _clientFactory.CreateClient(HTTP_CLIENT_NAME_NODE_RED);
            var content = new StringContent(JsonSerializer.Serialize(new
            {
                id = buttonId,
                level
            }), null, "application/json");

            _logger.LogTrace($"Reporting battery status {level} to node-red");

            var httpResponse = await client.PostAsync("buttons/battery", content);
            httpResponse.EnsureSuccessStatusCode();
        }
    }
}
