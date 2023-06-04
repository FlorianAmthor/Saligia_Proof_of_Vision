using Gamekit3D;
using UnityEngine;

namespace SaligiaProofOfVision.Enemies
{
    /*TODO: When the unit circles around the player make them strafe in general
     * Always look at player when the unit pursues and the player is in attack distance
     * These both would make the unit look more natural
    */
    public class SkeletonWarriorUnit : MeleeEnemyUnit
    {
        #region Animator Hashes
        public static readonly int hashShieldAttack = Animator.StringToHash("ShieldAttack");
        public static readonly int hashDefaultAttack = Animator.StringToHash("DefaultAttack");
        public static readonly int hashFrenziedAttack = Animator.StringToHash("FrenziedAttack");
        public static readonly int hashChargeStart = Animator.StringToHash("ChargeStart");
        public static readonly int hashChargeEnd = Animator.StringToHash("ChargeEnd");
        public static readonly int hashCancelCharge = Animator.StringToHash("CancelCharge");
        #endregion

        public new bool CanAttackAgain => CanFrenzyAttack || base.CanAttackAgain;

        #region Shield Attack Values
        [Space, Header("Shield Attack")]
        public MeleeWeapon shieldMeleeWeapon;
        [SerializeField] private float _shieldAttackDamage;
        [SerializeField, Tooltip("In seconds")] private float _shieldAttackCooldown;
        public float ShieldAttackDamage => _shieldAttackDamage + bonusDmg;
        [HideInInspector] public float lastShieldAttackTime = float.NegativeInfinity;
        public bool CanShieldAttack => lastShieldAttackTime + _shieldAttackCooldown <= Time.time;
        #endregion

        #region Frenzied Attack Values
        [Space, Header("Frenzied Attack")]
        [SerializeField] private float _frenziedAttackDamage;
        [SerializeField, Tooltip("In seconds")] private float _frenziedAttackCooldown;
        public float FrenziedAttackDamage => _frenziedAttackDamage + bonusDmg;

        [HideInInspector] public float lastFrenziedAttackTime = 0;
        public bool CanFrenzyAttack => lastFrenziedAttackTime + _frenziedAttackCooldown <= Time.time;
        #endregion

        #region Audio
        //TODO: Research if we can write a custom attribute drawer or custom editor that will add this Header to the audio header in the base class EnemyUnit
        [field: Header("Audio Warrior")]
        [field: SerializeField] public RandomAudioPlayer FrenziedAttackAudio { get; private set; }
        [field: SerializeField] public RandomAudioPlayer ChargeAttackAudio { get; private set; }
        [field: SerializeField] public RandomAudioPlayer ShieldAttackAudio { get; private set; }
        #endregion

        protected override void Start()
        {
            base.Start();
            SceneLinkedSMB<SkeletonWarriorUnit>.Initialise(controller.animator, this);
            shieldMeleeWeapon.Init(gameObject);
        }

        public override void AttackBegin()
        {
            lastTimeAttacked = Time.time;
            meleeWeapon.BeginAttack();
        }

        public override void AttackEnd()
        {
            meleeWeapon.EndAttack();
            //reset hashHit bool if we just exited the attack mode
            controller.animator.SetBool(hashHit, false);
            controller.animator.SetBool(hashInvulnerableHit, false);
        }

        public void ShieldAttackBegin()
        {
            lastShieldAttackTime = Time.time;
            //Set hashInAttack True if not already set by another behaviour before this one
            shieldMeleeWeapon.BeginAttack();
        }

        public void ShieldAttackEnd()
        {
            ResetHitAngle();
            shieldMeleeWeapon.EndAttack();
            //reset hashHit bool if we just exited the attack mode
            controller.animator.SetBool(hashHit, false);
            controller.animator.SetBool(hashInvulnerableHit, false);
        }

        public void ResetAlreadyHit()
        {
            meleeWeapon.ResetAlreadyHit();
        }
    }
}