using Gtk;
using UI = Gtk.Builder.ObjectAttribute;

namespace Contra.Models;

public abstract class Login : Window
{
    [UI] protected Box _container;
    [UI] protected Box _controlButtonContainer;

    [UI] private Button _cancelBtn;
    [UI] private Button _backBtn;
    [UI] private Button _nextBtn;

    protected Security.Cryptor _cryptor;

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
                AuthenticateUser();
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

    protected abstract void AuthenticateUser();

    public static Login View()
    {
        return Config.Setup ? new Normal() : new Setup();
    }
}

public class Normal : Login
{
    [UI] private Box _enterKey;
    [UI] private Label _keyError;
    [UI] private Entry _keyEntry;

    [UI] private Box _enterPassword;
    [UI] private Label _passwordError;
    [UI] private Entry _passwordEntry;

    [UI] private Box _enterNone;

    public Normal() : base(new Builder("Login.Normal.glade"))
    {
        switch (Config.Security)
        {
            case Config.SecurityLevel.Key:
                {
                    Scene.Scenes = new(new Scene[] { new(_enterKey) });
                    _container.Add(_enterKey);
                    break;
                }

            case Config.SecurityLevel.Password:
                {
                    Scene.Scenes = new(new Scene[] { new(_enterPassword) });
                    _container.Add(_enterPassword);
                    break;
                }

            case Config.SecurityLevel.None:
                {
                    Scene.Scenes = new(new Scene[] { new(_enterNone) });
                    _container.Add(_enterNone);
                    break;
                }
        }
        _container.ReorderChild(_controlButtonContainer, 1);
    }

    sealed override protected void AuthenticateUser()
    {
        switch (Config.Security)
        {
            case Config.SecurityLevel.Key:
                {
                    var key = Security.Key.FromString(_keyEntry.Text);
                    if (key == null)
                    {
                        _keyError.Text = "Enter a valid key";
                        return;
                    }

                    _cryptor = new(key);
                    try
                    {
                        if (_cryptor.Decrypt(Config.Check) != "Contrasena") throw new Exception();
                    }
                    catch
                    {
                        _keyError.Text = "Incorrect key";
                        return;
                    }
                    break;
                }

            case Config.SecurityLevel.Password:
                {
                    _cryptor = new(_passwordEntry.Text);
                    try
                    {
                        if (_cryptor.Decrypt(Config.Check) != "Contrasena") throw new Exception();
                    }
                    catch
                    {
                        _passwordError.Text = "Incorrect password";
                        return;
                    }
                    break;
                }

            case Config.SecurityLevel.None:
                {
                    _cryptor = new();
                    break;
                }
        }

        this.Destroy();
        App.AddWindow(new AppWindow(_cryptor));
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
            Config.Security = Config.SecurityLevel.Key;
            Scene.Scenes[1] = new(_keyBox);
        }
        else if (_passwordSelect.Active)
        {
            Config.Security = Config.SecurityLevel.Password;
            Scene.Scenes[1] = new(_passwordBox, null, SubmitPasswordEventHandler);
        }
        else if (_noneSelect.Active)
        {
            Config.Security = Config.SecurityLevel.None;
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
        _cryptor = new(key);
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
            _cryptor = new(_passwordEntry.Text);
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
        _cryptor = new();
        return true;
    }

    public Setup() : base(new Builder("Login.Setup.glade"))
    {
        Config.Check = "Contrasena";
        Scene.Scenes = new(new Scene[] {
            new(_methodSelectBox),
            new(_noneBox)
        });
        _cryptor = new();

        _keyWindow.Hide();
        _closeKeyWindowBtn.Clicked += (sender, e) => _keyWindow.Hide();

        _keySelect.Toggled += MethodSelectEventHandler;
        _passwordSelect.Toggled += MethodSelectEventHandler;
        _noneSelect.Toggled += MethodSelectEventHandler;

        _newKeyBtn.Clicked += GenerateKeyEventHandler;
    }

    sealed override protected void AuthenticateUser()
    {
        Config.Setup = true;
        Config.Check = _cryptor.Encrypt("Contrasena");

        this.Destroy();
        App.AddWindow(new AppWindow(_cryptor));
    }
}
