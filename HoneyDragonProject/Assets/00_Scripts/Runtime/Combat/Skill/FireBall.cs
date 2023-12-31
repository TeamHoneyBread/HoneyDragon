﻿using Cysharp.Threading.Tasks;
using RPG.Core;
using RPG.Core.Data;
using RPG.Core.Manager;
using RPG.Util;
using UnityEngine;

namespace RPG.Combat.Skill
{
    public class FireBall : ShootingSkill
    {
        public override void SetData(SkillData data)
        {
            base.SetData(data);
            CurrentLevel = data.Level;
        }

        public override async UniTaskVoid UseSkill()
        {
            while (true)
            {
                Activate(owner).Forget();
                await UniTask.Delay((int)(Data.CoolTime * 1000), false, PlayerLoopTiming.Update, this.GetCancellationTokenOnDestroy());
            }
        }

        protected override async UniTaskVoid Activate(Creature initiator)
        {
            for (int i = 0; i < Data.SpawnCount; ++i)
            {
                var target = Utility.FindNearestObject(initiator.transform, 50f, LayerMask.GetMask("Enemy"));
                var get = projectiles.Get();
                float randZ = Random.Range(-1f, 1f);
                float randX = Random.Range(-1f, 1f);
                Vector3 dir = new Vector3(randX, 0f, randZ).normalized;

                if (target != null)
                {
                    dir = (target.transform.position - initiator.position).normalized;
                }
                get.Fire(new DamageInfo(null, Random.Range(Data.MinDamage, Data.MaxDamage + 1), new KnockbackInfo(dir, 8f)), initiator.center, dir, Data.Speed, initiator);
                Managers.Instance.Sound.PlaySound(SoundType.Effect, activeSoundResourcePath);
                await UniTask.Delay(Data.SpawnRateMilliSecond, false, PlayerLoopTiming.Update, this.GetCancellationTokenOnDestroy());
            }
        }
    }
}
