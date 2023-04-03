using System.Text;
using SDL2;

namespace SDLib.Graphics;

public class FontRenderer : ITextureReturnable, IDisposable
{
    private IntPtr _rendererPtr;
    private Texture2D _texture;
    private FontFamily _bufferFamily;
    private string _bufferText;
    private bool _isFastCreate;

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
        _isFastCreate = true;
        _rendererPtr = IntPtr.Zero;
        _bufferText = string.Empty;
        _texture = new();
        _bufferFamily = new();
        FontFamily = new();
        Text = string.Empty;
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
    public Texture2D Render()
    {
        // フォントファミリーのパラメーターが変更されたら再度作り直す
        if (_isFastCreate
            ||Text != _bufferText
            || FontFamily.FontName != _bufferFamily.FontName
            || FontFamily.FontSize != _bufferFamily.FontSize
            || FontFamily.FontColor != _bufferFamily.FontColor)
        {
            CreateFontTexture();

            _isFastCreate = false;
            _bufferText = Text;
            _bufferFamily = FontFamily;
        }

        return _texture;
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
        _texture.Dispose();
        GC.SuppressFinalize(this);
    }

    private void CreateFontTexture()
    {
        if (string.IsNullOrWhiteSpace(Text))
            return;

        byte[] unicodeByte = Encoding.Unicode.GetBytes(Text);
        string unicodeText = Encoding.Unicode.GetString(unicodeByte);
        var textColor = new SDL.SDL_Color()
        {
            r = FontFamily.FontColor.R,
            g = FontFamily.FontColor.G,
            b = FontFamily.FontColor.B
        };

        // フォントを開く
        var font = SDL_ttf.TTF_OpenFont(FontFamily.FontName, FontFamily.FontSize);
        if (font == IntPtr.Zero)
            throw new Exception(SDL_ttf.TTF_GetError());

        // サーフェスの作成
        var surface = SDL_ttf.TTF_RenderUNICODE_Blended(font, unicodeText, textColor);
        if (surface == IntPtr.Zero)
            throw new Exception(SDL_ttf.TTF_GetError());

        _texture.Dispose();
        _texture = new(_rendererPtr, surface, false);

        SDL_ttf.TTF_CloseFont(font);
        SDL.SDL_FreeSurface(surface);
    }
}