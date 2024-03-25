using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MQTTnet.Client;
using MQTTnet;
using Interfaces;
using System.Text.Json;

namespace MqttDeviceSimulator
{
    public class DeviceSimulator(ILogger<DeviceSimulator> logger) : BackgroundService
    {
        private readonly ILogger<DeviceSimulator> _logger = logger;
        private readonly Random _random = new();
        private readonly MqttFactory _mqttFactory = new();

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var mqttClient = _mqttFactory.CreateMqttClient();
            var mqttClientOptions = new MqttClientOptionsBuilder()
                .WithTcpServer("localhost", 1883)
                .Build();

            try
            {
                await mqttClient.ConnectAsync(mqttClientOptions, stoppingToken);

                while (!stoppingToken.IsCancellationRequested)
                {
                    double temperature = 20 + _random.NextDouble() * 5;
                    string payload = JsonSerializer.Serialize(new { temperature = $"{temperature:0.0}" });
                    
                    var applicationMessage = new MqttApplicationMessageBuilder()
                        .WithTopic("samples/temperature/living_room")
                        .WithPayload(payload)
                        .Build();

                    await mqttClient.PublishAsync(applicationMessage, stoppingToken);

                    _logger.LogInformation($"MQTT application message is published: {payload}");

                    await Task.Delay(1000, stoppingToken);

                    Console.WriteLine("MQTT application message is published.");
            
                }
            } catch (OperationCanceledException) {
                _logger.LogInformation("Operation was canceled");
            } finally {
                if (mqttClient.IsConnected)
                {
                    await mqttClient.DisconnectAsync();
                }
            }
        }
    }
}   
