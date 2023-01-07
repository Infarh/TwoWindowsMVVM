namespace TwoWindowsMVVM.Models;

public class TextMessageModel
{
    public DateTime Time { get; }

    public string Text { get; }

    public TextMessageModel(string Message) : this(DateTime.Now, Message) { }

    public TextMessageModel(DateTime Time, string Message) => (this.Time, Text) = (Time, Message);
}
