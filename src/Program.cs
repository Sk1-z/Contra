using Gtk;

namespace Contra;

public static class App
{
    private static Application _app = new(null, GLib.ApplicationFlags.None);

    public static void AddWindow(Window window)
    {
        _app.AddWindow(window);
        window.ShowAll();
    }

    public static void Main()
    {
        Config.Get();
        Application.Init();
        _app.Register(GLib.Cancellable.Current);

        Models.LoginWindow.View();
        Application.Run();
    }
}
