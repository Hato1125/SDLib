using SDL2;
using SDLib.Input;
using SDLib.Graphics;

namespace SDLib.Gui;

public class UIElement : IDisposable
{
    #region Private Member

    private bool _isDisposing = false;
    private bool _isChildHovering = false;
    private bool _isWindowActive = true;

    #endregion

    #region Protected Member

    /// <summary>
    /// Rendererのポインタ
    /// </summary>
    protected readonly nint RendererPtr;

    /// <summary>
    /// Windowのポインタ
    /// </summary>
    protected readonly nint WindowPtr;

    /// <summary>
    /// UIの描画可能領域
    /// </summary>
    protected TextureArea? TextureArea;

    #endregion

    #region Public Member

    /// <summary>
    /// UIが入力を受け取るか
    /// </summary>
    public bool IsInput { get; set; } = true;

    /// <summary>
    /// UIの生成を行うか
    /// </summary>
    public bool IsBuild { get; set; } = true;

    /// <summary>
    /// UIのX座標
    /// </summary>
    public int X { get; set; }

    /// <summary>
    /// UIのY座標
    /// </summary>
    public int Y { get; set; }

    /// <summary>
    /// UIのWindow内での絶対X座標
    /// </summary>
    public int AbsoluteX { get => Super != null ? Super.X + X : X; }

    /// <summary>
    /// UIのWindow内での絶対Y座標
    /// </summary>
    public int AbsoluteY { get => Super != null ? Super.Y + Y : Y; }

    private int _width;
    /// <summary>
    /// UIの横幅
    /// </summary>
    public int Width
    {
        get => _width;
        set
        {
            if (value <= 0)
                throw new ArgumentException("A number less than 0 is set.");
            else
                _width = value;

            IsBuild = true;
        }
    }

    private int _height;
    /// <summary>
    /// UIの高さ
    /// </summary>
    public int Height
    {
        get => _height;
        set
        {
            if (value <= 0)
                throw new ArgumentException("A number less than 0 is set.");
            else
                _height = value;

            IsBuild = true;
        }
    }

    /// <summary>
    /// UIの親クラスのインスタンス
    /// </summary>
    public UIElement? Super { get; private set; }

    /// <summary>
    /// アクセスキーのキーコードのリスト
    /// </summary>
    public readonly HashSet<SDL.SDL_Scancode> KeyCodeList = new(255);

    /// <summary>
    /// UIの子要素のリスト
    /// </summary>
    public readonly List<UIElement> ChildrenList = new();

    #endregion

    #region Events

    /// <summary>
    /// UIの更新時に呼ばれる
    /// </summary>
    protected event Action<double>? OnUIUpdating = delegate { };

    /// <summary>
    /// UIのレンダリング時に呼ばれる
    /// </summary>
    protected event Action? OnUIRendering = delegate { };

    /// <summary>
    /// UIのレンダリング時に呼ばれる
    /// </summary>
    public event Action? OnPaint = delegate { };

    /// <summary>
    /// UIにホバーしたら呼ばれる
    /// </summary>
    public event Action? OnHovering = delegate { };

    /// <summary>
    /// UIが押されいてる間呼ばれる
    /// </summary>
    public event Action? OnPushing = delegate { };

    /// <summary>
    /// UIが押された瞬間のみ呼ばれる
    /// </summary>
    public event Action? OnPushed = delegate { };

    /// <summary>
    /// UIが離された瞬間のみ呼ばれる
    /// </summary>
    public event Action? OnSeparate = delegate { };

    #endregion

    /// <summary>
    /// UIElementを初期化する
    /// </summary>
    /// <param name="rendererPtr">Rendererのポインタ</param>
    /// <param name="windowPtr">Windowのポインタ</param>
    public UIElement(in nint rendererPtr, in nint windowPtr)
    {
        if (rendererPtr == nint.Zero)
            throw new ArgumentNullException(nameof(rendererPtr), "A null pointer was passed.");

        if (windowPtr == nint.Zero)
            throw new ArgumentNullException(nameof(windowPtr), "A null pointer was passed.");

        RendererPtr = rendererPtr;
        WindowPtr = windowPtr;
    }

    /// <summary>
    /// UIElementを初期化する
    /// </summary>
    /// <param name="rendererPtr">Rendererのポインタ</param>
    /// <param name="windowPtr">Windowのポインタ</param>
    /// <param name="width">横幅</param>
    /// <param name="height">高さ</param>
    public UIElement(in nint rendererPtr, in nint windowPtr, int width, int height)
        : this(rendererPtr, windowPtr)
    {
        Width = width;
        Height = height;
    }

    ~UIElement() => Disposing(false);

    /// <summary>
    /// UIの更新をする
    /// </summary>
    public void Update(double deltaTime)
    {
        if(IsBuild)
        {
            UIBuild();
            IsBuild = false;
        }

        foreach(var child in ChildrenList)
        {
            child.Update(deltaTime);
            child.Super = this;
            _isChildHovering = child.IsHovering();
        }

        CallInputEvent();
        OnUIUpdating?.Invoke(deltaTime);
    }

    /// <summary>
    /// UIのイベントを処理する
    /// </summary>
    /// <param name="e">SDLイベント</param>
    public void UpdateEvent(in SDL.SDL_Event e)
    {
        UpdatingUIEvent(e);
    }

    /// <summary>
    /// UIのレンダリングを行う
    /// </summary>
    /// <param name="returnRenderTarget">レンダリング終了後のレンダーターゲット</param>
    public void Render(nint? returnRenderTarget = null)
    {
        if (TextureArea == null)
            return;

        var render = () =>
        {
            OnUIRendering?.Invoke();
            OnPaint?.Invoke();

            foreach(var child in ChildrenList)
            {
                // 画面外にいるのにレンダリングするのは無駄なので
                // 画面外にいるときはレンダリングしない
                if (child.X >= -child.Width
                    && child.Y >= -child.Height
                    && child.X <= Width + child.Width
                    && child.Y <= Height + child.Height)
                    child.Render(TextureArea.GetTexture().GetTexturePtr());
            }
        };

        TextureArea.Render(render, returnRenderTarget ?? nint.Zero).Render(X, Y);
    }

    /// <summary>
    /// UIにホバーしているかを取得する
    /// </summary>
    public bool IsHovering()
    {
        if (!IsInput || !_isWindowActive || _isChildHovering)
            return false;

        SDL.SDL_GetWindowPosition(WindowPtr, out int x, out int y);
        (int mouseX, int mouseY) = (Mouse.X - x, Mouse.Y - y);

        if (mouseX >= AbsoluteX
            && mouseY >= AbsoluteY
            && mouseX <= AbsoluteX + Width
            && mouseY <= AbsoluteY + Height)
            return true;

        return false;
    }

    /// <summary>
    /// UIが押されているかを取得する
    /// </summary>
    public bool IsPushing()
    {
        if (!IsHovering())
            return false;

        if (Mouse.IsPushing(SDL.SDL_BUTTON_LEFT))
            return true;

        foreach(var keyCode in KeyCodeList)
            return Keyboard.IsPushing(keyCode);

        return false;
    }

    /// <summary>
    /// UIが押された瞬間を取得する
    /// </summary>
    public bool IsPushed()
    {
        if (!IsHovering())
            return false;

        if (Mouse.IsPushed(SDL.SDL_BUTTON_LEFT))
            return true;

        foreach (var keyCode in KeyCodeList)
            return Keyboard.IsPushed(keyCode);

        return false;
    }

    /// <summary>
    /// UIが離された瞬間を取得する
    /// </summary>
    public bool IsSeparate()
    {
        if (!IsHovering())
            return false;

        if (Mouse.IsSeparate(SDL.SDL_BUTTON_LEFT))
            return true;

        foreach (var keyCode in KeyCodeList)
            return Keyboard.IsSeparate(keyCode);

        return false;
    }

    /// <summary>
    /// UIの破棄を行う
    /// </summary>
    public void Dispose()
    {
        Disposing(true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// UIの破棄処理を実装する
    /// </summary>
    /// <param name="isDisposing">マネージメントリソースを破棄するか</param>
    protected virtual void Disposing(bool isDisposing)
    {
        if (_isDisposing)
            return;

        TextureArea?.Dispose();

        foreach (var child in ChildrenList)
            child.Dispose();

        _isDisposing = true;
    }

    /// <summary>
    /// イベントを処理する
    /// </summary>
    /// <param name="e">イベント</param>
    protected virtual void UpdatingUIEvent(in SDL.SDL_Event e)
    {
        switch (e.type)
        {
            case SDL.SDL_EventType.SDL_WINDOWEVENT:
                _isWindowActive = e.window.windowEvent switch
                {
                    SDL.SDL_WindowEventID.SDL_WINDOWEVENT_FOCUS_GAINED => true,
                    SDL.SDL_WindowEventID.SDL_WINDOWEVENT_FOCUS_LOST => false,
                    _ => _isWindowActive
                };
                break;
        }
    }

    /// <summary>
    /// UIの生成を行う
    /// </summary>
    protected virtual void UIBuild()
    {
        TextureArea?.Dispose();
        TextureArea = new(RendererPtr, Width, Height);
    }

    private void CallInputEvent()
    {
        if (IsHovering()) OnHovering?.Invoke();
        if (IsPushing()) OnPushing?.Invoke();
        if (IsPushed()) OnPushed?.Invoke();
        if (IsSeparate()) OnSeparate?.Invoke();
    }
}