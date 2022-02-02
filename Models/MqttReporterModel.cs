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

        private readonly IMqttClientOptions _options;

        private readonly string _mqttTopic;

        public MqttReporterModel(ILogger<NodeRedReporterModel> logger)
        {
            _logger = logger;

            _mqttTopic = Environment.GetEnvironmentVariable("MQTT_TOPIC") ?? "mystrom_button";

            var factory = new MqttFactory();
            _client = factory.CreateMqttClient();

            var optionsBuilder = new MqttClientOptionsBuilder()
                .WithClientId("MyStromRelay")
                .WithTcpServer(Environment.GetEnvironmentVariable("MQTT_SERVER"), int.Parse(Environment.GetEnvironmentVariable("MQTT_SERVER_PORT") ?? "0"))
                .WithTls(new MqttClientOptionsBuilderTlsParameters()
                {
                    UseTls = bool.Parse(Environment.GetEnvironmentVariable("MQTT_SERVER_USE_TLS" ?? "false")),
                    CertificateValidationHandler = (w) => true
                });

            var username = Environment.GetEnvironmentVariable("MQTT_SERVER_USERNAME");
            var password = Environment.GetEnvironmentVariable("MQTT_SERVER_PASSWORD");

            if (!String.IsNullOrWhiteSpace(username) && !String.IsNullOrWhiteSpace(password))
            {
                _logger.LogInformation("Using credentials from env.");
                optionsBuilder.WithCredentials(username, password);
            }

            _options = optionsBuilder.Build();

            _client.UseConnectedHandler(e => _logger.LogTrace("Connected with MQTT server"));
            _client.UseDisconnectedHandler(e => _logger.LogTrace("Disconnected from server"));
        }

        public async Task ReportButtonPress(string buttonId, string action)
        {
            var topic = $"{_mqttTopic}/{action}";

            _logger.LogTrace($"Reporting button press {buttonId}:{action} to MQTT topic {topic}");

            var applicationMessage = new MqttApplicationMessageBuilder()
                        .WithTopic(topic)
                        .WithPayload(buttonId)
                        .WithAtLeastOnceQoS()
                        .Build();

            await _client.ConnectAsync(_options);
            await _client.PublishAsync(applicationMessage);
            await _client.DisconnectAsync();
        }

        public async Task ReportBatteryStatus(string buttonId, int level)
        {
            var topic = $"{_mqttTopic}/battery";

            _logger.LogTrace($"Reporting battery status {level} to MQTT topic {topic}");

            var applicationMessage = new MqttApplicationMessageBuilder()
                        .WithTopic(topic)
                        .WithPayload($"{level}")
                        .WithAtLeastOnceQoS()
                        .Build();


            await _client.ConnectAsync(_options);
            await _client.PublishAsync(applicationMessage);
            await _client.DisconnectAsync();
        }
    }
}