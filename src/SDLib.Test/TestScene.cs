using System.Drawing;
using SDLib.Graphics;
using SDLib.Framework;
using SDLib.Input;

namespace SDLib.Test;

internal class TestScene : Scene
{
    private Texture2D? texture;
    private double scale;

    public override void Init(IReadOnlyAppInfo info)
    {
        texture = new(info.RenderPtr, $"{AppContext.BaseDirectory}test.png")
        {
            RenderPoint = ReferencePoint.Center,
        };

        base.Init(info);
    }

    public override void Update(IReadOnlyAppInfo info)
    {
        scale += info.DeltaTime.TotalSeconds * 100;
        if (scale >= 180)
            scale = 0;

        base.Update(info);
    }

    public override void Render(IReadOnlyAppInfo info)
    {
        if (texture != null)
        {
            double sin = Math.Sin(scale * Math.PI / 180) * 1.0;
            texture.WidthScale = (float)(0.1 + sin);
            texture.HeightScale = (float)(0.1 + sin);
            texture.Render(1280 / 2, 720 / 2);
        }

        base.Render(info);
    }

    public override void Finish()
    {
        base.Finish();
    }
}