using MqttReceiver;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddHostedService<MessageReceiver>();

var host = builder.Build();
host.Run();
