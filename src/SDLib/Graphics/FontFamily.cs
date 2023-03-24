using System.Drawing;

namespace SDLib.Graphics;

public struct FontFamily
{
    /// <summary>
    /// フォントのパス
    /// </summary>
    public string FontName { get; init; }

    /// <summary>
    /// フォントのサイズ
    /// </summary>
    public int FontSize { get; init; }

    /// <summary>
    /// フォントの色
    /// </summary>
    public Color FontColor { get; init; }

    /// <summary>
    /// フォントファミリーの初期化
    /// </summary>
    /// <param name="fontName">フォント名</param>
    /// <param name="fontSize">フォントサイズ</param>
    /// <param name="fontColor">フォントの色</param>
    public FontFamily(string fontName, int fontSize, Color? fontColor = null)
    {
        FontName = fontName;
        FontSize = fontSize;
        FontColor = fontColor ?? Color.White;
    }
}