using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MQTTnet.Client;
using MQTTnet.Client.Options;
using MQTTnet;

namespace MyStromRelay.Models
{
    public class MqttReporterModel : IReporterModel
    {
        private readonly ILogger<NodeRedReporterModel> _logger;
        private readonly IMqttClient _client;

        private readonly string _mqttTopic;

        public MqttReporterModel(ILogger<NodeRedReporterModel> logger)
        {
            _logger = logger;

            _mqttTopic = Environment.GetEnvironmentVariable("MQTT_TOPIC") ?? "mystrom_button";

            var factory = new MqttFactory();
            _client = factory.CreateMqttClient();

            var options = new MqttClientOptions
            {
                ChannelOptions = new MqttClientTcpOptions
                {
                    Server = Environment.GetEnvironmentVariable("MQTT_SERVER"),
                    Port = int.Parse(Environment.GetEnvironmentVariable("MQTT_SERVER_PORT") ?? "0"),
                    TlsOptions = new MqttClientTlsOptions
                    {
                        UseTls = bool.Parse(Environment.GetEnvironmentVariable("MQTT_SERVER_USE_TLS" ?? "false")),
                        AllowUntrustedCertificates = bool.Parse(Environment.GetEnvironmentVariable("MQTT_SERVER_ALLOW_UNTRUSTED_CERTS" ?? "false"))
                    }
                }
            };

            _client.ConnectAsync(options);

            _client.UseConnectedHandler(e => Console.WriteLine("### CONNECTED WITH SERVER ###"));
        }

        public async Task ReportButtonPress(string buttonId, string action)
        {
            var topic = $"{_mqttTopic}/{action}";

            _logger.LogTrace($"Reporting button press {buttonId}:{action} to MQTT topic {topic}");

            var applicationMessage = new MqttApplicationMessageBuilder()
                        .WithTopic(topic)
                        .WithPayload(buttonId)
                        .WithAtMostOnceQoS()
                        .Build();

            await _client.PublishAsync(applicationMessage);
        }

        public async Task ReportBatteryStatus(string buttonId, int level)
        {
            var topic = $"{_mqttTopic}/battery";

            _logger.LogTrace($"Reporting battery status {level} to MQTT topic {topic}");

            var applicationMessage = new MqttApplicationMessageBuilder()
                        .WithTopic(topic)
                        .WithPayload($"{level}")
                        .WithAtMostOnceQoS()
                        .Build();

            await _client.PublishAsync(applicationMessage);
        }
    }
}