using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TwoWindowsMVVM.Services;

public interface IMessageBus
{
    IDisposable RegisterHandler<T>(Action<T> Handler);

    void Send<T>(T message);
}
