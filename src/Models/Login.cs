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

    private void CancelHandler(object? sender, EventArgs e)
    {
        Application.Quit();
    }

    private void BackHandler(object? sender, EventArgs e)
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

    private void NextHandler(object? sender, EventArgs e)
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

    protected Login(Builder builder) : base(builder.GetRawOwnedObject("Login"))
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

    [UI] private Button _NewKeyBtn;

    private void MethodSelectHandler(object? sender, EventArgs e)
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

    private void GenerateKeyHandler(object? sender, EventArgs e)
    {
        Window generatedKeyDialog = new(" ");
        generatedKeyDialog.Resizable = false;
        generatedKeyDialog.IconName = "security-high";
        generatedKeyDialog.Modal = true;

        Label key = new("Placeholder");
        key.Visible = true;
        key.Margin = 20;
        key.Selectable = true;

        Pango.AttrList attr = new();
        attr.Insert(new Pango.AttrWeight(Pango.Weight.Ultraheavy));
        attr.Insert(new Pango.AttrScale(2));
        key.Attributes = attr;

        generatedKeyDialog.Add(key);
        App.AddWindow(generatedKeyDialog);
    }


    public Setup() : base(new Builder("Login.Setup.glade"))
    {
        _Scenes = new Box[] { _MethodSelectBox, _NoneBox };

        _KeySelect.Toggled += MethodSelectHandler;
        _PasswordSelect.Toggled += MethodSelectHandler;
        _NoneSelect.Toggled += MethodSelectHandler;

        _NewKeyBtn.Clicked += GenerateKeyHandler;
    }
}
