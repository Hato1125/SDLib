namespace SDLib.NewFramework;

public class Scene
{
    private readonly HashSet<Actor> _layzActorList = new();
    private readonly HashSet<Actor> _removeLayzActorList = new();

    /// <summary>
    /// 現在登録されているActorリスト
    /// </summary>
    /// <returns></returns>
    public readonly List<Actor> ActorList = new();

    /// <summary>
    /// このシーンが更新中かどうか
    /// </summary>
    public bool IsUpdating { get; set; }

    /// <summary>
    /// 初期化処理
    /// </summary>
    /// <param name="info">アプリケーションの情報</param>
    public virtual void Init(IReadOnlyAppInfo info)
    {
    }

    /// <summary>
    /// 更新処理
    /// </summary>
    /// <param name="info">アプリケーションの情報</param>
    public virtual void Update(IReadOnlyAppInfo info)
    {
        foreach (var actor in ActorList)
            actor.Update();
    }

    /// <summary>
    /// レンダリング処理
    /// </summary>
    /// <param name="info">アプリケーションの情報</param>
    public virtual void Render(IReadOnlyAppInfo info)
    {
        foreach (var actor in ActorList)
            actor.Render();
    }

    /// <summary>
    /// Actorの後始末を行う
    /// </summary>
    public virtual void ActorCleaning()
    {
        LazyActorAddPart();
        LayzRemoveActorPart();

        foreach(var actor in ActorList)
            actor.ComponentCleaning();
    }

    /// <summary>
    /// 終了処理
    /// </summary>
    /// <param name="info">アプリケーションの情報</param>
    public virtual void Finish(IReadOnlyAppInfo info)
    {
        foreach (var actor in ActorList)
            actor.Finish();
    }

    /// <summary>
    /// Actorを追加する
    /// </summary>
    /// <param name="actor">Actor</param>
    public void AddActor(Actor actor)
    {
        if (IsUpdating)
        {
            _layzActorList.Add(actor);
        }
        else
        {
            ActorList.Add(actor);
            ActorSort();
        }
    }

    /// <summary>
    /// Actorを削除する
    /// </summary>
    /// <param name="actor">Actor</param>
    public void RemoveActor(Actor actor)
    {
        if(!ActorList.Contains(actor))
            return;

        if(IsUpdating)
            _removeLayzActorList.Add(actor);
        else
            ActorList.Remove(actor);
    }

    /// <summary>
    /// 遅延されたActorの追加をActorListに追加する
    /// </summary>
    private void LazyActorAddPart()
    {
        if (IsUpdating || !_layzActorList.Any())
            return;

        Tracer.PrintInfo("Adding LayzActors.");
        foreach (var actor in _layzActorList)
            ActorList.Add(actor);

        _layzActorList.Clear();
        ActorSort();
    }

    /// <summary>
    /// 遅延されたActorの削除リストをActorListから削除する
    /// </summary>
    private void LayzRemoveActorPart()
    {
        if (IsUpdating || !_removeLayzActorList.Any())
            return;

        Tracer.PrintInfo("Removing LayzActors.");
        foreach(var actor in _removeLayzActorList)
            ActorList.Remove(actor);

        _removeLayzActorList.Clear();
    }

    /// <summary>
    /// ActorListをソートする
    /// </summary>
    private void ActorSort()
    {
        ActorList.Sort((x, y) => x.Order.CompareTo(y.Order));
    }
}