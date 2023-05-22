using SDL2;

namespace SDLib.Gui;

public class UITileAlignment : UIElement
{
    #region Private Member

    private bool _isScroll;

    #endregion

    #region Public Member

    /// <summary>
    /// Scroller
    /// </summary>
    public readonly UIScroller Scroller;

    /// <summary>
    /// 整列するか
    /// </summary>
    public bool IsAlignment { get; set; }

    private int _columnElementMaxNum;
    /// <summary>
    /// カラムに入るElementの最大個数
    /// </summary>
    public int ColumnElementMaxNum
    {
        get => _columnElementMaxNum;
        set
        {
            if(value <= 0)
                _columnElementMaxNum = 1;
            else
                _columnElementMaxNum = value;

            IsAlignment = true;
        }
    }

    private int _columnPadding;
    /// <summary>
    /// カラム方向のパディング
    /// </summary>
    public int ColumnPadding
    {
        get => _columnPadding;
        set
        {
            _columnPadding = value;
            IsAlignment = true;
        }
    }

    private int _rowPadding;
    /// <summary>
    /// 縦方向のパディング
    /// </summary>
    public int RowPadding
    {
        get => _rowPadding;
        set
        {
            _rowPadding = value;
            IsAlignment = true;
        }
    }

    #endregion

    /// <summary>
    /// UITileAlignmentを初期化する
    /// </summary>
    /// <param name="rendererPtr">Rendererのポインタ</param>
    /// <param name="windowPtr">Windowのポインタ</param>
    /// <param name="width">横幅</param>
    /// <param name="height">高さ</param>
    /// <param name="columnMaxElementNum">カラムに入るElementの最大個数</param>
    public UITileAlignment(nint rendererPtr, nint windowPtr, int width, int height, int columnMaxElementNum)
        : base(rendererPtr, windowPtr, width, height)
    {
        Scroller = new(0, 0, 0) { ScrollLine = 25 };
        ColumnElementMaxNum = columnMaxElementNum;
        ColumnPadding = 5;
        RowPadding = 5;
        OnUIUpdating += AlignmentTile;
    }

    protected override void UpdatingUIEvent(in SDL.SDL_Event e)
    {
        if (_isScroll)
        {
            Scroller.UpdateEvent(e);
            IsAlignment = true;
        }

        base.UpdatingUIEvent(e);
    }

    void AlignmentTile(double deltaTime)
    {
        if (!IsAlignment || !ChildrenList.Any())
            return;

        int columnMaxWidth = default;
        int columnWidth = default;
        int rowMaxHeight = default;
        int maxHeight = default;
        int width = _columnElementMaxNum;
        int height = ChildrenList.Count / _columnElementMaxNum + ChildrenList.Count % _columnElementMaxNum;

        for(int i = 0; i < height; i++)
        {
            for (int j = 0; j < width; j++)
            {
                var count = j + (width * i);
                var index = count != 0 ? j + (width * i) : 0;
                var rowHeight = (rowMaxHeight + RowPadding) * i;

                // indexが要素の個数を超えていない場合は処理する
                if (index <= ChildrenList.Count - 1)
                {
                    var element = ChildrenList[index];
                    element.X = columnWidth - columnMaxWidth;
                    element.Y = rowHeight + Scroller.ScrollValue;

                    columnWidth += element.Width + ColumnPadding;

                    if (rowMaxHeight < element.Height)
                        rowMaxHeight = element.Height;
                }
            }
            maxHeight += rowMaxHeight + RowPadding;
            columnMaxWidth = columnWidth;
        }

        // 次のフレームでスクロールする必要があるかを判定する
        maxHeight -= RowPadding;
        if(maxHeight > Height)
        {
            _isScroll = true;

            var diff = maxHeight - Height;
            Scroller.ScrollMin = -diff;
            Scroller.ScrollMax = 0;
        }
        else
        {
            _isScroll = false;
        }

        IsAlignment = false;
    }
}