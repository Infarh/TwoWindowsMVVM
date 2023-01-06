using System.Collections.ObjectModel;
using System.Windows.Input;

using TwoWindowsMVVM.Commands;
using TwoWindowsMVVM.Models;
using TwoWindowsMVVM.Services;
using TwoWindowsMVVM.ViewModels.Base;

namespace TwoWindowsMVVM.ViewModels;

public class SecondaryWindowViewModel : DialogViewModel, IDisposable
{
    private readonly IUserDialog _UserDialog;
    private readonly IMessageBus _MessageBus;
    private readonly IDisposable _MessageUnsubscriber;

    public SecondaryWindowViewModel()
    {
        Title = "Вторичное окно";
    }

    public SecondaryWindowViewModel(IUserDialog UserDialog, IMessageBus MessageBus) : this()
    {
        IsInDesignMode   = false;
        _UserDialog      = UserDialog;
        _MessageBus = MessageBus;

        _MessageUnsubscriber = MessageBus.RegisterHandler<Message>(msg => _Messages.Add(new(msg.Text)));
    }

    public void Dispose()
    {
        _MessageUnsubscriber.Dispose();
    }

    public bool IsInDesignMode { get; } = true;

    #region Message : string? - Сообщение

    /// <summary>Сообщение</summary>
    private string? _Message;

    /// <summary>Сообщение</summary>
    public string? Message { get => _Message; set => Set(ref _Message, value); }

    #endregion

    private readonly ObservableCollection<TextMessageModel> _Messages = new();

    public IEnumerable<TextMessageModel> Messages => _Messages;

    #region Command SendMessageCommand - Отправить сообщение первому окну

    /// <summary>Отправить сообщение первому окну</summary>
    private LambdaCommand? _SendMessageCommand;

    /// <summary>Отправить сообщение первому окну</summary>
    public ICommand SendMessageCommand => _SendMessageCommand ??= new(OnSendMessageCommandExecuted, p => p is string { Length: > 0 });

    /// <summary>Логика выполнения - Отправить сообщение первому окну</summary>
    private void OnSendMessageCommandExecuted(object? p)
    {
        _MessageBus.Send(new Message((string)p!));
    }

    #endregion

    #region Command OpenMainWindowCommand - Открыть первое окно

    /// <summary>Открыть первое окно</summary>
    private LambdaCommand? _OpenMainWindowCommand;

    /// <summary>Открыть первое окно</summary>
    public ICommand OpenMainWindowCommand => _OpenMainWindowCommand ??= new(OnOpenMainWindowCommandExecuted);

    /// <summary>Логика выполнения - Открыть первое окно</summary>
    private void OnOpenMainWindowCommandExecuted()
    {
        _UserDialog.OpenMainWindow();
    }

    #endregion

    #region Command ChangeToMainWindowCommand - Перейти во первое окно

    /// <summary>Перейти во первое окно</summary>
    private LambdaCommand? _ChangeToMainWindowCommand;

    /// <summary>Перейти во первое окно</summary>
    public ICommand ChangeToMainWindowCommand => _ChangeToMainWindowCommand ??= new(OnChangeToMainWindowCommandExecuted);

    /// <summary>Логика выполнения - Перейти во первое окно</summary>
    private void OnChangeToMainWindowCommandExecuted()
    {
        _UserDialog.OpenMainWindow();
        OnDialogComplete(EventArgs.Empty);
    }

    #endregion
}
