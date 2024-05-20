using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class ParticleManager : MonoBehaviour
{
    [SerializeField] List<ParticleSystem> coinParticlePool;
    [SerializeField] List<ParticleSystem> starParticlePool;
    [SerializeField] List<ParticleSystem> gemParticlePool;
    [SerializeField] List<ParticleSystem> trailParticlePool;

    [SerializeField] float trailHeight = 6f;
    [SerializeField] float trainDuration = 1f;

    bool hasTrail;

    public void PlayParticleAtPosition(CollectableType type, Vector3 position)
    {
        var particle = GetNextParticleFromPool(type);

        particle.transform.position = position;
        particle.Play();

        if (hasTrail)
        {
            var trailParticle = GetTrailParticleFromPool();

            trailParticle.transform.position = position;
            trailParticle.Play();

            trailParticle.transform.DOMoveY(transform.position.y + trailHeight, trainDuration).SetEase(Ease.OutQuad);
        }
    }

    List<ParticleSystem> GetParticlePool(CollectableType type)
    {
        switch (type)
        {
            case CollectableType.Coin:
                hasTrail = false;
                return coinParticlePool;
            case CollectableType.Star:
                hasTrail = true;
                return starParticlePool;
            case CollectableType.Gem:
                hasTrail = false;
                return gemParticlePool;
            default:
                return null;
        }
    }

    ParticleSystem GetNextParticleFromPool(CollectableType type)
    {
        var particlePool = GetParticlePool(type);
        var particle = particlePool[particlePool.Count - 1];

        particlePool.Remove(particle);
        particlePool.Insert(0, particle);

        return particle;
    }

    ParticleSystem GetTrailParticleFromPool()
    {
        var particle = trailParticlePool[trailParticlePool.Count - 1];

        trailParticlePool.Remove(particle);
        trailParticlePool.Insert(0, particle);

        return particle;
    }
}
