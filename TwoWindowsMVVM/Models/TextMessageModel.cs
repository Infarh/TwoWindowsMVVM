using TwoWindowsMVVM.ViewModels.Base;

namespace TwoWindowsMVVM.Models;

public class TextMessageModel : ViewModel
{
    public DateTime Time { get; }

    public string Text { get; }

    public TextMessageModel(string Message) : this(DateTime.Now, Message) { }

    public TextMessageModel(DateTime Time, string Message) => (this.Time, Text) = (Time, Message);
}
