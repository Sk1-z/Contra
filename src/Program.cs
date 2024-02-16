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

#if DEBUG
        AddWindow(new Models.AppWindow(new Security.Cryptor()));
#else
        AddWindow(Models.Login.View());
#endif
        Application.Run();
    }
}
