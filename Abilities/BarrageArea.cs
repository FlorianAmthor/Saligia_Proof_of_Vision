using Gamekit3D;
using SaligiaProofOfVision;
using System.Collections.Generic;
using UnityEngine;

public class BarrageArea : MonoBehaviour, IPooled<BarrageArea>
{
    private float _timeBetweenTicks;
    private float _tickDamage;
    private float _lastTickTime;
    private float _duration;
    private HashSet<EnemyUnit> _enemies;
    private CapsuleCollider _capsuleCollider;
    private bool _hasT2Rune;
    private bool _firstTick;
    private float _rootDuration;

    private bool _hasNullEnemy = false;

    [SerializeField]
    private ParticleSystem _particleSystem;

    public int poolID { get; set; }
    public ObjectPooler<BarrageArea> pool { get; set; }

    private void Start()
    {
        _enemies = new HashSet<EnemyUnit>();
        _capsuleCollider = GetComponent<CapsuleCollider>();
    }

    public void InitialSetup(Vector3 pos, float radius, float timeBetweenTicks, float tickDamage, float duration, params object[] data)
    {
        if (_enemies == null)
            _enemies = new HashSet<EnemyUnit>();
        if (_capsuleCollider == null)
            _capsuleCollider = GetComponent<CapsuleCollider>();

        _enemies.Clear();

        if (Physics.Raycast(pos, Vector3.down, out RaycastHit hit, 100, LayerMask.GetMask("Default"), QueryTriggerInteraction.Ignore))
            pos = hit.point;
        pos.y += 0.05f;
        transform.position = pos;
        _lastTickTime = 0;
        _capsuleCollider.radius = radius / 2;
        _timeBetweenTicks = timeBetweenTicks;
        _tickDamage = tickDamage;
        _duration = duration;
        _hasT2Rune = (bool)data[0];
        _rootDuration = (float)data[1];

        _firstTick = true;

        _particleSystem.Stop();
        _particleSystem.Clear();
        var ms = _particleSystem.main;
        ms.startSize = radius * 1.5f;
        ms.duration = duration;
        _particleSystem.Play();
    }

    private void Update()
    {
        _duration -= Time.deltaTime;
        if (_duration <= 0)
        {
            _particleSystem.Stop();
            _particleSystem.Clear();
            pool.Free(this);
            gameObject.SetActive(false);
            return;
        }
        if (_firstTick)
        {
            _firstTick = false;
            var objs = Physics.OverlapSphere(transform.position, _capsuleCollider.radius);
            List<EnemyUnit> enemies = new List<EnemyUnit>();
            foreach (var col in objs)
                if (col.CompareTag("Enemy"))
                    enemies.Add(col.GetComponent<EnemyUnit>());
            if (_hasT2Rune)
                EnemyManager.Instance.RootEnemies(new List<EnemyUnit>(enemies), _rootDuration);
        }
        if (_lastTickTime + _timeBetweenTicks <= Time.time)
        {
            _lastTickTime = Time.time;
            foreach (var enemy in _enemies)
            {
                if (enemy == null)
                {
                    _hasNullEnemy = true;
                    continue;
                }

                DamageMessage msg;

                msg.damage = _tickDamage;
                msg.damager = this;
                msg.direction = Vector3.zero;
                msg.damageSource = enemy.transform.position;

                enemy.ApplyDamage(msg);
            }

            if (_hasNullEnemy)
            {
                _enemies.RemoveWhere(x => x == null);
                _hasNullEnemy = false;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        //check if collider is enemey but should be with layer mask
        var enemy = other.GetComponent<EnemyUnit>();
        _enemies.Add(enemy);
    }

    private void OnTriggerExit(Collider other)
    {
        var enemy = other.GetComponent<EnemyUnit>();
        if (_enemies.Contains(enemy))
            _enemies.Remove(enemy);
    }
}
