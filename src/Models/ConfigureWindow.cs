using Gtk;
using UI = Gtk.Builder.ObjectAttribute;

namespace Contra.Models;

public class ConfigureWindow : Window
{
    private AppWindow _app;

    [UI] private RadioButton _keyBtn;
    [UI] private RadioButton _passwordBtn;
    [UI] private RadioButton _noneBtn;

    [UI] private RadioButton _creationBtn;
    [UI] private RadioButton _rCreationBtn;
    [UI] private RadioButton _alphabetBtn;
    [UI] private RadioButton _rAlphabetBtn;


    [UI] private Button _resetBtn;

    private void NewKeyEventHandler(object? sender, EventArgs e)
    {
        if (_keyBtn.Active)
        {
            Config.Security = Config.SecurityLevel.Key;
            var key = new Security.Key();
            Security.Key.AsWindow(key);

            var cryptor = new Security.Cryptor(key);
            _app.ChangeCryptor(cryptor);
            using (var sw = new StreamWriter(Config.CheckPath))
                sw.Write(cryptor.Encrypt("Contrasena"));
        }
    }

    private void SubmitPasswordEventHandler(object? sender, EventArgs e)
    {
        var passwordEntry = (Entry)sender;

        if (passwordEntry.Text.Length < 8)
        {
            passwordEntry.Text = "Password must be atleast 8 characters";
        }
        else
        {
            Config.Security = Config.SecurityLevel.Password;
            var cryptor = new Security.Cryptor(passwordEntry.Text);
            _app.ChangeCryptor(cryptor);
            using (var sw = new StreamWriter(Config.CheckPath))
                sw.Write(cryptor.Encrypt("Contrasena"));

            passwordEntry.Parent.Destroy();
        }
    }

    private void NewPasswordEventHandler(object? sender, EventArgs e)
    {
        if (_passwordBtn.Active)
        {
            var window = new Window("");
            window.DefaultWidth = 500;
            window.Resizable = false;
            window.Deletable = false;
            window.Modal = true;
            window.IconName = "security-high";

            var passwordEntry = new Entry();
            passwordEntry.Margin = 20;
            passwordEntry.Activated += SubmitPasswordEventHandler;

            window.Add(passwordEntry);
            App.AddWindow(window);
        }
    }

    private void NoMethodEventHandler(object? sender, EventArgs e)
    {
        if (_noneBtn.Active)
        {
            Config.Security = Config.SecurityLevel.None;
            var cryptor = new Security.Cryptor();
            _app.ChangeCryptor(cryptor);
            using (var sw = new StreamWriter(Config.CheckPath))
                sw.Write(cryptor.Encrypt("Contrasena"));
        }
    }

    private void SortByCreationEventHander(object? sender, EventArgs e)
    {
        if (_creationBtn.Active) Config.SortMethod = Config.Sort.Creation;
    }

    private void SortByRCreationEventHandler(object? sender, EventArgs e)
    {
        if (_rCreationBtn.Active) Config.SortMethod = Config.Sort.ReverseCreation;
    }

    private void SortByAlphabetEventHandler(object? sender, EventArgs e)
    {
        if (_alphabetBtn.Active) Config.SortMethod = Config.Sort.Alphabet;
    }

    private void SortByRAlphabetEventHandler(object? sender, EventArgs e)
    {
        if (_rAlphabetBtn.Active) Config.SortMethod = Config.Sort.ReverseAlphabet;
    }

    private void ResetEventHandler(object? sender, EventArgs e)
    {
        EntryManager.Reset();

        File.Delete(Config.ConfigPath);
        File.Delete(Config.CheckPath);
        File.Delete(Config.DataPath);

        this.Destroy();
        _app.Destroy();
        Config.Reset();
        Config.Get();
        LoginWindow.View();
    }

    public ConfigureWindow(AppWindow app) : this(new Builder("ConfigureWindow.glade"))
    {
        _app = app;
    }

    private ConfigureWindow(Builder builder) : base(builder.GetRawOwnedObject("Configure"))
    {
        builder.Autoconnect(this);
        DeleteEvent += (sender, e) => this.Destroy();

        switch (Config.Security)
        {
            case Config.SecurityLevel.Key:
                {
                    _keyBtn.Active = true;
                    break;
                }
            case Config.SecurityLevel.Password:
                {
                    _passwordBtn.Active = true;
                    break;
                }
            case Config.SecurityLevel.None:
                {
                    _noneBtn.Active = true;
                    break;
                }
        }

        switch (Config.SortMethod)
        {
            case Config.Sort.Creation:
                {
                    _creationBtn.Active = true;
                    break;
                }
            case Config.Sort.ReverseCreation:
                {
                    _rCreationBtn.Active = true;
                    break;
                }
            case Config.Sort.Alphabet:
                {
                    _alphabetBtn.Active = true;
                    break;
                }
            case Config.Sort.ReverseAlphabet:
                {
                    _rAlphabetBtn.Active = true;
                    break;
                }
        }

        _keyBtn.Clicked += NewKeyEventHandler;
        _passwordBtn.Clicked += NewPasswordEventHandler;
        _noneBtn.Clicked += NoMethodEventHandler;

        _creationBtn.Clicked += SortByCreationEventHander;
        _rCreationBtn.Clicked += SortByRCreationEventHandler;
        _alphabetBtn.Clicked += SortByAlphabetEventHandler;
        _rAlphabetBtn.Clicked += SortByRAlphabetEventHandler;

        _resetBtn.Clicked += ResetEventHandler;
    }
}
