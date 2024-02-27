using Gtk;

namespace Contra.Models;

public class EntryBox : Box
{
    public int Index;

    public int Order;

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
                Order,
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
        ((Label)EntryManager.Rows[Index]!.Child).Text = Label;
    }

    private void DeleteEventHandler(object? sender, EventArgs e)
    {
        this.Destroy();
        EntryManager.Rows[Index]!.Destroy();

        Scene.Scenes[Index + 1] = null;
        EntryManager.Boxes[Index] = null;
        EntryManager.Rows[Index] = null;

        EntryManager.OnDelete();
    }

    public EntryBox(int order, int index, Data.Entry entry)
    {
        Order = order;
        Index = index;

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

    public new int Index
    {
        get => _index;
        set { _index = value; Name = $"{_index}"; }
    }

    public string Label
    {
        get => ((Label)Child).Text;
    }

    public EntryRow(int index, string label)
    {
        Index = index;

        var entryLabel = new Label(label);

        this.Add(entryLabel);
        EntryManager.RowParent.Add(this);
        EntryManager.RowParent.ShowAll();
    }
}

public class EntryManager
{
    public delegate void OnDeleteHandler();

    public static OnDeleteHandler OnDelete;
    public static List<EntryBox?> Boxes = new();
    public static List<EntryRow?> Rows = new();

    public static Scene InitialScreen;
    public static ListBox RowParent;

    private static int _highestOrder = 0;
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

    public static void Sort()
    {
        EntryRow selectedRow = (EntryRow)RowParent.SelectedRow;
        RowParent.Children.ToList().ForEach(child => RowParent.Remove(child));

        List<EntryBox> boxes = new();
        foreach (EntryBox? box in Boxes) if (box != null) boxes.Add(box);

        switch (Config.SortMethod)
        {
            case Config.Sort.Creation:
                {
                    boxes.Sort((box1, box2) => { if (box1.Order > box2.Order) return 1; else return -1; });
                    break;
                }
            case Config.Sort.ReverseCreation:
                {
                    boxes.Sort((box1, box2) => { if (box1.Order > box2.Order) return 1; else return -1; });
                    boxes.Reverse();
                    break;
                }
            case Config.Sort.Alphabet:
                {
                    boxes.Sort((box1, box2) => string.Compare(box1.Label, box2.Label, true));
                    break;
                }
            case Config.Sort.ReverseAlphabet:
                {
                    boxes.Sort((box1, box2) => string.Compare(box1.Label, box2.Label, true));
                    boxes.Reverse();
                    break;
                }
        }
        boxes.ForEach(box => Console.WriteLine(box.ToEntry().Order));

        Boxes.Clear();
        Rows.Clear();
        _currentIndex = 0;

        Scene.Index = 0;
        Scene.Scenes.Clear();
        Scene.Scenes.Add(InitialScreen);

        foreach (EntryBox box in boxes) new EntryManager(box.ToEntry());
        foreach (EntryRow row in Rows) if (row.Label == selectedRow.Label)
            {
                RowParent.SelectRow(row);
                Scene.Index = Int32.Parse(row.Name) + 1;
                break;
            }
    }

    public static void Reset()
    {
        _highestOrder = 0;
        _currentIndex = 0;
        Rows.Clear();
        Boxes.Clear();
    }

    public EntryManager(Data.Entry entry)
    {
        if (entry.Order == _highestOrder) _highestOrder++;
        else _highestOrder = entry.Order;
        Index = _currentIndex;
        _currentIndex++;

        Row = new(Index, entry.Label);
        Rows.Add(Row);

        Box = new EntryBox(_highestOrder, Index, entry);
        Boxes.Add(Box);
        Scene.Scenes.Add(new(Box, SceneChangeEventHandler, SceneChangeEventHandler));
    }

    public EntryManager() : this(new(_highestOrder, "", "")) { }
}


