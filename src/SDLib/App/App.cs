using System.Diagnostics;
using System.Drawing;
using SDL2;

namespace SDLib;

public class App
{
    private double _fpsCounter;
    private double _fpsPeriodCounter;
    private string _windowTitle;
    private SDL.SDL_WindowFlags _windowFlag;
    private SDL.SDL_RendererFlags _renderFlag;
    private Size _windowSize;
    private Point? _windowPosition;
    private Size? _windowMinSize;
    private Size? _windowMaxSize;
    private readonly Stopwatch _totalStopwatch = new();
    private readonly Stopwatch _deltaStopwatch = new();
    private static readonly AppWindow _window = new();
    private static readonly AppInfo _info = new();

    /// <summary>
    /// Windowの情報
    /// </summary>
    public readonly IReadOnlyAppWindow Window = _window;

    /// <summary>
    /// アプリケーションの情報
    /// </summary>
    public readonly IReadOnlyAppInfo Info = _info;

    /// <summary>
    /// 初期化時に呼ばれる
    /// </summary>
    public event Action<IReadOnlyAppInfo>? OnInitialize = delegate { };

    /// <summary>
    /// ループ時に呼ばれる
    /// </summary>
    public event Action<IReadOnlyAppInfo>? OnMainLoop = delegate { };

    /// <summary>
    /// 終了時に呼ばれる
    /// </summary>
    public event Action<IReadOnlyAppInfo>? OnFinish = delegate { };

    /// <summary>
    /// イベント発生時に呼ばれる
    /// </summary>
    public event Action<IReadOnlyAppInfo, SDL.SDL_Event>? OnEvent = delegate { };

    /// <summary>
    /// 起動しているか
    /// </summary>
    public bool IsRunning { get; private set; }

    /// <summary>
    /// 現在のフレームレート
    /// </summary>
    public double Framerate { get; private set; }

    /// <summary>
    /// 最大フレームレート
    /// </summary>
    public double MaxFramerate { get; set; }

    /// <summary>
    /// Appを初期化する
    /// </summary>
    /// <param name="windowTitle">Windowのフラグ</param>
    /// <param name="windowFlag">Windowのフラグ</param>
    /// <param name="renderFlag">Rebdererのフラグ</param>
    /// <param name="windowSize">Windowのサイズ</param>
    /// <param name="windowPosition">Windowの位置</param>
    /// <param name="windowMinSize">Windowの最小サイズ</param>
    /// <param name="windowMaxSize">Windowの最大サイズ</param>
    public App(
        string windowTitle,
        SDL.SDL_WindowFlags windowFlag,
        SDL.SDL_RendererFlags renderFlag,
        Size windowSize,
        Point? windowPosition = null,
        Size? windowMinSize = null,
        Size? windowMaxSize = null)
    {
        IsRunning = true;
        MaxFramerate = 120;

        _windowTitle = windowTitle;
        _windowFlag = windowFlag;
        _renderFlag = renderFlag;
        _windowSize = windowSize;
        _windowPosition = windowPosition;
        _windowMinSize = windowMaxSize;
        _windowMaxSize = windowMaxSize;
    }

    /// <summary>
    /// アプリケーションを起動する
    /// </summary>
    public void Run()
    {
        Init();
        Loop();
        Finish();
    }

    private void Init()
    {
        if (_window.InitWindow(
            _windowTitle,
            _windowFlag,
            _windowSize,
            _windowPosition,
            _windowMinSize,
            _windowMaxSize) == -1)
        {
            throw new Exception(SDL.SDL_GetError());
        }

        if (_window.InitRender(_renderFlag) == -1)
        {
            SDL.SDL_DestroyWindow(Window.WindowPtr);
            throw new Exception(SDL.SDL_GetError());
        }

        _info.WindowPtr = _window.WindowPtr;
        _info.RenderPtr = _window.RenderPtr;

        InitSDL();
        OnInitialize?.Invoke(Info);
    }

    private void InitSDL()
    {
        if (SDL.SDL_Init(SDL.SDL_INIT_EVERYTHING) < 0)
        {
            _window.Dispose();
            throw new Exception(SDL.SDL_GetError());
        }

        if (SDL_image.IMG_Init(SDL_image.IMG_InitFlags.IMG_INIT_PNG) < 0)
        {
            _window.Dispose();
            throw new Exception(SDL_image.IMG_GetError());
        }

        if(SDL_ttf.TTF_Init() < 0)
        {
            _window.Dispose();
            throw new Exception(SDL_ttf.TTF_GetError());
        }
    }

    private void Loop()
    {
        while (IsRunning)
        {
            while (SDL.SDL_PollEvent(out SDL.SDL_Event e) != 0)
            {
                if (e.type == SDL.SDL_EventType.SDL_QUIT)
                    IsRunning = false;

                OnEvent?.Invoke(Info, e);
            }

            SDL.SDL_RenderClear(_window.RenderPtr);
            OnMainLoop?.Invoke(Info);
            SDL.SDL_RenderPresent(_window.RenderPtr);
            FramerateLimmiter();
            MeasurementFramerate();

            _info.DeltaTime = _deltaStopwatch.Elapsed;
            _info.TotalTime = _totalStopwatch.Elapsed;

            _deltaStopwatch.Restart();
        }
    }

    private void FramerateLimmiter()
    {
        if(MaxFramerate < 0)
            return;

        double ms = 1.0 / MaxFramerate;

        if (_deltaStopwatch.Elapsed.TotalSeconds < ms)
        {
            double sleepMs = (ms - _deltaStopwatch.Elapsed.TotalSeconds) * 1000.0;
            SDL.SDL_Delay((uint)sleepMs);
        }
    }

    private void MeasurementFramerate()
    {
        if (_fpsPeriodCounter >= 1)
        {
            Framerate = _fpsCounter;
            _fpsCounter = 0;
            _fpsPeriodCounter = 0;
        }
        else
        {
            _fpsCounter++;
            _fpsPeriodCounter += _deltaStopwatch.Elapsed.TotalSeconds;
        }
    }

    private void Finish()
    {
        OnFinish?.Invoke(_info);

        _window.Dispose();
        QuitSDL();
    }

    private void QuitSDL()
    {
        SDL_image.IMG_Quit();
        SDL_ttf.TTF_Quit();
        SDL.SDL_Quit();
    }
}