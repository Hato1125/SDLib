namespace SDLib.Gui;

public static class UIPosition
{
    /// <summary>
    /// 位置を計算する
    /// </summary>
    /// <param name="parentSize">親要素のサイズ</param>
    /// <param name="tergetSize">自分のサイズ</param>
    /// <param name="position">位置</param>
    public static float CalculatePosition(float parentSize, float tergetSize, UIHorizontal position) => position switch
    {
        UIHorizontal.Left => 0.0f,
        UIHorizontal.Center => (parentSize - tergetSize) / 2.0f,
        UIHorizontal.Right => parentSize - tergetSize,
        _ => 0.0f
    };

    /// <summary>
    /// 位置を計算する
    /// </summary>
    /// <param name="parentSize">親要素のサイズ</param>
    /// <param name="tergetSize">自分のサイズ</param>
    /// <param name="position">位置</param>
    public static float CalculatePosition(float parentSize, float tergetSize, UIVertical position) => position switch
    {
        UIVertical.Top => 0.0f,
        UIVertical.Center => (parentSize - tergetSize) / 2.0f,
        UIVertical.Bottom => parentSize - tergetSize,
        _ => 0.0f
    };
}

/// <summary>
/// 水平方向の位置
/// </summary>
public enum UIHorizontal
{
    Left,
    Center,
    Right,
}

/// <summary>
/// 垂直方向の位置
/// </summary>
public enum UIVertical
{
    Top,
    Center,
    Bottom,
}