using System.Drawing;
using SDLib;
using SDLib.Input;
using SDLib.Graphics;
using SDLib.Resource;
using SDL2;

namespace TestProj;

internal class Game : App
{
    private readonly TextureManager _textureManager = new();
    private Texture2D? _texture;
    private FontRenderer? font;

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
        var family = new FontFamily($"{AppContext.BaseDirectory}07やさしさゴシックボールド.ttf", 30, Color.White);
        font = new(info.RenderPtr, family);
        font.Text = "Font";
    }

    void Event(IReadOnlyAppInfo info, SDL.SDL_Event e)
    {
    }

    void Loop(IReadOnlyAppInfo info)
    {
        Keyboard.Update();

        if(Keyboard.IsPushed(SDL.SDL_Scancode.SDL_SCANCODE_A))
        {
            if(font != null)
                font.Text += "a";
        }

        font?.Render().Render(0,0);
    }

    void Finish(IReadOnlyAppInfo info)
    {
        _textureManager.DeleteAllTexture();
    }
}