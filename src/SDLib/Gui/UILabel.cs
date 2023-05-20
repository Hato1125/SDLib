using SDLib.Graphics;

namespace SDLib.Gui;

public class UILabel : UIElement
{
    #region Public Member

    /// <summary>
    /// フォントレンダラー
    /// </summary>
    public readonly FontRenderer FontRenderer;

    /// <summary>
    /// テキスト
    /// </summary>
    public string Text
    {
        get => FontRenderer.Text;
        set => FontRenderer.Text = value;
    }

    /// <summary>
    /// テキストの水平方向の位置
    /// </summary>
    public UIHorizontal TextHorizonta { get; set; }

    /// <summary>
    /// テキストの垂直方向の位置
    /// </summary>
    public UIVertical TextVertical { get; set; }

    #endregion

    /// <summary>
    /// Labelを初期化する
    /// </summary>
    /// <param name="rendererPtr">Rendererのポインタ</param>
    /// <param name="windowPtr">Windowのポインタ</param>
    /// <param name="width">横幅</param>
    /// <param name="height">高さ</param>
    /// <param name="family">フォントファミリー</param>
    public UILabel(nint rendererPtr, nint windowPtr, int width, int height, in FontFamily family)
        : base(rendererPtr, windowPtr, width, height)
    {
        FontRenderer = new(rendererPtr, windowPtr, family) { Text = nameof(UILabel) };
        OnUIRendering += RenderLabel;
    }

    protected override void Disposing(bool isDisposing)
    {
        FontRenderer?.Dispose();

        base.Disposing(isDisposing);
    }

    void RenderLabel()
    {
        var fontPosition = (
            X: UIPosition.CalculatePosition(Width, FontRenderer.GetTexture().ActualWidth, TextHorizonta),
            Y: UIPosition.CalculatePosition(Height, FontRenderer.GetTexture().ActualHeight, TextHorizonta)
        );

        FontRenderer.Render()?.Render(fontPosition.X, fontPosition.Y);
    }
}