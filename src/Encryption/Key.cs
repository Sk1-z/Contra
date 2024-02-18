using System.Security.Cryptography;
using Gtk;
using Pango;

namespace Contra.Security;

public class Key
{
    private readonly byte[] _key = new byte[Size.Key(Size.Unit.Byte)];

    public Key()
    {
        RandomNumberGenerator.Create().GetBytes(_key);
    }

    public static void AsWindow(Key key)
    {
        var window = new Window("");
        window.Resizable = false;
        window.Modal = true;
        window.IconName = "security-high";
        window.DeleteEvent += (sender, e) => window.Destroy();

        var label = new Label(key.AsString());
        label.Margin = 20;
        label.Selectable = true;

        var attr = new AttrList();
        attr.Insert(new AttrWeight(Weight.Ultrabold));
        attr.Insert(new AttrScale(1.5));

        label.Attributes = attr;
        window.Add(label);
        App.AddWindow(window);
    }

    public static Key? FromString(string key)
    {
        Key newKey = new();

        try
        {
            for (int i = 0; i < Size.Key(Size.Unit.Byte); i++) newKey._key[i] = Convert.ToByte(key[(2 * i)..((2 * i) + 2)], 16);
            return newKey;
        }
        catch
        {
            return null;
        }
    }

    public Byte[] Bytes()
    {
        return _key;
    }

    public string AsString()
    {
        string acc = "";
        for (int i = 0; i < Size.Key(Size.Unit.Byte); i++) acc += _key[i].ToString("X2");
        return acc;
    }
}
