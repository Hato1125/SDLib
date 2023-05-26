using System.Drawing;

namespace SDLib.Gui;

public class UIButtonBase : UIElement
{
    #region Private Member

    private double _fadeCounter;

    #endregion

    #region Protected Member

    /// <summary>
    /// 現在の色
    /// </summary>
    protected Color NowColor { get; private set; }

    /// <summary>
    /// 色を更新中か
    /// </summary>
    protected bool IsUpdateColor { get; private set; }

    /// <summary>
    /// 色が変更されたか
    /// </summary>
    protected bool IsChangeColor { get; private set; } 

    #endregion

    #region Public Member

    private Color _backColor;
    /// <summary>
    /// 背景色
    /// </summary>
    public Color BackColor
    {
        get => _backColor;
        set
        {
            _backColor = value;
            IsChangeColor = true;
        }
    }

    private Color _clickColor;
    /// <summary>
    /// ボタンがOnの時の色
    /// </summary>
    public Color ClickColor
    {
        get => _clickColor;
        set
        {
            _clickColor = value;
            IsChangeColor = true;
        }
    }

    private double _fadeMs;
    /// <summary>
    /// フェードする時間
    /// </summary>
    public double FadeMs
    {
        get => _fadeMs;
        set
        {
            if (value <= 0)
                _fadeMs = 0.1;
            else
                _fadeMs = value;
        }
    }

    private bool _isOn;
    /// <summary>
    /// Onか
    /// </summary>
    public bool IsOn
    {
        get => _isOn;
        set
        {
            _isOn = value;
            IsUpdateColor = true;
        }
    }

    #endregion

    #region Events

    /// <summary>
    /// 色の更新時に呼ばれる
    /// </summary>
    protected event Action? OnColorUpdating = delegate { };

    #endregion

    /// <summary>
    /// UIButtonBaseを初期化する
    /// </summary>
    /// <param name="rendererPtr">Rendererのポインタ</param>
    /// <param name="windowPtr">Windowのポインタ</param>
    /// <param name="width">横幅</param>
    /// <param name="height">高さ</param>
    public UIButtonBase(nint rendererPtr, nint windowPtr, int width, int height)
        : base(rendererPtr, windowPtr, width, height)
    {
        FadeMs = 0.125;
        BackColor = Color.White;
        ClickColor = Color.LightGray;

        OnUIUpdating += ImplOnOff;
        OnUIUpdating += UpdateColor;
    }

    void UpdateColor(double deltaTime)
    {
        var speedMs = 90 / FadeMs;

        if(IsOn)
        {
            _fadeCounter += deltaTime * speedMs;

            if (_fadeCounter > 90)
                _fadeCounter = 90;
        }
        else
        {
            _fadeCounter -= deltaTime * speedMs;

            if (_fadeCounter < 0)
                _fadeCounter = 0;
        }

        if(IsUpdateColor || IsChangeColor)
        {
            if (double.IsNaN(_fadeCounter))
                return;

            var sin = Math.Sin(_fadeCounter * Math.PI / 180);
            var scaleR = (int)(BackColor.R + sin * (ClickColor.R - BackColor.R));
            var scaleG = (int)(BackColor.G + sin * (ClickColor.G - BackColor.G));
            var scaleB = (int)(BackColor.B + sin * (ClickColor.B - BackColor.B));

            NowColor = Color.FromArgb(scaleR, scaleG, scaleB);
            OnColorUpdating?.Invoke();

            if (_fadeCounter == 90 || _fadeCounter == 0)
                IsUpdateColor = false;

            IsChangeColor = false;
        }
    }

    protected virtual void ImplOnOff(double deltaTime)
    {
    }
}