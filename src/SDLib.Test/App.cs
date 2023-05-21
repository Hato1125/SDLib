using SDL2;
using SDLib.Graphics;
using SDLib.Gui;
using SDLib.Input;
using System.Diagnostics;
using System.Drawing;

namespace SDLib.Test;

internal class App
{
    public readonly Stopwatch deltaStopwatch = new();
    public readonly SDL.SDL_WindowFlags WindowFlags = SDL.SDL_WindowFlags.SDL_WINDOW_SHOWN;
    public readonly SDL.SDL_RendererFlags RendererFlags = SDL.SDL_RendererFlags.SDL_RENDERER_ACCELERATED;
    public const int WIDTH = 800;
    public const int HEIGHT = 800;

    public double DeltaTime;
    public nint Renderer;
    public nint Window;

    public void Run()
    {
        //SDL.SDL_SetHintWithPriority(SDL.SDL_HINT_RENDER_DRIVER, "opengles2", SDL.SDL_HintPriority.SDL_HINT_DEFAULT);
        
        Window = SDL.SDL_CreateWindow(string.Empty, SDL.SDL_WINDOWPOS_CENTERED, SDL.SDL_WINDOWPOS_CENTERED, WIDTH, HEIGHT, WindowFlags);
        Renderer = SDL.SDL_CreateRenderer(Window, -1, RendererFlags);

        SDL.SDL_Init(SDL.SDL_INIT_SENSOR);
        SDL_ttf.TTF_Init();
        SDL_image.IMG_Init(SDL_image.IMG_InitFlags.IMG_INIT_PNG);

        Init();

        bool isRunning = true;
        while(isRunning)
        {
            while(SDL.SDL_PollEvent(out var e) != 0)
            {
                switch (e.type)
                {
                    case SDL.SDL_EventType.SDL_QUIT:
                        isRunning = false;
                        break;

                    default:
                        EventLoop(e);
                        break;
                }
            }

            SDL.SDL_SetRenderDrawColor(Renderer, 255, 255, 255, 255);
            SDL.SDL_RenderClear(Renderer);
            Loop();
            SDL.SDL_RenderPresent(Renderer);

            DeltaTime = deltaStopwatch.Elapsed.TotalSeconds;
            deltaStopwatch.Restart();
        }

        End();
        SDL.SDL_DestroyRenderer(Renderer);
        SDL.SDL_DestroyWindow(Window);
        SDL.SDL_Quit();
        SDL_ttf.TTF_Quit();
        SDL_image.IMG_Quit();
    }

    private readonly UIDisplay display = new();
    private UIButton? button;
    private UIButton? button2;
    private UIButton? children;
    private UIButton? children2;
    private UIButton? children3;

    private void Init()
    {
        var family = new FontFamily("segoeui.ttf", 24, Color.DeepSkyBlue);

        children = new(Renderer, Window, 200, 200, family, Color.FromArgb(0, 0, 255), Color.FromArgb(0, 0, 0))
        {
            X = 30,
            Y = 30,
            Text = string.Empty,
            Icon = new(Renderer, "beer-stein.png"),
        };

        children2 = new(Renderer, Window, 150, 150, family, Color.FromArgb(255, 0, 255), Color.FromArgb(0, 0, 0))
        {
            X = 10,
            Y = 10,
            Text = string.Empty,
            Icon = new(Renderer, "beer-stein.png"),
        };

        children3 = new(Renderer, Window, 30, 30, family, Color.FromArgb(0, 255, 255), Color.FromArgb(0, 0, 0))
        {
            X = 10,
            Y = 10,
            Text = string.Empty,
            Icon = new(Renderer, "beer-stein.png"),
        };

        button = new(Renderer, Window, 200, 200, family, Color.FromArgb(255, 0, 0), Color.FromArgb(0, 0, 0))
        {
            X = 70,
            Y = 60,
            Text = string.Empty,
            Icon = new(Renderer, "beer-stein.png"),
        };

        button2 = new(Renderer, Window, 200, 200, family, Color.FromArgb(0, 255, 0), Color.FromArgb(0, 0, 0))
        {
            X = 80,
            Y = 80,
            Text = string.Empty,
            Icon = new(Renderer, "beer-stein.png"),
        };

        children2.ChildrenList.Add(children3);
        children.ChildrenList.Add(children2);
        button.ChildrenList.Add(children);

        display.AddElement(button2);
        display.AddElement(button);
    }

    private void Loop()
    {
        Mouse.Update();
        Keyboard.Update();



        display.Update(DeltaTime);
        display.Render();
    }

    private void EventLoop(in SDL.SDL_Event e)
    {
        display.EventUpdate(e);
    }

    private void End()
    {
        display.Dispose();
    }
}