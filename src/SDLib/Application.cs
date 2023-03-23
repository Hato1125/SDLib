using System.Drawing;
using System.Diagnostics;
using SDL2;

namespace SDLib;

public class Application
{
    private readonly Stopwatch _totalWatch = new();
    private readonly Stopwatch _deltaWatch = new();
    private readonly Stopwatch _fpsWatch = new();
    private double _frameCounter;
    private bool _isRunning;
    private SDL.SDL_Event _sdlEvent;

    /// <summary>
    /// アプリケーションの時間
    /// </summary>
    public readonly AppTime Time = new();

    /// <summary>
    /// Windowのポインタ
    /// </summary>
    public IntPtr WindowPtr { get; private set; }

    /// <summary>
    /// レンダラーのポインタ
    /// </summary>
    public IntPtr RendererPtr { get; private set; }

    /// <summary>
    /// Windowの位置
    /// </summary>
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

    /// <summary>
    /// Windowのサイズ
    /// </summary>
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

    /// <summary>
    /// Windowの最小サイズ
    /// </summary>
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

    /// <summary>
    /// Windowの最大サイズ
    /// </summary>
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

    /// <summary>
    /// Windowのタイトル
    /// </summary>
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

    /// <summary>
    /// 最大フレームレート
    /// </summary>
    public double MaxFramerate { get; set; }

    /// <summary>
    /// 一秒に何回更新されるかを取得する
    /// </summary>
    public double FramesPerSecond { get; private set; }

    /// <summary>
    /// 初期化時に呼び出される
    /// </summary>
    public event Action<IntPtr>? OnInit = delegate { };

    /// <summary>
    /// イベントが発生した際に呼ばれる
    /// </summary>
    public event Action<SDL.SDL_Event>? OnEvent = delegate { };

    /// <summary>
    /// ループ時に呼び出される
    /// </summary>
    public event Action<AppTime, IntPtr>? OnRunning = delegate { };

    /// <summary>
    /// 終了時に呼び出される
    /// </summary>
    public event Action? OnQuit = delegate { };

    /// <summary>
    /// アプリケーションを初期化する
    /// </summary>
    /// <param name="windowTitle">タイトル</param>
    /// <param name="windowSize">サイズ</param>
    /// <param name="windowPos">位置</param>
    /// <param name="windowMinSize">最小サイズ</param>
    /// <param name="windowMaxSize">最大サイズ</param>
    public Application(
        string windowTitle,
        Size windowSize,
        Point? windowPos = null,
        Size? windowMinSize = null,
        Size? windowMaxSize = null)
    {
        _totalWatch.Start();
        MaxFramerate = 60;
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

        _isRunning = true;
    }

    /// <summary>
    /// アプリケーションを起動する
    /// </summary>
    public void Run()
    {
        OnInit?.Invoke(RendererPtr);

        _fpsWatch.Start();

        while (_isRunning)
        {
            while (SDL.SDL_PollEvent(out SDL.SDL_Event e) == 1)
            {
                _sdlEvent = e;
                if (e.type == SDL.SDL_EventType.SDL_QUIT)
                {
                    _isRunning = false;
                    break;
                }
            }

            OnEvent?.Invoke(_sdlEvent);
            SDL.SDL_RenderClear(RendererPtr);
            OnRunning?.Invoke(Time, RendererPtr);
            SDL.SDL_RenderPresent(RendererPtr);
            FramerateLimitter();

            Time.TotalTime = _totalWatch.Elapsed;
            Time.DeltaTime = _deltaWatch.Elapsed;
            _deltaWatch.Restart();
            CalclateFps();
        }

        _totalWatch.Stop();
        _deltaWatch.Stop();
        _fpsWatch.Stop();
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

    private void FramerateLimitter()
    {
        if (MaxFramerate == -1)
            return;

        double maxFrameMs = 1.0 / MaxFramerate;
        if (_deltaWatch.Elapsed.TotalSeconds < maxFrameMs)
        {
            double sleepMs = (maxFrameMs - _deltaWatch.Elapsed.TotalSeconds) * 1000.0;
            SDL.SDL_Delay((uint)sleepMs);
        }
    }

    private void CalclateFps()
    {
        _frameCounter++;

        if (_fpsWatch.Elapsed.TotalSeconds > 1)
        {
            FramesPerSecond = _frameCounter;
            _frameCounter = 0;
            _fpsWatch.Restart();
        }
    }
}