using Gtk;

namespace Contra;

public static class App
{
    private static Application _App = new(null, GLib.ApplicationFlags.None);

    public static void AddWindow(Window window)
    {
        _App.AddWindow(window);
        window.Show();
    }

    public static void Main()
    {
        Config.Get();
        Application.Init();
        _App.Register(GLib.Cancellable.Current);

        AddWindow(Model.Login.View());
        Application.Run();
    }
}
