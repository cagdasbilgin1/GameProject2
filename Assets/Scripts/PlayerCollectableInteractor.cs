using UnityEngine;
using Zenject;

public class PlayerCollectableInteractor : MonoBehaviour
{
    [Inject] ParticleManager particleManager;
    [Inject] SoundManager soundManager;
    [Inject] UIManager uiManager;

    int _score;

    void OnTriggerEnter(Collider other)
    {
        ICollectable collectable;
        if (other.TryGetComponent(out collectable))
        {
            particleManager.PlayParticleAtPosition(collectable.Type, other.transform.position);
            soundManager.PlaySound(collectable.CollectSound);
            collectable.SetPassive();
            Collect(collectable);
        }
    }

    void Collect(ICollectable collectable)
    {
        _score += collectable.Point;
        collectable.SetPassive();
        uiManager.UpdateScoreText(_score);
    }
}
