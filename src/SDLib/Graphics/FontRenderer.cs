using System.Drawing;
using System.Runtime.InteropServices;
using SDL2;

namespace SDLib.Graphics;

public class FontRenderer : IDisposable
{
    private Texture2D? _fontTexture;

    /// <summary>
    /// フォントファミリー
    /// </summary>
    /// <value></value>
    public FontFamily FontFamily { get; init; }

    /// <summary>
    /// 表示される文字
    /// </summary>
    public string Text { get; init; }

    /// <summary>
    /// フォントレンダラーを初期化する
    /// </summary>
    public FontRenderer()
    {
        _fontTexture = null;
        Text = string.Empty;
    }

    /// <summary>
    /// フォントレンダラーを初期化する
    /// </summary>
    /// <param name="renderer">レンダラー</param>
    /// <param name="fontFamily">フォントファミリー</param>
    /// <param name="text">テキスト</param>
    public FontRenderer(IntPtr renderer, FontFamily fontFamily, string text)
        : this()
    {
        FontFamily = fontFamily;

        //Unicodeに変換
        byte[] unicodeBytes = System.Text.Encoding.Unicode.GetBytes(text);
        string unicodeText = System.Text.Encoding.Unicode.GetString(unicodeBytes);
        Text = unicodeText;

        IntPtr fontPtr = SDL_ttf.TTF_OpenFont(fontFamily.FontName, fontFamily.FontSize);
        if (fontPtr == IntPtr.Zero)
            throw new Exception(SDL_ttf.TTF_GetError());

        // 文字のサーフェスの作成
        IntPtr textSurface = SDL_ttf.TTF_RenderUNICODE_Blended(fontPtr, unicodeText, ConvertSDLColor(fontFamily.FontColor));

        _fontTexture = new(renderer, textSurface);
        _fontTexture.ReferencePoint = ReferencePoint.Center;
        SDL_ttf.TTF_CloseFont(fontPtr);
    }

    /// <summary>
    /// Texture2Dを取得する
    /// </summary>
    /// <returns></returns>
    public Texture2D? GetTexture2D()
        => _fontTexture;

    /// <summary>
    /// フォントレンダラーを破棄する
    /// </summary>
    public void Dispose()
    {
        _fontTexture?.Dispose();
        GC.SuppressFinalize(this);
    }

    private SDL.SDL_Color ConvertSDLColor(Color color)
        => new() { r = color.R, g = color.G, b = color.B };
}