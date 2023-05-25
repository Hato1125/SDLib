using SDLib.Graphics;
using System.Drawing;
using SDLibRectangle = SDLib.Graphics.Rectangle;
using SDLibFontFamily = SDLib.Graphics.FontFamily;

namespace SDLib.Gui;

public class UIButton : UIElement
{
    #region Private Member

    private double _fadeCounter;
    private bool _isColorUpdate;
    private bool _isFade;

    #endregion

    #region Public Member

    /// <summary>
    /// 矩形
    /// </summary>
    public readonly SDLibRectangle Rectangle;

    /// <summary>
    /// フォントレンダラー
    /// </summary>
    public readonly FontRenderer FontRenderer;

    /// <summary>
    /// アイコンのテクスチャ
    /// </summary>
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
    /// フェードのMs
    /// </summary>
    public double FadeMs { get; set; }

    private Color _backColor;
    /// <summary>
    /// 背景色
    /// </summary>
    public Color BackColor
    {
        get => _backColor;
        set
        {
            _backColor = value;
            _isColorUpdate = true;
        }
    }

    private Color _clickColor;
    /// <summary>
    /// クリック時の背景色
    /// </summary>
    public Color ClickColor
    {
        get => _clickColor;
        set
        {
            _clickColor = value;
            _isColorUpdate = true;
        }
    }

    public int TextHorizontalOffset { get; set; }
    public int TextVerticalOffset { get; set; }
    public int IconHorizontalOffset { get; set; }
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
    /// Buttonを初期化する
    /// </summary>
    /// <param name="rendererPtr">Rendererのポインタ</param>
    /// <param name="windowPtr">Windowのポインタ</param>
    /// <param name="width">横幅</param>
    /// <param name="height">高さ</param>
    /// <param name="family">フォントファミリー</param>
    /// <param name="backColor">背景色</param>
    /// <param name="clickColor">クリック時の背景色</param>
    public UIButton(
        nint rendererPtr,
        nint windowPtr,
        int width,
        int height,
        in SDLibFontFamily family,
        in Color backColor,
        in Color clickColor)
        : base(rendererPtr, windowPtr, width, height)
    {
        Rectangle = new(rendererPtr, width, height, backColor);
        FontRenderer = new(rendererPtr, windowPtr, family) { Text = nameof(UIButton) };
        BackColor = backColor;
        ClickColor = clickColor;
        TextHorizontal = UIHorizontal.Center;
        TextVertical = UIVertical.Center;
        IconHorizontal = UIHorizontal.Center;
        IconVertical = UIVertical.Center;
        FadeMs = 0.125;

        OnUIUpdating += UpdateButton;
        OnUIRendering += RenderButton;
    }

    protected override void Disposing(bool isDisposing)
    {
        FontRenderer.Dispose();

        base.Disposing(isDisposing);
    }

    void UpdateButton(double deltaTime)
    {
        if (IsPushing())
        {
            if(_fadeCounter < 90)
            {
                _fadeCounter += deltaTime * (90 / FadeMs);
                _isFade = true;

                if (_fadeCounter > 90)
                    _fadeCounter = 90;
            }
            else
            {
                _isFade = false;
            }
        }
        else
        {
            if(_fadeCounter > 0)
            {
                _fadeCounter -= deltaTime * (90 / FadeMs);
                _isFade = true;

                if(_fadeCounter < 0)
                    _fadeCounter = 0;
            }
            else
            {
                _isFade = false;
            }
        }

        if (_isFade || _isColorUpdate)
        {
            var sin = Math.Sin(_fadeCounter * Math.PI / 180);
            var r = BackColor.R + (int)(sin * (ClickColor.R - BackColor.R));
            var g = BackColor.G + (int)(sin * (ClickColor.G - BackColor.G));
            var b = BackColor.B + (int)(sin * (ClickColor.B - BackColor.B));

            Rectangle.Color = Color.FromArgb(r, g, b);

            _isColorUpdate = false;
        }
    }

    void RenderButton()
    {
        Rectangle.Width = Width;
        Rectangle.Height = Height;
        Rectangle.Render(0, 0);

        if (!string.IsNullOrWhiteSpace(Text))
        {
            var fontPosition = (
                X: UIPosition.CalculatePosition(Width, FontRenderer.GetTexture().ActualWidth, TextHorizontal),
                Y: UIPosition.CalculatePosition(Height, FontRenderer.GetTexture().ActualHeight, TextVertical)
            );

            FontRenderer.Render()?.Render(fontPosition.X, fontPosition.Y);
        }

        if(Icon != null)
        {
            var iconPosition = (
                X: UIPosition.CalculatePosition(Width, Icon.ActualWidth, IconHorizontal),
                Y: UIPosition.CalculatePosition(Height, Icon.ActualHeight, IconVertical)
            );

            Icon.Render(iconPosition.X, iconPosition.Y);
        }
    }
}