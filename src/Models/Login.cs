using Gtk;
using UI = Gtk.Builder.ObjectAttribute;

namespace Contra.Model;

public abstract class Login : Window
{
    [UI] private Box _Container;
    [UI] private Box _ControlButtonContainer;

    protected int _SceneIndex = 0;
    protected Box[] _Scenes;

    [UI] protected Button _CancelBtn;
    [UI] protected Button _BackBtn;
    [UI] protected Button _NextBtn;

    public void CancelHandler(object? sender, EventArgs e)
    {
        Application.Quit();
    }

    public void BackHandler(object? sender, EventArgs e)
    {
        _Container.Remove(_Scenes[_SceneIndex]);
        _SceneIndex--;
        if (_SceneIndex == _Scenes.Length)
        {
            this.Destroy();
            App.AddWindow(Login.View());
        }
        else
        {
            _Container.Add(_Scenes[_SceneIndex]);
            _Container.ReorderChild(_ControlButtonContainer, 1);
        };
    }

    public void NextHandler(object? sender, EventArgs e)
    {
        _Container.Remove(_Scenes[_SceneIndex]);
        _SceneIndex++;
        if (_SceneIndex == _Scenes.Length)
        {
            this.Destroy();
            App.AddWindow(Login.View());
        }
        else
        {
            _Container.Add(_Scenes[_SceneIndex]);
            _Container.ReorderChild(_ControlButtonContainer, 1);
        };
    }

    public static Login View()
    {
        return Config.Setup ? new Normal() : new Setup();
    }

    public Login(Builder builder) : base(builder.GetRawOwnedObject("Login"))
    {
        builder.Autoconnect(this);
        DeleteEvent += (sender, e) => this.Hide();

        _CancelBtn.Clicked += CancelHandler;
        _BackBtn.Clicked += BackHandler;
        _NextBtn.Clicked += NextHandler;

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
    [UI] private Box _Container;
    [UI] private Box _MethodSelectBox;

    [UI] private Box _KeyBox;
    [UI] private Box _PasswordBox;
    [UI] private Box _NoneBox;

    [UI] private RadioButton _KeySelect;
    [UI] private RadioButton _PasswordSelect;
    [UI] private RadioButton _NoneSelect;

    private void GetSelectedMethod(object? sender, EventArgs e)
    {
        if (_KeySelect.Active)
        {
            Config.Level = Config.SecurityLevel.Key;
            _Scenes[1] = _KeyBox;
        }
        else if (_PasswordSelect.Active)
        {
            Config.Level = Config.SecurityLevel.Password;
            _Scenes[1] = _PasswordBox;
        }
        else if (_NoneSelect.Active)
        {
            Config.Level = Config.SecurityLevel.None;
            _Scenes[1] = _NoneBox;
        }
    }


    public Setup() : base(new Builder("Login.Setup.glade"))
    {
        _Scenes = new Box[] { _MethodSelectBox, _NoneBox };

        _KeySelect.Toggled += GetSelectedMethod;
        _PasswordSelect.Toggled += GetSelectedMethod;
        _NoneSelect.Toggled += GetSelectedMethod;
    }
}
