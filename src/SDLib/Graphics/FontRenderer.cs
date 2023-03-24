using System.Drawing;
using System.Numerics;
using System.Text;
using System.Runtime.InteropServices;
using SDL2;

namespace SDLib.Graphics;

public class FontRenderer : IDisposable
{
    private IntPtr _rendererPtr;
    private IntPtr _textTexturePtr;
    private IntPtr _renderRectPtr;
    private SDL.SDL_Rect _imageRect;
    private SDL.SDL_FRect _renderRect;
    private SDL.SDL_FRect _bufferRenderRect;
    private FontFamily _bufferFontFamily;
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
    /// アルファのモッド
    /// </summary>
    public byte AlphaMod { get; set; }

    /// <summary>
    /// 画像の回転率
    /// </summary>
    public double Rotation { get; set; }

    /// <summary>
    /// 描画時のブレンドモード
    /// </summary>
    public SDL.SDL_BlendMode BlendMode { get; set; }

    /// <summary>
    /// 描画時のレンダーフリップ
    /// </summary>
    public SDL.SDL_RendererFlip RenderFlip { get; set; }

    /// <summary>
    /// 描画時の描画基準点
    /// </summary>
    public ReferencePoint ReferencePoint { get; set; }

    /// <summary>
    /// 画像のスケール
    /// </summary>
    public Vector2 ImageScale { get; set; }

    /// <summary>
    /// 画像のサイズ
    /// </summary>
    public Size ImageSize { get; private set; }

    /// <summary>
    /// FontRendererを初期化する
    /// </summary>
    public FontRenderer()
    {
        _rendererPtr = IntPtr.Zero;
        _textTexturePtr = IntPtr.Zero;
        _imageRect = new();
        _renderRect = new();
        _isFastCreate = true;
        _bufferText = string.Empty;
        Text = string.Empty;
        AlphaMod = byte.MaxValue;
        Rotation = 0.0;
        BlendMode = SDL.SDL_BlendMode.SDL_BLENDMODE_BLEND;
        RenderFlip = SDL.SDL_RendererFlip.SDL_FLIP_NONE;
        ReferencePoint = ReferencePoint.TopLeft;
        ImageScale = Vector2.One;
        ImageSize = Size.Empty;
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
    /// フォントをレンダリングする
    /// </summary>
    /// <param name="x">X座標</param>
    /// <param name="y">Y座標</param>
    public void Draw(float x, float y)
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

        _renderRect.x = x;
        _renderRect.y = y;
        _renderRect.w = _imageRect.w * ImageScale.X;
        _renderRect.h = _imageRect.h * ImageScale.Y;
        WatchDrawRectChange();

        var refePoint = CalculateReferencePoint();
        SDL.SDL_SetTextureBlendMode(_textTexturePtr, BlendMode);
        SDL.SDL_SetTextureAlphaMod(_textTexturePtr, AlphaMod);
        SDL.SDL_RenderCopyExF(
            _rendererPtr,
            _textTexturePtr,
            ref _imageRect,
            _renderRectPtr,
            Rotation,
            ref refePoint,
            RenderFlip
        );
    }

    /// <summary>
    /// フォントを破棄する
    /// </summary>
    public void Dispose()
    {
        SDL.SDL_DestroyTexture(_textTexturePtr);
        Marshal.FreeCoTaskMem(_renderRectPtr);
    }

    private void WatchDrawRectChange()
    {
        if (_bufferRenderRect.x != _renderRect.x
            || _bufferRenderRect.y != _renderRect.y
            || _bufferRenderRect.w != _renderRect.w
            || _bufferRenderRect.h != _renderRect.h)
        {
            Marshal.StructureToPtr(_renderRect, _renderRectPtr, false);
            _bufferRenderRect = _renderRect;
        }
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

        IntPtr textPtr = SDL_ttf.TTF_RenderUNICODE_Solid(fontPtr, unicodeText, color);
        if (textPtr == IntPtr.Zero)
            throw new Exception(SDL.SDL_GetError());

        var surface = Marshal.PtrToStructure<SDL.SDL_Surface>(textPtr);

        _imageRect.x = 0;
        _imageRect.y = 0;
        _imageRect.w = surface.w;
        _imageRect.h = surface.h;
        ImageSize = new(surface.w, surface.h);
        _textTexturePtr = SDL.SDL_CreateTextureFromSurface(_rendererPtr, textPtr);

        // フォントとサーフェスの破棄
        SDL_ttf.TTF_CloseFont(fontPtr);
        SDL.SDL_FreeSurface(textPtr);

        // テクスチャが正しく設定されていない可能性があるので同じじゃないかを判定する
        if (_textTexturePtr != nowTexturePtr)
            SDL.SDL_DestroyTexture(nowTexturePtr);
    }

    private SDL.SDL_FPoint CalculateReferencePoint() => ReferencePoint switch
    {
        ReferencePoint.TopLeft => new() { x = 0, y = 0 },
        ReferencePoint.TopCenter => new() { x = ImageSize.Width / 2, y = 0 },
        ReferencePoint.TopRight => new() { x = ImageSize.Width, y = 0 },
        ReferencePoint.CenterLeft => new() { x = 0, y = ImageSize.Height / 2 },
        ReferencePoint.Center => new() { x = ImageSize.Width / 2, y = ImageSize.Height / 2 },
        ReferencePoint.CenterRight => new() { x = ImageSize.Width, y = ImageSize.Height / 2 },
        ReferencePoint.BottomLeft => new() { x = 0, y = ImageSize.Height },
        ReferencePoint.BottomCenter => new() { x = ImageSize.Width / 2, y = ImageSize.Height },
        ReferencePoint.BottomRight => new() { x = ImageSize.Width, y = ImageSize.Height },
        _ => new() { x = 0, y = 0 }
    };
}