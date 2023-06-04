using System;

namespace SaligiaProofOfVision
{
    public interface IDamageable
    {
        void ApplyDamage(DamageMessage dmgMessage, Action<EnemyUnit> killCallback = null);
    }
}