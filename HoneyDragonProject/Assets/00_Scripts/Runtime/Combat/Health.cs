﻿using DamageNumbersPro;
using RPG.Core.Manager;
using System;
using UnityEngine;

namespace RPG.Combat
{
    public class Health : MonoBehaviour, ITakeDamageable
    {
        public int MaxHp;
        public int CurrentHp;
        private float ratio;

        public bool IsAlive { get; private set; }
        public event Action<float> OnHealthChanged;
        public event Action OnHit;
        public event Action OnDie;
        public event Func<int, int> DamageHandle;

        public DamageNumber FloatingDamage;

        private void Awake()
        {
            CurrentHp = MaxHp;

            if (FloatingDamage != null)
            {
                FloatingDamage.enablePooling = true;
            }
        }

        public void SetHp(int hp)
        {
            CurrentHp = MaxHp = hp;
            IsAlive = true;
            OnHealthChanged?.Invoke(CurrentHp / MaxHp);
        }

        public void TakeDamage(DamageInfo damageInfo)
        {
            if (IsAlive == false) return;

            if (DamageHandle != null)
            {
                // 방어력 같은 거 처리
                damageInfo.Damage = DamageHandle.Invoke(damageInfo.Damage);
            }

            if (FloatingDamage != null)
            {
                FloatingDamage.Spawn(transform.position, damageInfo.Damage);
            }


            int damagedHp = Mathf.Clamp(CurrentHp - damageInfo.Damage, 0, MaxHp);
            CurrentHp = damagedHp;

            ratio = CurrentHp / (float)MaxHp;
            OnHealthChanged?.Invoke(ratio);
            OnHit?.Invoke();

            if (damagedHp <= 0)
            {
                // DO SOMETHING
                OnDie?.Invoke();
                IsAlive = false;
            }

            if (IsAlive && damageInfo.IsKnockback)
            {
                // knockbackable 컴포넌트를 가져와서 있으면 넉백 시킨다.
                var knockbackable = GetComponent<Knockbackable>();
                if (knockbackable != null)
                {
                    knockbackable.Knockback(damageInfo.knockbackInfo);
                }
            }

        }
    }
}
