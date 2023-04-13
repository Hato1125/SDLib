using System.Drawing;
using SDL2;

namespace SDLib.Graphics;

public static class Shape
{
    /// <summary>
    /// 矩形をレンダリングする
    /// </summary>
    /// <param name="renderer">レンダラー</param>
    /// <param name="x">X座標</param>
    /// <param name="y">Y座標</param>
    /// <param name="width">横幅</param>
    /// <param name="height">高さ</param>
    /// <param name="color">色</param>
    /// <param name="opacity">透明度</param>
    public static void RenderRect(IntPtr renderer, int x, int y, int width, int height, Color color, byte opacity = 255)
    {
        SDL.SDL_Rect shapeRect;
        shapeRect.x = x;
        shapeRect.y = y;
        shapeRect.w = width;
        shapeRect.h = height;

        SDL.SDL_GetRenderDrawColor(renderer, out byte r, out byte g, out byte b, out byte a);
        SDL.SDL_SetRenderDrawColor(renderer, color.R, color.G, color.B, opacity);
        SDL.SDL_RenderFillRect(renderer, ref shapeRect);
        SDL.SDL_SetRenderDrawColor(renderer, r, g, b, a);
    }

    /// <summary>
    /// ジオメトリーをレンダリングする
    /// </summary>
    /// <param name="renderer">レンダラー</param>
    /// <param name="positions">頂点の位置</param>
    /// <param name="colors">頂点の色</param>
    /// <param name="opacitys">頂点の透明度</param>
    public static void RenderGeometry(IntPtr renderer, (int X, int Y)[] positions, Color[] colors, byte[] opacitys)
    {
        var geometryVertex = new SDL.SDL_Vertex[positions.Length];
        var geometryIndices = new int[positions.Length];
        for (int i = 0; i < geometryVertex.Length; i++)
        {
            geometryIndices[i] = i;
            geometryVertex[i] = new()
            {
                position = new()
                {
                    x = positions[i].X,
                    y = positions[i].Y
                },
                color = new()
                {
                    r = colors[i].R,
                    g = colors[i].G,
                    b = colors[i].B,
                    a = opacitys[i]
                }
            };
        }

        SDL.SDL_GetRenderDrawColor(renderer, out byte r, out byte g, out byte b, out byte a);
        SDL.SDL_RenderGeometry(renderer,
            IntPtr.Zero,
            geometryVertex,
            geometryVertex.Length,
            geometryIndices,
            geometryIndices.Length
        );
    }
}