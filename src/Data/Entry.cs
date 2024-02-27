namespace Contra.Data;

[Serializable]
public class Entry
{
    public int Order;
    public string Label;
    public string Password;
    public string? Username;
    public string? URL;
    public string? Note;

    public Entry() { }

    public Entry(
            int order,
            string label,
            string password,
            string? username = null,
            string? url = null,
            string? note = null
    )
    {
        Order = order;
        Label = label;
        Username = username;
        Password = password;
        URL = url;
        Note = note;
    }
}
