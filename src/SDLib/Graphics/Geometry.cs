using SDL2;

namespace SDLib.Graphics;

public class Geometry
{
    private readonly IntPtr _renderer;

    /// <summary>
    /// Geometryの頂点情報
    /// </summary>
    public SDL.SDL_Vertex[] Vertexs { get; set; }

    /// <summary>
    /// 頂点のインデックス
    /// </summary>
    public int[] VertexIndex { get; set; }

    /// <summary>
    /// Geometryを初期化する
    /// </summary>
    /// <param name="renderer">レンダラー</param>
    /// <param name="vertexs">頂点情報</param>
    /// <param name="vertexIndex">頂点インデックス</param>
    public Geometry(IntPtr renderer, SDL.SDL_Vertex[] vertexs, int[]? vertexIndex = null)
    {
        if (renderer == IntPtr.Zero)
            throw new ArgumentNullException(nameof(renderer), "An invalid pointer was passed.");

        _renderer = renderer;
        Vertexs = vertexs;
        if(vertexIndex != null)
        {
            VertexIndex = vertexIndex;
        }
        else
        {
            VertexIndex = new int[Vertexs.Length];
            for (int i = 0; i < Vertexs.Length; i++)
                VertexIndex[i] = i;
        }
    }

    /// <summary>
    /// Geometryを初期化する
    /// </summary>
    /// <param name="texture">テクスチャポインター</param>
    public void Render(IntPtr? texture = null)
    {
        texture = texture ?? IntPtr.Zero;

        SDL.SDL_RenderGeometry(
            _renderer,
            texture.Value,
            Vertexs,
            VertexIndex.Length,
            VertexIndex,
            VertexIndex.Length
        );
    }
}