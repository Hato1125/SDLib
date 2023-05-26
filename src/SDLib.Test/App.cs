﻿using SDL2;
using SDLib.Graphics;
using SDLib.Gui;
using SDLib.Input;
using System.Diagnostics;
using System.Drawing;

namespace SDLib.Test;

internal class App
{
    public readonly Stopwatch deltaStopwatch = new();
    public readonly SDL.SDL_WindowFlags WindowFlags = SDL.SDL_WindowFlags.SDL_WINDOW_SHOWN | SDL.SDL_WindowFlags.SDL_WINDOW_RESIZABLE;
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
    private UITileAlignment? tile;
    private UIToggleButton[]? button;
    private UIScroller scorll = new(0, -100, 100)
    {
        ScrollLine = 10,
    };

    private void Init()
    {
        var family = new FontFamily("segoeui.ttf", 20, Color.Red, FontStyle.Bold);

        tile = new(Renderer, Window, 1000, 1000, 2)
        {
            ColumnElementMaxNum = 5,
            ColumnPadding = 10,
            RowPadding = 10,
            X = 0,
            Y = 0,
        };

        button = new UIToggleButton[200];

        for (int i = 0; i < button.Length; i++)
        {
            button[i] = new(Renderer, Window, 120, 50, family)
            {
                BackColor = Color.Black,
                ClickColor = Color.Blue,
                Text = "Akasoko"
            };
            tile.ChildrenList.Add(button[i]);
        }

        display.AddElement(tile);
    }

    private void Loop()
    {
        Mouse.Update();
        Keyboard.Update();

        if (Keyboard.IsPushed(SDL.SDL_Scancode.SDL_SCANCODE_DOWN))
            tile.ColumnElementMaxNum--;

        if (Keyboard.IsPushed(SDL.SDL_Scancode.SDL_SCANCODE_UP))
            tile.ColumnElementMaxNum++;

        display.Update(DeltaTime);
        display.Render();
    }

    private void EventLoop(in SDL.SDL_Event e)
    {
        display.EventUpdate(e);
        scorll.UpdateEvent(e);
    }

    private void End()
    {
        display.Dispose();
    }
}