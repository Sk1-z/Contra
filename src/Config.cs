using System.Xml.Serialization;
using System;

namespace Contra;

public static class Config
{
    public static readonly string Path =
#if DEBUG
@"test/";
#else
Environment.GetFolderPath(Environment.OSVersion.Platform == PlatformID.Unix ? 
        Environment.SpecialFolder.UserProfile :
        Environment.SpecialFolder.Personal) + @"/.contra/";
#endif

    public static readonly string ConfigPath =
#if DEBUG
@"test/debug.xml";
#else
Path + @"contra.xml";
#endif

    public static readonly string CheckPath =
#if DEBUG
@"test/check.dat";
#else
Path + @"check.dat";
#endif

    public static readonly string DataPath =
#if DEBUG
@"test/user.dat";
#else
Path + @"user.dat";
#endif

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

    private static Fields? _internal = null;

    public static bool Setup
    {
        get => _internal!.Setup;
        set { _internal!.Setup = value; Set(); }
    }

    public static SecurityLevel Security
    {
        get => _internal!.Security;
        set { _internal!.Security = value; Set(); }
    }

    public static string Check
    {
        get
        {
            var sr = new StreamReader(CheckPath);
            return sr.ReadToEnd();
        }
        set { using (var sw = new StreamWriter(CheckPath)) sw.Write(value); }
    }

    public static void Get()
    {
        if (!Directory.Exists(Path)) Directory.CreateDirectory(Path);
        if (!File.Exists(ConfigPath)) File.CreateText(ConfigPath).Close();
        if (!File.Exists(CheckPath)) File.Create(CheckPath).Close();
        if (!File.Exists(DataPath)) File.Create(DataPath).Close();

        try
        {
            _internal = (Fields)new XmlSerializer(typeof(Fields)).Deserialize(new StreamReader(ConfigPath))!;
        }
        catch
        {
            Set();
        }
    }

    public static void Set()
    {
        if (_internal == null)
        {
            _internal = new Fields
            {
                Setup = false,
                Security = SecurityLevel.None
            };

            Check = "Contrasena";
        }

        new XmlSerializer(typeof(Fields)).Serialize(new StreamWriter(ConfigPath), _internal);
    }
}

