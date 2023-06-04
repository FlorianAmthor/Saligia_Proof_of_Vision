using Gamekit3D;
using System.Collections.Generic;
using UnityEngine;

namespace SaligiaProofOfVision
{
    public class GravitationalOrbProjectile : Projectile
    {
        private float _radius;
        private float _timeBetweenDamageTicks;
        private float _tickDamage;
        private float _graviationalForce;
        private float _duration;

        private bool _hasExploded = false;

        private LayerMask _afflictedObjects;
        private float _explodeTime;
        private float _lastOrbDamageTick;

        private bool _hasT2Rune;
        private float _knockBackForce;

        private Dictionary<EnemyUnit, int> _affectedEnemies;
        private Player _player;

        [SerializeField]
        private ParticleSystem _waveParticleSystem;

        private void Start()
        {
            _affectedEnemies = new Dictionary<EnemyUnit, int>();
        }

        protected override void Update()
        {
            if (_hasExploded)
                return;

            if (timeToLive != -1)
                if (timeActivated + timeToLive <= Time.time)
                    Explode();
        }

        protected override void FixedUpdate()
        {
            if (!_hasExploded)
            {
                rb.MovePosition(transform.position + direction * speed * Time.fixedDeltaTime);
                return;
            }

            if (_explodeTime + _duration <= Time.time)
            {
                if (_hasT2Rune)
                {
                    var cols = Physics.OverlapSphere(transform.position, _radius, _afflictedObjects);
                    foreach (var col in cols)
                    {
                        if (!col.CompareTag("Enemy"))
                            continue;
                        var enemy = col?.GetComponent<EnemyUnit>();
                        enemy?.Controller.Rigidbody.AddExplosionForce(_knockBackForce, transform.position, _radius, 0, ForceMode.Impulse);
                    }
                }
                _hasExploded = false;

                foreach (var enemy in _affectedEnemies.Keys)
                {
                    enemy.Controller.ClearForce();
                    enemy.ApplyHardCC(false);
                }

                _affectedEnemies.Clear();

                pool.Free(this);
                gameObject.SetActive(false);
            }
            else
                Tick();

            foreach (var enemy in _affectedEnemies.Keys)
            {
                var dir = transform.position - enemy.transform.position;
                dir.y = 0;
                enemy.Controller.AddForce(dir.normalized * Time.fixedTime * 0.1f, false);
                //enemy.Controller.Rigidbody.AddExplosionForce(-_graviationalForce, transform.position, _radius * 2);
                //enemy.transform.RotateAround(transform.position, Vector3.up, _affectedEnemies[enemy] * Time.fixedDeltaTime);
                //enemy.transform.LookAt(_player.transform);
            }
        }

        private void OnDrawGizmos()
        {
            Gizmos.DrawWireSphere(transform.position, _radius);
        }

        private void Tick()
        {
            var cols = Physics.OverlapSphere(transform.position, _radius, _afflictedObjects);
            var saveTickTime = false;

            foreach (var col in cols)
            {
                if (!col.CompareTag("Enemy"))
                    continue;
                var enemy = col?.GetComponent<EnemyUnit>();
                if (enemy == null)
                    continue;

                if (!_affectedEnemies.ContainsKey(enemy))
                {
                    _affectedEnemies.Add(enemy, Random.Range(60, 240));
                }

                if (_lastOrbDamageTick + _timeBetweenDamageTicks <= Time.time && _hasT2Rune)
                {
                    DamageMessage msg;
                    msg.damager = this;
                    msg.damage = _tickDamage;
                    msg.direction = Vector3.zero;
                    msg.damageSource = enemy.transform.position;

                    saveTickTime = true;
                    enemy.ApplyDamage(msg);
                }
            }

            var nullEnemies = new List<EnemyUnit>();

            foreach (var enemy in _affectedEnemies.Keys)
            {
                if (enemy == null)
                {
                    nullEnemies.Add(enemy);
                    continue;
                }
                enemy.ApplyHardCC(true);
            }

            foreach (var enemy in nullEnemies)
                _affectedEnemies.Remove(enemy);

            if (saveTickTime)
                _lastOrbDamageTick = Time.time;
        }

        public override void InitialSetup(Vector3 dir, BaseUnit owner, float impactDamage, float speed, float timeToLive = -1, params object[] data)
        {
            base.InitialSetup(dir, owner, impactDamage, speed, timeToLive);
            GetComponent<Rigidbody>().useGravity = true;
            GetComponent<Rigidbody>().isKinematic = false;
            _lastOrbDamageTick = 0;
            _hasExploded = false;
            _radius = (float)data[0];
            _timeBetweenDamageTicks = (float)data[1];
            _tickDamage = (float)data[2];
            _graviationalForce = (float)data[3];
            _duration = (float)data[4];
            _afflictedObjects = (LayerMask)data[5];
            _hasT2Rune = (bool)data[6];
            _knockBackForce = (float)data[7];

            var ms = _waveParticleSystem.main;
            ms.startSize = _radius / 2;

            _affectedEnemies = new Dictionary<EnemyUnit, int>();
            if (_player == null)
                _player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        }

        protected override void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.CompareTag("Terrain"))
                return;
            if (!_hasExploded)
                Explode();
        }

        //protected override void OnTriggerEnter(Collider other)
        //{
        //    if (collision.gameObject.CompareTag("Terrain"))
        //        return;
        //    if (!_hasExploded)
        //        Explode();
        //}

        private void Explode()
        {
            GetComponent<Rigidbody>().useGravity = false;
            GetComponent<Rigidbody>().isKinematic = true;
            _explodeTime = Time.time;
            _hasExploded = true;

            //TODO: Set size of waves when it explodes to the correct size(radius), don't show orb waves before
            var cols = Physics.OverlapSphere(transform.position, _radius, _afflictedObjects.value);

            DamageMessage msg;
            msg.damage = impactDamage;
            msg.damager = _player;

            foreach (var col in cols)
            {
                var enemy = col.GetComponent<EnemyUnit>();
                if (enemy == null)
                    continue;
                msg.direction = transform.position - enemy.transform.position;
                msg.damageSource = enemy.transform.position;
                enemy.ApplyDamage(msg);
            }
        }
    }
}