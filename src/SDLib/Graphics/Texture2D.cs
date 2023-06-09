using System.Drawing;
using SDL2;

namespace SDLib.Graphics;

public class Texture2D : ITextureReturnable, IDisposable
{
    private readonly IntPtr _rendererPtr = IntPtr.Zero;
    private IntPtr _surfacePtr = IntPtr.Zero;
    private IntPtr _texturePtr = IntPtr.Zero;
    private SDL.SDL_Rect _imageRect;
    private SDL.SDL_FRect _drawRect;

    /// <summary>
    /// 画像の横幅
    /// </summary>
    public readonly int Width;

    /// <summary>
    /// 画像の高さ
    /// </summary>
    public readonly int Height;

    /// <summary>
    /// 横幅のスケール
    /// </summary>
    public float WidthScale { get; set; }

    /// <summary>
    /// 高さのスケール
    /// </summary>
    public float HeightScale { get; set; }

    /// <summary>
    /// 実際の画像の横幅
    /// </summary>
    public float ActualWidth { get => Width * WidthScale; }

    /// <summary>
    /// 実際の画像の高さ
    /// </summary>
    public float ActualHeight { get => Height * HeightScale; }

    /// <summary>
    /// アルファのモッド
    /// </summary>
    public byte AlphaMod { get; set; }

    /// <summary>
    /// 画像の回転率
    /// </summary>
    public double Rotation { get; set; }

    /// <summary>
    /// 描画時の画像の輝度
    /// </summary>
    public Color Brightness { get; set; }

    /// <summary>
    /// 回転時の描画基準点
    /// </summary>
    public ReferencePoint RotationPoint { get; set; }

    /// <summary>
    /// レンダリングの基準点
    /// </summary>
    public ReferencePoint RenderPoint { get; set; }

    /// <summary>
    /// 描画時のブレンドモード
    /// </summary>
    public SDL.SDL_BlendMode BlendMode { get; set; }

    /// <summary>
    /// 描画時のレンダーフリップ
    /// </summary>
    public SDL.SDL_RendererFlip RenderFlip { get; set; }

    /// <summary>
    /// Texture2Dを初期化する
    /// </summary>
    public Texture2D(IntPtr renderer)
    {
        if (renderer == IntPtr.Zero)
            throw new ArgumentNullException(nameof(renderer), "An invalid pointer was passed.");

        _rendererPtr = renderer;
        WidthScale = 1f;
        HeightScale = 1f;
        AlphaMod = byte.MaxValue;
        Rotation = 0;
        Brightness = Color.White;
        RotationPoint = ReferencePoint.TopLeft;
        RenderPoint = ReferencePoint.TopLeft;
        BlendMode = SDL.SDL_BlendMode.SDL_BLENDMODE_BLEND;
        RenderFlip = SDL.SDL_RendererFlip.SDL_FLIP_NONE;
    }

    /// <summary>
    /// Texture2Dを初期化する
    /// </summary>
    /// <param name="renderer">レンダラー</param>
    /// <param name="fileName">ファイル名</param>
    public Texture2D(IntPtr renderer, string fileName, bool isSurfaceDispose = false)
        : this(renderer)
    {
        _surfacePtr = SDL_image.IMG_Load(fileName);
        if (_surfacePtr == IntPtr.Zero)
            throw new Exception(SDL_image.IMG_GetError());

        _texturePtr = SDL.SDL_CreateTextureFromSurface(renderer, _surfacePtr);
        if (_texturePtr == IntPtr.Zero)
            throw new Exception(SDL.SDL_GetError());

        if (isSurfaceDispose)
        {
            SDL.SDL_FreeSurface(_surfacePtr);
            _surfacePtr = IntPtr.Zero;
        }

        SDL.SDL_QueryTexture(_texturePtr, out uint _, out int _, out int w, out int h);
        (Width, Height) = (w, h);
    }

    /// <summary>
    /// Texture2Dを初期化する
    /// </summary>
    /// <param name="renderer">レンダラー</param>
    /// <param name="surfacePtr">サーフェスのポインタ</param>
    /// <param name="rect">画像のサイズ</param>
    public Texture2D(IntPtr renderer, IntPtr image, bool isTexture, bool isSurfaceDispose = false)
        : this(renderer)
    {
        if (image == IntPtr.Zero)
            throw new ArgumentNullException(nameof(image), "An invalid pointer was passed.");

        if (isTexture)
        {
            _texturePtr = image;
        }
        else
        {
            _surfacePtr = image;

            _texturePtr = SDL.SDL_CreateTextureFromSurface(renderer, image);
            if (_texturePtr == IntPtr.Zero)
                throw new Exception(SDL.SDL_GetError());

            if (isSurfaceDispose)
            {
                SDL.SDL_FreeSurface(_surfacePtr);
                _surfacePtr = IntPtr.Zero;
            }
        }

        _rendererPtr = renderer;

        SDL.SDL_QueryTexture(_texturePtr, out uint _, out int _, out int w, out int h);
        (Width, Height) = (w, h);
    }

    ~Texture2D() => Dispose();

    /// <summary>
    /// Textureをレンダリングする
    /// </summary>
    /// <param name="x">X座標</param>
    /// <param name="y">Y座標</param>
    public void Render(float x, float y, System.Drawing.Rectangle? rectangle = null)
    {
        if (rectangle == null)
            rectangle = new(0, 0, Width, Height);

        var rotationPoint = CalcRotationPoint();
        var renderPoint = CalcRenderPoint();
        _imageRect.x = rectangle.Value.X;
        _imageRect.y = rectangle.Value.Y;
        _imageRect.w = rectangle.Value.Width;
        _imageRect.h = rectangle.Value.Height;
        _drawRect.x = x + renderPoint.X + rectangle.Value.X;
        _drawRect.y = y + renderPoint.Y + rectangle.Value.Y;
        _drawRect.w = (rectangle.Value.Width - rectangle.Value.X) * WidthScale;
        _drawRect.h = (rectangle.Value.Height - rectangle.Value.Y) * HeightScale;

        SDL.SDL_SetTextureBlendMode(_texturePtr, BlendMode);
        SDL.SDL_SetTextureAlphaMod(_texturePtr, AlphaMod);
        SDL.SDL_SetTextureColorMod(_texturePtr, Brightness.R, Brightness.G, Brightness.B);
        SDL.SDL_RenderCopyExF(_rendererPtr, _texturePtr, ref _imageRect, ref _drawRect, Rotation, ref rotationPoint, RenderFlip);
    }

    /// <summary>
    /// サーフェスポインタを取得する
    /// </summary>
    /// <returns>サーフェスポインタ</returns>
    public IntPtr GetSurfacePtr()
        => _surfacePtr;

    /// <summary>
    /// テクスチャポインタを取得する
    /// </summary>
    /// <returns>テクスチャポインタ</returns>
    public IntPtr GetTexturePtr()
        => _texturePtr;

    /// <summary>
    /// Textureを取得する
    /// </summary>
    /// <returns>Texture2D</returns>
    public Texture2D GetTexture()
        => this;

    /// <summary>
    /// Texture2Dを破棄する
    /// </summary>
    public void Dispose()
    {
        if (_texturePtr != IntPtr.Zero)
        {
            SDL.SDL_DestroyTexture(_texturePtr);
            _texturePtr = IntPtr.Zero;
        }

        if (_surfacePtr != IntPtr.Zero)
        {
            SDL.SDL_FreeSurface(_surfacePtr);
            _surfacePtr = IntPtr.Zero;
        }

        GC.SuppressFinalize(this);
    }

    private SDL.SDL_FPoint CalcRotationPoint() => RotationPoint switch
    {
        ReferencePoint.TopLeft => new() { x = 0, y = 0 },
        ReferencePoint.TopCenter => new() { x = Width / 2, y = 0 },
        ReferencePoint.TopRight => new() { x = Width, y = 0 },
        ReferencePoint.CenterLeft => new() { x = 0, y = Height / 2 },
        ReferencePoint.Center => new() { x = Width / 2, y = Height / 2 },
        ReferencePoint.CenterRight => new() { x = Width, y = Height / 2 },
        ReferencePoint.BottomLeft => new() { x = 0, y = Height },
        ReferencePoint.BottomCenter => new() { x = Width / 2, y = Height },
        ReferencePoint.BottomRight => new() { x = Width, y = Height },
        _ => new() { x = 0, y = 0 }
    };

    private (float X, float Y) CalcRenderPoint() => RenderPoint switch {
        ReferencePoint.TopLeft => (0, 0),
        ReferencePoint.TopCenter => (ActualWidth / 2, 0),
        ReferencePoint.TopRight => (ActualWidth, 0),
        ReferencePoint.CenterLeft => (0, ActualHeight / 2),
        ReferencePoint.Center => (ActualWidth / 2, ActualHeight / 2),
        ReferencePoint.CenterRight => (ActualWidth, ActualHeight / 2),
        ReferencePoint.BottomLeft => (0, ActualHeight),
        ReferencePoint.BottomCenter => (ActualWidth / 2, ActualHeight),
        ReferencePoint.BottomRight => (ActualWidth, ActualHeight),
        _ => (0, 0)
    };
}

public enum ReferencePoint
{
    TopLeft,
    TopCenter,
    TopRight,
    CenterLeft,
    Center,
    CenterRight,
    BottomLeft,
    BottomCenter,
    BottomRight,
}