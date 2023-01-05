using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace TwoWindowsMVVM.ViewModels.Base;

public abstract class ViewModel : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;

    // [NotifyPropertyChangedInvocator]
    protected virtual void OnPropertyChanged([CallerMemberName] string PropertyName = null!) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(PropertyName));

    // [NotifyPropertyChangedInvocator]
    protected virtual bool Set<T>(ref T field, T value, [CallerMemberName] string PropertyName = null!)
    {
        if (Equals(field, value)) return false;

        field = value;
        OnPropertyChanged(PropertyName);
        return true;
    }

    private readonly Lazy<Dictionary<string, object?>> _Values = new(() => new(), LazyThreadSafetyMode.PublicationOnly);

    protected virtual T? Get<T>([CallerMemberName] string PropertyName = null!)
    {
        if (PropertyName is null) throw new ArgumentNullException(nameof(PropertyName));

        if (!_Values.IsValueCreated) return default;

        if (_Values.Value is not { Count: > 0 } values) return default;

        return values.TryGetValue(PropertyName, out var value) ? (T?)value : default;
    }

    protected virtual bool Set<T>(T value, [CallerMemberName] string PropertyName = null!)
    {
        if (PropertyName is null) throw new ArgumentNullException(nameof(PropertyName));

        var values = _Values.Value;
        if (values.TryGetValue(PropertyName, out var old_value) && Equals(old_value, value))
            return false;

        values[PropertyName] = value;
        OnPropertyChanged(PropertyName);
        return true;
    }
}
