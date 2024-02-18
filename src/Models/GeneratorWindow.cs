using Gtk;
using UI = Gtk.Builder.ObjectAttribute;

namespace Contra.Models;

public class GeneratorWindow : Window
{
    private static char[] _letters = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz".ToCharArray();
    private static char[] _digits = "0123456789".ToCharArray();
    private static char[] _symbols = "!@#$%^&*()-_+=<>?.,;:".ToCharArray();

    [UI] private Label _generatedPassword;
    [UI] private Button _generateBtn;

    [UI] private Entry _lengthEntry;

    [UI] private ToggleButton _useLetters;
    [UI] private ToggleButton _useDigits;
    [UI] private ToggleButton _useSymbols;

    private string GeneratePasswordFromArray(int length, List<char> chars)
    {
        var random = new Random();
        using (var sw = new StringWriter())
        {
            for (int i = 0; i < length; i++) sw.Write(chars[random.Next(0, chars.Count() - 1)]);
            return sw.ToString();
        }
    }

    private void GeneratePasswordEventHandler(object? sender, EventArgs e)
    {
        var charSet = new List<char>();

        if (_useLetters.Active) charSet.AddRange(_letters);
        if (_useDigits.Active) charSet.AddRange(_digits);
        if (_useSymbols.Active) charSet.AddRange(_symbols);
        if (charSet.Count() == 0) return;

        int length = _lengthEntry.Text == "" ? 0 : Int32.Parse(_lengthEntry.Text);

        _generatedPassword.Text = GeneratePasswordFromArray(length, charSet);
    }

    public GeneratorWindow() : this(new Builder("GeneratorWindow.glade")) { }

    public GeneratorWindow(Builder builder) : base(builder.GetRawOwnedObject("Generator"))
    {
        builder.Autoconnect(this);
        DeleteEvent += (sender, e) => this.Destroy();

        _generateBtn.Clicked += GeneratePasswordEventHandler;
    }
}
