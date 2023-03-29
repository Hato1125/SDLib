namespace SDLib;

public struct AppInfo
{
    /// <summary>
    /// レンダラーのポインタ
    /// </summary>
    public IntPtr RendererPtr { get; set; }

    /// <summary>
    /// ウィンドウのポインタ
    /// </summary>
    public IntPtr WindowPtr { get; set; }
}