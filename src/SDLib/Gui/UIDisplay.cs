using SDL2;

namespace SDLib.Gui;

public class UIDisplay
{
    private readonly List<UIElement> _elementList = new();

    /// <summary>
    /// UI要素を追加する
    /// </summary>
    /// <param name="element">UI要素</param>
    public void AddElement(UIElement element)
        => _elementList.Add(element);

    /// <summary>
    /// UI要素を破棄する
    /// </summary>
    /// <param name="element">UI要素</param>
    public void RemoveElement(UIElement element)
        => _elementList.Remove(element);

    /// <summary>
    /// UIを更新する
    /// </summary>
    /// <param name="deltaTime">デルタタイム</param>
    public void Update(double deltaTime)
    {
        bool isHovering = false;
        for(int i = _elementList.Count - 1; i >= 0; i--)
        {
            var element = _elementList[i];

            if(!isHovering && element.IsHovering())
            {
                isHovering = true;
                element.IsDisplayInput = true;
            }
            else if(element.IsChildrenHovering)
            {
                isHovering = true;
                element.IsDisplayInput = false;
            }
            else
            {
                if (isHovering)
                    element.IsDisplayInput = false;
                else
                    element.IsDisplayInput = true;
            }

            element.Update(deltaTime);
        }
    }

    /// <summary>
    /// UIのイベントを更新する
    /// </summary>
    /// <param name="e">SDLイベント</param>
    public void EventUpdate(in SDL.SDL_Event e)
    {
        foreach (var element in _elementList)
            element.UpdateEvent(e);
    }

    /// <summary>
    /// UIをレンダリングする
    /// </summary>
    public void Render()
    {
        foreach (var element in _elementList)
            element.Render();
    }

    /// <summary>
    /// UIを破棄する
    /// </summary>
    public void Dispose()
    {
        foreach (var element in _elementList)
            element.Dispose();
    }
}