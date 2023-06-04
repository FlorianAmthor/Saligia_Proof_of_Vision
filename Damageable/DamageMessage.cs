using UnityEngine;

namespace SaligiaProofOfVision
{
    public struct DamageMessage
    {
        public MonoBehaviour damager;
        public float damage;
        public Vector3 direction;
        public Vector3 damageSource;
    }
}