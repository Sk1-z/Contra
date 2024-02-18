using Gtk;

namespace Contra.Models;

public class EntryBox : Box
{
    private int _index;

    public string Label
    {
        get => ((Entry)((Box)Children[1]).Children[0]).Text;
    }

    public string Username
    {
        get => ((Entry)((Box)Children[3]).Children[0]).Text;
    }

    public string Password
    {
        get => ((Entry)((Box)Children[5]).Children[0]).Text;
    }

    public string URL
    {
        get => ((Entry)((Box)Children[7]).Children[0]).Text;
    }

    public string Note
    {
        get => ((TextView)((Viewport)((ScrolledWindow)Children[9]).Child).Child).Buffer.Text;
    }

    public Data.Entry ToEntry()
    {
        return new(
                Label,
                Password,
                Username,
                URL,
                Note
                );
    }

    private void CopyToClipboard(string str)
    {
        Clipboard clipboard = Clipboard.Get(Gdk.Atom.Intern("CLIPBOARD", false));
        clipboard.Text = str;
    }

    private void CopyUsernameEventHandler(object? sender, EventArgs e)
    {
        CopyToClipboard(Username);
    }

    private void CopyPasswordEventHandler(object? sender, EventArgs e)
    {
        CopyToClipboard(Password);
    }

    private void CopyURLEventHandler(object? sender, EventArgs e)
    {
        CopyToClipboard(URL);
    }

    private void LabelChangedEventHandler(object? sender, EventArgs e)
    {
        ((Label)EntryManager.Rows[_index]!.Child).Text = Label;
    }

    private void DeleteEventHandler(object? sender, EventArgs e)
    {
        this.Destroy();
        EntryManager.Rows[_index]!.Destroy();

        Scene.Scenes[_index + 1] = null;
        EntryManager.Boxes[_index] = null;
        EntryManager.Rows[_index] = null;

        EntryManager.OnDelete();
    }

    public EntryBox(int index, Data.Entry entry)
    {
        _index = index;

        Orientation = Orientation.Vertical;
        Margin = 20;
        Spacing = 20;

        var fields = new string[] { "Label", "Username", "Password", "URL", "Note" };
        for (int i = 0; i < 5; i++)
        {
            var label = new Label(fields[i]);
            label.Halign = Align.Start;
            label.MarginStart = 20;
            Add(label);

            var fieldEntry = new Entry();
            fieldEntry.Hexpand = true;

            if (i == 4)
            {
                var scrollWindow = new ScrolledWindow();
                scrollWindow.HscrollbarPolicy = PolicyType.Never;
                scrollWindow.VscrollbarPolicy = PolicyType.External;
                scrollWindow.Expand = true;

                var viewport = new Viewport();

                var textView = new TextView();
                textView.WrapMode = WrapMode.Word;
                textView.Buffer.Text = entry.Note;

                viewport.Add(textView);
                scrollWindow.Add(viewport);
                Add(scrollWindow);
            }
            else
            {
                var eventBtn = new Button("Copy");

                switch (i)
                {
                    case 0:
                        {
                            fieldEntry.Text = entry.Label;
                            fieldEntry.Changed += LabelChangedEventHandler;
                            eventBtn.Label = "Delete";
                            eventBtn.Clicked += DeleteEventHandler;
                            break;
                        }
                    case 1:
                        {
                            fieldEntry.Text = entry.Username;
                            eventBtn.Clicked += CopyUsernameEventHandler;
                            break;
                        }
                    case 2:
                        {
                            fieldEntry.Text = entry.Password;
                            eventBtn.Clicked += CopyPasswordEventHandler;
                            break;
                        }
                    case 3:
                        {
                            fieldEntry.Text = entry.URL;
                            eventBtn.Clicked += CopyURLEventHandler;
                            break;
                        }
                }

                var copyBox = new Box(Orientation.Horizontal, 20);
                copyBox.Add(fieldEntry);
                copyBox.Add(eventBtn);
                Add(copyBox);
            }
        }

        this.ShowAll();
    }
}

public class EntryRow : ListBoxRow
{
    private int _index;

    public EntryRow(int index, ListBox rowParent, string label)
    {
        _index = index;
        Name = $"{_index}";

        var entryLabel = new Label(label);

        this.Add(entryLabel);
        rowParent.Add(this);
        rowParent.ShowAll();
    }
}

public class EntryManager
{
    public delegate void OnDeleteHandler();

    public static OnDeleteHandler OnDelete;
    public static List<EntryBox?> Boxes = new();
    public static List<EntryRow?> Rows = new();

    private static int _currentIndex = 0;
    public int Index;

    public EntryBox Box;
    public EntryRow Row;

    public static List<Data.Entry> Entries()
    {
        var entries = new List<Data.Entry>();
        foreach (EntryBox? box in Boxes) if (box != null) entries.Add(box.ToEntry());
        return entries;
    }

    public static bool NoEntries()
    {
        foreach (Box? box in Boxes) if (box != null) return false;
        return true;
    }

    public static int FirstEntryIndex()
    {
        for (int i = 0; i < Boxes.Count() - 1; i++) if (Boxes[i] != null) return i;
        return -1;
    }

    public static int LastEntryIndex()
    {
        for (int i = Boxes.Count() - 1; i > -1; i--) if (Boxes[i] != null) return i;
        return -1;
    }

    private bool SceneChangeEventHandler()
    {
        if (Box.Label == null || Box.Label == "" || Box.Label.Trim() == "") return false;
        return true;
    }

    public EntryManager(ListBox rowParent, Data.Entry entry)
    {
        Index = _currentIndex;
        _currentIndex++;

        Row = new(Index, rowParent, entry.Label);
        Rows.Add(Row);

        Box = new EntryBox(Index, entry);
        Boxes.Add(Box);
        Scene.Scenes.Add(new(Box, SceneChangeEventHandler, SceneChangeEventHandler));
    }
}


