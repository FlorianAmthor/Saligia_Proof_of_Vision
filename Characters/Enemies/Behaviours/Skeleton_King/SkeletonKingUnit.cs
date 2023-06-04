using Gamekit3D;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace SaligiaProofOfVision.Enemies
{
    //TODO: Introduce a "Pulled" and "Reset" state for enemies to set some variables like cooldown fields etc.

    public class SkeletonKingUnit : MeleeEnemyUnit
    {
        #region Animator Hashes
        public static readonly int hashAttackIndex = Animator.StringToHash("AttackIndex");
        public static readonly int hashRaiseDead = Animator.StringToHash("RaiseDead");
        #endregion

        #region Skeleton King Audio
        [field: Header("Skeleton King Audio")]
        [field: SerializeField] public RandomAudioPlayer RaiseDeadAudio { get; private set; }
        #endregion

        #region Animation Parameters
        [SerializeField] private AnimationClip[] _defaultAttackAnimations;
        public int NumAttackAnimations => _defaultAttackAnimations.Length;
        #endregion

        #region Raise Dead Values
        [Header("Raise Dead Ability")]
        [SerializeField] private List<GameObject> _raisableUnits;
        [SerializeField] private int _numUnitsPerRaise;
        [SerializeField] private int _maxRaisedUnits;
        [SerializeField] private float _raiseDeadCooldown;
        [SerializeField] private float _maxSpawnDistance;

        private List<EnemyUnit> _raisedEnemies;
        private float _lastTimeRaiseDead = 0;
        public bool CanRaiseDead => _lastTimeRaiseDead + _raiseDeadCooldown <= Time.time && _raisedEnemies.Count < _maxRaisedUnits;
        #endregion

        protected override void OnEnable()
        {
            base.OnEnable();
            GameEvents.trashEnemyDeathEvent += OnEnemyDeath;
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            GameEvents.trashEnemyDeathEvent -= OnEnemyDeath;
        }

        protected override void Start()
        {
            base.Start();
            _raisedEnemies = new List<EnemyUnit>();
            SceneLinkedSMB<SkeletonKingUnit>.Initialise(controller.animator, this);
        }

        public void RaiseDead()
        {
            _lastTimeRaiseDead = Time.time;

            int unitsToRaise = _maxRaisedUnits-_raisedEnemies.Count;

            if (_numUnitsPerRaise < unitsToRaise)
                unitsToRaise = _numUnitsPerRaise;

            for (int i = 0; i < unitsToRaise; i++)
            {
                int index = Random.Range(0, _raisableUnits.Count);

                Vector3 randomPos = transform.position + Random.insideUnitSphere * _maxSpawnDistance;

                NavMeshHit hit;

                NavMesh.SamplePosition(randomPos, out hit, _maxSpawnDistance, NavMesh.AllAreas);

                Vector3 spawnPos = hit.position;
                Quaternion rotation = Quaternion.LookRotation(transform.forward, transform.up);
                var enemy = EnemyManager.Instance.SpawnEnemy(_raisableUnits[index], spawnPos, rotation);
                _raisedEnemies.Add(enemy);

                //TODO: Make this work correctly
                enemy.Target = PlayerController.Instance;
                enemy.ForceFindTarget();
                enemy.StartPursuit();
            }
        }

        public override void Death(DamageMessage msg)
        {
            List<EnemyUnit> tempEnemies = new List<EnemyUnit>(_raisedEnemies);
            foreach (var enemy in tempEnemies)
            {
                if (enemy == null)
                    continue;
                DamageMessage killRaisedMsg;
                killRaisedMsg.damage = 0;
                killRaisedMsg.damager = this;
                killRaisedMsg.damageSource = enemy.transform.position;
                killRaisedMsg.direction = Vector3.zero;
                enemy.Death(killRaisedMsg);
            }
            base.Death(msg);
        }

        private void OnEnemyDeath(EnemyUnit enemy)
        {
            if (_raisedEnemies.Contains(enemy))
                _raisedEnemies.Remove(enemy);
        }
        //TODO: Jump Ability?
    }
}