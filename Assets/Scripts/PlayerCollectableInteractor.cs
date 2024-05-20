using UnityEngine;
using Zenject;

public class PlayerCollectableInteractor : MonoBehaviour
{
    [Inject] ParticleManager particleManager;
    [Inject] SoundManager soundManager;

    int score;

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

    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        
    }

    void Collect(ICollectable collectable)
    {
        score += collectable.Point;
        collectable.SetPassive();
    }
}
