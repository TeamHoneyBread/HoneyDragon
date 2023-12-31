using Cysharp.Threading.Tasks;
using RPG.Core;
using RPG.Core.Manager;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Combat.Skill
{
    public class WaterTornado : SpawnSkill
    {
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
            for (int i = 0; i < spawnCount; ++i)
            {
                if (spawnObjects.TryGet(out var get) == true)
                {
                    Vector2 position = (Random.insideUnitCircle * 5f);
                    Vector3 spawnPos = initiator.position + new Vector3(position.x, 0f, position.y);
                    spawnPos.y = 0f;
                    get.Spawn(spawnPos, Data);
                }

                Managers.Instance.Sound.PlaySound(SoundType.Effect, activeSoundResourcePath);
                await UniTask.Delay(Data.SpawnRateMilliSecond, false, PlayerLoopTiming.Update, this.GetCancellationTokenOnDestroy());
            }
        }
    }
}
