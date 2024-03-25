using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MqttDeviceSimulator;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddHostedService<DeviceSimulator>();
builder.Services.AddHostedService<DeviceSimulatorB>();

var host = builder.Build();
host.Run();