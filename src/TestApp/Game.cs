using System.Drawing;
using SDLib;
using SDL2;

namespace TestProj;

internal class Game : App
{
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
    }

    void Event(IReadOnlyAppInfo info, SDL.SDL_Event e)
    {
    }

    void Loop(IReadOnlyAppInfo info)
    {
    }

    void Finish(IReadOnlyAppInfo info)
    {
    }
}