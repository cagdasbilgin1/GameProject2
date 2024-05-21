using UnityEngine;
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
        stackManager.OnStackCanNotBeConnectedToPath += GameOver;
    }

    void OnDisable()
    {
        loadingManager.OnLoadingPanelClosed -= GameStart;
        player.OnPlayerFellDown -= GameOver;
        stackManager.OnStackCanNotBeConnectedToPath -= GameOver;
    }

    void GameStart()
    {
        player.GameStartCall();
        stackManager.GameStartCall();
    }

    void GameOver()
    {
        player.GameOverCall();
        stackManager.GameOverCall();
    }
}
