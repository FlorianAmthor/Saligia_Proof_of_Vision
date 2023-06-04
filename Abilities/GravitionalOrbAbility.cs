using Gamekit3D;
using System.Collections;
using UnityEngine;

namespace SaligiaProofOfVision.Abilities
{
    [CreateAssetMenu(fileName = "GravitationalOrb", menuName = "Abilities/Gravitational Orb")]
    public class GravitionalOrbAbility : Ability
    {
        [SerializeField] private GameObject _projectile;

        [SerializeField] private float _radius;
        [SerializeField] private float _projectileSpeed;
        [SerializeField] private float _timeBetweenDamageTicks;
        [SerializeField] private float _tickDamage;
        [SerializeField] private float _graviationalForce;
        [SerializeField] private float _duration;
        [SerializeField] private float _knockBackForce;

        private Projectile _loadedProjectile = null;
        private ObjectPooler<Projectile> _projectilePool;

        private Vector3 _direction;

        private void OnDisable()
        {
            _loadedProjectile = null;
            _projectilePool = null;
        }

        public override void UpdateTarget(Vector3 vec)
        {
            base.UpdateTarget(vec);
            _direction = vec;
        }

        public override IEnumerator Use(BaseUnit owner = null, params object[] args)
        {
            if(_projectilePool == null)
            {
                _projectilePool = new ObjectPooler<Projectile>();
                _projectilePool.Initialize(3, _projectile.GetComponent<Projectile>());
            }

            cooldown.SetBase();
            var spawnPos = owner.transform.position + _direction;
            spawnPos.y += 0.5f;
            _loadedProjectile = _projectilePool.GetNew();

            //TODO: Maybe parent ability projectile to new gameobject for cleaner object structure
            //_loadedProjectile.transform.SetParent(_combatTextCanvas.transform);
            _loadedProjectile.transform.position = spawnPos;
            _loadedProjectile.InitialSetup(_direction, owner, 0f, _projectileSpeed, 5, _radius, _timeBetweenDamageTicks, _tickDamage, _graviationalForce, _duration, afflictedObjects, HasT2Rune, _knockBackForce);
            _loadedProjectile = null;
            yield return "";
        }
    }
}