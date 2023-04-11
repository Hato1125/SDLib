using System.Numerics;
using SDLib.Graphics;

namespace SDLib.Framework;

public class TextureComponent : Component
{
    /// <summary>
    /// Texture2D
    /// </summary>
    public Texture2D? Texture { get; set; }

    /// <summary>
    /// Textureをレンダリングする位置
    /// </summary>
    public Vector2 Position { get; set; }

    /// <summary>
    /// TextureComponentを初期化する
    /// </summary>
    /// <param name="owner">持ち主のActorクラス</param>
    public TextureComponent(Actor owner)
        : base(owner)
    {
    }

    /// <summary>
    /// コンポーネントを描画する
    /// </summary>
    protected override void ComponentRender()
    {
        Texture?.Render(Position.X, Position.Y);
        base.ComponentRender();
    }
}