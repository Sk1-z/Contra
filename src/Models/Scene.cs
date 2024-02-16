using Gtk;

namespace Contra.Models;

public class Scene
{
    public Box Model;

    public delegate bool SceneEventHandler();

    public SceneEventHandler Back;
    public SceneEventHandler Next;

    public Scene(Box model)
    {
        Model = model;
        Back = () => { return true; };
        Next = () => { return true; };
    }

    public Scene(Box model, SceneEventHandler? back, SceneEventHandler? next)
    {
        Model = model;
        if (back != null) Back = back;
        if (next != null) Next = next;
    }

    public static int Index = 0;
    public static List<Scene> Scenes = new();

    public static Scene Current()
    {
        return Scenes[Index];
    }

    public static bool AtFirst()
    {
        return Index == 0;
    }

    public static bool AtLast()
    {
        return Index + 1 == Scenes.Count();
    }
}
