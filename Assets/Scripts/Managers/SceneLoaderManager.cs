using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoaderManager : MonoBehaviour , IProvidable
{
    private const string INITIAL_SCENE_NAME = "InitialScene";
    private const string GAME_SCENE_NAME =  "GameScene";

    void Awake()
    {
        ServiceProvider.Register(this);
    }

    public void LoadInitialScene()
    {
        LoadScene(INITIAL_SCENE_NAME);
    }

    public void LoadGameScene()
    {
        LoadScene(GAME_SCENE_NAME);
    }

    private void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

}
