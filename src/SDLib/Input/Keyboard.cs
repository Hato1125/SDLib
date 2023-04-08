using System.Runtime.InteropServices;
using SDL2;

namespace SDLib.Input;

public static class Keyboard
{
    private static IntPtr _keyState = IntPtr.Zero;
    private static readonly byte[] _keyStates = new byte[512];
    private static readonly sbyte[] _value = new sbyte[512];

    /// <summary>
    /// 更新する
    /// </summary>
    public static void Update()
    {
        _keyState = SDL.SDL_GetKeyboardState(out int length);

        // 配列にコピーする
        Marshal.Copy(_keyState, _keyStates, 0, length);

        for (int i = 0; i < _keyStates.Length; i++)
        {
            if (_keyStates[i] == 1)
                _value[i] = (sbyte)(IsPushing((SDL.SDL_Scancode)i) ? 2 : 1);
            else
                _value[i] = (sbyte)(IsPushing((SDL.SDL_Scancode)i) ? -1 : 0);
        }
    }

    /// <summary>
    /// ボタンを押している間を取得する
    /// </summary>
    /// <param name="keyCode">キーコード</param>
    public static bool IsPushing(SDL.SDL_Scancode keyCode)
        => _value[(int)keyCode] > 0;

    /// <summary>
    /// ボタンを押した瞬間を取得する
    /// </summary>
    /// <param name="keyCode">キーコード</param>
    public static bool IsPushed(SDL.SDL_Scancode keyCode)
        => _value[(int)keyCode] == 1;

    /// <summary>
    /// ボタンを離した瞬間を取得する
    /// </summary>
    /// <param name="keyCode">キーコード</param>
    public static bool IsSeparate(SDL.SDL_Scancode keyCode)
        => _value[(int)keyCode] == -1;
}