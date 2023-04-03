using System.Drawing;
using System.Numerics;
using System.Runtime.InteropServices;
using SDL2;

namespace SDLib.Graphics;

public class Texture2D : ITextureReturnable, IDisposable
{
    private readonly SDL.SDL_Surface _surface;
    private readonly IntPtr _surfacePtr;
    private readonly IntPtr _texturePtr;
    private readonly IntPtr _rendererPtr;
    private IntPtr _drawRectPtr;
    private SDL.SDL_FRect _drawRect;
    private SDL.SDL_FRect _diffDrawRect;

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
    /// 描画時のレクタングル
    /// </summary>
    public SDL.SDL_Rect ImageRectangle { get; private set; }

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
    public Size ImageSize { get; init; }

    /// <summary>
    /// スケールを考慮した画像のサイズ
    /// </summary>
    public SizeF ScaleSize
    {
        get
        {
            return new(
                ImageSize.Width * ImageScale.X,
                ImageSize.Height * ImageScale.Y
           );
        }
    }

    /// <summary>
    /// Texture2Dを初期化する
    /// </summary>
    public Texture2D()
    {
        _surfacePtr = IntPtr.Zero;
        _texturePtr = IntPtr.Zero;
        _rendererPtr = IntPtr.Zero;
        _drawRectPtr = IntPtr.Zero;
        AlphaMod = byte.MaxValue;
        Rotation = 0.0;
        BlendMode = SDL.SDL_BlendMode.SDL_BLENDMODE_BLEND;
        RenderFlip = SDL.SDL_RendererFlip.SDL_FLIP_NONE;
        ReferencePoint = ReferencePoint.TopLeft;
        ImageScale = Vector2.One;
        ImageSize = Size.Empty;

        _drawRectPtr = Marshal.AllocCoTaskMem(Marshal.SizeOf<SDL.SDL_FRect>());
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

        _rendererPtr = renderer;
        _surfacePtr = SDL_image.IMG_Load(fileName);
        if (_surfacePtr == IntPtr.Zero)
            throw new Exception(SDL_image.IMG_GetError());

        _texturePtr = SDL.SDL_CreateTextureFromSurface(renderer, _surfacePtr);
        _surface = Marshal.PtrToStructure<SDL.SDL_Surface>(_surfacePtr);
        ImageSize = new(_surface.w, _surface.h);
        ImageRectangle = new() { w = _surface.w, h = _surface.h };
    }

    /// <summary>
    /// Texture2Dを初期化する
    /// </summary>
    /// <param name="renderer">レンダラー</param>
    /// <param name="surfacePtr">サーフェスのポインタ</param>
    /// <param name="rect">画像のサイズ</param>
    public Texture2D(IntPtr renderer, IntPtr imagePtr, SetImagePointerType setType, SDL.SDL_Rect? rect = null)
        : this()
    {
        if (renderer == IntPtr.Zero || imagePtr == IntPtr.Zero)
            throw new Exception("An invalid handle was passed.");

        _rendererPtr = renderer;

        if (setType == SetImagePointerType.SurfacePointer)
        {
            _surfacePtr = imagePtr;
            _texturePtr = SDL.SDL_CreateTextureFromSurface(renderer, _surfacePtr);
        }
        else
        {
            _texturePtr = imagePtr;
        }

        if (setType == SetImagePointerType.SurfacePointer)
            _surface = Marshal.PtrToStructure<SDL.SDL_Surface>(imagePtr);

        SDL.SDL_QueryTexture(_texturePtr, out uint _, out int _, out int w, out int h);
        ImageSize = new(w, h);
        ImageRectangle = new() { w = w, h = h };
    }

    /// <summary>
    /// Textureをレンダリングする
    /// </summary>
    /// <param name="x">X座標</param>
    /// <param name="y">Y座標</param>
    public void Render(float x, float y)
    {
        var imageRect = ImageRectangle;
        var refePoint = CalculateReferencePoint();
        _drawRect.x = x - refePoint.x;
        _drawRect.y = y - refePoint.y;
        _drawRect.w = ImageSize.Width * ImageScale.X;
        _drawRect.h = ImageSize.Height * ImageScale.Y;

        WatchDrawRectChange();
        SDL.SDL_SetTextureBlendMode(_texturePtr, BlendMode);
        SDL.SDL_SetTextureAlphaMod(_texturePtr, AlphaMod);
        SDL.SDL_RenderCopyExF(
            _rendererPtr,
            _texturePtr,
            IntPtr.Zero,
            _drawRectPtr,
            Rotation,
            ref refePoint,
            RenderFlip
        );
    }

    /// <summary>
    /// Surfaceを取得する
    /// </summary>
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
        SDL.SDL_DestroyTexture(_texturePtr);
        SDL.SDL_FreeSurface(_surfacePtr);
        Marshal.FreeCoTaskMem(_drawRectPtr);
        GC.SuppressFinalize(this);
    }

    private void WatchDrawRectChange()
    {
        if (_diffDrawRect.x != _drawRect.x
            || _diffDrawRect.y != _drawRect.y
            || _diffDrawRect.w != _drawRect.w
            || _diffDrawRect.h != _drawRect.h)
        {
            Marshal.StructureToPtr(_drawRect, _drawRectPtr, false);
            _diffDrawRect = _drawRect;
        }
    }

    private SDL.SDL_FPoint CalculateReferencePoint() => ReferencePoint switch
    {
        ReferencePoint.TopLeft => new() { x = 0, y = 0 },
        ReferencePoint.TopCenter => new() { x = ImageSize.Width / 2, y = 0 },
        ReferencePoint.TopRight => new() { x = ImageSize.Width, y = 0 },
        ReferencePoint.CenterLeft => new() { x = 0, y = ImageSize.Height / 2 },
        ReferencePoint.Center => new() { x = ScaleSize.Width / 2, y = ScaleSize.Height / 2 },
        ReferencePoint.CenterRight => new() { x = ImageSize.Width, y = ImageSize.Height / 2 },
        ReferencePoint.BottomLeft => new() { x = 0, y = ImageSize.Height },
        ReferencePoint.BottomCenter => new() { x = ImageSize.Width / 2, y = ImageSize.Height },
        ReferencePoint.BottomRight => new() { x = ImageSize.Width, y = ImageSize.Height },
        _ => new() { x = 0, y = 0 }
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

public enum SetImagePointerType
{
    SurfacePointer,
    TexturePointer,
}