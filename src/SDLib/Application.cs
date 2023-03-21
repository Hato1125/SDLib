using System.Drawing;
using System.Diagnostics;
using SDL2;

namespace SDLib;

public class Application
{
    private readonly Stopwatch _totalWatch = new();
    private readonly Stopwatch _deltaWatch = new();

    public readonly AppTime Time = new();
    public IntPtr WindowPtr { get; private set; }
    public IntPtr RendererPtr { get; private set; }
    public SDL.SDL_Event SDLEvent { get; private set; }

    public Point WindowPoint
    {
        get
        {
            if (WindowPtr == IntPtr.Zero)
                return Point.Empty;

            SDL.SDL_GetWindowPosition(WindowPtr, out int x, out int y);
            return new(x, y);
        }
        set
        {
            if (WindowPtr == IntPtr.Zero)
                return;

            SDL.SDL_SetWindowPosition(WindowPtr, value.X, value.Y);
        }
    }

    public Size WindowSize
    {
        get
        {
            if (WindowPtr == nint.Zero)
                return Size.Empty;

            SDL.SDL_GetWindowSize(WindowPtr, out int w, out int h);
            return new(w, h);
        }
        set
        {
            if (WindowPtr == IntPtr.Zero)
                return;

            SDL.SDL_SetWindowSize(WindowPtr, value.Width, value.Height);
        }
    }

    public Size WindowMinSize
    {
        get
        {
            if (WindowPtr == IntPtr.Zero)
                return Size.Empty;

            SDL.SDL_GetWindowMinimumSize(WindowPtr, out int w, out int h);
            return new(w, h);
        }
        set
        {
            if (WindowPtr == IntPtr.Zero)
                return;

            SDL.SDL_SetWindowMinimumSize(WindowPtr, value.Width, value.Height);
        }
    }

    public Size WindowMaxSize
    {
        get
        {
            if (WindowPtr == IntPtr.Zero)
                return Size.Empty;

            SDL.SDL_GetWindowMaximumSize(WindowPtr, out int w, out int h);
            return new(w, h);
        }
        set
        {
            if (WindowPtr == IntPtr.Zero)
                return;

            SDL.SDL_SetWindowMaximumSize(WindowPtr, value.Width, value.Height);
        }
    }

    public string WindowTitle
    {
        get
        {
            if (WindowPtr == IntPtr.Zero)
                return string.Empty;

            return SDL.SDL_GetWindowTitle(WindowPtr);
        }
        set
        {
            if (WindowPtr == IntPtr.Zero)
                return;

            SDL.SDL_SetWindowTitle(WindowPtr, value);
        }
    }

    public event Action? OnInit = delegate { };
    public event Action<AppTime>? OnRunning = delegate { };
    public event Action? OnQuit = delegate { };

    public Application(
        string windowTitle,
        Size windowSize,
        Point? windowPos = null,
        Size? windowMinSize = null,
        Size? windowMaxSize = null)
    {
        _totalWatch.Start();
        InitWindow(windowTitle, windowSize, windowPos);
        InitRenderer();
        InitSDL();

        SDL.SDL_GetDisplayBounds(0, out SDL.SDL_Rect rect);
        if (windowMinSize != null)
            WindowMinSize = windowMinSize.Value;
        else
            WindowMinSize = new(0, 0);

        if (windowMaxSize != null)
            WindowMaxSize = windowMaxSize.Value;
        else
            WindowMaxSize = new(rect.w, rect.h);
    }

    public void Run()
    {
        OnInit?.Invoke();

        bool isRunning = true;
        while (isRunning)
        {
            while (SDL.SDL_PollEvent(out SDL.SDL_Event e) == 1)
            {
                SDLEvent = e;
                if (e.type == SDL.SDL_EventType.SDL_QUIT)
                {
                    isRunning = false;
                    break;
                }
            }

            SDL.SDL_RenderClear(RendererPtr);
            OnRunning?.Invoke(Time);
            SDL.SDL_RenderPresent(RendererPtr);

            Time.TotalTime = _totalWatch.Elapsed;
            Time.DeltaTime = _deltaWatch.Elapsed;
            _deltaWatch.Restart();
        }

        _totalWatch.Stop();
        _deltaWatch.Stop();
        OnQuit?.Invoke();
        DestroySDL();
    }

    private void InitWindow(string title, Size size, Point? pos)
    {
        Tracer.PrintInfo("Create a window.");

        Point winPosition = new(
            pos == null ? SDL.SDL_WINDOWPOS_CENTERED : pos.Value.X,
            pos == null ? SDL.SDL_WINDOWPOS_CENTERED : pos.Value.Y
        );

        WindowPtr = SDL.SDL_CreateWindow(
            title,
            winPosition.X,
            winPosition.Y,
            size.Width,
            size.Height,
            SDL.SDL_WindowFlags.SDL_WINDOW_SHOWN
            | SDL.SDL_WindowFlags.SDL_WINDOW_RESIZABLE
        );

        if (WindowPtr == IntPtr.Zero)
            throw new Exception(SDL.SDL_GetError());
    }

    private void InitRenderer()
    {
        Tracer.PrintInfo("Create a renderer.");

        RendererPtr = SDL.SDL_CreateRenderer(
            WindowPtr,
            -1,
            SDL.SDL_RendererFlags.SDL_RENDERER_ACCELERATED
            | SDL.SDL_RendererFlags.SDL_RENDERER_PRESENTVSYNC
        );

        if (RendererPtr == IntPtr.Zero)
            throw new Exception(SDL.SDL_GetError());
    }

    private void InitSDL()
    {
        Tracer.PrintInfo("Initialize sdllibs.");

        if (SDL.SDL_Init(SDL.SDL_INIT_VIDEO) < 0)
            throw new Exception(SDL.SDL_GetError());

        if (SDL_image.IMG_Init(SDL_image.IMG_InitFlags.IMG_INIT_PNG) < 0)
            throw new Exception(SDL_image.IMG_GetError());

        if (SDL_ttf.TTF_Init() < 0)
            throw new Exception(SDL_ttf.TTF_GetError());

        if (SDL_mixer.Mix_Init(SDL_mixer.MIX_InitFlags.MIX_INIT_MP3) < 0)
            throw new Exception(SDL_mixer.Mix_GetError());
    }

    private void DestroySDL()
    {
        Tracer.PrintInfo("Destroy sdllibs.");

        SDL.SDL_DestroyWindow(WindowPtr);
        SDL.SDL_DestroyRenderer(RendererPtr);
        SDL.SDL_Quit();
        SDL_image.IMG_Quit();
        SDL_ttf.TTF_Quit();
        SDL_mixer.Mix_Quit();
    }
}