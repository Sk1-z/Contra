using Gtk;
using UI = Gtk.Builder.ObjectAttribute;

namespace Contra.Models;

public class AppWindow : Window
{
    private Security.Cryptor _cryptor;

    [UI] private Button _configureBtn;
    [UI] private Button _generatorBtn;

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
            _container.Remove(Scene.Current().Model);
            if (Scene.Index == EntryManager.FirstEntryIndex() + 1) Scene.Index = EntryManager.LastEntryIndex() + 1;
            else Scene.Index--;

            if (EntryManager.NoEntries())
            {
                ResetScenes();
            }
            else
            {
                _container.Add(Scene.Current(false).Model);
                _passwordSelectionList.SelectRow(EntryManager.Rows[Scene.Index - 1]);
                _container.ReorderChild(_controlButtonContainer, 1);
            }
        }
    }

    private void NextEventHandler(object? sender, EventArgs e)
    {
        if (Scene.Current().Next())
        {
            _container.Remove(Scene.Current().Model);
            if (Scene.Index == EntryManager.LastEntryIndex() + 1) Scene.Index = EntryManager.FirstEntryIndex() + 1;
            else Scene.Index++;

            if (EntryManager.NoEntries())
            {
                ResetScenes();
            }
            else
            {
                _container.Add(Scene.Current(false).Model);
                _passwordSelectionList.SelectRow(EntryManager.Rows[Scene.Index - 1]);
                _container.ReorderChild(_controlButtonContainer, 1);
            }
        }
    }

    private void RowActivatedEventHandler(object? sender, ListRowActivatedArgs e)
    {
        if (Scene.Current().Next() && Scene.Current().Back())
        {
            _container.Remove(Scene.Current().Model);
            Scene.Index = Convert.ToInt32(e.Row.Name) + 1;
            _container.Add(Scene.Current().Model);
            _container.ReorderChild(_controlButtonContainer, 1);
        }
    }

    private void SearchEventHandler(object? sender, EventArgs e)
    {
        _passwordSelectionList.Children.ToList().ForEach((child) => _passwordSelectionList.Remove(child));

        string? search = _passwordListSearch.Text;

        if (search == null) foreach (EntryRow? row in EntryManager.Rows) _passwordSelectionList.Add(row);
        else foreach (EntryRow? row in EntryManager.Rows)
                if (row != null && ((Label)((Box)row.Child).Children[0]).Text.Contains(search))
                    _passwordSelectionList.Add(row);
    }

    private void NewEntryEventHandler(object? sender, EventArgs e)
    {
        var em = new EntryManager(_passwordSelectionList, new("", ""));
        _passwordSelectionList.SelectRow(em.Row);

        _container.Remove(Scene.Current().Model);
        Scene.Index = em.Index + 1;
        _container.Add(Scene.Current().Model);
        _container.ReorderChild(_controlButtonContainer, 1);
    }

    private void ExitEventHandler(object? sender, EventArgs e)
    {
        string data = Contra.Data.Storage.Store();
        using (var sw = new StreamWriter(Config.DataPath))
        {
            sw.Write(_cryptor.Encrypt(data));
            sw.Flush();
        }

        Application.Quit();
    }

    private void ResetScenes()
    {
        if (Scene.Scenes.Count() == 2 || EntryManager.NoEntries())
        {
            Scene.Index = 0;
            _container.Add(_initialScreen);
        }
        else if (Scene.Index == 1)
        {
            Scene.Index++;
            _container.Add(Scene.Current(false).Model);
            _passwordSelectionList.SelectRow(EntryManager.Rows[Scene.Index - 1]);
        }
        else
        {
            Scene.Index--;
            _container.Add(Scene.Current(true).Model);
            _passwordSelectionList.SelectRow(EntryManager.Rows[Scene.Index - 1]);
        }
        _container.ReorderChild(_controlButtonContainer, 1);

    }

    private void NewConfigureEventHandler(object? sender, EventArgs e)
    {
        using (var sr = new StreamReader(Config.DataPath))
            App.AddWindow(new ConfigureWindow(this));
    }

    private void NewGeneratorEventHandler(object? sender, EventArgs e)
    {

    }

    public AppWindow(Security.Cryptor cryptor) : this(new Builder("AppWindow.glade"))
    {
        _cryptor = cryptor;

        string data = new StreamReader(Config.DataPath).ReadToEnd();
        if (data.Length > 32) Contra.Data.Storage.Restore(_passwordSelectionList, _cryptor.Decrypt(data));

        if (Scene.Scenes.Count() > 1) NextEventHandler(_nextBtn, new EventArgs());
    }

    private AppWindow(Builder builder) : base(builder.GetRawOwnedObject("App"))
    {
        builder.Autoconnect(this);
        DeleteEvent += ExitEventHandler;

        EntryManager.OnDelete = ResetScenes;

        Scene.Index = 0;
        Scene.Scenes = new(new Scene[] { new(_initialScreen) });

        _configureBtn.Clicked += NewConfigureEventHandler;
        _generatorBtn.Clicked += NewGeneratorEventHandler;

        _newBtn.Clicked += NewEntryEventHandler;

        _backBtn.Clicked += BackEventHandler;
        _nextBtn.Clicked += NextEventHandler;

        _passwordSelectionList.ListRowActivated += RowActivatedEventHandler;
        _passwordListSearch.Changed += SearchEventHandler;
    }

    public void ChangeCryptor(Security.Cryptor cryptor)
    {
        _cryptor = cryptor;
    }
}
