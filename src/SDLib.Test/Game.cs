using System.Drawing;
using SDL2;
using SDLib;
using SDLib.Input;
using SDLib.NewFramework;

namespace SDLib.Test;

internal class Game : App
{
    private Actor TestActor1;
    private Actor TestActor2;
    public readonly Scene scene = new();

    public Game(
        string windowTitle,
        SDL.SDL_WindowFlags windowFlag,
        SDL.SDL_RendererFlags renderFlag,
        Size windowSize,
        Point? windowPosition = null,
        Size? windowMinSize = null,
        Size? windowMaxSize = null)
        : base(
            windowTitle,
            windowFlag,
            renderFlag,
            windowSize,
            windowPosition,
            windowMinSize,
            windowMaxSize)
    {
        OnInitialize += Init;
        OnEvent += Event;
        OnMainLoop += Loop;
        OnFinish += Finish;
    }

    void Init(IReadOnlyAppInfo info)
    {
        TestActor1 = new Actor(scene, 1);
        TestActor2 = new Actor(scene, 2);

        foreach(var com in scene.ActorList)
            Console.WriteLine(com.Order);
    }

    void Event(IReadOnlyAppInfo info, SDL.SDL_Event e)
    {
    }

    void Loop(IReadOnlyAppInfo info)
    {
        Keyboard.Update();

        scene.IsUpdating = true;

        scene.Update(info);
        scene.Render(info);

        scene.IsUpdating = false;
        scene.ActorCleaning();
    }

    void Finish(IReadOnlyAppInfo info)
    {
    }
}