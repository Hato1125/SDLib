using System.Drawing;
using SDL2;

namespace SDLib;

public class AppWindow : IReadOnlyAppWindow, IDisposable
{
    /// <summary>
    /// Windowのポインタ
    /// </summary>
    public IntPtr WindowPtr { get; private set; }

    /// <summary>
    /// レンダラーのポインタ
    /// </summary>
    public IntPtr RenderPtr { get; private set; }

    /// <summary>
    /// Windowの位置
    /// </summary>
    public Point WindowPosition
    {
        get
        {
            if (WindowPtr != IntPtr.Zero)
            {
                SDL.SDL_GetWindowPosition(WindowPtr, out int x, out int y);
                return new(x, y);
            }

            return Point.Empty;
        }
        set
        {
            if (WindowPtr != IntPtr.Zero)
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
            if (WindowPtr != IntPtr.Zero)
            {
                SDL.SDL_GetWindowSize(WindowPtr, out int w, out int h);
                return new(w, h);
            }

            return Size.Empty;
        }
        set
        {
            if (WindowPtr != IntPtr.Zero)
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
            if (WindowPtr != IntPtr.Zero)
            {
                SDL.SDL_GetWindowMinimumSize(WindowPtr, out int w, out int h);
                return new(w, h);
            }

            return Size.Empty;
        }
        set
        {
            if (WindowPtr != IntPtr.Zero)
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
            if (WindowPtr != IntPtr.Zero)
            {
                SDL.SDL_GetWindowMaximumSize(WindowPtr, out int w, out int h);
                return new(w, h);
            }

            return Size.Empty;
        }
        set
        {
            if (WindowPtr != IntPtr.Zero)
                SDL.SDL_SetWindowMaximumSize(WindowPtr, value.Width, value.Height);
        }
    }

    /// <summary>
    /// Windowのタイトル
    /// </summary>
    public string WindowTitle
    {
        get => WindowPtr != IntPtr.Zero ? SDL.SDL_GetWindowTitle(WindowPtr) : string.Empty;
        set
        {
            if (WindowPtr != IntPtr.Zero)
                SDL.SDL_SetWindowTitle(WindowPtr, value);
        }
    }

    /// <summary>
    /// Windowを初期化する
    /// </summary>
    /// <param name="windowTitle">Windowのタイトル</param>
    /// <param name="flags">Windowのフラグ</param>
    /// <param name="windowSize">Windowのサイズ</param>
    /// <param name="windowPosition">Windowの位置</param>
    /// <param name="windowMinSize">Windowの最小サイズ</param>
    /// <param name="windowMaxSize">Windowの最大サイズ</param>
    public sbyte InitWindow(string windowTitle,
        SDL.SDL_WindowFlags flags,
        Size windowSize,
        Point? windowPosition = null,
        Size? windowMinSize = null,
        Size? windowMaxSize = null)
    {
        var windowPos = (
            X: windowPosition != null ? windowPosition.Value.X : SDL.SDL_WINDOWPOS_CENTERED,
            Y: windowPosition != null ? windowPosition.Value.Y : SDL.SDL_WINDOWPOS_CENTERED
        );

        WindowPtr = SDL.SDL_CreateWindow(
            windowTitle,
            windowPos.X,
            windowPos.Y,
            windowSize.Width,
            windowSize.Height,
            flags
        );

        if (windowMinSize != null)
            WindowMinSize = windowMinSize.Value;

        if (windowMaxSize != null)
            WindowMaxSize = windowMaxSize.Value;

        return (sbyte)(WindowPtr == IntPtr.Zero ? -1 : 0);
    }

    /// <summary>
    /// レンダラーを初期化する
    /// </summary>
    /// <param name="flags">レンダラーのフラグ</param>
    public sbyte InitRender(SDL.SDL_RendererFlags flags)
    {
        RenderPtr = SDL.SDL_CreateRenderer(WindowPtr, -1, flags);

        return (sbyte)(RenderPtr == IntPtr.Zero ? -1 : 0);
    }

    /// <summary>
    /// Windowを破棄する
    /// </summary>
    public void Dispose()
    {
        SDL.SDL_DestroyWindow(WindowPtr);
        SDL.SDL_DestroyRenderer(RenderPtr);
        GC.SuppressFinalize(this);
    }
}