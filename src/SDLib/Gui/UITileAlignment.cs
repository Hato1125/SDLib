namespace SDLib.Gui;

public class UITileAlignment : UIElement
{
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
        ColumnElementMaxNum = columnMaxElementNum;
        ColumnPadding = 5;
        RowPadding = 5;
        OnUIUpdating += AlignmentTile;
    }

    void AlignmentTile(double deltaTime)
    {
        if (!IsAlignment || !ChildrenList.Any())
            return;

        int columnMaxWidth = default;
        int columnWidth = default;
        int rowMaxHeight = default;
        int width = _columnElementMaxNum;
        int height = ChildrenList.Count / _columnElementMaxNum + ChildrenList.Count % _columnElementMaxNum;

        for(int i = 0; i < height; i++)
        {
            for (int j = 0; j < width; j++)
            {
                var count = j + (width * i);
                var index = count != 0 ? j + (width * i) : 0;
                var rowHeight = (rowMaxHeight + RowPadding) * i;

                // indexが要素の個数を超えていない場合はセットする
                if (index <= ChildrenList.Count - 1)
                {
                    var element = ChildrenList[index];
                    element.X = columnWidth - columnMaxWidth;
                    element.Y = rowHeight;

                    columnWidth += element.Width + ColumnPadding;

                    if (rowMaxHeight < element.Height)
                        rowMaxHeight = element.Height;
                }
            }
            columnMaxWidth = columnWidth;
        }

        IsAlignment = false;
    }
}