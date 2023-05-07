using System.Drawing;
using SDL2;

namespace SDLib.Graphics;

public class Rectangle
{
    private readonly IntPtr _renderer;
    private SDL.SDL_Rect _rect;

    /// <summary>
    /// 矩形の横幅
    /// </summary>
    public int Width { get; set; }

    /// <summary>
    /// 矩形の高さ
    /// </summary>
    public int Height { get; set; }

    /// <summary>
    /// 矩形の透明度
    /// </summary>
    public byte Opacity { get; set; }

    /// <summary>
    /// 矩形の色
    /// </summary>
    public Color Color { get; set; }

    /// <summary>
    /// 矩形を初期化する
    /// </summary>
    /// <param name="renderer">レンダラー</param>
    /// <param name="width">横幅</param>
    /// <param name="height">高さ</param>
    /// <param name="color">色</param>
    /// <param name="opacity">透明度</param>
    public Rectangle(nint renderer, int width, int height, Color? color = null, byte opacity = 255)
    {
        if (renderer == IntPtr.Zero)
            throw new ArgumentNullException(nameof(renderer), "An invalid pointer was passed.");

        _renderer = renderer;

        Width = width;
        Height = height;
        Color = color ?? Color.White;
        Opacity = opacity;
    }

    /// <summary>
    /// 矩形をレンダリングする
    /// </summary>
    /// <param name="x">X座標</param>
    /// <param name="y">Y座標</param>
    public void Render(int x, int y)
    {
        _rect.x = x;
        _rect.y = y;
        _rect.w = Width;
        _rect.h = Height;

        SDL.SDL_GetRenderDrawBlendMode(_renderer, out SDL.SDL_BlendMode blend);
        SDL.SDL_GetRenderDrawColor(_renderer, out byte r, out byte g, out byte b, out byte a);
        SDL.SDL_SetRenderDrawBlendMode(_renderer, SDL.SDL_BlendMode.SDL_BLENDMODE_BLEND);
        SDL.SDL_SetRenderDrawColor(_renderer, Color.R, Color.G, Color.B, Opacity);
        SDL.SDL_RenderFillRect(_renderer, ref _rect);
        SDL.SDL_SetRenderDrawColor(_renderer, r, g, b, a);
        SDL.SDL_SetRenderDrawBlendMode(_renderer, blend);
    }
}