using SaligiaProofOfVision;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Gamekit3D
{
    public class MeleeWeapon : Weapon
    {
        [HideInInspector]
        public float damage = 1;

        [Serializable]
        public class AttackPoint
        {
            public float radius;
            public Vector3 offset;
            public Transform attackRoot;

#if UNITY_EDITOR
            //editor only as it's only used in editor to display the path of the attack that is used by the raycast
            [NonSerialized] public List<Vector3> previousPositions = new List<Vector3>();
#endif
        }

        public ParticleSystem hitParticlePrefab;
        public LayerMask targetLayers;

        public AttackPoint[] attackPoints = new AttackPoint[0];

        [Header("Audio")] public RandomAudioPlayer attackHitAudio;
        public RandomAudioPlayer attackAudio;

        public bool throwingHit
        {
            get { return m_IsThrowingHit; }
            set { m_IsThrowingHit = value; }
        }

        protected Vector3[] m_PreviousPos = null;
        protected Vector3 m_Direction;

        protected bool m_IsThrowingHit = false;
        protected bool m_InAttack = false;

        const int PARTICLE_COUNT = 10;
        protected ParticleSystem[] m_ParticlesPool = new ParticleSystem[PARTICLE_COUNT];
        protected int m_CurrentParticle = 0;

        protected static RaycastHit[] s_RaycastHitCache = new RaycastHit[32];
        protected static Collider[] s_ColliderCache = new Collider[32];

        private HashSet<Collider> _alreadyHitColliders;


        private void Awake()
        {
            if (hitParticlePrefab != null)
            {
                for (int i = 0; i < PARTICLE_COUNT; ++i)
                {
                    m_ParticlesPool[i] = Instantiate(hitParticlePrefab);
                    m_ParticlesPool[i].Stop();
                }
            }
            WeaponType = WeaponType.Melee;
        }

        /// <summary>
        /// Sets the <paramref name="owner"/> the weapon and a optional <paramref name="hitCallback"/> for custom actions apart from damage
        /// </summary>
        /// <param name="owner"></param>
        /// <param name="hitCallback"></param>
        public override void Init(GameObject owner, Action hitCallback = null)
        {
            base.Init(owner, hitCallback);
            _alreadyHitColliders = new HashSet<Collider>();
        }

        public void BeginAttack()
        {
            if (attackAudio != null)
                attackAudio.PlayRandomClip();

            if (_alreadyHitColliders == null)
                _alreadyHitColliders = new HashSet<Collider>();
            _alreadyHitColliders.Clear();
            m_InAttack = true;

            m_PreviousPos = new Vector3[attackPoints.Length];

            for (int i = 0; i < attackPoints.Length; ++i)
            {
                Vector3 worldPos = attackPoints[i].attackRoot.position +
                                   attackPoints[i].attackRoot.TransformVector(attackPoints[i].offset);
                m_PreviousPos[i] = worldPos;

#if UNITY_EDITOR
                attackPoints[i].previousPositions.Clear();
                attackPoints[i].previousPositions.Add(m_PreviousPos[i]);
#endif
            }
        }

        public void EndAttack()
        {
            m_InAttack = false;

#if UNITY_EDITOR
            for (int i = 0; i < attackPoints.Length; ++i)
            {
                attackPoints[i].previousPositions.Clear();
            }
#endif
        }

        public void ResetAlreadyHit()
        {
            _alreadyHitColliders.Clear();
        }

        private void FixedUpdate()
        {
            if (m_InAttack)
            {
                for (int i = 0; i < attackPoints.Length; ++i)
                {
                    AttackPoint pts = attackPoints[i];

                    Vector3 worldPos = pts.attackRoot.position + pts.attackRoot.TransformVector(pts.offset);
                    Vector3 attackVector = worldPos - m_PreviousPos[i];

                    if (attackVector.magnitude < 0.001f)
                    {
                        // A zero vector for the sphere cast don't yield any result, even if a collider overlap the "sphere" created by radius. 
                        // so we set a very tiny microscopic forward cast to be sure it will catch anything overlaping that "stationary" sphere cast
                        attackVector = Vector3.forward * 0.0001f;
                    }

                    Ray r = new Ray(worldPos, attackVector.normalized);

                    int contacts = Physics.SphereCastNonAlloc(r, pts.radius, s_RaycastHitCache, attackVector.magnitude,
                        ~0,
                        QueryTriggerInteraction.Ignore);

                    for (int k = 0; k < contacts; ++k)
                    {
                        Collider col = s_RaycastHitCache[k].collider;

                        if (col != null)
                        {
                            if (!_alreadyHitColliders.Contains(col))
                                CheckDamage(col, pts);
                        }
                    }

                    m_PreviousPos[i] = worldPos;

#if UNITY_EDITOR
                    pts.previousPositions.Add(m_PreviousPos[i]);
#endif
                }
            }
        }

        private bool CheckDamage(Collider col, AttackPoint pts)
        {
            BaseUnit unit = col.GetComponent<BaseUnit>();
            if (unit == null)
            {
                return false;
            }

            if (unit.gameObject == Owner)
                return true; //ignore self harm, but do not end the attack (we don't "bounce" off ourselves)

            if ((targetLayers.value & (1 << col.gameObject.layer)) == 0)
            {
                //hit an object that is not in our layer, this end the attack. we "bounce" off it
                return false;
            }

            _alreadyHitColliders.Add(col);

            if (attackHitAudio != null)
            {
                var renderer = col.GetComponent<Renderer>();
                if (!renderer)
                    renderer = col.GetComponentInChildren<Renderer>();
                if (renderer)
                    attackHitAudio.PlayRandomClip(renderer.sharedMaterial);
                else
                    attackHitAudio.PlayRandomClip();
            }


            DamageMessage data;

            data.damage = damage;
            data.damager = this;
            data.direction = (col.transform.position - Owner.transform.position).normalized;
            data.damageSource = Owner.transform.position;

            unit.ApplyDamage(data);

            if (hitParticlePrefab != null)
            {
                m_ParticlesPool[m_CurrentParticle].transform.position = pts.attackRoot.transform.position;
                m_ParticlesPool[m_CurrentParticle].time = 0;
                m_ParticlesPool[m_CurrentParticle].Play();
                m_CurrentParticle = (m_CurrentParticle + 1) % PARTICLE_COUNT;
            }

            hitCallback?.Invoke();

            return true;
        }

#if UNITY_EDITOR

        private void OnDrawGizmosSelected()
        {
            for (int i = 0; i < attackPoints.Length; ++i)
            {
                AttackPoint pts = attackPoints[i];

                if (pts.attackRoot != null)
                {
                    Vector3 worldPos = pts.attackRoot.TransformVector(pts.offset);
                    Gizmos.color = new Color(1.0f, 1.0f, 1.0f, 0.4f);
                    Gizmos.DrawSphere(pts.attackRoot.position + worldPos, pts.radius);
                }

                if (pts.previousPositions.Count > 1)
                {
                    UnityEditor.Handles.DrawAAPolyLine(10, pts.previousPositions.ToArray());
                }
            }
        }

#endif
    }
}