namespace Interfaces
{
    public record IotMessage<T> ( T Message, DateTimeOffset Timestamp, string Type);
}
