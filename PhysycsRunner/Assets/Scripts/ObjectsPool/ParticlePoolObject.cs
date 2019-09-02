using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
public class ParticlePoolObject : PoolObject
{
    ParticleSystem _particles;
    ParticleSystem Particles
    {
        get
        {
            if (_particles == null)
                _particles = GetComponent<ParticleSystem>();

            return _particles;
        }
    }

    [SerializeField] bool _destroyAfterPlay;

    private void Update()
    {
        if (_destroyAfterPlay && Particles)
            if (!Particles.IsAlive())
                Destroy();
    }

    public override void ResetState()
    {

    }
}
