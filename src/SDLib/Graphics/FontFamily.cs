using System.Drawing;
using SDL2;

namespace SDLib.Graphics;

public readonly struct FontFamily
{
    /// <summary>
    /// フォントのパス
    /// </summary>
    public readonly string FontName;

    /// <summary>
    /// フォントのサイズ
    /// </summary>
    public readonly int FontSize;

    /// <summary>
    /// フォントの色
    /// </summary>
    public readonly Color FontColor;

    /// <summary>
    /// フォントのスタイル
    /// </summary>
    public readonly FontStyle FontStyle;

    /// <summary>
    /// フォントファミリーの初期化
    /// </summary>
    /// <param name="fontName">フォント名</param>
    /// <param name="fontSize">フォントサイズ</param>
    /// <param name="fontColor">フォントの色</param>
    public FontFamily(string fontName, int fontSize, Color? fontColor = null, FontStyle? fontStyle = null)
    {
        FontName = fontName;
        FontSize = fontSize;
        FontColor = fontColor ?? Color.White;
        FontStyle = fontStyle ?? FontStyle.Normal;
    }
}

public enum FontStyle
{
    Normal = SDL_ttf.TTF_STYLE_NORMAL,
    Bold = SDL_ttf.TTF_STYLE_BOLD,
    Italic = SDL_ttf.TTF_STYLE_ITALIC,
    Underline = SDL_ttf.TTF_STYLE_UNDERLINE,
    Strikethrough = SDL_ttf.TTF_STYLE_STRIKETHROUGH,
}