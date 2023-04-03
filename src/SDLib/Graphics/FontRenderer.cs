using System.Drawing;
using System.Numerics;
using System.Text;
using System.Runtime.InteropServices;
using SDL2;

namespace SDLib.Graphics;

public class FontRenderer : ITextureReturnable, IDisposable
{
    private IntPtr _rendererPtr;
    private IntPtr _textTexturePtr;
    private IntPtr _renderRectPtr;
    private SDL.SDL_FRect _bufferRenderRect;
    private FontFamily _bufferFontFamily;
    private Texture2D _texture;
    private bool _isFastCreate;
    private string _bufferText;

    /// <summary>
    /// フォントファミリー
    /// </summary>
    public FontFamily FontFamily { get; set; }

    /// <summary>
    /// テキスト
    /// </summary>
    public string Text { get; set; }

    /// <summary>
    /// FontRendererを初期化する
    /// </summary>
    public FontRenderer()
    {
        _rendererPtr = IntPtr.Zero;
        _textTexturePtr = IntPtr.Zero;
        _texture = new();
        _isFastCreate = true;
        _bufferText = string.Empty;
        Text = string.Empty;
        FontFamily = new(string.Empty, 0);

        _renderRectPtr = Marshal.AllocCoTaskMem(Marshal.SizeOf<SDL.SDL_Rect>());
    }

    /// <summary>
    /// FontRendererを初期化する
    /// </summary>
    /// <param name="renderer">レンダラー</param>
    /// <param name="fontFamily">フォントファミリー</param>
    public FontRenderer(IntPtr renderer, FontFamily fontFamily)
        : this()
    {
        if (renderer == IntPtr.Zero)
            throw new Exception("An invalid pointer was passed.");

        _rendererPtr = renderer;
        FontFamily = fontFamily;
    }

    /// <summary>
    /// フォントレンダラーを更新する
    /// </summary>
    public void Update()
    {
        if (_isFastCreate
            || Text != _bufferText
            || FontFamily.FontName != _bufferFontFamily.FontName
            || FontFamily.FontSize != _bufferFontFamily.FontSize
            || FontFamily.FontColor != _bufferFontFamily.FontColor)
        {
            CreateFontTexture();

            _isFastCreate = false;
            _bufferText = Text;
            _bufferFontFamily = FontFamily;
        }
    }

    /// <summary>
    /// Textureを取得する
    /// </summary>
    /// <returns>Texture2D</returns>
    public Texture2D GetTexture()
        => _texture;

    /// <summary>
    /// フォントを破棄する
    /// </summary>
    public void Dispose()
    {
        SDL.SDL_DestroyTexture(_textTexturePtr);
        Marshal.FreeCoTaskMem(_renderRectPtr);
    }

    private void CreateFontTexture()
    {
        if (string.IsNullOrWhiteSpace(Text))
            return;

        Tracer.PrintInfo("Create font texture.");

        // 文字をUnicodeに直す
        byte[] unicodeBytes = Encoding.Unicode.GetBytes(Text);
        string unicodeText = Encoding.Unicode.GetString(unicodeBytes);

        var color = new SDL.SDL_Color()
        {
            r = FontFamily.FontColor.R,
            g = FontFamily.FontColor.G,
            b = FontFamily.FontColor.B,
        };

        // フォントと文字のサーフェスの作成
        IntPtr nowTexturePtr = _textTexturePtr;

        IntPtr fontPtr = SDL_ttf.TTF_OpenFont(FontFamily.FontName, FontFamily.FontSize);
        if (fontPtr == IntPtr.Zero)
            throw new Exception(SDL_ttf.TTF_GetError());

        IntPtr textPtr = SDL_ttf.TTF_RenderUNICODE_Blended(fontPtr, unicodeText, color);
        if (textPtr == IntPtr.Zero)
            throw new Exception(SDL.SDL_GetError());

        var surface = Marshal.PtrToStructure<SDL.SDL_Surface>(textPtr);

        _textTexturePtr = SDL.SDL_CreateTextureFromSurface(_rendererPtr, textPtr);
        _texture = new(_rendererPtr, _textTexturePtr, true);

        // フォントとサーフェスの破棄
        SDL_ttf.TTF_CloseFont(fontPtr);
        SDL.SDL_FreeSurface(textPtr);

        // テクスチャが正しく設定されていない可能性があるので同じじゃないかを判定する
        if (_textTexturePtr != nowTexturePtr)
            SDL.SDL_DestroyTexture(nowTexturePtr);
    }
}