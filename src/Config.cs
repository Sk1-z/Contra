using System.Xml.Serialization;
using System;

namespace Contra;

public static class Config
{
    public static readonly string Path =
#if DEBUG
@"test/";
#else
Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "/.config/contra/";
#endif

    public static readonly string ConfigPath =
#if DEBUG
@"test/debug.xml";
#else
Path + "contra.xml";
#endif

    public static readonly string CheckPath =
#if DEBUG
@"test/check.dat";
#else
Path + "check.dat";
#endif

    public static readonly string DataPath =
#if DEBUG
@"test/user.dat";
#else
Path + "user.dat";
#endif

    public static void Get()
    {
        try
        {
            if (!File.Exists(ConfigPath)) File.CreateText(ConfigPath);
            if (!File.Exists(CheckPath)) File.Create(CheckPath);
            if (!File.Exists(DataPath)) File.Create(DataPath);
        }
        catch (DirectoryNotFoundException)
        {
            Directory.CreateDirectory(Path);
            if (!File.Exists(ConfigPath)) File.CreateText(ConfigPath);
            if (!File.Exists(CheckPath)) File.Create(CheckPath);
            if (!File.Exists(DataPath)) File.Create(DataPath);
        }

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
