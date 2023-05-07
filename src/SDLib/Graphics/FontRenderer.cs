using SDL2;

namespace SDLib.Graphics;

public class FontRenderer : ITextureReturnable, IDisposable
{
    private readonly IntPtr _rendererPtr;
    private Texture2D _texture;
    private FontFamily _bufferFamily;
    private string _bufferText;
    private bool _isFastCreate;

    private TextureArea? _textureArea;

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
    public FontRenderer(IntPtr renderer)
    {
        if (renderer == IntPtr.Zero)
            throw new ArgumentException("An invalid pointer was passed.", nameof(renderer));

        _rendererPtr = renderer;
        _isFastCreate = true;
        _rendererPtr = IntPtr.Zero;
        _bufferText = string.Empty;
        _texture = new(renderer);
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
        : this(renderer)
    {
        _rendererPtr = renderer;
        FontFamily = fontFamily;
    }

    /// <summary>
    /// フォントをレンダリングする
    /// </summary>
    public Texture2D? Render()
    {
        // フォントファミリーのパラメーターが変更されたら再度作り直す
        if (_isFastCreate
            || Text != _bufferText
            || FontFamily.FontName != _bufferFamily.FontName
            || FontFamily.FontSize != _bufferFamily.FontSize
            || FontFamily.FontColor != _bufferFamily.FontColor)
        {
            CreateFontTexture();

            _isFastCreate = false;
            _bufferText = Text;
            _bufferFamily = FontFamily;
        }

        return _textureArea?.GetTexture();
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

    /// <summary>
    /// フォントテクスチャを作成する
    /// </summary>
    private void CreateFontTexture()
    {
        if (string.IsNullOrWhiteSpace(Text))
            return;

        if (Text.Contains('\n'))
            CreateMultiLineText(Text);
        else
            CreateSingleLineText(Text);
    }

    private void CreateMultiLineText(string text)
    {
        // 文字を改行コードごとに分割する
        var splitText = text.Split('\n');

        // 複数行分の文字Textureを作成する
        var textureSize = new (int width, int height)[splitText.Length];
        var textTexture = new Texture2D[splitText.Length];
        var fontPtr = SDL_ttf.TTF_OpenFont(FontFamily.FontName, FontFamily.FontSize);
        var fontColor = new SDL.SDL_Color()
        {
            r = FontFamily.FontColor.R,
            g = FontFamily.FontColor.G,
            b = FontFamily.FontColor.B
        };

        if (fontPtr == IntPtr.Zero)
            throw new Exception(SDL_ttf.TTF_GetError());

        SDL_ttf.TTF_SetFontStyle(fontPtr, (int)FontFamily.FontStyle);

        // サーフェスの作成
        for (int i = 0; i < splitText.Length; i++)
        {
            var surface = SDL_ttf.TTF_RenderUNICODE_Blended(fontPtr, splitText[i], fontColor);
            if (surface == IntPtr.Zero)
                throw new Exception(SDL_ttf.TTF_GetError());

            textTexture[i] = new(_rendererPtr, surface, false);
            textureSize[i] = (textTexture[i].Width, textTexture[i].Height);
        }

        // サイズを計算
        var textSize = (width: 0, height: 0);
        for (int i = 0; i < splitText.Length; i++)
            textSize.height += textureSize[i].height;

        textSize.width = textureSize.Max(textureSize => textureSize.width);

        // 文字たちを合成
        _textureArea?.Dispose();

        _textureArea = new(_rendererPtr, textSize.width, textSize.height);
        _textureArea.Render(() =>
        {
            for (int i = 0; i < splitText.Length; i++)
                textTexture[i].Render(0, textTexture[i].Height * i);
        });
        _texture = _textureArea.GetTexture();

        // 破棄処理
        foreach (var texture in textTexture)
            texture.Dispose();

        SDL_ttf.TTF_CloseFont(fontPtr);
    }

    private void CreateSingleLineText(string text)
    {
        var textColor = new SDL.SDL_Color()
        {
            r = FontFamily.FontColor.R,
            g = FontFamily.FontColor.G,
            b = FontFamily.FontColor.B
        };

        // フォントを開く
        var fontPtr = SDL_ttf.TTF_OpenFont(FontFamily.FontName, FontFamily.FontSize);
        if (fontPtr == IntPtr.Zero)
            throw new Exception(SDL_ttf.TTF_GetError());

        SDL_ttf.TTF_SetFontStyle(fontPtr, (int)FontFamily.FontStyle);

        // サーフェスの作成
        var surface = SDL_ttf.TTF_RenderUNICODE_Blended(fontPtr, text, textColor);
        if (surface == IntPtr.Zero)
            throw new Exception(SDL_ttf.TTF_GetError());

        var texture = new Texture2D(_rendererPtr, surface, false);

        // 文字たちを合成
        _textureArea?.Dispose();

        _textureArea = new(_rendererPtr, texture.Width, texture.Height);
        _textureArea.Render(() => texture.Render(0, 0));
        _texture = _textureArea.GetTexture();

        texture.Dispose();
        SDL_ttf.TTF_CloseFont(fontPtr);
    }
}