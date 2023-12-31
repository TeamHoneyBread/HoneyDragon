using Cysharp.Threading.Tasks;
using RPG.Core;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Pool;

namespace RPG.Combat.Projectile
{
    public abstract class ProjectileBase : MonoBehaviour, IPoolable<ProjectileBase>
    {
        [SerializeField] protected float speed;

        protected DamageInfo dmgInfo;
        protected Vector3 startPos;
        protected Vector3 direction;
        protected Vector3 calculatedPosition;
        protected const float LIFE_TIME = 5f;
        protected float elapsedTime;
        protected bool IsAlive;

        protected Rigidbody rig;
        protected Creature target;
        protected ObjectPool<ProjectileBase> owner;

        [SerializeField] protected TrailRenderer trailRenderer;
        [SerializeField] protected GameObject mainVfx;
        [SerializeField] protected GameObject muzzleVfx;
        [SerializeField] protected GameObject hitVfx;

        protected bool isDestroyed = false;
        public bool IsDestroyed => isDestroyed;

        private void Awake()
        {
            rig = GetComponent<Rigidbody>();
            trailRenderer = mainVfx.GetComponentInChildren<TrailRenderer>(true);
            rig.useGravity = false;
        }

        public void SetPool(ObjectPool<ProjectileBase> owner)
        {
            this.owner = owner;
        }

        void FixedUpdate()
        {
            if (!IsAlive) return;
            UpdateLifeTime();
        }

        public void Fire(DamageInfo dmgInfo, Vector3 startPos, Vector3 dir, float speed, Creature target = null)
        {
            this.transform.forward = direction = dir;
            this.transform.position = startPos;
            this.startPos = startPos;
            this.speed = speed;
            this.dmgInfo = dmgInfo;
            this.target = target;

            ResetProjectile();
            rig.velocity = dir * speed;

            muzzleVfx.transform.forward = gameObject.transform.forward;
            ParticleSystem ps = muzzleVfx.GetComponentInChildren<ParticleSystem>(true);
            DelayDisable(muzzleVfx, (int)(ps.main.duration * 1000f)).Forget();
        }

        protected virtual void ResetProjectile()
        {
            IsAlive = true;
            elapsedTime = 0f;

            // 트레일 초기화
            if (trailRenderer != null)
            {
                trailRenderer.Clear();
            }

            mainVfx.SetActive(true);
            muzzleVfx.SetActive(true);
            hitVfx.SetActive(false);
        }

        protected async UniTaskVoid DelayDisable(UnityEngine.GameObject obj, int milliTime)
        {
            await UniTask.Delay(milliTime, false, PlayerLoopTiming.Update, this.GetCancellationTokenOnDestroy());
            obj.SetActive(false);
        }

        protected async UniTaskVoid DestroyParticle(int milliTime)
        {

            mainVfx.SetActive(false);

            await UniTask.Delay(milliTime, false, PlayerLoopTiming.Update, this.GetCancellationTokenOnDestroy());
            IsAlive = false;
            muzzleVfx.SetActive(false);
            hitVfx.SetActive(false);
            owner.Release(this);
        }

        protected void DestroyParticleImmediately()
        {
            IsAlive = false;
            mainVfx.SetActive(false);
            muzzleVfx.SetActive(false);
            hitVfx.SetActive(false);
            owner.Release(this);
        }

        private void UpdateLifeTime()
        {
            elapsedTime += Time.fixedDeltaTime;
            if (elapsedTime > LIFE_TIME)
            {
                DestroyParticleImmediately();
                elapsedTime = 0f;
                return;
            }
        }

        protected abstract void OnTriggerEnter(Collider other);

        public virtual void OnGetAction()
        {
            gameObject.SetActive(true);
        }

        public virtual void OnReleaseAction()
        {
            gameObject.SetActive(false);
        }

        public virtual void OnDestroyAction()
        {
            Destroy(gameObject);
        }
    }
}