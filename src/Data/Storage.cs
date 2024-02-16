using Gtk;

namespace Contra.Data;

public static class Storage
{
    public static string Store(List<Models.EntryBox> Boxes)
    {
        using (var sw = new StringWriter())
        {
            foreach (Models.EntryBox box in Boxes)
            {
                sw.WriteLine("START TEXT");
                sw.WriteLine(box.Label);
                sw.WriteLine(box.Username);
                sw.WriteLine(box.Password);
                sw.WriteLine(box.URL);
                sw.WriteLine(box.Note);
                sw.WriteLine("END TEXT");
            }

            return sw.ToString();
        }
    }

    public static void Restore(ListBox rowParent, string data)
    {
        string chunk = "";
        using (var sr = new StringReader(data))
        {
            while (true)
            {
                string? line = sr.ReadLine()!;
                if (line == null) break;
                line += '\n';

                if (line == "START TEXT\n") chunk = "";
                else if (line == "END TEXT\n")
                {
                    using (var cr = new StringReader(chunk))
                    {
                        new Models.EntryManager(rowParent, new(
                                    cr.ReadLine().Trim(),
                                    cr.ReadLine().Trim(),
                                    cr.ReadLine().Trim(),
                                    cr.ReadLine().Trim(),
                                    cr.ReadToEnd()
                        ));
                    }
                }
                else
                {
                    chunk += line;
                }
            }
        }
    }
}
