using System.Drawing;
using SDLibRectangle = SDLib.Graphics.Rectangle;

namespace SDLib.Gui;

public class UIPanel : UIElement
{
    /// <summary>
    /// 矩形
    /// </summary>
    public readonly SDLibRectangle Rectangle;

    /// <summary>
    /// パネルの背景色
    /// </summary>
    public Color BackColor
    {
        get => Rectangle.Color;
        set => Rectangle.Color = value;
    }

    /// <summary>
    /// Panelの初期化を行う
    /// </summary>
    /// <param name="rendererPtr">Rendererのポインタ</param>
    /// <param name="windowPtr">Windowのポインタ</param>
    /// <param name="width">横幅</param>
    /// <param name="height">高さ</param>
    /// <param name="color">背景色</param>
    public UIPanel(nint rendererPtr, nint windowPtr, int width, int height, in Color? color = null)
        : base(rendererPtr, windowPtr, width, height)
    {
        Rectangle = new(rendererPtr, width, height, color);
        OnUIRendering += RenderPanel;
    }

    void RenderPanel()
    {
        Rectangle.Width = Width;
        Rectangle.Height = Height;

        Rectangle.Render(0, 0);
    }
}