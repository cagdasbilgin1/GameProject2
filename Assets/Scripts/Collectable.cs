using DG.Tweening;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class Collectable : MonoBehaviour, ICollectable
{
    [SerializeField] CollectableType _type;
    [SerializeField] AudioClip _collectSound;
    [SerializeField] int _point;
    [SerializeField] float _rotationSpeed = 50f;

    Tweener _rotationTween;

    public CollectableType Type => _type;
    public AudioClip CollectSound => _collectSound;
    public int Point => _point;

    void OnEnable()
    {
        RotateY();
    }

    void OnDisable()
    {
        _rotationTween.Kill();
    }

    void RotateY()
    {
        var rotationDuration = 360f / _rotationSpeed;

        _rotationTween = transform.DORotate(new Vector3(0, 360, 0), rotationDuration, RotateMode.FastBeyond360)
            .SetEase(Ease.Linear)
            .SetLoops(-1, LoopType.Incremental);
    }

    public void SetPassive()
    {
        gameObject.SetActive(false);
    }
}
