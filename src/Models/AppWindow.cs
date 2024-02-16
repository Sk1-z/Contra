using System.Text.Json;
using Gtk;
using UI = Gtk.Builder.ObjectAttribute;

namespace Contra.Models;

public class AppWindow : Window
{
    private Security.Cryptor _cryptor;

    [UI] private Button _newBtn;

    [UI] private SearchEntry _passwordListSearch;
    [UI] private ListBox _passwordSelectionList;

    [UI] private Button _backBtn;
    [UI] private Button _nextBtn;
    [UI] private Box _container;
    [UI] private Box _controlButtonContainer;
    [UI] private Box _initialScreen;

    private void BackEventHandler(object? sender, EventArgs e)
    {
        if (Scene.Current().Back())
        {
            if (Scene.Index > 1)
            {
                _container.Remove(Scene.Current().Model);
                Scene.Index--;
                _passwordSelectionList.SelectRow(_passwordSelectionList.GetRowAtIndex(Scene.Index - 1));
                _container.Add(Scene.Current().Model);
                _container.ReorderChild(_controlButtonContainer, 1);
            }
        }
    }

    private void NextEventHandler(object? sender, EventArgs e)
    {
        if (Scene.Current().Next())
        {
            if (!Scene.AtLast())
            {
                _container.Remove(Scene.Current().Model);
                Scene.Index++;
                _passwordSelectionList.SelectRow(_passwordSelectionList.GetRowAtIndex(Scene.Index - 1));
                _container.Add(Scene.Current().Model);
                _container.ReorderChild(_controlButtonContainer, 1);
            }
        }
    }

    public void RowActivatedEventHandler(object? sender, ListRowActivatedArgs e)
    {
        if (Scene.Current().Next())
        {
            Scene.Index = Convert.ToInt32(e.Row.Name);
            _container.Remove(_container.Children[0]);
            _container.Add(Scene.Current().Model);
            _container.ShowAll();
            _container.ReorderChild(_controlButtonContainer, 1);
        }
    }

    public void SearchEventHandler(object? sender, EventArgs e)
    {
        _passwordSelectionList.Children.ToList().ForEach((child) => _passwordSelectionList.Remove(child));

        string? search = _passwordListSearch.Text;

        foreach (EntryRow row in EntryManager.Rows)
        {
            if (search == null)
            {
                _passwordSelectionList.Add(row);
            }
            else if (((Label)row.Child).Text.Contains(search))
            {
                _passwordSelectionList.Add(row);
            }
        }
    }

    public void NewEntryEventHandler(object? sender, EventArgs e)
    {
        var em = new EntryManager(_passwordSelectionList, new("", ""));
        _passwordSelectionList.SelectRow(em.Row);

        _container.Remove(Scene.Current().Model);
        Scene.Index = em.Index;
        _container.Add(Scene.Current().Model);
        _container.ReorderChild(_controlButtonContainer, 1);
    }

    public void ExitEventHandler(object? sender, EventArgs e)
    {
        string data = Contra.Data.Storage.Store(EntryManager.Boxes);
        using (var sw = new StreamWriter(Config.DataPath))
        {
            sw.Write(_cryptor.Encrypt(data));
            sw.Flush();
        }

        Application.Quit();
    }

    public AppWindow(Security.Cryptor cryptor) : this(new Builder("AppWindow.glade"))
    {
        _cryptor = cryptor;

        string data = new StreamReader(Config.DataPath).ReadToEnd();
        if (data.Length > 32) Contra.Data.Storage.Restore(_passwordSelectionList, _cryptor.Decrypt(data));
    }

    protected AppWindow(Builder builder) : base(builder.GetRawOwnedObject("App"))
    {
        builder.Autoconnect(this);
        DeleteEvent += ExitEventHandler;

        Scene.Index = 0;
        Scene.Scenes = new(new Scene[] { new(_initialScreen) });

        _newBtn.Clicked += NewEntryEventHandler;

        _backBtn.Clicked += BackEventHandler;
        _nextBtn.Clicked += NextEventHandler;

        _passwordSelectionList.ListRowActivated += RowActivatedEventHandler;
        _passwordListSearch.Changed += SearchEventHandler;
    }
}
