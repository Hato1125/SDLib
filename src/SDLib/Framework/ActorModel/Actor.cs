namespace SDLib.Framework;

public class Actor : IDisposable
{
    /// <summary>
    /// Disposeを実行したか
    /// </summary>
    protected bool IsDispose { get; set; }

    /// <summary>
    /// Actorの持ち主のSceneクラス
    /// </summary>
    protected Scene Owner { get; set; }
    
    /// <summary>
    /// Actorの状態
    /// </summary>
    public ActorState State { get; set; }

    /// <summary>
    /// コンポーネントのリスト
    /// </summary>
    private readonly List<Component> ComponentList = new();

    /// <summary>
    /// Actorを初期化する
    /// </summary>
    /// <param name="owner">持ち主のSceneクラス</param>
    public Actor(Scene owner)
    {
        Owner = owner;
        Owner.AddActor(this);
    }

    /// <summary>
    /// 更新する
    /// </summary>
    public void Update()
        => ActorUpdate();

    /// <summary>
    /// レンダリングする
    /// </summary>
    public void Render()
        => ActorRender();

    /// <summary>
    /// 破棄する
    /// </summary>
    public void Dispose()
    {
        ActorDispose(true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Actorを更新する
    /// </summary>
    protected virtual void ActorUpdate()
    {
    }

    /// <summary>
    /// Actorをレンダリングする
    /// </summary>
    protected virtual void ActorRender()
    {
    }

    /// <summary>
    /// Actorを破棄する
    /// </summary>
    protected virtual void ActorDispose(bool isDispose)
    {
        if (IsDispose)
            return;

        IsDispose = true;
    }

    /// <summary>
    /// コンポーネントを追加する
    /// </summary>
    /// <param name="component">コンポーネント</param>
    public void AddComponent(Component component)
        => ComponentList.Add(component);

    /// <summary>
    /// コンポーネントを破棄する
    /// </summary>
    /// <param name="component">コンポーネント</param>
    public void RemoveComponent(Component component)
        => ComponentList.Remove(component);

    /// <summary>
    /// Actorの状態の列挙型
    /// </summary>
    public enum ActorState
    {
        Active,
        Dead,
    }
}