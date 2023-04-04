using SDL2;

namespace SDLib.Input;

public static class Mouse
{
    private static readonly sbyte[] _value = new sbyte[5];

    /// <summary>
    /// マウスのX方向の絶対座標
    /// </summary>
    public static int X { get; private set; }

    /// <summary>
    /// マウスのY方向の絶対座標
    /// </summary>
    public static int Y { get; private set; }

    /// <summary>
    /// 更新する
    /// </summary>
    public static void Update()
    {
        uint state = SDL.SDL_GetGlobalMouseState(out int x, out int y);
        X = x;
        Y = y;

        for (uint i = 0; i < _value.Length; i++)
        {
            if ((state & i) != 0)
                _value[i] = (sbyte)(IsPushing(i) ? 2 : 1);
            else
                _value[i] = (sbyte)(IsPushing(i) ? -1 : 0);
        }
    }

    /// <summary>
    /// 押されている間を取得する
    /// </summary>
    /// <param name="keyCode">キーコード</param>
    public static bool IsPushing(uint keyCode)
        => _value[keyCode] > 0;

    /// <summary>
    /// 押された瞬間を取得する
    /// </summary>
    /// <param name="keyCode">キーコード</param>
    public static bool IsPushed(uint keyCode)
        => _value[keyCode] == 1;

    /// <summary>
    /// 離された瞬間を取得する
    /// </summary>
    /// <param name="keyCode">キーコード</param>
    public static bool IsSeparate(uint keyCode)
        => _value[keyCode] == -1;
}