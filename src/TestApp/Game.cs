using System.Drawing;
using SDLib;
using SDLib.Graphics;
using SDLib.Resource;
using SDL2;

namespace TestProj;

internal class Game : App
{
    private readonly TextureManager _textureManager = new();
    private Texture2D? _texture;

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
        _texture = _textureManager.LoadTexture(info.RenderPtr, $"{AppContext.BaseDirectory}test.png");
    }

    void Event(IReadOnlyAppInfo info, SDL.SDL_Event e)
    {
    }

    void Loop(IReadOnlyAppInfo info)
    {
        _texture?.Render(0, 0);
    }

    void Finish(IReadOnlyAppInfo info)
    {
        _textureManager.DeleteAllTexture();
    }
}