namespace Contra.Data;

public class Entry
{
    public string Label;
    public string? Username;
    public string? Password;
    public string? URL;
    public string? Note;

    public Entry(
            string label,
            string password,
            string? username = null,
            string? url = null,
            string? note = null
    )
    {
        Label = label;
        Username = username;
        Password = password;
        URL = url;
        Note = note;
    }
}
