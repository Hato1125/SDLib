using System.Drawing;

namespace SDLib;

public interface IReadOnlyAppWindow
{
    IntPtr WindowPtr { get; }
    IntPtr RenderPtr { get; }
    Point WindowPosition { get; set; }
    Size WindowSize { get; set; }
    Size WindowMinSize { get; set; }
    Size WindowMaxSize { get; set; }
    string WindowTitle { get; set; }
}