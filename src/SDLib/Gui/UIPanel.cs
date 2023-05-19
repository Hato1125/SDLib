using System.Drawing;
using SDLibRectangle = SDLib.Graphics.Rectangle;

namespace SDLib.Gui;

public class UIPanel : UIElement
{
    private readonly SDLibRectangle _rectangle;

    /// <summary>
    /// パネルの背景色
    /// </summary>
    public Color BackColor
    {
        get => _rectangle.Color;
        set => _rectangle.Color = value;
    }

    /// <summary>
    /// Panelの初期化を行う
    /// </summary>
    /// <param name="rendererPtr">Rendererのポインタ</param>
    /// <param name="windowPtr">Windowのポインタ</param>
    /// <param name="width">横幅</param>
    /// <param name="height">高さ</param>
    /// <param name="color">背景色</param>
    public UIPanel(nint rendererPtr, nint windowPtr, int width, int height, Color? color = null)
        : base(rendererPtr, windowPtr, width, height)
    {
        _rectangle = new(rendererPtr, width, height, color);
        OnUIRendering += RenderPanel;
    }

    void RenderPanel()
    {
        _rectangle.Width = Width;
        _rectangle.Height = Height;

        _rectangle.Render(0, 0);
    }
}