namespace SDLib;

public interface IReadOnlyAppInfo
{
    TimeSpan TotalTime { get; }
    TimeSpan DeltaTime { get; }
    IntPtr WindowPtr { get; }
    IntPtr RenderPtr { get; }
}