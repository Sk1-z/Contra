using Gtk;
using UI = Gtk.Builder.ObjectAttribute;

namespace Contra.Model;

public abstract class Login : Window
{
    protected int _SceneIndex = 0;
    protected Box[] _Scenes;

    [UI] protected Button _CancelBtn;
    [UI] protected Button _BackBtn;
    [UI] protected Button _NextBtn;

    public void CancelHandler(object sender, EventArgs e)
    {
        Application.Quit();
    }

    public void BackHandler(object sender, EventArgs e)
    {
        this.Remove(_Scenes[_SceneIndex]);
        _SceneIndex--;
        if (_SceneIndex == -1)
        {
            // Placeholder
            Application.Quit();
        }
        else
        {
            this.Add(_Scenes[_SceneIndex]);
        }
    }

    public void NextHandler(object sender, EventArgs e)
    {
        this.Remove(_Scenes[_SceneIndex]);
        _SceneIndex++;
        if (_SceneIndex == _Scenes.Length)
        {
            // Placeholder
            App.AddWindow(Login.View());
        }
        else
        {
            this.Add(_Scenes[_SceneIndex]);
        }
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
    [UI] private Box _EncryptionMethodSelectView = null;

    public Setup() : base(new Builder("Login.Setup.glade"))
    {
        _Scenes = new[] { _EncryptionMethodSelectView };
    }
}
