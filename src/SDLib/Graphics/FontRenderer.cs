using System.Drawing;
using System.Runtime.InteropServices;
using SDL2;

namespace SDLib.Graphics;

public class FontRenderer : IDisposable
{
    private Texture2D? _fontTexture;
    private Texture2D? _boaderTexture;

    public FontFamily FontFamily { get; init; }

    public FontRenderer()
    {
        _fontTexture = null;
        _boaderTexture = null;
    }

    public FontRenderer(IntPtr renderer, FontFamily fontFamily, string text)
        : this()
    {
        FontFamily = fontFamily;

        //Unicodeに変換
        byte[] unicodeBytes = System.Text.Encoding.Unicode.GetBytes(text);
        string unicodeText = System.Text.Encoding.Unicode.GetString(unicodeBytes);

        IntPtr fontPtr = SDL_ttf.TTF_OpenFont(fontFamily.FontName, fontFamily.FontSize);

        // 文字のサーフェスの作成
        IntPtr textSurface = SDL_ttf.TTF_RenderUNICODE_Blended(fontPtr, unicodeText, ConvertSDLColor(fontFamily.FontColor));

        _fontTexture = new(renderer, textSurface);
        _fontTexture.ReferencePoint = ReferencePoint.Center;
        SDL_ttf.TTF_CloseFont(fontPtr);
    }

    public Texture2D? GetTexture()
        => _fontTexture;

    public void Dispose()
    {
        _boaderTexture?.Dispose();
        _fontTexture?.Dispose();
    }

    private SDL.SDL_Color ConvertSDLColor(Color color)
        => new() { r = color.R, g = color.G, b = color.B };
}