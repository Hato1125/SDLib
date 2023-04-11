namespace SDLib.Framework;

public class Component : IDisposable
{
    /// <summary>
    /// Disposeを実行したか
    /// </summary>
    protected bool IsDispose { get; set; }

    /// <summary>
    /// 持ち主のActorクラス
    /// </summary>
    public readonly Actor Owner;

    /// <summary>
    /// Componentを初期化する
    /// </summary>
    /// <param name="owner">持ち主のActorクラス</param>
    public Component(Actor owner)
    {
        Owner = owner;
        Owner.AddComponent(this);
    }

    /// <summary>
    /// 更新する
    /// </summary>
    public void Update()
        => ComponentUpdate();

    /// <summary>
    /// レンダリングする
    /// </summary>
    public void Render()
        => ComponentRender();

    /// <summary>
    /// 破棄する
    /// </summary>
    public void Dispose()
    {
        ComponentDispose(true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// コンポーネントを更新する
    /// </summary>
    protected virtual void ComponentUpdate()
    {
    }

    /// <summary>
    /// コンポーネントを描画する
    /// </summary>
    protected virtual void ComponentRender()
    {
    }

    /// <summary>
    /// コンポーネントを破棄する
    /// </summary>
    protected virtual void ComponentDispose(bool isDisposeing)
    {
        if(IsDispose)
            return;

        IsDispose = true;
    }
}