using Gtk;

namespace Contra.Models;

public class EntryBox : Box
{
    private int _index;

    public string Label
    {
        get => ((Entry)Children[1]).Text;
        set { ((Entry)Children[1]).Text = value; ((Label)EntryManager.Rows[_index - 1].Child).Text = value; }
    }

    public string Username
    {
        get => ((Entry)((Box)Children[3]).Children[0]).Text;
        set => ((Entry)((Box)Children[3]).Children[0]).Text = value;

    }

    public string Password
    {
        get => ((Entry)((Box)Children[5]).Children[0]).Text;
        set => ((Entry)((Box)Children[5]).Children[0]).Text = value;
    }

    public string URL
    {
        get => ((Entry)((Box)Children[7]).Children[0]).Text;
        set => ((Entry)((Box)Children[7]).Children[0]).Text = value;
    }

    public string Note
    {
        get => ((TextView)((ScrolledWindow)Children[9]).Child).Buffer.Text;
        set => ((TextView)((ScrolledWindow)Children[9]).Child).Buffer.Text = value;
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

            if (i == 0)
            {
                fieldEntry.Text = entry.Label;
                fieldEntry.Changed += (sender, e) => ((Label)EntryManager.Rows[_index - 1].Child).Text = fieldEntry.Text;

                Add(fieldEntry);
            }
            else if (i == 4)
            {
                var scrollWindow = new ScrolledWindow();
                scrollWindow.HscrollbarPolicy = PolicyType.Never;
                scrollWindow.VscrollbarPolicy = PolicyType.External;
                scrollWindow.Expand = true;

                var textView = new TextView();
                textView.WrapMode = WrapMode.Word;
                textView.Buffer.Text = entry.Note;

                scrollWindow.Add(textView);
                Add(scrollWindow);
            }
            else
            {
                switch (i)
                {
                    case 1: { fieldEntry.Text = entry.Username; break; }
                    case 2: { fieldEntry.Text = entry.Password; break; }
                    case 3: { fieldEntry.Text = entry.URL; break; }
                }

                var copyBox = new Box(Orientation.Horizontal, 20);
                copyBox.Add(fieldEntry);
                copyBox.Add(new Button("Copy"));
                Add(copyBox);
            }
        }

        this.ShowAll();
    }
}

public class EntryRow : ListBoxRow
{
    public EntryRow(int index, ListBox rowParent, string label)
    {
        Visible = true;
        Name = $"{index}";

        using (var entryLabel = new Label(label))
        {
            entryLabel.Visible = true;
            Add(entryLabel);
        }

        rowParent.Add(this);
        rowParent.ShowAll();
    }
}

public class EntryManager
{
    public static List<EntryBox> Boxes = new();
    public static List<EntryRow> Rows = new();

    private Data.Entry _entry;

    private static int _currentIndex = 1;
    public int Index;
    public EntryBox Box;
    public EntryRow Row;

    public EntryManager(ListBox rowParent, Data.Entry entry) : this(_currentIndex, rowParent, entry)
    {
        _currentIndex++;
    }

    public EntryManager(int index, ListBox rowParent, Data.Entry entry)
    {
        Index = index;
        _entry = entry;

        Row = new(index, rowParent, entry.Label);
        Rows.Add(Row);

        Box = new(index, entry);
        Boxes.Add(Box);
        Scene.Scenes.Add(new(Box));
    }
}


