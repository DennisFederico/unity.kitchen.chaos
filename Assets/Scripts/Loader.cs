using Unity.Netcode;
using UnityEngine.SceneManagement;

public static class Loader
{
    public enum Scene {
        GameScene,
        LoadingScene,
        MainMenu,
        LobbyScene,
        CharacterSelectionScene,
    }

    private static Scene _targetScene;
    
    public static void Load(Scene scene) {
        _targetScene = scene;
        SceneManager.LoadScene(Scene.LoadingScene.ToString());
    }

    public static void LoadNetwork(Scene scene) {
        NetworkManager.Singleton.SceneManager.LoadScene(scene.ToString(), LoadSceneMode.Single);
    }

    public static void LoaderCallback() {
        SceneManager.LoadScene(_targetScene.ToString());
    }
}
