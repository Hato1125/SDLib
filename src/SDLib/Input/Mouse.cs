using SDL2;

namespace SDLib.Input;

public static class Mouse
{
    private static readonly sbyte[] _value = new sbyte[5];
    private static readonly bool[] _keyState = new bool[5];

    /// <summary>
    /// マウスのX座標
    /// </summary>
    public static int X { get; private set; }

    /// <summary>
    /// マウスのY座標
    /// </summary>
    public static int Y { get; private set; }

    /// <summary>
    /// 水平方向のスクロール量
    /// </summary>
    public static float WheelX { get; private set; }

    /// <summary>
    /// 垂直方向のスクロール量
    /// </summary>
    public static float WheelY { get; private set; }

    /// <summary>
    /// 右にスクロールしているか
    /// </summary>
    public static bool IsRight { get; private set; }

    /// <summary>
    /// 上にスクロールしているか
    /// </summary>
    public static bool IsUp { get; private set; }

    /// <summary>
    /// 更新する
    /// </summary>
    /// <param name="e">イベント</param>
    public static void Update(SDL.SDL_Event e)
    {
        SDL.SDL_GetMouseState(out int x, out int y);
        X = x;
        Y = y;

        if (e.type == SDL.SDL_EventType.SDL_MOUSEWHEEL)
        {
            WheelX = e.wheel.x;
            WheelY = e.wheel.y;

            if (WheelY > 0)
                IsUp = true;
            else
                IsUp = false;

            if (WheelY > 0)
                IsRight = true;
            else
                IsRight = false;
        }

        for (uint i = 0; i < _value.Length; i++)
        {
            // 現在押されいてるキーはtrueにし、離されたらfalseにする
            // これにより、押している間を取得することができる
            if (e.type == SDL.SDL_EventType.SDL_MOUSEBUTTONDOWN)
            {
                if (e.button.button == (uint)GetKeyCode(i))
                {
                    if (!_keyState[i])
                        _keyState[i] = true;
                }
            }
            else if (e.type == SDL.SDL_EventType.SDL_MOUSEBUTTONUP)
            {
                if (e.button.button == (uint)GetKeyCode(i))
                {
                    if (_keyState[i])
                        _keyState[i] = false;
                }
            }

            if (_keyState[i])
                _value[i] = (sbyte)(IsPushing(GetKeyCode(i)) ? 2 : 1);
            else
                _value[i] = (sbyte)(IsPushing(GetKeyCode(i)) ? -1 : 0);
        }
    }

    /// <summary>
    /// ボタンを押している間を取得する
    /// </summary>
    /// <param name="keyCode">キーコード</param>
    public static bool IsPushing(MouseKey keyCode)
        => _value[GetKeyCodeIndex(keyCode)] > 0;

    /// <summary>
    /// ボタンを押した瞬間を取得する
    /// </summary>
    /// <param name="keyCode">キーコード</param>
    public static bool IsPushed(MouseKey keyCode)
         => _value[GetKeyCodeIndex(keyCode)] == 1;

    /// <summary>
    /// ボタンを離した瞬間を取得する
    /// </summary>
    /// <param name="keyCode">キーコード</param>
    public static bool IsSeparate(MouseKey keyCode)
        => _value[GetKeyCodeIndex(keyCode)] == -1;

    private static MouseKey GetKeyCode(uint index) => index switch
    {
        0 => MouseKey.Left,
        1 => MouseKey.Right,
        2 => MouseKey.Middle,
        3 => MouseKey.Input1,
        4 => MouseKey.Input2,
        _ => MouseKey.Left
    };

    private static int GetKeyCodeIndex(MouseKey key) => key switch
    {
        MouseKey.Left => 0,
        MouseKey.Right => 1,
        MouseKey.Middle => 2,
        MouseKey.Input1 => 3,
        MouseKey.Input2 => 4,
        _ => 0
    };
}

/// <summary>
/// マウスのキー
/// </summary>
public enum MouseKey : uint
{
    Left = SDL.SDL_BUTTON_LEFT,
    Right = SDL.SDL_BUTTON_RIGHT,
    Middle = SDL.SDL_BUTTON_MIDDLE,
    Input1 = SDL.SDL_BUTTON_X1,
    Input2 = SDL.SDL_BUTTON_X2,
}