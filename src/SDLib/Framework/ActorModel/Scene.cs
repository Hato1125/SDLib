using SDLib;

namespace SDLib.Framework;

public class Scene
{
    /// <summary>
    /// 延期されたActorのリスト
    /// </summary>
    public readonly List<Actor> DelayActors = new();

    /// <summary>
    /// 死んだActorのリスト
    /// </summary>
    /// <returns></returns>
    public readonly List<Actor> DeadActors = new();

    /// <summary>
    /// Actorのリスト
    /// </summary>
    public readonly List<Actor> Actors = new();

    /// <summary>
    /// Sceneが初期化をしたか
    /// </summary>
    public bool IsInit { get; set; } = true;

    /// <summary>
    /// Sceneが更新中、またはレンダリング中か
    /// </summary>
    public bool IsUpdating { get; private set; }

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
        IsUpdating = true;
        foreach (var actor in Actors)
            actor.Update();
    }

    /// <summary>
    /// レンダリング処理
    /// </summary>
    /// <param name="info">アプリケーションの情報</param>
    public virtual void Render(IReadOnlyAppInfo info)
    {
        foreach (var actor in Actors)
            actor.Render();
        IsUpdating = false;

        // Actorの後処理
        AddDelayActors();
        AddDeadActor();
        RemoveDeadActorList();
    }

    /// <summary>
    /// 終了処理
    /// </summary>
    public virtual void Finish()
    {
        IsUpdating = false;

        foreach (var actor in Actors)
            actor.Dispose();

        Actors.Clear();
    }

    /// <summary>
    /// SceneにActorを追加する
    /// </summary>
    /// <param name="actor">Actor</param>
    public void AddActor(Actor actor)
    {
        if (IsUpdating)
            DelayActors.Add(actor);
        else
            Actors.Add(actor);
    }

    /// <summary>
    /// Sceneに登録されているActorを消去する
    /// </summary>
    /// <param name="actor">Actor</param>
    public void RemoveActor(Actor actor)
    {
        if (!Actors.Contains(actor))
            return;

        Actors.Remove(actor);
    }

    /// <summary>
    /// 追加を延期されたActorをActorsに追加する
    /// </summary>
    private void AddDelayActors()
    {
        if (DelayActors.Count <= 0)
            return;

        foreach (var actor in DelayActors)
        {
            if (actor.State != Actor.ActorState.Dead)
                AddActor(actor);
        }

        DelayActors.Clear();
    }

    /// <summary>
    /// 状態がDeadのActorを死んだActorリストに入れる
    /// </summary>
    private void AddDeadActor()
    {
        foreach (var actor in Actors)
        {
            if (actor.State == Actor.ActorState.Dead)
            {
                // そのままActor.Removeを行うことができないので
                // いちいちDeadActorsに入れてRemoveする
                DeadActors.Add(actor);
            }
        }
    }

    /// <summary>
    /// 死んだActorたちを消去する
    /// </summary>
    private void RemoveDeadActorList()
    {
        foreach (var actor in DeadActors)
            RemoveActor(actor);

        DeadActors.Clear();
    }
}
