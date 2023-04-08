using SDLib.Graphics;

namespace SDLib.Resource;

public class TextureManager
{
    /// <summary>
    /// Textureリスト
    /// </summary>
    private readonly HashSet<Texture2D> _textures = new();

    /// <summary>
    /// Textureを読み込む
    /// </summary>
    /// <param name="renderer">レンダラー</param>
    /// <param name="fileName">ファイル名</param>
    public Texture2D LoadTexture(IntPtr renderer, string fileName)
    {
        var texture = new Texture2D(renderer, fileName);
        _textures.Add(texture);

        return texture;
    }

    /// <summary>
    /// Textureを読み込む
    /// </summary>
    /// <param name="renderer">レンダラー</param>
    /// <param name="fileName">ファイル名</param>
    public Texture2D[] LoadTexture(IntPtr renderer, string[] fileName)
    {
        var textures = new Texture2D[fileName.Length];

        for (int i = 0; i < fileName.Length; i++)
            textures[i] = LoadTexture(renderer, fileName[i]);

        return textures;
    }

    /// <summary>
    /// 登録されているテクスチャを破棄する
    /// </summary>
    /// <param name="texture">テクスチャ</param>
    public void DeleteTexture(Texture2D texture)
    {
        if (!_textures.Contains(texture))
            return;

        texture.Dispose();
        _textures.Remove(texture);
    }

    /// <summary>
    /// 登録されているテクスチャを破棄する
    /// </summary>
    /// <param name="textures">テクスチャ</param>
    public void DeleteTexture(Texture2D[] textures)
    {
        foreach (var texture in textures)
            DeleteTexture(texture);
    }

    /// <summary>
    /// 登録されているテクスチャをすべて削除する 
    /// </summary>
    public void DeleteAllTexture()
    {
        foreach (var texture in _textures)
            texture.Dispose();

        _textures.Clear();
    }
}