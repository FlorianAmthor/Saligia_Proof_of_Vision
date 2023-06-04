using Gamekit3D;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SaligiaProofOfVision.Abilities
{
    [CreateAssetMenu(fileName = "Scythe", menuName = "Abilities/Scythe")]
    public class Scythe : Ability
    {
        [HideInInspector] public MeleeWeapon MeleeWeapon { get; private set; }
        [SerializeField] private int _hauntDamage;
        [SerializeField] private float _timeBetweenMoves;

        [SerializeField, Space]
        private List<DamageStruct> _attacks;

        private int _numOfMoves = 0;
        private float _lastTimeUsed = 0;
        private Player _player;

        public Action<EnemyUnit> KillCallback;
        public static Action<EnemyUnit> WeaponCollisionCallback;

        [Serializable]
        public struct DamageStruct
        {
            public float damage;
            [Tooltip("Name of the Animation Trigger")]
            public string animationTrigger;
        }

        private void OnEnable()
        {
            _numOfMoves = 0;
            KillCallback += EnemyKilled;
        }

        private void OnDisable()
        {
            KillCallback -= EnemyKilled;
        }

        public override void Init(params object[] optionalData)
        {
            base.Init();
            MeleeWeapon = (MeleeWeapon)optionalData[0];
        }

        public override IEnumerator Use(BaseUnit owner = null, params object[] args)
        {
            if (_player == null)
                _player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
            if (MeleeWeapon.Owner == null)
                MeleeWeapon.Init(_player.gameObject, OnEnemyHit);

            cooldown.SetBase();

            if (_lastTimeUsed + _timeBetweenMoves < Time.time)
                _numOfMoves = 0;

            var attackConfig = _attacks[_numOfMoves];
            MeleeWeapon.damage = attackConfig.damage;
            _player.Animator.SetTrigger(attackConfig.animationTrigger);

            _numOfMoves++;
            _numOfMoves = _numOfMoves % 3;
            _lastTimeUsed = Time.time;

            yield return "";
        }

        private void OnEnemyHit()
        {
            _player.RecudeMana(ManaCost);
        }

        private void EnemyKilled(EnemyUnit enemy)
        {
            if (!HasT2Rune)
                return;
            var nextEnemy = EnemyManager.Instance.GetNearestEnemy(enemy);
            //TODO: spawn particles
            if (nextEnemy != null)
            {
                DamageMessage msg;
                msg.damage = _hauntDamage;
                msg.damager = _player;
                msg.direction = nextEnemy.transform.position;
                msg.damageSource = _player.transform.position;

                nextEnemy.ApplyDamage(msg);
            }
        }
    }
}