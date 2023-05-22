using SDL2;

namespace SDLib.Gui;

public class UIScroller
{
    private int _scrollValue;
    /// <summary>
    /// 現在のスクロールの値
    /// </summary>
    public int ScrollValue
    {
        get => _scrollValue;
        set
        {
            if (value > ScrollMax)
                _scrollValue = ScrollMax;
            else if (value < ScrollMin)
                _scrollValue = ScrollMin;
            else
                _scrollValue = value;
        }
    }

    /// <summary>
    /// 一階のスクロールで何ピクセル移動するか
    /// </summary>
    public int ScrollLine { get; set; }

    /// <summary>
    /// 最小値
    /// </summary>
    public int ScrollMin { get; set; }

    /// <summary>
    /// 最大値
    /// </summary>
    public int ScrollMax { get; set; }

    /// <summary>
    /// UIScrollerを初期化する
    /// </summary>
    /// <param name="scrollBegin">初期値</param>
    /// <param name="min">最小値</param>
    /// <param name="max">最大値</param>
    public UIScroller(int scrollBegin, int min, int max)
    {
        ScrollMin = min;
        ScrollMax = max;
        ScrollValue = scrollBegin;
    }

    /// <summary>
    /// イベントを処理する
    /// </summary>
    /// <param name="e">SDLイベント</param>
    public void UpdateEvent(in SDL.SDL_Event e)
    {
        if (e.type != SDL.SDL_EventType.SDL_MOUSEWHEEL)
            return;

        if (e.wheel.y > 0)
            ScrollValue += ScrollLine;
        else if(e.wheel.y < 0)
            ScrollValue -= ScrollLine;
    }
}