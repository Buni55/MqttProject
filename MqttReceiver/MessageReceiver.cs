using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Interfaces;
using MQTTnet;
using MQTTnet.Client;
using System;
using System.Text;
using System.Text.Json;
using MQTTnet.Packets;
using MQTTnet.Protocol;
using System.Threading;
using System.Threading.Tasks;


namespace MqttReceiver
{
    public class MessageReceiver(ILogger<MessageReceiver> logger) : BackgroundService
    {
        private readonly ILogger<MessageReceiver> _logger = logger;
        private readonly MqttFactory _mqttFactory = new();

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var mqttFactory = new MqttFactory();

            using (var mqttClient = mqttFactory.CreateMqttClient())
            {
                var mqttClientOptions = new MqttClientOptionsBuilder()
                    .WithTcpServer("localhost", 1883)
                    .Build();

                mqttClient.ApplicationMessageReceivedAsync += async e =>
                {
                    var messagePayload = Encoding.UTF8.GetString(e.ApplicationMessage.Payload);
                    _logger.LogInformation($"Received message: {messagePayload} on topic: {e.ApplicationMessage.Topic}");
                    // Your processing logic here
                };

                await mqttClient.ConnectAsync(mqttClientOptions, stoppingToken);

                var mqttSubscribeOptions = new MqttClientSubscribeOptionsBuilder()
                    .WithTopicFilter(f => { f.WithTopic("samples/temperature/living_room"); })
                    .WithTopicFilter(f => { f.WithTopic("samples/temperature/kitchen"); })
                    .Build();

                await mqttClient.SubscribeAsync(mqttSubscribeOptions, stoppingToken);
                _logger.LogInformation("MQTT client subscribed to topic.");

                while (!stoppingToken.IsCancellationRequested)
                {
                    await Task.Delay(1000, stoppingToken);
                }

                if (mqttClient.IsConnected)
                {
                    var disconnectOptions = new MqttClientDisconnectOptions();
                    await mqttClient.DisconnectAsync(disconnectOptions, stoppingToken);
                }
            }
        }
    }
}
