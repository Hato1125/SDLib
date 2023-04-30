namespace SDLib.NewFramework;

public class Actor
{
    private readonly HashSet<Component> _layzComponentList = new();
    private readonly HashSet<Component> _removeLayzComponentList = new();

    public readonly List<Component> ComponentList = new();

    /// <summary>
    /// Actorの持ち主のSceneクラス
    /// </summary>
    protected readonly Scene Owner;

    /// <summary>
    /// 実行される順番
    /// </summary>
    public int Order { get; set; }

    /// <summary>
    /// Actorを初期化する
    /// </summary>
    /// <param name="owner">Sceneクラス</param>
    /// <param name="order">順番</param>
    public Actor(Scene owner, int order = int.MaxValue)
    {
        Owner = owner;
        Order = order;

        Owner.AddActor(this);
    }

    /// <summary>
    /// Actorの更新をする
    /// </summary>
    public void Update()
        => UpdateActor();

    /// <summary>
    /// Actorのレンダリングを行う
    /// </summary>
    public void Render()
        => RenderActor();

    /// <summary>
    /// Actorの終了処理
    /// </summary>
    public void Finish()
        => FinishActor();

    /// <summary>
    /// Componentの後始末を行う
    /// </summary>
    public void ComponentCleaning()
    {
        LayzComponentAddPart();
        LayzRemoveActorPart();
    }

    /// <summary>
    /// Componentを追加する
    /// </summary>
    public void AddComponent(Component component)
    {
        if (Owner.IsUpdating)
        {
            _layzComponentList.Add(component);
        }
        else
        {
            ComponentList.Add(component);
            ComponentSort();
        }
    }
    /// <summary>
    /// Componentを破棄する
    /// </summary>
    public void RemoveComponent(Component component)
    {
        if(!ComponentList.Contains(component))
            return;

        if (Owner.IsUpdating)
            _removeLayzComponentList.Add(component);
        else
            ComponentList.Remove(component);
    }

    /// <summary>
    /// 遅延されたComponentの追加リストをComponentListに追加する
    /// </summary>
    private void LayzComponentAddPart()
    {
        if (Owner.IsUpdating || !_layzComponentList.Any())
            return;

        Tracer.PrintInfo("Adding LayzComponents.");
        foreach (var component in _layzComponentList)
            ComponentList.Add(component);

        _layzComponentList.Clear();
        ComponentSort();
    }

    /// <summary>
    /// 遅延されたComponentの削除リストをComponentListから削除する
    /// </summary>
    private void LayzRemoveActorPart()
    {
        if (Owner.IsUpdating || !_removeLayzComponentList.Any())
            return;

        Tracer.PrintInfo("Removing LayzComponents.");
        foreach (var actor in _removeLayzComponentList)
            ComponentList.Remove(actor);

        _removeLayzComponentList.Clear();
    }

    /// <summary>
    /// ComponentListをソートする
    /// </summary>
    private void ComponentSort()
    {
        ComponentList.Sort((x, y) => x.Order.CompareTo(y.Order));
    }


    /// <summary>
    /// 更新処理
    /// </summary>
    protected virtual void UpdateActor()
    {
    }

    /// <summary>
    /// レンダリング処理
    /// </summary>
    protected virtual void RenderActor()
    {
    }

    /// <summary>
    /// 終了処理
    /// </summary>
    protected virtual void FinishActor()
    {
    }
}