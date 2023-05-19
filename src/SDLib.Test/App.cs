using SDL2;
using SDLib.Gui;
using SDLib.Input;
using System.Drawing;

namespace SDLib.Test;

internal class App
{
    public readonly SDL.SDL_WindowFlags WindowFlags = SDL.SDL_WindowFlags.SDL_WINDOW_SHOWN;
    public readonly SDL.SDL_RendererFlags RendererFlags = SDL.SDL_RendererFlags.SDL_RENDERER_ACCELERATED | SDL.SDL_RendererFlags.SDL_RENDERER_PRESENTVSYNC;
    public const int WIDTH = 800;
    public const int HEIGHT = 800;

    public nint Renderer;
    public nint Window;

    public void Run()
    {
        Window = SDL.SDL_CreateWindow(string.Empty, SDL.SDL_WINDOWPOS_CENTERED, SDL.SDL_WINDOWPOS_CENTERED, WIDTH, HEIGHT, WindowFlags);
        Renderer = SDL.SDL_CreateRenderer(Window, 0, RendererFlags);

        SDL.SDL_Init(SDL.SDL_INIT_SENSOR);
        SDL_image.IMG_Init(SDL_image.IMG_InitFlags.IMG_INIT_PNG);

        Init();

        bool isRunning = true;
        while(isRunning)
        {
            while(SDL.SDL_PollEvent(out SDL.SDL_Event e) != 0)
            {
                switch(e.type)
                {
                    case SDL.SDL_EventType.SDL_QUIT:
                        isRunning = false;
                        break;

                    default:
                        EventLoop(e);
                        break;
                }

                SDL.SDL_RenderClear(Renderer);
                Loop();
                SDL.SDL_RenderPresent(Renderer);
            }
        }

        SDL.SDL_DestroyRenderer(Renderer);
        SDL.SDL_DestroyWindow(Window);
        SDL.SDL_Quit();
        SDL_image.IMG_Quit();
    }

    private UIPanel? panel;
    private UIPanel? child;

    private void Init()
    {
        panel = new(Renderer, Window, 200, 200, Color.White)
        {
            X = 100,
            Y = 100,
        };

        child = new(Renderer, Window, 50, 50, Color.Blue)
        {
            X = 50,
            Y = 50,
        };

        panel.ChildrenList.Add(child);
    }

    private void Loop()
    {
        Mouse.Update();
        Keyboard.Update();

        panel?.Update();
        panel?.Render();
    }

    private void EventLoop(in SDL.SDL_Event e)
    {
        panel?.UpdateEvent(e);
    }

    private void End()
    {

    }
}