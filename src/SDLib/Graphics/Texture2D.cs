using System.Drawing;
using SDL2;

namespace SDLib.Graphics;

public class Texture2D : ITextureReturnable, IDisposable
{
    private readonly IntPtr _rendererPtr;
    private IntPtr _surfacePtr;
    private IntPtr _texturePtr;
    private SDL.SDL_FRect _renderRect;

    /// <summary>
    /// 画像の横幅
    /// </summary>
    public int Width { get; private set; }

    /// <summary>
    /// 画像の高さ
    /// </summary>
    public int Height { get; private set; }

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
    public RotationPoint RotationPoint { get; set; }

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
    public Texture2D()
    {
        _surfacePtr = IntPtr.Zero;
        _texturePtr = IntPtr.Zero;
        AlphaMod = byte.MaxValue;
        Rotation = 0;
        Brightness = Color.White;
        RotationPoint = RotationPoint.TopLeft;
        BlendMode = SDL.SDL_BlendMode.SDL_BLENDMODE_BLEND;
        RenderFlip = SDL.SDL_RendererFlip.SDL_FLIP_NONE;
    }

    /// <summary>
    /// Texture2Dを初期化する
    /// </summary>
    /// <param name="renderer">レンダラー</param>
    /// <param name="fileName">ファイル名</param>
    public Texture2D(IntPtr renderer, string fileName)
        : this()
    {
        if (renderer == IntPtr.Zero)
            throw new Exception("An invalid handle was passed.");

        _surfacePtr = SDL_image.IMG_Load(fileName);
        if (_surfacePtr == IntPtr.Zero)
            throw new Exception(SDL_image.IMG_GetError());

        _rendererPtr = renderer;
        _texturePtr = SDL.SDL_CreateTextureFromSurface(renderer, _surfacePtr);

        SDL.SDL_QueryTexture(_texturePtr, out uint _, out int _, out int w, out int h);
        (Width, Height) = (w, h);
    }

    /// <summary>
    /// Texture2Dを初期化する
    /// </summary>
    /// <param name="renderer">レンダラー</param>
    /// <param name="surfacePtr">サーフェスのポインタ</param>
    /// <param name="rect">画像のサイズ</param>
    public Texture2D(IntPtr renderer, IntPtr imagePtr, bool isTexture)
        : this()
    {
        if (renderer == IntPtr.Zero || imagePtr == IntPtr.Zero)
            throw new ArgumentException("An invalid argument was passed.");

        if (isTexture)
        {
            _texturePtr = imagePtr;
        }
        else
        {
            _surfacePtr = imagePtr;
            _texturePtr = SDL.SDL_CreateTextureFromSurface(renderer, imagePtr);
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
    public void Render(float x, float y)
    {
        var refePoint = CalculateRotationPoint();
        _renderRect.x = x - refePoint.x;
        _renderRect.y = y - refePoint.y;
        _renderRect.w = Width * WidthScale;
        _renderRect.h = Height * HeightScale;

        SDL.SDL_SetTextureBlendMode(_texturePtr, BlendMode);
        SDL.SDL_SetTextureAlphaMod(_texturePtr, AlphaMod);
        SDL.SDL_SetTextureColorMod(_texturePtr, Brightness.R, Brightness.G, Brightness.B);
        SDL.SDL_RenderCopyExF(_rendererPtr, _texturePtr, IntPtr.Zero, ref _renderRect, Rotation, ref refePoint, RenderFlip);
    }

    /// <summary>
    /// サーフェスポインタを取得する
    /// </summary>
    /// <returns>サーフェスポインタ</returns>
    public IntPtr GetSurface()
        => _surfacePtr;

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

    private SDL.SDL_FPoint CalculateRotationPoint() => RotationPoint switch
    {
        RotationPoint.TopLeft => new() { x = 0, y = 0 },
        RotationPoint.TopCenter => new() { x = Width / 2, y = 0 },
        RotationPoint.TopRight => new() { x = Width, y = 0 },
        RotationPoint.CenterLeft => new() { x = 0, y = Height / 2 },
        RotationPoint.Center => new() { x = Width / 2, y = Height / 2 },
        RotationPoint.CenterRight => new() { x = Width, y = Height / 2 },
        RotationPoint.BottomLeft => new() { x = 0, y = Height },
        RotationPoint.BottomCenter => new() { x = Width / 2, y = Height },
        RotationPoint.BottomRight => new() { x = Width, y = Height },
        _ => new() { x = 0, y = 0 }
    };
}

public enum RotationPoint
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