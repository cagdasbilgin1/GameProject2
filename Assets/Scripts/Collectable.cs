using DG.Tweening;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class Collectable : MonoBehaviour, ICollectable
{
    [SerializeField] CollectableType _type;
    [SerializeField] int _point;
    public CollectableType Type => _type;
    public int Point => _point;

    [SerializeField] float rotationSpeed = 50f;
    Tweener rotationTween;

    void OnEnable()
    {
        RotateY();
    }

    void OnDisable()
    {
        rotationTween.Kill();
    }

    void RotateY()
    {
        var rotationDuration = 360f / rotationSpeed;

        rotationTween = transform.DORotate(new Vector3(0, 360, 0), rotationDuration, RotateMode.FastBeyond360)
            .SetEase(Ease.Linear)
            .SetLoops(-1, LoopType.Incremental);
    }

    public void SetPassive()
    {
        gameObject.SetActive(false);
    }
}
