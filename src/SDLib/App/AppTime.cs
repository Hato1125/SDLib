namespace SDLib;

public struct AppTime
{
    /// <summary>
    /// 起動してからの時間
    /// </summary>
    public TimeSpan TotalTime { get; set; }

    /// <summary>
    /// 位置フレームの経過時間
    /// </summary>
    public TimeSpan DeltaTime { get; set; }
}