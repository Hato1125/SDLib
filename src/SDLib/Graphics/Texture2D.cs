using System.Drawing;
using System.Numerics;
using System.Runtime.InteropServices;
using SDL2;

namespace SDLib.Graphics;

public class Texture2D : IDisposable
{
    private readonly SDL.SDL_Surface _surface;
    private readonly IntPtr _surfacePtr;
    private readonly IntPtr _texturePtr;
    private readonly IntPtr _rendererPtr;
    private IntPtr _drawRectPtr;
    private SDL.SDL_FRect _drawRect;
    private SDL.SDL_Rect _imageRect;
    private SDL.SDL_FRect _diffDrawRect;

    /// <summary>
    /// �A���t�@�̃��b�h
    /// </summary>
    public byte AlphaMod { get; set; }

    /// <summary>
    /// �摜�̉�]��
    /// </summary>
    public double Rotation { get; set; }

    /// <summary>
    /// �`�掞�̃u�����h���[�h
    /// </summary>
    public SDL.SDL_BlendMode BlendMode { get; set; }

    /// <summary>
    /// �`�掞�̃����_�[�t���b�v
    /// </summary>
    public SDL.SDL_RendererFlip RenderFlip { get; set; }

    /// <summary>
    /// �`�掞�̕`���_
    /// </summary>
    public ReferencePoint ReferencePoint { get; set; }

    /// <summary>
    /// �摜�̃X�P�[��
    /// </summary>
    public Vector2 ImageScale { get; set; }

    /// <summary>
    /// �摜�̃T�C�Y
    /// </summary>
    public Size ImageSize { get; init; }

    /// <summary>
    /// �X�P�[�����l�������摜�̃T�C�Y
    /// </summary>
    public SizeF ScaleSize
    {
        get
        {
            return new(
                ImageSize.Width * ScaleSize.Width,
                ImageSize.Height * ScaleSize.Height
           );
        }
    }

    /// <summary>
    /// Texture2D������������
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
    }

    /// <summary>
    /// Texture2D������������
    /// </summary>
    /// <param name="renderer">�����_���[</param>
    /// <param name="fileName">�t�@�C����</param>
    /// <exception cref="Exception"></exception>
    public Texture2D(IntPtr renderer, string fileName)
        : this()
    {
        if(renderer == IntPtr.Zero)
            throw new Exception("An invalid handle was passed.");

        _rendererPtr = renderer;
        _surfacePtr = SDL_image.IMG_Load(fileName);
        if (_surfacePtr == IntPtr.Zero)
            throw new Exception(SDL_image.IMG_GetError());

        _texturePtr = SDL.SDL_CreateTextureFromSurface(renderer, _surfacePtr);
        _surface = Marshal.PtrToStructure<SDL.SDL_Surface>(_surfacePtr);
        _imageRect = new() { w = _surface.w, h = _surface.h };
        ImageSize = new(_surface.w, _surface.h);
    }

    /// <summary>
    /// Texture2D������������
    /// </summary>
    /// <param name="renderer">�����_���[</param>
    /// <param name="surfacePtr">�T�[�t�F�X�̃|�C���^</param>
    /// <exception cref="Exception"></exception>
    public Texture2D(IntPtr renderer, IntPtr surfacePtr)
        : this()
    {
        if (renderer == IntPtr.Zero || surfacePtr == IntPtr.Zero)
            throw new Exception("An invalid handle was passed.");

        _rendererPtr = renderer;
        _surfacePtr = surfacePtr;
        _texturePtr = SDL.SDL_CreateTextureFromSurface(renderer, _surfacePtr);
        _surface = Marshal.PtrToStructure<SDL.SDL_Surface>(surfacePtr);
        _imageRect = new() { w = _surface.w, h = _surface.h };
        ImageSize = new(_surface.w, _surface.h);
    }

    /// <summary>
    /// Texture���@�`�悷��
    /// </summary>
    /// <param name="x">X���W</param>
    /// <param name="y">Y���W</param>
    public void Draw(float x, float y)
    {
        var refePoint = CalculateReferencePoint();
        _drawRect.x = x;
        _drawRect.y = y;
        _drawRect.w = ImageSize.Width * ImageScale.X;
        _drawRect.h = ImageSize.Height * ImageScale.Y;

        WatchDrawRectChange();
        SDL.SDL_SetTextureBlendMode(_texturePtr, BlendMode);
        SDL.SDL_SetTextureAlphaMod(_texturePtr, AlphaMod);
        SDL.SDL_RenderCopyExF(
            _rendererPtr,
            _texturePtr,
            ref _imageRect,
            _drawRectPtr,
            Rotation,
            ref refePoint,
            RenderFlip
        );
    }

    /// <summary>
    /// Surface���擾����
    /// </summary>
    public SDL.SDL_Surface GetSurface()
        => _surface;

    /// <summary>
    /// Textur�̃|�C���^���擾����
    /// </summary>
    /// <returns></returns>
    public IntPtr GetTexture()
        => _texturePtr;

    /// <summary>
    /// Texture2D��j������
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
        if(_diffDrawRect.x != _drawRect.x
            || _diffDrawRect.y != _drawRect.y
            || _diffDrawRect.w != _drawRect.w
            || _diffDrawRect.h != _drawRect.h)
        {
            Tracer.PrintInfo("Convert drawing information structure to pointer");

            _drawRectPtr = Marshal.AllocCoTaskMem(Marshal.SizeOf<SDL.SDL_FRect>());
            Marshal.StructureToPtr(_drawRect, _drawRectPtr, false);

            _diffDrawRect = _drawRect;
        }
    }

    public SDL.SDL_FPoint CalculateReferencePoint() => ReferencePoint switch
    {
        ReferencePoint.TopLeft => new() { x = 0, y = 0 },
        ReferencePoint.TopCenter => new() { x = ImageSize.Width / 2, y = 0 },
        ReferencePoint.TopRight => new() { x = ImageSize.Width, y = 0 },
        ReferencePoint.CenterLeft => new() { x = 0, y = ImageSize.Height / 2 },
        ReferencePoint.Center => new() { x = ImageSize.Width / 2, y = ImageSize.Height / 2 },
        ReferencePoint.CenterRight => new() { x = ImageSize.Width,  y =ImageSize.Height / 2},
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