using Gtk;
using UI = Gtk.Builder.ObjectAttribute;

namespace Contra.Models;

public abstract class Login : Window
{
    [UI] protected Box _container;
    [UI] protected Box _controlButtonContainer;

    [UI] protected Button _cancelBtn;
    [UI] protected Button _backBtn;
    [UI] protected Button _nextBtn;

    protected Security.Cryptor Cryptor;

    private void CancelEventHandler(object? sender, EventArgs e)
    {
        Application.Quit();
    }

    protected void BackEventHandler(object? sender, EventArgs e)
    {
        if (Scene.Current().Back())
        {
            if (Scene.AtFirst())
            {
                Application.Quit();
            }
            else
            {
                _container.Remove(Scene.Current().Model);
                Scene.Index--;
                _container.Add(Scene.Current().Model);
                _container.ReorderChild(_controlButtonContainer, 1);
            }
        }
    }

    protected void NextEventHandler(object? sender, EventArgs e)
    {
        if (Scene.Current().Next())
        {
            if (Scene.AtLast())
            {
                Authenticate();
            }
            else
            {
                _container.Remove(Scene.Current().Model);
                Scene.Index++;
                _container.Add(Scene.Current().Model);
                _container.ReorderChild(_controlButtonContainer, 1);
            }
        }
    }

    protected Login(Builder builder) : base(builder.GetRawOwnedObject("Login"))
    {
        builder.Autoconnect(this);
        DeleteEvent += (sender, e) => this.Hide();

        _cancelBtn.Clicked += CancelEventHandler;
        _backBtn.Clicked += BackEventHandler;
        _nextBtn.Clicked += NextEventHandler;
    }

    public void Authenticate()
    {
        if (Config.Check == "Contrasena") Config.Check = Cryptor.Encrypt("Contrasena");
        else if (Cryptor.Decrypt(Config.Check) != "Contrasena") return;

        this.Destroy();
        App.AddWindow(new AppWindow(Cryptor));
    }

    public static Login View()
    {
        return Config.Setup ? new Normal() : new Setup();
    }
}

public class Normal : Login
{
    public Normal() : base(new Builder("Login.Normal.glade"))
    {
    }
}

public class Setup : Login
{
    [UI] private Box _methodSelectBox;

    [UI] private Box _keyBox;
    [UI] private Box _passwordBox;
    [UI] private Box _noneBox;

    [UI] private RadioButton _keySelect;
    [UI] private RadioButton _passwordSelect;
    [UI] private RadioButton _noneSelect;

    private void MethodSelectEventHandler(object? sender, EventArgs e)
    {
        if (_keySelect.Active)
        {
            Config.Level = Config.SecurityLevel.Key;
            Scene.Scenes[1] = new(_keyBox);
        }
        else if (_passwordSelect.Active)
        {
            Config.Level = Config.SecurityLevel.Password;
            Scene.Scenes[1] = new(_passwordBox, null, SubmitPasswordEventHandler);
        }
        else if (_noneSelect.Active)
        {
            Config.Level = Config.SecurityLevel.None;
            Scene.Scenes[1] = new(_noneBox, null, NoSecuritySelectEventHandler);
        }
    }

    [UI] private Button _newKeyBtn;

    [UI] private Window _keyWindow;
    [UI] private Label _keyLabel;
    [UI] private Button _closeKeyWindowBtn;

    private void GenerateKeyEventHandler(object? sender, EventArgs e)
    {
        Security.Key key = new();

        _keyLabel.Text = key.ToString();
        Cryptor = new(key);
        _keyWindow.ShowAll();
    }

    [UI] private Entry _passwordEntry;
    [UI] private Entry _passwordEntryConfirm;
    [UI] private Label _passwordLabel;

    private bool SubmitPasswordEventHandler()
    {
        if (_passwordEntry.Text.Length < 8)
        {
            _passwordLabel.Text = "Password must be atleast 8 characters";
        }
        else if (_passwordEntry.Text == _passwordEntryConfirm.Text)
        {
            _passwordLabel.Text = "";
            Cryptor = new(_passwordEntry.Text);
            return true;
        }
        else
        {
            _passwordLabel.Text = "Entries do not match";
        }
        return false;
    }

    private bool NoSecuritySelectEventHandler()
    {
        Cryptor = new();
        return true;
    }

    public Setup() : base(new Builder("Login.Setup.glade"))
    {
        Scene.Scenes = new Scene[] {
            new(_methodSelectBox),
            new(_noneBox)
        };
        Cryptor = new();

        _keyWindow.Hide();
        _closeKeyWindowBtn.Clicked += (sender, e) => _keyWindow.Hide();

        _keySelect.Toggled += MethodSelectEventHandler;
        _passwordSelect.Toggled += MethodSelectEventHandler;
        _noneSelect.Toggled += MethodSelectEventHandler;

        _newKeyBtn.Clicked += GenerateKeyEventHandler;
    }
}
