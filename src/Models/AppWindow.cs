using Gtk;
using UI = Gtk.Builder.ObjectAttribute;

namespace Contra.Models;

public class AppWindow : Window
{
    Security.Cryptor Cryptor;

    public AppWindow(Security.Cryptor cryptor) : this(new Builder("AppWindow.glade"))
    {

    }

    protected AppWindow(Builder builder) : base(builder.GetRawOwnedObject("App"))
    {
        builder.Autoconnect(this);
        DeleteEvent += (sender, e) => Application.Quit();
    }
}
