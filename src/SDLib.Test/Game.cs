using System.Drawing;
using SDL2;
using SDLib;
using SDLib.Framework;
using SDLib.Input;
using SDLib.NewFramework;

namespace SDLib.Test;

internal class Game : App
{
    public readonly TestScene test = new TestScene();

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
        SceneManager.RegistScene("Test", test);
    }

    void Event(IReadOnlyAppInfo info, SDL.SDL_Event e)
    {
    }

    void Loop(IReadOnlyAppInfo info)
    {
        Keyboard.Update();
        SceneManager.ViewScene(info);
    }

    void Finish(IReadOnlyAppInfo info)
    {
    }
}