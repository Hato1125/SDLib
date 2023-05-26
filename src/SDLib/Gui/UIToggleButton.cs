using SDLib.Graphics;

namespace SDLib.Gui;

public class UIToggleButton : UIButton
{

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
    public UIToggleButton(
        nint rendererPtr,
        nint windowPtr,
        int width,
        int height,
        in FontFamily family)
        : base(rendererPtr, windowPtr, width, height, family)
    {
    }

    protected override void ImplOnOff(double deltaTime)
    {
        if (IsSeparate())
            IsOn = !IsOn;
    }
}