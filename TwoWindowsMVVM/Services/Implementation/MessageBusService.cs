namespace TwoWindowsMVVM.Services.Implementation;

public class MessageBusService : IMessageBus
{
    private class Subscription<T> : IDisposable
    {
        private readonly WeakReference<MessageBusService> _Bus;
        public Action<T> Handler { get; }

        public Subscription(MessageBusService Bus, Action<T> Handler)
        {
            this.Handler = Handler;
            _Bus     = new(Bus);
        }

        public void Dispose()
        {
            if(!_Bus.TryGetTarget(out var bus))
                return;

            var @lock = bus._Lock;
            @lock.EnterWriteLock();
            var message_type = typeof(T);
            try
            {
                if(!bus._Subscriptions.TryGetValue(message_type, out var refs))
                    return;

                var updated_refs = refs.Where(r => r.IsAlive).ToList();

                WeakReference? current_ref = null;
                foreach (var @ref in updated_refs)
                    if (ReferenceEquals(@ref.Target, this))
                    {
                        current_ref = @ref;
                        break;
                    }

                if(current_ref is null)
                    return;

                updated_refs.Remove(current_ref);

                bus._Subscriptions[message_type] = updated_refs;
            }
            finally
            {
                @lock.ExitWriteLock();
            }
        }
    }

    private readonly ReaderWriterLockSlim _Lock = new();
    private readonly Dictionary<Type, IEnumerable<WeakReference>> _Subscriptions = new();

    public IDisposable RegisterHandler<T>(Action<T> handler)
    {
        var subscription = new Subscription<T>(this, handler);

        _Lock.EnterWriteLock();
        try
        {
            var subscription_ref = new WeakReference(subscription);
            var message_type     = typeof(T);
            _Subscriptions[message_type] = _Subscriptions.TryGetValue(message_type, out var subscriptions)
                ? subscriptions.Append(subscription_ref)
                : new[] { subscription_ref };
        }
        finally
        {
            _Lock.ExitWriteLock();
        }

        return subscription;
    }

    private IEnumerable<Action<T>>? GetHandlers<T>()
    {
        var exist_die_refs = false;
        var handlers       = new List<Action<T>>();
        var message_type   = typeof(T);
        _Lock.EnterReadLock();
        try
        {
            if (!_Subscriptions.TryGetValue(message_type, out var refs))
                return null;

            foreach(var @ref in refs)
                if (@ref.Target is Subscription<T> { Handler: var handler })
                    handlers.Add(handler);
                else
                    exist_die_refs = true;
        }
        finally
        {
            _Lock.ExitReadLock();
        }

        if (!exist_die_refs) return handlers;

        _Lock.EnterWriteLock();
        try
        {
            if (_Subscriptions.TryGetValue(message_type, out var refs))
                if (refs.Where(r => r.IsAlive).ToArray() is not { Length: > 0 } new_refs)
                    _Subscriptions.Remove(message_type);
                else
                    _Subscriptions[message_type] = new_refs;
        }
        finally
        {
            _Lock.ExitWriteLock();
        }

        return handlers;
    }

    public void Send<T>(T message)
    {
        if(GetHandlers<T>() is not { } handlers)
            return;

        foreach (var handler in handlers)
            handler(message);
    }

    public Task SendAsync<T>(T message, CancellationToken Cancel = default)
    {
        if (Cancel.IsCancellationRequested)
            return Task.FromCanceled(Cancel);

        if (GetHandlers<T>() is not { } handlers)
            return Task.CompletedTask;

        return Task.Factory.StartNew(
            o =>
            {
                var hnd = (IEnumerable<Action<T>>)((object[])o!)[0];
                var msg = (T)((object[])o)[1];
                var cancel = (CancellationToken)((object[])o)[1];

                foreach (var handler in hnd)
                {
                    cancel.ThrowIfCancellationRequested();
                    handler(msg);
                }

                cancel.ThrowIfCancellationRequested();
            },
            new object[] { handlers, message!, Cancel }, 
            Cancel);
    }
}