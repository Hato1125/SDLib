using SDLib.Graphics;
using System.Drawing;
using SDLibFontFamily = SDLib.Graphics.FontFamily;
using SDLibRectangle = SDLib.Graphics.Rectangle;


namespace SDLib.Gui;

public class UIButton : UIButtonBase
{
    #region Public Member

    /// <summary>
    /// FontRenderer
    /// </summary>
    public readonly FontRenderer FontRenderer;

    /// <summary>
    /// Rectangle
    /// </summary>
    public readonly SDLibRectangle Rectangle;
    public Texture2D? Icon { get; set; }

    /// <summary>
    /// テキスト
    /// </summary>
    public string Text
    {
        get => FontRenderer.Text;
        set => FontRenderer.Text = value;
    }

    /// <summary>
    /// テキストの水平方向のオフセット
    /// </summary>
    public int TextHorizontalOffset { get; set; }

    /// <summary>
    /// テキストの垂直方向のオフセット
    /// </summary>
    public int TextVerticalOffset { get; set; }

    /// <summary>
    /// アイコンの水平方向のオフセット
    /// </summary>
    public int IconHorizontalOffset { get; set; }

    /// <summary>
    /// アイコンの垂直方向のオフセット
    /// </summary>
    public int IconVerticalOffset { get; set; }

    /// <summary>
    /// テキストの水平方向の位置
    /// </summary>
    public UIHorizontal TextHorizontal { get; set; }

    /// <summary>
    /// テキストの垂直方向の位置
    /// </summary>
    public UIVertical TextVertical { get; set; }

    /// <summary>
    /// アイコンの水平方向の位置
    /// </summary>
    public UIHorizontal IconHorizontal { get; set; }

    /// <summary>
    /// アイコンの垂直方向の位置
    /// </summary>
    public UIVertical IconVertical { get; set; }

    #endregion

    /// <summary>
    /// UIButtonを初期化する
    /// </summary>
    /// <param name="rendererPtr">Rendererのポインタ</param>
    /// <param name="windowPtr">Windowのポインタ</param>
    /// <param name="width">横幅</param>
    /// <param name="height">高さ</param>
    public UIButton(nint rendererPtr, nint windowPtr, int width, int height, SDLibFontFamily family)
        : base(rendererPtr, windowPtr, width, height)
    {
        FontRenderer = new(rendererPtr, windowPtr, family);
        Rectangle = new(rendererPtr, width, height, Color.Empty);
        TextHorizontal = UIHorizontal.Center;
        TextVertical = UIVertical.Center;
        Text = nameof(UIButton);

        OnColorUpdating += () => Rectangle.Color = NowColor;
        OnUIRendering += RenderRectangle;
        OnUIRendering += RenderText;
        OnUIRendering += RenderIcon;
    }

    void RenderRectangle()
    {
        Rectangle.Width = Width;
        Rectangle.Height = Height;
        Rectangle.Render(0, 0);
    }

    void RenderText()
    {
        if (string.IsNullOrWhiteSpace(Text))
            return;

        if (FontRenderer.Render() == null)
            return;

        var fontPosition = (
            X: UIPosition.CalculatePosition(Width, FontRenderer.GetTexture().Width, TextHorizontal) + TextHorizontalOffset,
            Y: UIPosition.CalculatePosition(Height, FontRenderer.GetTexture().Height, TextVertical) + TextVerticalOffset
        );

        FontRenderer.GetTexture().Render(fontPosition.X, fontPosition.Y);
    }

    void RenderIcon()
    {
        if (Icon == null)
            return;

        var iconPosition = (
            X: UIPosition.CalculatePosition(Width, Icon.ActualWidth, IconHorizontal) + IconHorizontalOffset,
            Y: UIPosition.CalculatePosition(Height, Icon.ActualHeight, IconVertical) + IconVerticalOffset
        );

        Icon.Render(iconPosition.X, iconPosition.Y);
    }

    protected override void ImplOnOff(double deltaTime)
    {
        IsOn = IsPushing();
    }
}