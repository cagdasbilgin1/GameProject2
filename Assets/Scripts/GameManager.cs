using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;

public class GameManager : MonoBehaviour
{
    [Inject] Player player;
    [Inject] StackManager stackManager;
    [Inject] LoadingManager loadingManager;

    void OnEnable()
    {
        loadingManager.OnLoadingPanelClosed += GameStart;
        player.OnPlayerFellDown += GameOver;
    }

    void OnDisable()
    {
        loadingManager.OnLoadingPanelClosed -= GameStart;
        player.OnPlayerFellDown -= GameOver;
    }

    void GameStart()
    {
        stackManager.GameStartCall();
    }

    void GameOver()
    {
        player.GameOverCall();
        stackManager.GameOverCall();

        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
