using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class ParticleManager : MonoBehaviour
{
    [SerializeField] List<ParticleSystem> _coinParticlePool;
    [SerializeField] List<ParticleSystem> _starParticlePool;
    [SerializeField] List<ParticleSystem> _gemParticlePool;
    [SerializeField] List<ParticleSystem> _trailParticlePool;

    [SerializeField] float _trailHeight = 6f;
    [SerializeField] float _trainDuration = 1f;

    bool _hasTrail;

    public void PlayParticleAtPosition(CollectableType type, Vector3 position)
    {
        var particle = GetNextParticleFromPool(type);

        particle.transform.position = position;
        particle.Play();

        if (_hasTrail)
        {
            var trailParticle = GetTrailParticleFromPool();

            trailParticle.transform.position = position;
            trailParticle.Play();

            trailParticle.transform.DOMoveY(transform.position.y + _trailHeight, _trainDuration).SetEase(Ease.OutQuad);
        }
    }

    List<ParticleSystem> GetParticlePool(CollectableType type)
    {
        switch (type)
        {
            case CollectableType.Coin:
                _hasTrail = false;
                return _coinParticlePool;
            case CollectableType.Star:
                _hasTrail = true;
                return _starParticlePool;
            case CollectableType.Gem:
                _hasTrail = false;
                return _gemParticlePool;
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
        var particle = _trailParticlePool[_trailParticlePool.Count - 1];

        _trailParticlePool.Remove(particle);
        _trailParticlePool.Insert(0, particle);

        return particle;
    }
}
