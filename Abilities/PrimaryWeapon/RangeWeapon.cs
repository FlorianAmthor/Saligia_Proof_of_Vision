﻿using SaligiaProofOfVision;
using UnityEngine;

namespace Gamekit3D
{
    public class RangeWeapon : Weapon
    {
        public Vector3 muzzleOffset;

        public Projectile projectile;

        public Projectile loadedProjectile
        {
            get { return m_LoadedProjectile; }
        }

        protected Projectile m_LoadedProjectile = null;
        protected ObjectPooler<Projectile> m_ProjectilePool;

        private void Start()
        {
            m_ProjectilePool = new ObjectPooler<Projectile>();
            m_ProjectilePool.Initialize(20, projectile);
        }

        public void Attack(Vector3 target, BaseUnit owner, float impactDamage, float speed, float timeToLive = -1, params object[] data)
        {
            AttackProjectile(target, owner, impactDamage, speed, timeToLive, data);
        }

        public void LoadProjectile()
        {
            if (m_LoadedProjectile != null)
                return;

            m_LoadedProjectile = m_ProjectilePool.GetNew();
            m_LoadedProjectile.transform.SetParent(transform, false);
            m_LoadedProjectile.transform.localPosition = muzzleOffset;
            m_LoadedProjectile.transform.localRotation = Quaternion.identity;
        }

        void AttackProjectile(Vector3 target, BaseUnit owner, float impactDamage, float speed, float timeToLive = -1, params object[] data)
        {
            if (m_LoadedProjectile == null) LoadProjectile();

            m_LoadedProjectile.transform.SetParent(null, true);
            m_LoadedProjectile.InitialSetup(target, owner, impactDamage, speed, timeToLive, data);
            m_LoadedProjectile = null; //once shot, we don't own the projectile anymore, it does it's own life.
        }

#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            Vector3 worldOffset = transform.TransformPoint(muzzleOffset);
            UnityEditor.Handles.color = Color.yellow;
            UnityEditor.Handles.DrawLine(worldOffset + Vector3.up * 0.4f, worldOffset + Vector3.down * 0.4f);
            UnityEditor.Handles.DrawLine(worldOffset + Vector3.forward * 0.4f, worldOffset + Vector3.back * 0.4f);
        }
#endif
    }
}