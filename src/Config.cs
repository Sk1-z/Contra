using System.Xml.Serialization;

namespace Contra;

public static class Config
{
    public static readonly string ConfigPath =
#if DEBUG
@"debug.xml";
#else
@"~/.config/contra/contra.xml";
#endif

    public static readonly string CheckPath =
#if DEBUG
@"check.dat";
#else
@"~/.config/contra/check.dat";
#endif

    public static void Get()
    {
        if (!File.Exists(ConfigPath)) File.CreateText(ConfigPath);
        if (!File.Exists(CheckPath)) File.Create(CheckPath);

        try
        {
            Internal = (Fields)new XmlSerializer(typeof(Fields)).Deserialize(new StreamReader(ConfigPath))!;
        }
        catch
        {
            Set();
        }
    }

    public static void Set()
    {
        if (Internal == null)
        {
            Internal = new Fields
            {
                Setup = false,
                Security = SecurityLevel.None
            };

            Check = "Contrasena";
        }

        new XmlSerializer(typeof(Fields)).Serialize(new StreamWriter(ConfigPath), Internal);
    }

    public static void Clean()
    {
        File.Delete(ConfigPath);
    }

    public enum SecurityLevel
    {
        Key,
        Password,
        None,
    }

    public class Fields
    {
        public bool Setup { get; set; }
        public SecurityLevel Security { get; set; }
    }

    static Fields? Internal = null;

    public static bool Setup
    {
        get => Internal!.Setup;
        set { Internal!.Setup = value; Set(); }
    }

    public static SecurityLevel Security
    {
        get => Internal!.Security;
        set { Internal!.Security = value; Set(); }
    }

    public static string Check
    {
        get { var sr = new StreamReader(CheckPath); return sr.ReadToEnd(); }
        set { var sw = new StreamWriter(CheckPath); sw.Write(value); sw.Flush(); }
    }
}
