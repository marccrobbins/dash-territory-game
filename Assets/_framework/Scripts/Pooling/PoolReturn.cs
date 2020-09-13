using System.Collections;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Framework.Pooling
{
    public class PoolReturn : MonoBehaviour, IPoolable
    {
        public PoolType type;
        public PoolReturnType poolReturnType;
        
        [ShowIf("poolReturnType", PoolReturnType.Delay)]
        public float returnDelay;
        
        [ShowIf("type", PoolType.GameObject)]
        [ShowIf("poolReturnType", PoolReturnType.Duration)]
        public float returnDuration;

        private Coroutine deSpawnCoroutine;
        
        #region GameObjectCollision

        private void OnTriggerEnter(Collider other)
        {
            PoolManager.Instance.DeSpawn(gameObject);
        }

        private void OnCollisionEnter(Collision other)
        {
            PoolManager.Instance.DeSpawn(gameObject);
        }

        #endregion GameObjectCollision
        
        #region ParticleCollision

        private void OnParticleTrigger()
        {
            PoolManager.Instance.DeSpawn(gameObject);
        }

        private void OnParticleCollision(GameObject other)
        {
            PoolManager.Instance.DeSpawn(gameObject);
        }
        
        #endregion ParticleCollision

        private IEnumerator DeSpawnRoutine(float duration)
        {
            yield return new WaitForSeconds(duration);
            PoolManager.Instance.DeSpawn(gameObject);
        }

        #region IPoolable

        public void OnSpawn()
        {
            if (poolReturnType == PoolReturnType.Collision ||
                poolReturnType == PoolReturnType.Manual) return;
            
            var timeValue = 0f;
            switch (type)
            {
                case PoolType.GameObject:
                    if (poolReturnType == PoolReturnType.Delay) timeValue = returnDelay;
                    else if (poolReturnType == PoolReturnType.Duration) timeValue = returnDuration;
                    break;
                case PoolType.Particle:
                    if (poolReturnType == PoolReturnType.Delay) timeValue = returnDelay;
                    else if (poolReturnType == PoolReturnType.Duration)
                    {
                        var particle = GetComponent<ParticleSystem>();
                        if (!particle) break;
                        timeValue = particle.main.duration;
                    }
                    break;
                case PoolType.Sound:
                    if (poolReturnType == PoolReturnType.Delay) timeValue = returnDelay;
                    else if (poolReturnType == PoolReturnType.Duration)
                    {
                        var audioSource = GetComponent<AudioSource>();
                        if (!audioSource) break;
                        var clip = audioSource.clip;
                        if (!clip) break;
                        timeValue = clip.length;
                    }
                    break;
            }
            
            if (deSpawnCoroutine != null) StopCoroutine(deSpawnCoroutine);
            deSpawnCoroutine = StartCoroutine(DeSpawnRoutine(timeValue));
        }

        public void OnDeSpawn()
        {
            if (deSpawnCoroutine != null) StopCoroutine(deSpawnCoroutine);
        }

        #endregion IPoolable
    }
    
    public enum PoolType
    {
        GameObject = 0,
        Particle,
        Sound
    }

    public enum PoolReturnType
    {
        Manual,
        Delay,
        Duration,
        Collision
    }
}
