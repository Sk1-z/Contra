using Gtk;
using System.Xml.Serialization;

namespace Contra.Data;

public static class Storage
{
    public static string Store()
    {
        using (var sw = new StringWriter())
        {
            new XmlSerializer(typeof(List<Entry>)).Serialize(sw, Models.EntryManager.Entries());
            return sw.ToString();
        }
    }

    public static void Restore(ListBox rowParent, string data)
    {
        var entries = (List<Entry>)new XmlSerializer(typeof(List<Entry>)).Deserialize(new StringReader(data))!;

        foreach (Entry entry in entries)
        {
            new Models.EntryManager(rowParent, entry);
        }
    }
}
