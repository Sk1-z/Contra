using Gtk;
using UI = Gtk.Builder.ObjectAttribute;

namespace Contra.Models;

public class GeneratorWindow : Window
{
    public GeneratorWindow() : this(new Builder("GeneratorWindow.glade"))
    {

    }

    public GeneratorWindow(Builder builder) : base(builder.GetRawOwnedObject("Generator"))
    {
        builder.Autoconnect(this);
    }
}
