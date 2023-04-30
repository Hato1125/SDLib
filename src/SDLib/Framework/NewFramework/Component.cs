namespace SDLib.NewFramework;

public class Component
{
    /// <summary>
    /// Componentの持ち主のActorクラス
    /// </summary>
    protected readonly Actor Owner;

    /// <summary>
    /// Componentの実行される順番
    /// </summary>
    public int Order { get; set; }

    /// <summary>
    /// Componentを初期化する
    /// </summary>
    /// <param name="owner">Componentの持ち主のActorクラス</param>
    /// <param name="order">Componentの実行される順番</param>
    public Component(Actor owner, int order = int.MaxValue)
    {
        Owner = owner;
        Order = order;

        Owner.AddComponent(this);
    }

    /// <summary>
    /// Componentの更新をする
    /// </summary>
    public void Update()
        => UpdateComponent();

    /// <summary>
    /// Componentのレンダリングをする
    /// </summary>
    public void Render()
        => RenderComponent();

    /// <summary>
    /// Componentの終了処理
    /// </summary>
    public void Finish()
        => FinishComponent();

    /// <summary>
    /// 更新処理
    /// </summary>
    protected virtual void UpdateComponent()
    {
    }

    /// <summary>
    /// レンダリング処理
    /// </summary>
    protected virtual void RenderComponent()
    {
    }

    /// <summary>
    /// 終了処理
    /// </summary>
    protected virtual void FinishComponent()
    {
    }
}