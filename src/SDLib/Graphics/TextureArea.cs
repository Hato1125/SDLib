using SDL2;

namespace SDLib.Graphics;

public class TextureArea : ITextureReturnable, IDisposable
{
    private IntPtr _rendererPtr;
    private Texture2D _texture;

    /// <summary>
    /// TextureAreaを初期化する
    /// </summary>
    /// <param name="renderer">レンダラー</param>
    /// <param name="width">横幅</param>
    /// <param name="height">高さ</param>
    public TextureArea(IntPtr renderer, int width, int height)
    {
        _rendererPtr = renderer;
        var format = SDL.SDL_PIXELFORMAT_ARGB8888;
        var access = (int)SDL.SDL_TextureAccess.SDL_TEXTUREACCESS_TARGET;
        var texture = SDL.SDL_CreateTexture(renderer, format, access, width, height);

        _texture = new(renderer, texture, true);
    }

    /// <summary>
    /// レンダリングする
    /// </summary>
    /// <param name="render">レンダリング</param>
    /// <param name="returnRenderer">レンダラーを戻すときのテクスチャ</param>
    /// <returns>Texture2D</returns>
    public Texture2D Render(Action render, IntPtr? returnRenderer = null)
    {
        var returnRenderTarget = returnRenderer != null ? returnRenderer.Value : IntPtr.Zero;

        SDL.SDL_SetRenderDrawBlendMode(_rendererPtr, SDL.SDL_BlendMode.SDL_BLENDMODE_BLEND);
        SDL.SDL_SetRenderDrawColor(_rendererPtr, 0, 0, 0, 0);
        SDL.SDL_SetRenderTarget(_rendererPtr, _texture.GetTexturePtr());
        SDL.SDL_RenderClear(_rendererPtr);
        render.Invoke();
        SDL.SDL_SetRenderTarget(_rendererPtr, returnRenderTarget);

        return _texture;
    }

    /// <summary>
    /// Textureを取得する
    /// </summary>
    /// <returns>Texture2D</returns>
    public Texture2D GetTexture()
        => _texture;

    /// <summary>
    /// Texture2Dを破棄する
    /// </summary>
    public void Dispose()
    {
        _texture.Dispose();
        GC.SuppressFinalize(this);
    }
}