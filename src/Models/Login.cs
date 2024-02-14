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


    private void CancelEventHandler(object? sender, EventArgs e)
    {
        Application.Quit();
    }

    protected void BackEventHandler(object? sender, EventArgs e)
    {
        if (Scene.Current().Back())
        {
            _container.Remove(Scene.Current().Model);
            Scene.Index--;
            if (Scene.Index == -1)
            {
                this.Destroy();
                Scene.Index = 0;
                App.AddWindow(Login.View());
            }
            else
            {
                _container.Add(Scene.Current().Model);
                _container.ReorderChild(_controlButtonContainer, 1);
            }
        }
    }

    protected void NextEventHandler(object? sender, EventArgs e)
    {
        if (Scene.Current().Next())
        {
            _container.Remove(Scene.Current().Model);
            Scene.Index++;
            if (Scene.AtLast())
            {
                this.Destroy();
                Scene.Index = 0;
                App.AddWindow(Login.View());
            }
            else
            {
                _container.Add(Scene.Current().Model);
                _container.ReorderChild(_controlButtonContainer, 1);
            }
        }
    }

    public static Login View()
    {
        return Config.Setup ? new Normal() : new Setup();
    }

    public static void Check()
    {

    }

    protected Login(Builder builder) : base(builder.GetRawOwnedObject("Login"))
    {
        builder.Autoconnect(this);
        DeleteEvent += (sender, e) => this.Hide();

        _cancelBtn.Clicked += CancelEventHandler;
        _backBtn.Clicked += BackEventHandler;
        _nextBtn.Clicked += NextEventHandler;
    }
}

class Normal : Login
{
    public Normal() : base(new Builder("Login.Normal.glade"))
    {
    }
}

class Setup : Login
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
            Scene.Scenes[1] = new(_noneBox);
        }
    }

    [UI] private Button _newKeyBtn;

    [UI] private Window _keyWindow;
    [UI] private Label _keyLabel;
    [UI] private Button _closeKeyWindowBtn;

    private void GenerateKeyEventHandler(object? sender, EventArgs e)
    {
        _keyLabel.Text = new Security.Key().ToString();
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
            return true;
        }
        else
        {
            _passwordLabel.Text = "Entries do not match";
        }
        return false;
    }

    public Setup() : base(new Builder("Login.Setup.glade"))
    {
        Scene.Scenes = new Scene[] {
            new(_methodSelectBox),
            new(_noneBox)
        };

        App.AddWindow(_keyWindow);
        _keyWindow.Hide();
        _closeKeyWindowBtn.Clicked += (sender, e) => _keyWindow.Hide();

        _keySelect.Toggled += MethodSelectEventHandler;
        _passwordSelect.Toggled += MethodSelectEventHandler;
        _noneSelect.Toggled += MethodSelectEventHandler;

        _newKeyBtn.Clicked += GenerateKeyEventHandler;
    }
}
