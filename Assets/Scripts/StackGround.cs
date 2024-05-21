using DG.Tweening;
using UnityEngine;

public class StackGround : MonoBehaviour
{
    [SerializeField] Renderer _renderer;

    float _fallPositionOffset = 50;
    float _fallDuration = 2;
    float _riseDuration = 2;
    float _scaleDownDuration = 1;

    public Material Material => _renderer.sharedMaterial;

    public void SetMaterial(Material material)
    {
        _renderer.material = material;
    }

    public void FallDownAndScaleDown()
    {
        var sequence = DOTween.Sequence();

        sequence.Append(transform.DOMoveY(transform.position.y - _fallPositionOffset, _fallDuration)
            .SetEase(Ease.InQuad));

        sequence.Join(transform.DOScale(0f, _scaleDownDuration)
            .SetDelay(_fallDuration - _scaleDownDuration).SetEase(Ease.Linear));

        sequence.Play();
        sequence.OnComplete(() => gameObject.SetActive(false));
    }

    public void RiseUp()
    {
        var sequence = DOTween.Sequence();

        var positionY = transform.position.y;
        transform.position = new Vector3(transform.position.x, -_fallPositionOffset, transform.position.z);

        gameObject.SetActive(true);
        sequence.Append(transform.DOMoveY(positionY, _riseDuration)
            .SetEase(Ease.OutQuad));
    }

    private void OnDisable()
    {
        transform.DOKill();

        transform.position = Vector3.zero;
        transform.localScale = Vector3.one;
    }

    private void OnDestroy()
    {
        transform.DOKill();
    }
}
