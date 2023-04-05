namespace SDLib;

public class AppInfo : IReadOnlyAppInfo
{
    /// <summary>
    /// 起動してからの時間
    /// </summary>
    public TimeSpan TotalTime { get; set; }

    /// <summary>
    /// 一フレームにかかった時間
    /// </summary>
    public TimeSpan DeltaTime { get; set; }

    /// <summary>
    /// ウィンドウのポインタ
    /// </summary>
    public IntPtr WindowPtr { get; set; }

    /// <summary>
    /// レンダラーのポインタ
    /// </summary>
    public IntPtr RenderPtr { get; set; }
}