namespace TwoWindowsMVVM.Services.Implementations;

public class MessageBusService : IMessageBus
{
    public IDisposable RegisterHandler<T>(Action<T> Handler) { throw new NotImplementedException(); }

    public void Send<T>(T message) { throw new NotImplementedException(); }
}
