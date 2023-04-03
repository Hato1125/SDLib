using System.Drawing;
using SDLib.Graphics;
using SDL2;

namespace TestProj;

internal class Program
{
    private static IntPtr _windowPtr = IntPtr.Zero;
    private static IntPtr _renderPtr = IntPtr.Zero;
    private static double wave = 0;

    [STAThread]
    private static void Main()
    {
        if (SDL.SDL_Init(SDL.SDL_INIT_VIDEO) < 0)
            throw new Exception(SDL.SDL_GetError());

        if (SDL_image.IMG_Init(SDL_image.IMG_InitFlags.IMG_INIT_PNG) < 0)
            throw new Exception(SDL_image.IMG_GetError());

        SDL_ttf.TTF_Init();

        _windowPtr = SDL.SDL_CreateWindow(
            "CleanUI Debug Window",
            SDL.SDL_WINDOWPOS_CENTERED,
            SDL.SDL_WINDOWPOS_CENTERED,
            1280,
            720,
            SDL.SDL_WindowFlags.SDL_WINDOW_SHOWN |
            SDL.SDL_WindowFlags.SDL_WINDOW_RESIZABLE
        );

        _renderPtr = SDL.SDL_CreateRenderer(
            _windowPtr,
            -1,
            SDL.SDL_RendererFlags.SDL_RENDERER_ACCELERATED |
            SDL.SDL_RendererFlags.SDL_RENDERER_PRESENTVSYNC
        );

        SDL.SDL_SetHint(SDL.SDL_HINT_RENDER_SCALE_QUALITY, "2");
        bool isRunning = true;

        Texture2D nachyo = new(_renderPtr, $"{AppContext.BaseDirectory}natyo.png");
        nachyo.WidthScale = 0.35f;
        nachyo.HeightScale = 0.35f;

        while (isRunning)
        {

            while (SDL.SDL_PollEvent(out SDL.SDL_Event e) == 1)
            {
                if (e.type == SDL.SDL_EventType.SDL_QUIT)
                    isRunning = false;
            }

            SDL.SDL_SetRenderDrawColor(_renderPtr, 255, 255, 255, 255);
            SDL.SDL_RenderClear(_renderPtr);

            nachyo.Render(0, 0);

            SDL.SDL_RenderPresent(_renderPtr);
        }

        SDL.SDL_DestroyWindow(_windowPtr);
        SDL.SDL_DestroyRenderer(_renderPtr);
        SDL.SDL_Quit();
        SDL_image.IMG_Quit();
    }
}