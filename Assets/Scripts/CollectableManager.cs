using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;

public class CollectableManager : MonoBehaviour
{
    [SerializeField] List<Collectable> _coinPool;
    [SerializeField] List<Collectable> _starPool;
    [SerializeField] List<Collectable> _gemPool;
    [SerializeField] float _startYPosition = 10f;
    [SerializeField] float _duration = 1f;

    public void HandOutAllCollectables(float minZ, float maxZ)
    {
        var usedZPositions = new HashSet<float>();

        foreach (var collectablePool in new List<List<Collectable>> { _coinPool, _starPool, _gemPool })
        {
            foreach (var collectable in collectablePool)
            {
                float randZ;
                do
                {
                    randZ = Random.Range(minZ, maxZ);
                } while (usedZPositions.Contains(randZ));

                usedZPositions.Add(randZ);

                collectable.transform.position = new Vector3(0, _startYPosition, randZ);
                collectable.gameObject.SetActive(true);
                collectable.transform.DOMoveY(0, _duration).SetEase(Ease.OutBack);
            }
        }
    }
}