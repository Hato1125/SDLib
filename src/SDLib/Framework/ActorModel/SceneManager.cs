namespace SDLib.Framework;

public static class SceneManager
{
    /// <summary>
    /// Sceneのリスト
    /// </summary>
    private static readonly Dictionary<string, Scene> _sceneList = new();

    /// <summary>
    /// Sceneのインスタンス
    /// </summary>
    private static Scene? _sceneInstance = null;

    /// <summary>
    /// 次のフレームにシーンセットを実行するか
    /// </summary>
    private static bool _isNextFrameSet = false;

    /// <summary>
    /// 次のフレームにシーンセットするシーン名
    /// </summary>
    private static string _nextFrameSetSceneName = string.Empty;

    /// <summary>
    /// 現在のSceneの名前を取得する
    /// </summary>
    public static string SceneName { get; private set; } = string.Empty;

    /// <summary>
    /// 現在のSceneのActorの数
    /// </summary>
    public static long SceneActorNumber { get; private set; } = 0;

    /// <summary>
    /// Sceneを登録する
    /// </summary>
    /// <param name="sceneName">シーン名</param>
    /// <param name="scene">シーン</param>
    /// <param name="isFastSetScene">もし何もシーンがセットされていなければこのSceneをセットするか</param>
    public static void RegistScene(string sceneName, Scene scene, bool isFastSetScene = true)
    {
        _sceneList.Add(sceneName, scene);

        if (isFastSetScene && SceneName == string.Empty)
            SetScene(sceneName);
    }

    /// <summary>
    /// Sceneをセットする
    /// </summary>
    /// <param name="sceneName">シーン名</param>
    public static void SetScene(string sceneName)
    {
        // 現在のシーンが更新中なのにシーンをセットすると、
        // UpdateでActorListを回しているから例外が発生する
        // なのでシーンがアップデート中の場合は次のフレームで
        // シーンが更新する前にシーンをセットする
        if (_sceneInstance != null && _sceneInstance.IsUpdating)
        {
            Tracer.PrintInfo("NextFrame SetScene.");

            _isNextFrameSet = true;
            _nextFrameSetSceneName = sceneName;
        }
        else
        {
            Tracer.PrintInfo("NowFrame SetScene.");
            SetSceneInfo(sceneName);
        }
    }

    /// <summary>
    /// Scene情報をセットする
    /// </summary>
    private static void SetSceneInfo(string sceneName)
    {
        if (_sceneInstance != null)
        {
            Tracer.PrintInfo($"Finish {SceneName} Scene.");
            _sceneInstance.IsInit = true;
            _sceneInstance.Finish();
        }

        SceneName = sceneName;
        _sceneInstance = _sceneList[sceneName];

        Tracer.PrintInfo($"Set {sceneName} Scene.");
    }

    /// <summary>
    /// 登録されているSceneを消去する
    /// </summary>
    /// <param name="sceneName">シーン名</param>
    public static void RemoveScene(string sceneName)
    {
        Tracer.PrintInfo($"Remove {sceneName} Scene.");
        _sceneList.Remove(sceneName);
    }

    /// <summary>
    /// 登録されているSceneをすべて削除する
    /// </summary>
    public static void RemoveAllScene()
    {
        Tracer.PrintInfo($"Remove All Scene.");

        foreach (var scene in _sceneList)
            scene.Value.Finish();

        _sceneList.Clear();
    }

    /// <summary>
    /// Sceneを表示する
    /// </summary>
    /// <param name="info">アプリケーションの情報</param>
    public static void ViewScene(IReadOnlyAppInfo info)
    {
        if (_sceneInstance == null)
            return;

        if (_sceneInstance.IsInit)
        {
            _sceneInstance.Init(info);
            _sceneInstance.IsInit = false;
        }
        else
        {
            // シーンをセットする
            if (_isNextFrameSet)
            {
                SetSceneInfo(_nextFrameSetSceneName);

                _isNextFrameSet = false;
                _nextFrameSetSceneName = string.Empty;

                // シーンをセットしたらInitなしにUpdate、Renderが実行されるのでreturnさせる
                return;
            }

            _sceneInstance.Update(info);
            _sceneInstance.Render(info);
            SceneActorNumber = _sceneInstance.Actors.Count;
        }
    }
}