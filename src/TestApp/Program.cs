using System.Drawing;
using SDLib.Graphics;
using SDLib;
using System.Runtime.InteropServices;
using SDL2;

namespace TestProj;

internal class Program
{
    [STAThread]
    private static void Main()
    {
        var app = new Game(
            "Test",
            SDL.SDL_WindowFlags.SDL_WINDOW_SHOWN,
            SDL.SDL_RendererFlags.SDL_RENDERER_ACCELERATED,
            new (1280, 720)
        );

        app.Run();
    }
}