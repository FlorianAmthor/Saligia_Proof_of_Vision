using Gamekit3D;
using UnityEngine;

namespace SaligiaProofOfVision.Enemies
{
    public class SkeletonBerserkerUnit : MeleeEnemyUnit
    {
        #region Animator Hashes
        public static readonly int hashDefaultAttack = Animator.StringToHash("DefaultAttack");
        public static readonly int hashEndSwingAttack = Animator.StringToHash("EndSwingAttack");
        public static readonly int hashSwingAttack = Animator.StringToHash("SwingAttack");
        #endregion

        #region Swing Attack Values
        [Space, Header("Swing Attack")]
        [SerializeField] private float _swingAttackDamage;
        [SerializeField, Tooltip("In seconds")] private float _swingAttackCooldown;
        [field: SerializeField, Tooltip("In seconds")] public float SwingTime { get; private set; }
        public float SwingAttackDamage => _swingAttackDamage + bonusDmg;
        private float _lastSwingAttackTime;
        public bool CanSwingAttack => _lastSwingAttackTime + _swingAttackCooldown <= Time.time;
        #endregion

        #region Audio
        //TODO: Research if we can write a custom attribute drawer or custom editor that will add this Header to the audio header in the base class EnemyUnit
        [field: Header("Audio Berserker")]
        [field: SerializeField] public RandomAudioPlayer SwingAttackAudio { get; private set; }
        #endregion

        protected override void Start()
        {
            base.Start();
            SceneLinkedSMB<SkeletonBerserkerUnit>.Initialise(controller.animator, this);
        }

        public void SwingAttack()
        {
            meleeWeapon.damage = SwingAttackDamage;
            _lastSwingAttackTime = Time.time;
        }
    }
}